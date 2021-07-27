using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Sync;
using WarehouseHandheld.Models.Accounts;
using System.Net.Http;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Services;
using System.Linq.Expressions;
using WarehouseHandheld.Resources;
using System.Collections.Generic;
using Plugin.Connectivity;

namespace WarehouseHandheld.Modules.Accounts
{
    public class AccountsModule : IAccountsModule
    {
        public AccountsModule()
        {
        }
        bool isSyncingAccounts = false;
        /*
         * This method has following responsibilities, need to be refactored 
        1. Adding log entry in sync log table before sending request to service
        2. Getting Records from service layer
        3. Adding fetched data to database
        4. Sending Acknowledgement service call 
        5. Updating Sync Log entry according to service response
        
        similar changes need to be done in all other modules
        */
        public async Task SyncAccounts()
        {
            if (!CrossConnectivity.Current.IsConnected || !await Util.Util.IsConnected())
            {
                if (!CrossConnectivity.Current.IsConnected)
                    AppStrings.NoInternet.ToToast();
                return;
            }
            if (isSyncingAccounts)
            {
                "Already syncing accounts".ToToast();
                return;
            }
            isSyncingAccounts = true;

            DateTime date;
            SyncLog synclog = await App.Database.SyncLog.GetSyncLogByTableName(Database.DatabaseConfig.Tables.Accounts.ToString());

            if (synclog != null && synclog.RequestedTime != DateTime.MinValue)
                date = synclog.RequestedTime;
            else
                date = ModulesConfig.SyncDate;

            string serialNo = ModulesConfig.SerialNo;

            if (synclog == null)
                synclog = await InitializeSyncLog();

            //Update Sync Log before sending request
            synclog.SerialNo = serialNo;
            synclog.RequestUrl = WebServiceConfig.SyncAccounts;
            synclog.RequestedTime = DateTime.UtcNow;

            //Service Call
            AccountSyncCollection accounts = await App.WarehouseService.Accounts.GetAccountsAsync(date, serialNo);
            if (accounts == null)
            {
                Constants.ApiErrorMsg.ToToast();
                isSyncingAccounts = false;
                return;
            }
            
            //Adding fetched data to database
            await AddAccountsToDatabase(accounts);

            //acknowledgement service call
            HttpResponseMessage ackResponse = await App.WarehouseService.Acknowledgement.VerifyAckAsync(accounts.TerminalLogId.ToString(), accounts.Accounts.Count, serialNo);

            synclog.ResponseTime = DateTime.UtcNow;
            synclog.Synced = false;
            synclog.ErrorCode = (int)ackResponse.StatusCode;

            if ((int)ackResponse.StatusCode == 200)
            {
                synclog.TerminalLogId = accounts.TerminalLogId.ToString();
                synclog.LastSynced = DateTime.UtcNow;
                synclog.Synced = true;
                synclog.ResultCount = accounts.Count;
                synclog.ErrorCode = 0;
                await App.Database.SyncLog.UpdateSyncLogItem(synclog);
            }
            isSyncingAccounts = false;
        }

        private async Task<SyncLog> InitializeSyncLog()
        {
            SyncLog synclog = new SyncLog();
            synclog.TableName = Database.DatabaseConfig.Tables.Accounts.ToString();
            synclog.RequestUrl = WebServiceConfig.SyncAccounts;
            await App.Database.SyncLog.AddSyncLogItem(synclog);
            return synclog;
        }

        private async Task AddAccountsToDatabase(AccountSyncCollection accounts)
        {
            if (accounts != null && accounts.Accounts != null)
                await App.Database.Accounts.AddUpdateAccounts(accounts.Accounts);
        }

        public async Task<List<AccountSync>> GetAllAccounts()
        {
            return await App.Database.Accounts.GetAllAccounts();
        }
    }
}

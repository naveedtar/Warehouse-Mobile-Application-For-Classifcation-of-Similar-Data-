using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Sync;
using WarehouseHandheld.Services;
using WarehouseHandheld.Models.InventoryStocks;
using WarehouseHandheld.Resources;
using System.Net.Http;
using WarehouseHandheld.Extensions;
using System.Collections.Generic;
using Plugin.Connectivity;

namespace WarehouseHandheld.Modules.InventoryStocks
{
    public class InventoryStocksModule : IInventoryStocksModule
    {
        public InventoryStocksModule()
        {
        }
        bool isSyncingInventoryStocks = false;
        public async Task SyncInventoryStocks()
        {
            if (!CrossConnectivity.Current.IsConnected || !await Util.Util.IsConnected())
            {
                if (!CrossConnectivity.Current.IsConnected)
                    AppStrings.NoInternet.ToToast();
                return;
            }
            if (isSyncingInventoryStocks){
                "Already syncing inventory stocks".ToToast();
                return;
            }
            isSyncingInventoryStocks = true;

            DateTime date;
            SyncLog synclog = await App.Database.SyncLog.GetSyncLogByTableName(Database.DatabaseConfig.Tables.InventoryStocks.ToString());

            if (synclog != null && synclog.RequestedTime != DateTime.MinValue)
                date = synclog.RequestedTime;
            else
                date = ModulesConfig.SyncDate;

            string serialNo = ModulesConfig.SerialNo;

            if (synclog == null)
                synclog = await InitializeSyncLog();

            //Update Sync Log before sending request
            synclog.SerialNo = serialNo;
            synclog.RequestUrl = WebServiceConfig.SyncInventoryStocks;
            synclog.RequestedTime = DateTime.UtcNow;

            //Service Call
            InventoryStockSyncCollection inventoryStocks = await App.WarehouseService.InventoryStock.GetInventoryStockAsync(date, serialNo);
            if (inventoryStocks == null)
            {
                Constants.ApiErrorMsg.ToToast();
                isSyncingInventoryStocks = false;
                return;
            }
            
            //Adding fetched data to database
            await AddInventoryStocksToDatabase(inventoryStocks);

            //acknowledgement service call
            HttpResponseMessage ackResponse = await App.WarehouseService.Acknowledgement.VerifyAckAsync(inventoryStocks.TerminalLogId.ToString(), inventoryStocks.InventoryStocks.Count, serialNo);

            synclog.ResponseTime = DateTime.UtcNow;
            synclog.Synced = false;
            synclog.ErrorCode = (int)ackResponse.StatusCode;

            if ((int)ackResponse.StatusCode == 200)
            {
                synclog.TerminalLogId = inventoryStocks.TerminalLogId.ToString();
                synclog.LastSynced = DateTime.UtcNow;
                synclog.Synced = true;
                synclog.ResultCount = inventoryStocks.Count;
                synclog.ErrorCode = 0;
                await App.Database.SyncLog.UpdateSyncLogItem(synclog);
            }

            isSyncingInventoryStocks = false;

        }

        private async Task<SyncLog> InitializeSyncLog()
        {
            SyncLog synclog = new SyncLog();
            synclog.TableName = Database.DatabaseConfig.Tables.InventoryStocks.ToString();
            synclog.RequestUrl = WebServiceConfig.SyncInventoryStocks;
            await App.Database.SyncLog.AddSyncLogItem(synclog);
            return synclog;
        }

        private async Task AddInventoryStocksToDatabase(InventoryStockSyncCollection inventoryStock)
        {
            if (inventoryStock != null && inventoryStock.InventoryStocks != null)
                await App.Database.InventoryStocks.AddUpdateInventoryStocks(inventoryStock.InventoryStocks);
        }

        public async Task<List<InventoryStockSync>> GetAllInventoryStock()
        {
            return await App.Database.InventoryStocks.GetAllInventoryStocks();
        }
    }
}

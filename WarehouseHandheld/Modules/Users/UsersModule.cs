using System;
using WarehouseHandheld.Models.Sync;
using WarehouseHandheld.Models.Users;
using System.Threading.Tasks;
using WarehouseHandheld.Services;
using System.Net.Http;
using WarehouseHandheld.Extensions;
using xBrainLab.Security.Cryptography;
using WarehouseHandheld.Resources;
using System.Linq;
using System.Collections.Generic;
using Plugin.Connectivity;
using WarehouseHandheld.Models.GeoLocation;
using Xamarin.Forms;
using WarehouseHandheld.Helpers;

namespace WarehouseHandheld.Modules.Users
{
    public class UsersModule : IUsersModule
    {
        private int loggedInUserId;
        bool isSyncingUsers = false;
        public int LoggedInUserId 
        {
            get { return loggedInUserId; }
            set { loggedInUserId = value; }
        }

        public UsersModule()
        {
        }

        public async Task SyncUsers()
        {
            if (!CrossConnectivity.Current.IsConnected || !await Util.Util.IsConnected())
            {
                if (!CrossConnectivity.Current.IsConnected)
                    AppStrings.NoInternet.ToToast();
                return;
            }
            if (isSyncingUsers)
            {
                "Already syncing users".ToToast();
                return;
            }
            isSyncingUsers = true;

            DateTime date;
            SyncLog synclog = await App.Database.SyncLog.GetSyncLogByTableName(Database.DatabaseConfig.Tables.Users.ToString());

            if (synclog != null && synclog.RequestedTime != DateTime.MinValue)
                date = synclog.RequestedTime;
            else
                date = ModulesConfig.SyncDate;

            string serialNo = ModulesConfig.SerialNo;

            if (synclog == null)
                synclog = await InitializeSyncLog();

            //Update Sync Log before sending request
            synclog.SerialNo = serialNo;
            synclog.RequestUrl = WebServiceConfig.SyncUsers;
            synclog.RequestedTime = DateTime.UtcNow;

            //Service Call
            UsersSyncCollection users = await App.WarehouseService.Users.GetUsersAsync(date, serialNo);
            if (users == null)
            {
                Constants.ApiErrorMsg.ToToast();
                isSyncingUsers = false;
                return;
            }
            
            //Adding fetched data to database
            await AddUsersToDatabase(users);

            //acknowledgement service call
            HttpResponseMessage ackResponse = await App.WarehouseService.Acknowledgement.VerifyAckAsync(users.TerminalLogId.ToString(), users.Users.Count, serialNo);

            synclog.ResponseTime = DateTime.UtcNow;
            synclog.Synced = false;
            synclog.ErrorCode = (int)ackResponse.StatusCode;

            if ((int)ackResponse.StatusCode == 200)
            {
                synclog.TerminalLogId = users.TerminalLogId.ToString();
                synclog.LastSynced = DateTime.UtcNow;
                synclog.Synced = true;
                synclog.ResultCount = users.count;
                synclog.ErrorCode = 0;
                await App.Database.SyncLog.UpdateSyncLogItem(synclog);
            }
            isSyncingUsers = false;
        }

        public async Task SyncUserLocation()
        {
            var request = new TerminalGeoLocationViewModel();
            request.Id = Guid.NewGuid();
            var terminalData = await App.Database.Vehicle.GetTerminalMetaData();
            if (terminalData != null && terminalData.PostGeoLocation)
            {
                request.TerminalId = terminalData.TerminalId;
                var location = await DependencyService.Get<ILocationHelper>().GetLocation();
                if (location != null)
                {
                    request.Latitude = location.Latitude;
                    request.Longitude = location.Longitude;
                }
                else
                {
                    "Something wrong with location. Please enable location.".ToToast();
                }
            }

            request.Date = DateTime.Now;
            request.LoggedInUserId = App.Users.LoggedInUserId;


            request.TenantId = ModulesConfig.TenantID;
            request.SerialNo = ModulesConfig.SerialNo;
            if (terminalData != null && terminalData.PostGeoLocation)
            {
                var result = await App.WarehouseService.PostGeoLocation.PostUserLocationAsync(request);
                if (result == null)
                {
                    "Error while posting user loction".ToToast();
                }
            }
        }

        private async Task<SyncLog> InitializeSyncLog()
        {
            SyncLog synclog = new SyncLog();
            synclog.TableName = Database.DatabaseConfig.Tables.Users.ToString();
            synclog.RequestUrl = WebServiceConfig.SyncUsers;
            await App.Database.SyncLog.AddSyncLogItem(synclog);
            return synclog;
        }

        private async Task AddUsersToDatabase(UsersSyncCollection users)
        {
            if (users != null && users.Users != null)
                await App.Database.Users.AddUpdateUsers(users.Users);
        }

        public async Task<List<UserSync>> GetDrivers()
        {
            return (await App.Database.Users.GetAllUsers()).FindAll((obj) => obj.IsResource && obj.ResourceId > 0).ToList();
        }

        public async Task<bool> Login(string username, string password)
        {
            string passwordHash = MD5.GetHashString(password).ToLower();
            var userId = await App.Database.Users.LoginUser(username,passwordHash);
            if (userId != 0)
            {
                LoggedInUserId = userId;
                return true;
            }
            else
                return false;
        }

        public async Task<bool> VerifyUserPass(string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                password = MD5.GetHashString(password).ToLower();
                return await App.Database.Users.VerifyUserPass(password);
            }
            return false;
        }
    }
}

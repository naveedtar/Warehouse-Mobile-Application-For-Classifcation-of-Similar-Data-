using System;
using System.Threading.Tasks;
using System.Net.Http;

using System.Collections.Generic;
using WarehouseHandheld.Models.Sync;
using WarehouseHandheld.Services;
using WarehouseHandheld.Models.Vehicles;
using Plugin.Connectivity;
using WarehouseHandheld.Resources;
using WarehouseHandheld.Extensions;
using Xamarin.Forms;
using WarehouseHandheld.Helpers;

namespace WarehouseHandheld.Modules.Accounts
{
    public class VehiclesModule : IVehiclesModule
    {
        public VehiclesModule()
        {
        }
        bool isSyncingVehicles = false;
        bool isSyncingTerminalDate = false;
        public async Task SyncVehicles()
        {
            SyncTerminalMetaData();
            if (!CrossConnectivity.Current.IsConnected || !await Util.Util.IsConnected())
            {
                if (!CrossConnectivity.Current.IsConnected)
                    AppStrings.NoInternet.ToToast();
                return;
            }
            if (isSyncingVehicles)
            {
                "Already syncing vehicles".ToToast();
                return;
            }
            isSyncingVehicles = true;

            DateTime date;
            SyncLog synclog = await App.Database.SyncLog.GetSyncLogByTableName(Database.DatabaseConfig.Tables.Vehicles.ToString());

            if (synclog != null && synclog.RequestedTime != DateTime.MinValue)
                date = synclog.RequestedTime;
            else
                date = ModulesConfig.SyncDate;

            string serialNo = ModulesConfig.SerialNo;

            if (synclog == null)
                synclog = await InitializeSyncLog();

            //Update Sync Log before sending request
            synclog.SerialNo = serialNo;
            synclog.RequestUrl = WebServiceConfig.SyncVehicles;
            synclog.RequestedTime = DateTime.UtcNow;

            //Service Call
            MarketVehiclesSyncCollection vehicles = await App.WarehouseService.Vehicles.GetVehiclesAsync(date, serialNo);
            if (vehicles == null)
            {
                Constants.ApiErrorMsg.ToToast();
                isSyncingVehicles = false;
                return;
            }
            
            //Adding fetched data to database
            await AddVehiclesToDatabase(vehicles);

            //acknowledgement service call
            HttpResponseMessage ackResponse = await App.WarehouseService.Acknowledgement.VerifyAckAsync(vehicles.TerminalLogId.ToString(), vehicles.Vehicles.Count, serialNo);

            synclog.ResponseTime = DateTime.UtcNow;
            synclog.Synced = false;
            synclog.ErrorCode = (int)ackResponse.StatusCode;

            if ((int)ackResponse.StatusCode == 200)
            {
                synclog.TerminalLogId = vehicles.TerminalLogId.ToString();
                synclog.LastSynced = DateTime.UtcNow;
                synclog.Synced = true;
                synclog.ResultCount = vehicles.Count;
                synclog.ErrorCode = 0;
                await App.Database.SyncLog.UpdateSyncLogItem(synclog);
            }

            isSyncingVehicles = false;

        }

        public async Task<TerminalMetadataSync> SyncTerminalMetaData()
        {
            var date = DateTime.UtcNow;

            string serialNo = ModulesConfig.SerialNo;
            try
            {
                if (isSyncingTerminalDate)
                {
                    "Already syncing terminal data".ToToast();
                    return null;
                }
                isSyncingTerminalDate = true;
                var result = await App.WarehouseService.Vehicles.GetTerminalMetadataAsync(date, serialNo);
                if (result != null)
                {
                    ModulesConfig.TenantID = result.TenantId;
                    ModulesConfig.WareHouseID = result.ParentWarehouseId;
                    if (result.PalletTrackingScheme == 0)
                    {
                        result.PalletTrackingScheme = PalletTrackingSchemeEnum.DontEnforce;
                    }
                    ModulesConfig.TrackingScheme = result.PalletTrackingScheme;
                    var ImageHelper = DependencyService.Get<IImageHelper>();
                    var TerminalData = await App.Database.Vehicle.GetTerminalMetaData();
                    if(TerminalData!=null && !string.IsNullOrEmpty(TerminalData.LogoPath))
                    {
                        ImageHelper.DeleteImage(TerminalData.LogoPath);
                    }
                    if(result.TenantLogo!=null)
                    {
                        result.LogoPath =  await ImageHelper.AddImage(result.TenantLogo);
                    }
                    await App.Database.Vehicle.AddUpdateTerminalMetaData(result);
                    isSyncingTerminalDate = false;
                    return result;
                }
                isSyncingTerminalDate = false;
            }
            catch(Exception ex) {
                isSyncingTerminalDate = false;
            }
            return null;
        }

        private async Task<SyncLog> InitializeSyncLog()
        {
            SyncLog synclog = new SyncLog();
            synclog.TableName = Database.DatabaseConfig.Tables.Vehicles.ToString();
            synclog.RequestUrl = WebServiceConfig.SyncVehicles;
            await App.Database.SyncLog.AddSyncLogItem(synclog);
            return synclog;
        }

        private async Task AddVehiclesToDatabase(MarketVehiclesSyncCollection vehicles)
        {
            if (vehicles != null && vehicles.Vehicles != null)
                await App.Database.Vehicle.AddUpdateVehicles(vehicles.Vehicles);
        }

        public async Task<List<MarketVehiclesSync>> GetAllVehicles()
        {
            return await App.Database.Vehicle.GetAllVehicles();
        }

    }
}

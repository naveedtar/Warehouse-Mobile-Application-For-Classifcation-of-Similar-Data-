using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Connectivity;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Models.ProductStockLocation;
using WarehouseHandheld.Models.StockMovement;
using WarehouseHandheld.Models.Sync;
using WarehouseHandheld.Resources;
using WarehouseHandheld.Services;

namespace WarehouseHandheld.Modules.StockMovement
{
    public class StockMovementModule : IStockMovementModule
    {
        bool isSyncingStockLocations;
        bool isSyncingStockMovement;
        bool isProductStockLocation;

        public async Task SyncLocations()
        {
            if (!CrossConnectivity.Current.IsConnected || !await Util.Util.IsConnected())
            {
                if (!CrossConnectivity.Current.IsConnected)
                    AppStrings.NoInternet.ToToast();
                return;
            }
            if (isSyncingStockLocations)
            {
                "Already syncing stock locations".ToToast();
            }
            isSyncingStockLocations = true;

            DateTime date;
            SyncLog synclog = await App.Database.SyncLog.GetSyncLogByTableName(Database.DatabaseConfig.Tables.StockLocationMovement.ToString());

            if (synclog != null && synclog.RequestedTime != DateTime.MinValue)
                date = synclog.RequestedTime;
            else
                date = ModulesConfig.SyncDate;

            string serialNo = ModulesConfig.SerialNo;

            if (synclog == null)
                synclog = await InitializeSyncLog(Database.DatabaseConfig.Tables.StockLocationMovement.ToString(), WebServiceConfig.SyncStockLocationMovement);

            //Update Sync Log before sending request
            synclog.SerialNo = serialNo;
            synclog.RequestUrl = WebServiceConfig.SyncStockLocationMovement;
            synclog.RequestedTime = DateTime.UtcNow;

            //Service Call
            LocationSyncCollection stockLocations = await App.WarehouseService.StockMovement.GetStockLocationsAsync(date, serialNo);
            if (stockLocations == null)
            {
                Constants.ApiErrorMsg.ToToast();
                isSyncingStockLocations = false;
                return;
            }

            //Adding fetched data to database
            await AddStockLocationsToDatabase(stockLocations);

            //acknowledgement service call
            HttpResponseMessage ackResponse = await App.WarehouseService.Acknowledgement.VerifyAckAsync(stockLocations.TerminalLogId.ToString(), stockLocations.LocationSync.Count, serialNo);

            synclog.ResponseTime = DateTime.UtcNow;
            synclog.Synced = false;
            synclog.ErrorCode = (int)ackResponse.StatusCode;

            if ((int)ackResponse.StatusCode == 200)
            {
                synclog.TerminalLogId = stockLocations.TerminalLogId.ToString();
                synclog.LastSynced = DateTime.UtcNow;
                synclog.Synced = true;
                synclog.ResultCount = stockLocations.Count;
                synclog.ErrorCode = 0;
                await App.Database.SyncLog.UpdateSyncLogItem(synclog);
            }
            isSyncingStockLocations = false;
        }



        private async Task<SyncLog> InitializeSyncLog(string tableName, string requestUrl)
        {
            SyncLog synclog = new SyncLog();
            synclog.TableName = tableName;
            synclog.RequestUrl = requestUrl;
            await App.Database.SyncLog.AddSyncLogItem(synclog);
            return synclog;
        }

        private async Task AddStockLocationsToDatabase(LocationSyncCollection stockLocations)
        {
            if (stockLocations.LocationSync != null && stockLocations.LocationSync.Any())
                await App.Database.StockMovements.AddUpdateStockLocations(stockLocations.LocationSync);
        }

        public async Task SyncStockMovement()
        {

            {
                var logsToPost = await App.Database.SyncLog.GetAllSyncLogsByTableName(Database.DatabaseConfig.Tables.StockMovementViewModel.ToString(), true, false);
                if (logsToPost != null && logsToPost.Any())
                {
                    foreach (var syncLog in logsToPost)
                    {
                        syncLog.IsPending = true;
                        await App.Database.SyncLog.UpdateSyncLogItem(syncLog);
                        bool Synced = false;
                        var stockMovementToPost = JsonConvert.DeserializeObject<StockMovemeneCollectionViewModel>(syncLog.request);
                        var isSubmitted = await App.WarehouseService.StockMovement.PostStockMovementAsync(stockMovementToPost);
                        if (isSubmitted)
                        {
                            //string serialNo = ModulesConfig.SerialNo;
                            //HttpResponseMessage ackResponse = await App.WarehouseService.Acknowledgement.VerifyAckAsync(palletsTrackings.TerminalLogId, palletsTrackings.Count, serialNo);
                            "Stock Movement posted Successfully.".ToToast();
                            Synced = true;
                        }
                        else if (App.WarehouseService.StockMovement.HandleConflictStatusStockMovement())
                        {
                            Synced = true;
                            //"Already posted Pallet Tracking.".ToToast();
                        }
                        else
                        {
                            Synced = false;
                            "Some issue with stock movement sync.".ToToast();
                        }
                        syncLog.IsPending = false;
                        syncLog.Synced = Synced;
                        await App.Database.SyncLog.UpdateSyncLogItem(syncLog);

                    }

                }
            }
        }

        public async Task SyncProductLocationStock()
        {
            if (!CrossConnectivity.Current.IsConnected || !await Util.Util.IsConnected())
            {
                if (!CrossConnectivity.Current.IsConnected)
                    AppStrings.NoInternet.ToToast();
                return;
            }
            if (isProductStockLocation)
            {
                "Already syncing product stock locations".ToToast();
            }
            isProductStockLocation = true;

            DateTime date;
            SyncLog synclog = await App.Database.SyncLog.GetSyncLogByTableName(Database.DatabaseConfig.Tables.ProductLocationStock.ToString());

            if (synclog != null && synclog.RequestedTime != DateTime.MinValue)
                date = synclog.RequestedTime;
            else
                date = ModulesConfig.SyncDate;

            string serialNo = ModulesConfig.SerialNo;

            if (synclog == null)
                synclog = await InitializeSyncLog(Database.DatabaseConfig.Tables.ProductLocationStock.ToString(), WebServiceConfig.SyncProductLocationStocksSync);

            //Update Sync Log before sending request
            synclog.SerialNo = serialNo;
            synclog.RequestUrl = WebServiceConfig.SyncProductLocationStocksSync;
            synclog.RequestedTime = DateTime.UtcNow;

            //Service Call
            ProductLocationStocksSyncCollection productLocationStocks = await App.WarehouseService.ProductStockLocation.GetAllProductStockLocations(date, serialNo);
            if (productLocationStocks == null)
            {
                Constants.ApiErrorMsg.ToToast();
                isProductStockLocation = false;
                return;
            }

            //Adding fetched data to database
            await AddProductStockLocationsToDatabase(productLocationStocks);

            //acknowledgement service call
            HttpResponseMessage ackResponse = await App.WarehouseService.Acknowledgement.VerifyAckAsync(productLocationStocks.TerminalLogId.ToString(), productLocationStocks.ProductLocationStocksSync.Count, serialNo);

            synclog.ResponseTime = DateTime.UtcNow;
            synclog.Synced = false;
            synclog.ErrorCode = (int)ackResponse.StatusCode;

            if ((int)ackResponse.StatusCode == 200)
            {
                synclog.TerminalLogId = productLocationStocks.TerminalLogId.ToString();
                synclog.LastSynced = DateTime.UtcNow;
                synclog.Synced = true;
                synclog.ResultCount = productLocationStocks.Count;
                synclog.ErrorCode = 0;
                await App.Database.SyncLog.UpdateSyncLogItem(synclog);
            }
            isSyncingStockLocations = false;
        }
        
        private async Task AddProductStockLocationsToDatabase(ProductLocationStocksSyncCollection productLocationStocks)
        {
            if (productLocationStocks.ProductLocationStocksSync != null && productLocationStocks.ProductLocationStocksSync.Any())
                await App.Database.ProductLocationStock.AddUpdateProductStockLocation(productLocationStocks.ProductLocationStocksSync);
        }
    }
}

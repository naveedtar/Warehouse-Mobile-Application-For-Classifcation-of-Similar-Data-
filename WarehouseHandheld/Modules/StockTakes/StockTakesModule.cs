using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Sync;
using WarehouseHandheld.Services;
using WarehouseHandheld.Models.StockTakes;
using WarehouseHandheld.Resources;
using WarehouseHandheld.Extensions;
using System.Net.Http;
using System.Collections.Generic;
using WarehouseHandheld.Models.Products;
using Plugin.Connectivity;

namespace WarehouseHandheld.Modules.StockTakes
{
    public class StockTakesModule : IStockTakesModule
    {
        public StockTakesModule()
        {
        }
        bool isSyncingStockTakes = false;
        public bool IsBusy;

        public async Task SyncStockTakes()
        {
            if (!CrossConnectivity.Current.IsConnected || !await Util.Util.IsConnected())
            {
                if (!CrossConnectivity.Current.IsConnected)
                    AppStrings.NoInternet.ToToast();
                return;
            }
            if (isSyncingStockTakes)
            {
                "Already syncing Stock takes".ToToast();
                return;
            }
            isSyncingStockTakes = true;

            DateTime date;
            SyncLog synclog = await App.Database.SyncLog.GetSyncLogByTableName(Database.DatabaseConfig.Tables.StockTakes.ToString());

            if (synclog != null && synclog.RequestedTime != DateTime.MinValue)
                date = synclog.RequestedTime;
            else
                date = ModulesConfig.SyncDate;

            string serialNo = ModulesConfig.SerialNo;

            if (synclog == null)
                synclog = InitializeSyncLog();

            //Update Sync Log before sending request
            synclog.SerialNo = serialNo;
            synclog.RequestUrl = WebServiceConfig.SyncStockTakes;
            synclog.RequestedTime = DateTime.UtcNow;

            //Service Call
            StockTakeSyncCollection stockTakes = await App.WarehouseService.StockTakes.GetStockTakesAsync(date, serialNo);

            //Adding fetched data to database
            await AddStockTakesToDatabase(stockTakes);
            if (stockTakes == null)
            {
                Constants.ApiErrorMsg.ToToast();
                isSyncingStockTakes = false;
                return;
            }

            //acknowledgement service call
            HttpResponseMessage ackResponse = await App.WarehouseService.Acknowledgement.VerifyAckAsync(stockTakes.TerminalLogId.ToString(), stockTakes.StockTakes.Count, serialNo);

            synclog.ResponseTime = DateTime.UtcNow;
            synclog.Synced = false;
            synclog.ErrorCode = (int)ackResponse.StatusCode;

            if ((int)ackResponse.StatusCode == 200)
            {
                synclog.TerminalLogId = stockTakes.TerminalLogId.ToString();
                synclog.LastSynced = DateTime.UtcNow;
                synclog.Synced = true;
                synclog.ResultCount = stockTakes.Count;
                synclog.ErrorCode = 0;
                await App.Database.SyncLog.UpdateSyncLogItem(synclog);
            }
            //PostAllScanStockProduct();
            isSyncingStockTakes = false;
        }

        private SyncLog InitializeSyncLog()
        {
            SyncLog synclog = new SyncLog();
            synclog.TableName = Database.DatabaseConfig.Tables.StockTakes.ToString();
            synclog.RequestUrl = WebServiceConfig.SyncStockTakes;
            App.Database.SyncLog.AddSyncLogItem(synclog);
            return synclog;
        }

        private async Task AddStockTakesToDatabase(StockTakeSyncCollection stockTakes)
        {
            if (stockTakes != null && stockTakes.StockTakes != null)
                await App.Database.StockTakes.AddUpdateStockTakes(stockTakes.StockTakes);
        }

        public async Task<List<StockTakeSync>> GetStockTakes()
        {
            return await App.Database.StockTakes.GetStockTakes();
        }

        public async Task<StockTakeProductCodeScanResponse> PostScanStockTakeProduct(string LocationCode, ProductMasterSync product, int StockTakeId, int WareHouseId, string batchNumber, DateTime? expiryDate, string serial, string PalletSerial, decimal quantity, bool exisitingItem = false)
        {
            var terminalData = await App.Database.Vehicle.GetTerminalMetaData();

            StockTakeProductCodeScanRequest stockTakeProductScanRequest = new StockTakeProductCodeScanRequest();
            stockTakeProductScanRequest.ProductCode = product.SKUCode;
            stockTakeProductScanRequest.ScannedQuantity = Convert.ToInt32(quantity);
            if (terminalData != null)
            {
                stockTakeProductScanRequest.CurrentTenantId = terminalData.TenantId;
            }
            stockTakeProductScanRequest.TerminalSerial = terminalData.Serial;
            stockTakeProductScanRequest.TransactionLogId = Guid.NewGuid();
            stockTakeProductScanRequest.AuthUserId = App.Users.LoggedInUserId;
            stockTakeProductScanRequest.LocationCode = LocationCode;
            stockTakeProductScanRequest.StockTakeId = StockTakeId;
            stockTakeProductScanRequest.ProductSerial = serial;
            stockTakeProductScanRequest.WarehouseId = WareHouseId;
            stockTakeProductScanRequest.PalletSerial = PalletSerial;
            stockTakeProductScanRequest.NewProductBarcode = product.BarCode;
            stockTakeProductScanRequest.NewProductBarcode2 = product.BarCode2;
            stockTakeProductScanRequest.NewProductName = product.Name;
            stockTakeProductScanRequest.ProductId = product.ProductId;
            stockTakeProductScanRequest.BatchNumber = batchNumber;
            stockTakeProductScanRequest.ExpiryDate = expiryDate;
            stockTakeProductScanRequest.BatchRequired = true;
            if (product.ProductId == 0)
            {
                stockTakeProductScanRequest.NotExistingItem = true;
                stockTakeProductScanRequest.ForceSerialCreate = true;
            }
            if (!string.IsNullOrEmpty(serial) && !exisitingItem)
            {
                stockTakeProductScanRequest.IsSerialised = true;
                stockTakeProductScanRequest.NotExistingItem = true;
            }
            if (product.ProcessByPallet == true && !exisitingItem)
            {
                stockTakeProductScanRequest.NotExistingItem = true;
            }
            stockTakeProductScanRequest.IsProcessByPallet = product.ProcessByPallet;
            //await App.Database.StockTakes.AddUpdateStockTakeProductScanrequest(stockTakeProductScanRequest);
            return await PostAllScanStockProduct(stockTakeProductScanRequest);
        }

        public async Task UpdateQuantity(decimal Quantity, int stockDetailId, string batchnumber, DateTime? expiryDate)
        {

            //var ScanRequest = await App.Database.StockTakes.GetStockTakeProductScanrequest(StockTakeRequestId);
            //var quantityRequest = await App.Database.StockTakes.GetStockQuantityUpdateRequest(StockTakeRequestId);
            //if (quantityRequest == null)
            //{
            //    quantityRequest = new StockDetailQuantityUpdateRequest();
            //}
            //if (Quantity != quantityRequest.NewQuantity)
            //{
            //quantityRequest.NewQuantity = Quantity;
            //quantityRequest.StockTakeRequestId = StockTakeRequestId;
            //quantityRequest.IsSynced = false;
            //ScanRequest.ScannedQuantity = Convert.ToInt32(Quantity);
            //await App.Database.StockTakes.AddUpdateStockQuantityUpdateRequest(quantityRequest);
            //await App.Database.StockTakes.AddUpdateStockTakeProductScanrequest(ScanRequest);
            var updateRequest = new StockDetailQuantityUpdateRequest();
            updateRequest.NewQuantity = Quantity;
            updateRequest.TransactionLogId = Guid.NewGuid();
            updateRequest.BatchNumber = batchnumber;
            updateRequest.ExpiryDate = expiryDate;
            updateRequest.StockTakeDetailId = stockDetailId;

            PostUpdateQuantityRequests();
            var response = await App.WarehouseService.StockTakes.UpdateStockQuantity(updateRequest);
            if (response != null && response.Success)
            {
                "Quantity Updated Successfully.".ToToast();
                updateRequest.IsSynced = true;
                //await App.Database.StockTakes.AddUpdateStockQuantityUpdateRequest(updateRequest);
            }
            else if (App.WarehouseService.StockTakes.HandleStatusConflictUpdate())
            {
                updateRequest.IsSynced = true;
            }
            //}
        }

        public void PostUpdateQuantityRequests()
        {
            if (!IsBusy && CrossConnectivity.Current.IsConnected)
            {
                IsBusy = true;
                Task.Factory.StartNew(async () =>
                {
                    var AllQuantityUpdateRequests = (await App.Database.StockTakes.GetAllStockQuantityUpdateRequests()).FindAll((obj) => !obj.IsSynced);
                    foreach (var quantityUpdateRequest in AllQuantityUpdateRequests)
                    {
                        var ScanRequest = await App.Database.StockTakes.GetStockTakeProductScanrequest(quantityUpdateRequest.StockTakeRequestId);
                        if (ScanRequest != null && ScanRequest.StockTakeId != 0)
                        {
                            quantityUpdateRequest.StockTakeDetailId = ScanRequest.StockTakeId;

                            var response = await App.WarehouseService.StockTakes.UpdateStockQuantity(quantityUpdateRequest);
                            if (response != null && response.Success)
                            {
                                "Quantity Updated Successfully.".ToToast();
                                quantityUpdateRequest.IsSynced = true;
                                await App.Database.StockTakes.AddUpdateStockQuantityUpdateRequest(quantityUpdateRequest);
                            }
                            else if (App.WarehouseService.StockTakes.HandleStatusConflictUpdate())
                            {
                                quantityUpdateRequest.IsSynced = true;
                            }
                        }
                    }
                    IsBusy = false;
                });
            }
        }

        public async Task<StockTakeProductCodeScanResponse> PostAllScanStockProduct(StockTakeProductCodeScanRequest stockTakeRequest)
        {

            var stockTakeResponse = await App.WarehouseService.StockTakes.RecordStockScan(stockTakeRequest);
            if (stockTakeResponse != null && (stockTakeResponse.Response == null || !stockTakeResponse.Response.Success) && !App.WarehouseService.StockTakes.HandleStatusConflictScan())
            {
                await Util.Util.ShowErrorPopupWithBeep(stockTakeResponse.Response.FailureMessage);
                //await App.Database.StockTakes.DeleteStockTakeProductRequest(stockTakeRequest);
                return stockTakeResponse;
            }
            else if (stockTakeResponse != null)
            {
                "Product Added Successfully.".ToToast();
                return stockTakeResponse;
            }
            return stockTakeResponse;
        }
    }
}

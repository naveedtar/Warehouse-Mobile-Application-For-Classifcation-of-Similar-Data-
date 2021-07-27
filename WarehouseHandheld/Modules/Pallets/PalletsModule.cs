using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Sync;
using WarehouseHandheld.Services;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Resources;
using System.Net.Http;
using WarehouseHandheld.Extensions;
using System.Collections.Generic;
using WarehouseHandheld.Models.Orders;
using Plugin.Connectivity;
using System.Linq;
using Newtonsoft.Json;
using WarehouseHandheld.Models.Vehicles;
using WarehouseHandheld.Models.OrderProcesses;
using System.Diagnostics;

namespace WarehouseHandheld.Modules.Pallets
{
    public class PalletsModule : IPalletsModule
    {
        public PalletsModule()
        {
        }
        bool isSyncingPallets = false;
        bool isSyncingPalletsMethods = false;
        bool isSyncingPalletsProducts = false;
        bool isSyncingPalletTracking = false;
        bool isSyncingPalletDispatches = false;

        public async Task CheckOrPostPalletTracking()
        {
          
            //var AlllLogs = await App.Database.SyncLog.GetAll();
            //var logsToPost = AlllLogs.FindAll((obj) => obj.TableName == Database.DatabaseConfig.Tables.PalletTracking.ToString() && obj.IsPost && !obj.Synced);
            var logsToPost = await App.Database.SyncLog.GetAllSyncLogsByTableName(Database.DatabaseConfig.Tables.PalletTracking.ToString(), true, false);
            foreach (var syncLog in logsToPost)
            {
                syncLog.IsPending = true;
                await App.Database.SyncLog.UpdateSyncLogItem(syncLog);
                bool Synced = false;
                var palletTrackingToPost = JsonConvert.DeserializeObject<PalletTrackingSyncCollection>(syncLog.request);
                var palletsTrackings = await PostPalletTracking(palletTrackingToPost);

                if (palletsTrackings != null && palletsTrackings.PalletTrackingSync != null)
                {
                    //string serialNo = ModulesConfig.SerialNo;
                    //HttpResponseMessage ackResponse = await App.WarehouseService.Acknowledgement.VerifyAckAsync(palletsTrackings.TerminalLogId, palletsTrackings.Count, serialNo);
                    "Pallet Tracking posted Successfully.".ToToast();
                    Synced = true;
                }
                else if (App.WarehouseService.Pallets.HandleConflictStatusPalletTracking())
                {
                    Synced = true;
                    //"Already posted Pallet Tracking.".ToToast();
                }
                else
                {
                    Synced = false;
                    "Some issue with pallet tracking Sync.".ToToast();
                }
                syncLog.IsPending = false;
                syncLog.Synced = Synced;
                await App.Database.SyncLog.UpdateSyncLogItem(syncLog);

            }
        }



        public async Task<PalletTrackingSyncCollection> PostPalletTracking(PalletTrackingSyncCollection palletTrackingSync)
        {
            if (CrossConnectivity.Current.IsConnected && await Util.Util.IsConnected())
            {
                return await App.WarehouseService.Pallets.PostPalletTrackingAsync(palletTrackingSync);
            }
            else
            {
                return null;
            }
        }

        public async Task SyncPallets()
        {
            await SyncPalletMethods();
            await SyncPalletProducts();
            if (!CrossConnectivity.Current.IsConnected || !await Util.Util.IsConnected())
            {
                if (!CrossConnectivity.Current.IsConnected)
                    AppStrings.NoInternet.ToToast();
                return;
            }
            if (isSyncingPallets)
            {
                "Already syncing pallets".ToToast();
                return;
            }
            isSyncingPallets = true;

            DateTime date;
            SyncLog synclog = await App.Database.SyncLog.GetSyncLogByTableName(Database.DatabaseConfig.Tables.Pallets.ToString());

            if (synclog != null && synclog.RequestedTime != DateTime.MinValue)
                date = synclog.RequestedTime;
            else
                date = ModulesConfig.SyncDate;

            string serialNo = ModulesConfig.SerialNo;

            if (synclog == null)
                synclog = await InitializeSyncLog(Database.DatabaseConfig.Tables.Pallets.ToString(), WebServiceConfig.SyncPallets);

            //Update Sync Log before sending request
            synclog.SerialNo = serialNo;
            synclog.RequestUrl = WebServiceConfig.SyncPallets;
            synclog.RequestedTime = DateTime.UtcNow;

            //Service Call
            PalletsSyncCollection pallets = await App.WarehouseService.Pallets.GetPalletsAsync(date, serialNo);
            if (pallets != null)
            {
                await AddPalletsToDatabase(pallets);
            }
            else
            {
                Constants.ApiErrorMsg.ToToast();
            }

            //acknowledgement service call
            if (pallets != null)
            {
                HttpResponseMessage ackResponse = await App.WarehouseService.Acknowledgement.VerifyAckAsync(pallets.TerminalLogId.ToString(), pallets.Pallets.Count, serialNo);

                synclog.ResponseTime = DateTime.UtcNow;
                synclog.Synced = false;
                synclog.ErrorCode = (int)ackResponse.StatusCode;

                if ((int)ackResponse.StatusCode == 200)
                {
                    synclog.TerminalLogId = pallets.TerminalLogId.ToString();
                    synclog.LastSynced = DateTime.UtcNow;
                    synclog.Synced = true;
                    synclog.ResultCount = pallets.Count;
                    synclog.ErrorCode = 0;
                    await App.Database.SyncLog.UpdateSyncLogItem(synclog);
                }
            }
            isSyncingPallets = false;
            await App.Vehicles.SyncVehicles();
        }

        public async Task SyncPalletProducts()
        {
            if (!CrossConnectivity.Current.IsConnected || !await Util.Util.IsConnected())
            {
                if (!CrossConnectivity.Current.IsConnected)
                    AppStrings.NoInternet.ToToast();
                return;
            }
            if (isSyncingPalletsProducts)
            {
                "Already syncing pallet products".ToToast();
                return;
            }
            isSyncingPalletsProducts = true;

            DateTime date;
            SyncLog synclog = await App.Database.SyncLog.GetSyncLogByTableName(Database.DatabaseConfig.Tables.PalletProducts.ToString());

            if (synclog != null && synclog.RequestedTime != DateTime.MinValue)
                date = synclog.RequestedTime;
            else
                date = ModulesConfig.SyncDate;

            string serialNo = ModulesConfig.SerialNo;

            if (synclog == null)
                synclog = await InitializeSyncLog(Database.DatabaseConfig.Tables.PalletProducts.ToString(), WebServiceConfig.SyncPalletProducts);

            //Update Sync Log before sending request
            synclog.SerialNo = serialNo;
            synclog.RequestUrl = WebServiceConfig.SyncPalletProducts;
            synclog.RequestedTime = DateTime.UtcNow;

            //Service Call
            PalletProductsSyncCollection palletProducts = await App.WarehouseService.Pallets.GetPalletProductsAsync(date, serialNo);
            if (palletProducts != null)
            {
                await AddPalletProductsToDatabase(palletProducts);
            }
            else
            {
                Constants.ApiErrorMsg.ToToast();
            }
            //acknowledgement service call
            if (palletProducts != null)
            {
                HttpResponseMessage ackResponse = await App.WarehouseService.Acknowledgement.VerifyAckAsync(palletProducts.TerminalLogId.ToString(), palletProducts.PalletProducts.Count, serialNo);

                synclog.ResponseTime = DateTime.UtcNow;
                synclog.Synced = false;
                synclog.ErrorCode = (int)ackResponse.StatusCode;

                if ((int)ackResponse.StatusCode == 200)
                {
                    synclog.TerminalLogId = palletProducts.TerminalLogId.ToString();
                    synclog.LastSynced = DateTime.UtcNow;
                    synclog.Synced = true;
                    synclog.ResultCount = palletProducts.Count;
                    synclog.ErrorCode = 0;
                    await App.Database.SyncLog.UpdateSyncLogItem(synclog);
                }
            }
            isSyncingPalletsProducts = false;
        }

        public async Task SyncPalletMethods ()
        {
            if (!CrossConnectivity.Current.IsConnected || !await Util.Util.IsConnected())
            {
                if (!CrossConnectivity.Current.IsConnected)
                    AppStrings.NoInternet.ToToast();
                return;
            }
            if (isSyncingPalletsMethods)
            {
                "Already syncing pallets Methods".ToToast();
                return;
            }
            isSyncingPalletsMethods = true;

            DateTime date;
            SyncLog synclog = await App.Database.SyncLog.GetSyncLogByTableName(Database.DatabaseConfig.Tables.PalletMethods.ToString());

            if (synclog != null && synclog.RequestedTime != DateTime.MinValue)
                date = synclog.RequestedTime;
            else
                date = ModulesConfig.SyncDate;
            
            string serialNo = ModulesConfig.SerialNo;

            if (synclog == null)
                synclog = await InitializeSyncLog(Database.DatabaseConfig.Tables.PalletMethods.ToString(), WebServiceConfig.SyncPalletDispatchMethods);

            //Update Sync Log before sending request
            synclog.SerialNo = serialNo;
            synclog.RequestUrl = WebServiceConfig.SyncPalletDispatchMethods;
            synclog.RequestedTime = DateTime.UtcNow;

            //Service Call

            PalletDispatchMethodSyncCollection PalletMethods = await App.WarehouseService.Pallets.GetPalletDispatchMethodsAsync(date, serialNo);

            if (PalletMethods != null)
            {
                await AddPalletDispatchMethodsToDatabase(PalletMethods);
                HttpResponseMessage ackResponse = await App.WarehouseService.Acknowledgement.VerifyAckAsync(PalletMethods.TerminalLogId.ToString(), PalletMethods.PalletDispatchMethods.Count, serialNo);

                synclog.ResponseTime = DateTime.UtcNow;
                synclog.Synced = false;
                synclog.ErrorCode = (int)ackResponse.StatusCode;

                if ((int)ackResponse.StatusCode == 200)
                {
                    synclog.TerminalLogId = PalletMethods.TerminalLogId.ToString();
                    synclog.LastSynced = DateTime.UtcNow;
                    synclog.Synced = true;
                    synclog.ResultCount = PalletMethods.Count;
                    synclog.ErrorCode = 0;
                    await App.Database.SyncLog.UpdateSyncLogItem(synclog);
                }
            }
            else
            {
                Constants.ApiErrorMsg.ToToast();
            }
            isSyncingPalletsMethods = false;
        }

        public async Task SyncPalletTracking(bool isSyncModule = true)
        {

            if (!CrossConnectivity.Current.IsConnected || !await Util.Util.IsConnected())
            {
                if (!CrossConnectivity.Current.IsConnected)
                    AppStrings.NoInternet.ToToast();
                return;
            }

            if (isSyncingPalletTracking)
            {
                "Already syncing pallet tracking".ToToast();
                return;
            }
            isSyncingPalletTracking = true;

            await CheckOrPostPalletTracking();

            DateTime date;
            SyncLog synclog = await App.Database.SyncLog.GetSyncLogByTableName(Database.DatabaseConfig.Tables.PalletTracking.ToString());

            if (synclog != null && synclog.RequestedTime != DateTime.MinValue)
                date = synclog.RequestedTime;
            else
                date = ModulesConfig.SyncDate;

            string serialNo = ModulesConfig.SerialNo;

            if (synclog == null)
                synclog = await InitializeSyncLog(Database.DatabaseConfig.Tables.PalletTracking.ToString(), WebServiceConfig.GetPalletTracking);

            //Update Sync Log before sending request
            synclog.SerialNo = serialNo;
            synclog.RequestUrl = WebServiceConfig.GetPalletTracking;
            synclog.RequestedTime = DateTime.UtcNow;
          
            //Service Call
            var pallets = await App.WarehouseService.Pallets.GetPalletTrackingAsync(date, serialNo);

            if (pallets != null)
            {
                await AddPalletTrackingToDatabase(pallets);
            }
            else
            {
                Constants.ApiErrorMsg.ToToast();
                isSyncingPalletTracking = false;
                return;
            }

            //acknowledgement service call
            HttpResponseMessage ackResponse = await App.WarehouseService.Acknowledgement.VerifyAckAsync(pallets.TerminalLogId.ToString(), pallets.PalletTrackingSync.Count, serialNo);

            synclog.ResponseTime = DateTime.UtcNow;
            synclog.Synced = false;
            synclog.ErrorCode = (int)ackResponse.StatusCode;

            if ((int)ackResponse.StatusCode == 200)
            {
                synclog.TerminalLogId = pallets.TerminalLogId.ToString();
                synclog.LastSynced = DateTime.UtcNow;
                synclog.Synced = true;
                synclog.ResultCount = pallets.Count;
                synclog.ErrorCode = 0;
               
                await App.Database.SyncLog.UpdateSyncLogItem(synclog);

            }
            isSyncingPalletTracking = false;

            if (isSyncModule)
            {
                await App.Database.Pallets.DeletePalletTrackingsWithArchivedStatus();
            }
        }

        private async Task<SyncLog>InitializeSyncLog(string tableName, string requestURL)
        {
            SyncLog synclog = new SyncLog();
            synclog.TableName = tableName;
            synclog.RequestUrl = requestURL;
            await App.Database.SyncLog.AddSyncLogItem(synclog);
            return synclog;
        }

        private async Task AddPalletsToDatabase(PalletsSyncCollection pallets)
        {
            if (pallets != null && pallets.Pallets != null)
                await App.Database.Pallets.AddUpdatePallets(pallets.Pallets);
        }

        public async Task AddPalletTrackingToDatabase(PalletTrackingSyncCollection pallets)
        {
            if (pallets != null && pallets.PalletTrackingSync != null)
                await App.Database.Pallets.AddUpdatePalletTracking(pallets.PalletTrackingSync);
        }

        private async Task AddPalletProductsToDatabase(PalletProductsSyncCollection pallets)
        {
            if (pallets != null && pallets.PalletProducts != null)
                await App.Database.Pallets.AddUpdatePalletProducts(pallets.PalletProducts);
        }

        private async Task AddPalletDispatchMethodsToDatabase(PalletDispatchMethodSyncCollection palletMethods)
        {
            if (palletMethods != null && palletMethods.PalletDispatchMethods != null)
                await App.Database.Pallets.AddUpdatePalletDispatchMethods(palletMethods.PalletDispatchMethods);
        }

        public async Task<List<PalletSync>> GetPallets()
        {
            return (await App.Database.Pallets.GetAllPallets()).OrderByDescending((obj)=>obj.DateCreated).ToList();
        }


        public async Task<List<PalletSync>> GetPalletsByOrderId(int orderId)
        {
            var pallets = (await App.Database.Pallets.GetAllPalletsByOrderId(orderId)).OrderByDescending((obj) => obj.PalletNumber).ToList();
            return pallets;
        }


        public async Task<List<PalletProductsSync>> GetPalletProducts()
        {
            return await App.Database.Pallets.GetAllPalletProducts();
        }

        public async Task<List<PalletTrackingSync>> GetPalletTracking()
        {
            return await App.Database.Pallets.GetAllPalletTrackings();
        }

        public async Task<List<PalletDispatchMethodSync>> GetPalletDispatchMethods()
        {
            return await App.Database.Pallets.GetAllPalletDispatchMethods();
        }

        public async Task<PalletCreateResponse> AddPallet(int accountID)
        {
            return await App.WarehouseService.Pallets.AddPallet(accountID);
        }

        public async Task AddPalletInDB(PalletSync pallet)
        {
            pallet.PalletDispatchInfo = null;
            List<PalletSync> pallets = new List<PalletSync>();
            pallets.Add(pallet);
            await App.Database.Pallets.AddUpdatePallets(pallets);
        }
        public async Task<List<PalletProducts>> GetPalletProductsWithPalletId(int PalletId, DateTime PalletCreatedDate)
        {
            List<PalletProducts> PalletProductsFound = new List<PalletProducts>();
            var AllPalletProducts = await App.Database.Pallets.GetAllPalletProducts();
            var AllProducts = await App.Database.Products.GetAllProducts();
            var Products = AllPalletProducts.FindAll((obj) => obj.CurrentPalletID == PalletId);
            foreach(var product in Products)
            {
                var palletProduct = new PalletProducts();
                palletProduct.PalletProduct = product;
                var ProductFound = AllProducts.Find((obj)=>obj.ProductId == product.ProductID);
                if(ProductFound != null)
                {
                    palletProduct.ProductName = ProductFound.Name;
                }
                palletProduct.CreatedDate = PalletCreatedDate;
                PalletProductsFound.Add(palletProduct);
            }
            return PalletProductsFound;
        }
        public async Task<PalletProductsSyncCollection> AddPalletItem(PalletSync pallet, OrderDetailsProduct orderProduct, double qty, OrderProcessSync orderProcess)
        {
            var PalletProductsSyncCollection = new PalletProductsSyncCollection();
            var PalletProducts = new List<PalletProductsSync>();
            var palletProduct = new PalletProductsSync();
            palletProduct.AccountID = pallet.RecipientAccountID;
            palletProduct.ProductID = orderProduct.Product.ProductId;
            palletProduct.CurrentPalletID = pallet.PalletID;
            palletProduct.OrderID = orderProduct.OrderDetails.OrderID;
           
            palletProduct.PalletQuantity = qty;
            PalletProducts.Add(palletProduct);
            PalletProductsSyncCollection.PalletProducts = PalletProducts;
            PalletProductsSyncCollection.SerialNo = ModulesConfig.SerialNo;
            PalletProductsSyncCollection.TerminalLogId = new Guid();
            PalletProductsSyncCollection.TransactionLogId = Guid.NewGuid();
            palletProduct.CurrentPalletID = pallet.PalletID;
            palletProduct.CreatedBy = App.Users.LoggedInUserId;
            palletProduct.DateCreated = DateTime.UtcNow;

            var orderProcessDetail = orderProcess.OrderProcessDetails.FirstOrDefault(x => x.OrderDetailID == (int)orderProduct.OrderDetails.OrderDetailID);
            if (orderProcessDetail != null)
                palletProduct.OrderProcessDetailID = orderProcessDetail.OrderProcessDetailID;
            //else
                //palletProduct.OrderProcessDetailID = 0;

            var palletProducts = await App.WarehouseService.Pallets.AddPalletItem(PalletProductsSyncCollection, ModulesConfig.SerialNo);
            if (palletProducts != null)
            {
                await AddPalletProductsToDatabase(palletProducts);
            }
            return palletProducts;
        }

        public async Task<List<PalletSync>> DispatchPallet(List<PalletSync> pallets)
        {
            foreach (var pallet in pallets)
            {
                pallet.DateCompleted = DateTime.Now;
                pallet.SerialNumber = ModulesConfig.SerialNo;
                pallet.CreatedBy = pallet.RecipientAccount.AccountID;
            }
            var dispatchPallet = await App.WarehouseService.Pallets.DispatchPallet(pallets[0], ModulesConfig.SerialNo, pallets[0].PalletID);
            if (dispatchPallet != null)
            {
                foreach (var pallet in pallets)
                {
                    pallet.IsDispatched = true;
                    pallet.PalletDispatchInfo.PalletsDispatchID = dispatchPallet.PalletDispatchInfo.PalletsDispatchID;
                    pallet.PalletsDispatchID = dispatchPallet.PalletDispatchInfo.PalletsDispatchID;
                }
            }
            return pallets;
        }

        public async Task<PalletTrackingSync> GetPalletTrackingBySerial(string palletSerial, int productId)
        {
            return await App.Database.Pallets.GetPalletTrackingBySerial(palletSerial, productId);
        }


        public async Task<PalletTrackingSync> GetPalletTrackingForGoodsOut(int productId, List<PalleTrackingProcess> PalletTrackingProcesses, List<PalleTrackingProcess> PalletTrackingProcessesForSameProduct, List<PalletTrackingSync> SkippedPallets, PalletTrackingSchemeEnum scheme)
        {
            return await App.Database.Pallets.GetPalletTrackingForGoodsOut(productId, PalletTrackingProcesses, PalletTrackingProcessesForSameProduct, SkippedPallets, scheme);
        }

        public async Task<PalletTrackingSync> GetPalletTrackingForGoodsOutWithSerial(int productId, List<PalleTrackingProcess> PalletTrackingProcesses, List<PalleTrackingProcess> PalletTrackingProcessesForAll, List<PalletTrackingSync> SkippedPallets, PalletTrackingSchemeEnum trackingScheme, string serial)
        {
            return await App.Database.Pallets.GetPalletTrackingForGoodsOutWithSerial(productId, PalletTrackingProcesses, PalletTrackingProcessesForAll, SkippedPallets, trackingScheme, serial);
        }

        public async Task<PalletTrackingSync> GetPalletTrackingWithTrackingId(int trackingId)
        {
            return await App.Database.Pallets.GetAllPalletTrackingsWithTrackingId(trackingId);
        }

        public async Task<PalletTrackingSync> GetPalletTrackingWithSerial(string serial)
        {
            return await App.Database.Pallets.GetAllPalletTrackingsWithSerial(serial);
        }



        public async Task<List<PalletDispatchSync>> GetAllPalletDispatches()
        {
            var palletDispatches = await App.Database.PalletDispatch.GetAllPalletDispatches();

            //foreach (var palletDispatch in palletDispatches)
            //{
                // Implemented in Table Method
                //var orderProcess = await App.Database.OrderProcesses.GetOrderProcessByOrderProcessId(palletDispatch.OrderProcessID);
                //if (orderProcess != null)
                //{
                //    palletDispatch.OrderProcess = orderProcess;
                //    var order = await App.Database.Orders.GetOrderById(orderProcess.OrderID ?? 0);
                //    if (order != null)
                //    {
                //        palletDispatch.Order = order;
                //    }

                //}

                // Implemented in Table Method

                //if (palletDispatch.MarketVehicleID != null && palletDispatch.MarketVehicleID != 0)
                //{
                //    var marketVehicle = await App.Database.Vehicle.GetVehicleById((int)palletDispatch.MarketVehicleID);
                //    if (marketVehicle != null)
                //    {
                //        palletDispatch.MarketVehicle = marketVehicle;
                //    }
                //}
            //}
            //return palletDispatches.FindAll(x => x.MarketVehicleID.Equals(terminal.MarketVehicleId));
            return palletDispatches;
        }


        public async Task SyncPalletDispatches()
        {
            if (!CrossConnectivity.Current.IsConnected || !await Util.Util.IsConnected())
            {
                if (!CrossConnectivity.Current.IsConnected)
                    AppStrings.NoInternet.ToToast();
                return;
            }
            if (isSyncingPalletDispatches)
            {
                "Already syncing pallet dispatches".ToToast();
                return;
            }
            isSyncingPalletDispatches = true;

            DateTime date;
            SyncLog synclog = await App.Database.SyncLog.GetSyncLogByTableName(Database.DatabaseConfig.Tables.PalletDispatch.ToString());

            if (synclog != null && synclog.RequestedTime != DateTime.MinValue)
                date = synclog.RequestedTime;
            else
                date = ModulesConfig.SyncDate;

            string serialNo = ModulesConfig.SerialNo;

            if (synclog == null)
                synclog = await InitializeSyncLog(Database.DatabaseConfig.Tables.PalletDispatch.ToString(), WebServiceConfig.SyncPalletDispatches);

            //Update Sync Log before sending request
            synclog.SerialNo = serialNo;
            synclog.RequestUrl = WebServiceConfig.SyncPalletDispatches;
            synclog.RequestedTime = DateTime.UtcNow;

            //Service Call
            PalletsDispatchSyncCollection palletsDispatch = await App.WarehouseService.Pallets.GetPalletsDispatchesAsync(date, serialNo);
            if (palletsDispatch == null)
            {
                Constants.ApiErrorMsg.ToToast();
                isSyncingPalletDispatches = false;
                return;
            }
            isSyncingPalletDispatches = false;
            //Adding fetched data to database
            await AddPalletDispatchToDatabase(palletsDispatch);

            //acknowledgement service call
            HttpResponseMessage ackResponse = await App.WarehouseService.Acknowledgement.VerifyAckAsync(palletsDispatch.TerminalLogId.ToString(), palletsDispatch.Count, serialNo);

            synclog.ResponseTime = DateTime.UtcNow;
            synclog.Synced = false;
            synclog.ErrorCode = (int)ackResponse.StatusCode;

            if ((int)ackResponse.StatusCode == 200)
            {
                synclog.TerminalLogId = palletsDispatch.TerminalLogId.ToString();
                synclog.LastSynced = DateTime.UtcNow;
                synclog.Synced = true;
                synclog.ResultCount = palletsDispatch.Count;
                synclog.ErrorCode = 0;
                await App.Database.SyncLog.UpdateSyncLogItem(synclog);
            }
        }

        public async Task<PalletDispatchProgress> UpdatePalletDispatchProgress(PalletDispatchProgress palletDispatchProgress)
        {
            if (palletDispatchProgress != null)
            {
                var palletDispatchProgressResponse = await App.WarehouseService.Pallets.PostPalletsDispatchesAsync(palletDispatchProgress);
                if (App.WarehouseService.Pallets.HandleConflictStatusPostDispatchPallet())
                {
                    return palletDispatchProgressResponse;
                }
                if (palletDispatchProgressResponse != null)
                {
                    await AddPalletDispatchProgressToDatabase(palletDispatchProgressResponse);
                    return palletDispatchProgressResponse;
                }
                else
                {
                    await AddPalletDispatchProgressToDatabase(palletDispatchProgress);
                    return palletDispatchProgress;
                }
            }
            return null;
        }




        private async Task AddPalletDispatchProgressToDatabase(PalletDispatchProgress palletsDispatchProgress)
        {
            if (palletsDispatchProgress != null)
                await App.Database.PalletDispatch.AddUpdatePalletDispatchProgress(palletsDispatchProgress);

        }

        private async Task AddPalletDispatchToDatabase(PalletsDispatchSyncCollection palletsDispatch)
        {
            if (palletsDispatch != null && palletsDispatch.PalletDispatchSync != null)
                await App.Database.PalletDispatch.AddUpdatePalletDispatch(palletsDispatch.PalletDispatchSync);

        }

        public async Task<List<PalletProductsSync>> GetPalletProductsByPalletId(int palletID)
        {
            var palletProducts = await App.Database.Pallets.GetAllPalletProductsByPalletId(palletID);
            return palletProducts;
        }
    }
}

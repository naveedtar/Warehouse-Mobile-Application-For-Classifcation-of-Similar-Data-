using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Connectivity;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Models.OrderProcesses;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Models.Sync;
using WarehouseHandheld.Resources;
using WarehouseHandheld.Services;
using Xamarin.Forms;

namespace WarehouseHandheld.Modules.OrderProcesses
{
    public class OrderProcessesModule : IOrderProcessesModule
    {
        public OrderProcessesModule()
        {
        }
        bool isSyncingOrderProcesses = false;
        bool isOrderProcess = false;
        List<int> IdsToDelete = new List<int>();
        public Action<bool> RefreshOrderItemsUi { get; set; }

        public async Task CheckOrPostOrders()
        {
            var AlllLogs = await App.Database.SyncLog.GetAll();
            var logsToPost = AlllLogs.FindAll((obj) => obj.TableName == Database.DatabaseConfig.Tables.OrderProcesses.ToString() && obj.IsPost && !obj.Synced);
            foreach (var syncLog in logsToPost)
            {
                syncLog.IsPending = true;
                await App.Database.SyncLog.UpdateSyncLogItem(syncLog);
                bool Synced = false;
                var orderToPost = JsonConvert.DeserializeObject<OrderProcessesSyncCollection>(syncLog.request);
                var orders = await PostOrderProcesses(orderToPost);
                if (orders != null && orders.Count > 0)
                {
                    //orderToPost.TerminalLogId = Guid.NewGuid();
                    //string serialNo = ModulesConfig.SerialNo;
                    //HttpResponseMessage ackResponse = await App.WarehouseService.Acknowledgement.VerifyAckAsync(orderToPost.TerminalLogId.ToString(),orderToPost.Count,serialNo);
                    //syncLog.ResponseTime = DateTime.UtcNow;
                    //syncLog.Synced = false;
                    //syncLog.ErrorCode = (int)ackResponse.StatusCode;

                    //if ((int)ackResponse.StatusCode == 200)
                    //{
                    //    syncLog.TerminalLogId = orderToPost.TerminalLogId.ToString();
                    //    syncLog.LastSynced = DateTime.UtcNow;
                    //    syncLog.Synced = true;
                    //    syncLog.ResultCount = orderToPost.Count;
                    //    syncLog.ErrorCode = 0;
                    //    await App.Database.SyncLog.UpdateSyncLogItem(syncLog);
                    //}

                    "Order Processed Successfully.".ToToast();
                    Synced = true;
                    IdsToDelete.Add(orderToPost.OrderProcesses[0].OrderProcessID);
                    var rejectedSerials = "";
                    foreach (var order in orders)
                    {
                        if (order.SerialProcessStatus != null && order.SerialProcessStatus.RejectedSerials != null)
                        {
                            foreach (var rejectedSerial in order.SerialProcessStatus.RejectedSerials)
                            {
                                rejectedSerials += " " + rejectedSerial + ",";
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(rejectedSerials))
                    {
                        rejectedSerials = rejectedSerials.Trim(',');
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await App.Current.MainPage.DisplayAlert("Rejected Serials", rejectedSerials, "OK");
                        });
                    }
                }
                else if (App.WarehouseService.OrderProcesses.HandleStatusConflict())
                {
                    Synced = true;
                    //"Order request already processed".ToToast();
                }
                else
                {
                    Synced = false;
                    "Some issue with order processing.".ToToast();
                }
                syncLog.IsPending = false;
                syncLog.Synced = Synced;
                await App.Database.SyncLog.UpdateSyncLogItem(syncLog);
                //await SyncOrderProcesses();
            }
        }


        public async Task SyncOrderProcesses()
        {
            if (!CrossConnectivity.Current.IsConnected || !await Util.Util.IsConnected())
            {
                if (!CrossConnectivity.Current.IsConnected)
                    AppStrings.NoInternet.ToToast();
                return;
            }

            if (isSyncingOrderProcesses)
            {
                "Already syncing order processes".ToToast();
                return;
            }
            isSyncingOrderProcesses = true;

            await CheckOrPostOrders();

            DateTime date;
            var tableName = Database.DatabaseConfig.Tables.OrderProcesses.ToString();
            var synclog = await App.Database.SyncLog.GetSyncLogByTableName(Database.DatabaseConfig.Tables.OrderProcesses.ToString());

            if (synclog != null && synclog.RequestedTime != DateTime.MinValue)
                date = synclog.RequestedTime;
            else
                date = ModulesConfig.SyncDate;

            string serialNo = ModulesConfig.SerialNo;
            if (synclog == null)
                synclog = await InitializeSyncLog();

            //Update Sync Log before sending request
            synclog.SerialNo = serialNo;
            synclog.RequestUrl = WebServiceConfig.SyncOrders;
            synclog.RequestedTime = DateTime.UtcNow;
            await App.Database.SyncLog.UpdateSyncLogItem(synclog);

            //Service Call
            OrderProcessesSyncCollection orderProcesses = await App.WarehouseService.OrderProcesses.GetOrderProcessesAsync(date, ModulesConfig.SerialNo);
            if (orderProcesses == null)
            {
                Constants.ApiErrorMsg.ToToast();
                isSyncingOrderProcesses = false;
                return;
            }
            //await AddNotSyncedOrders(orderProcesses);

            //Adding fetched data to database
            await AddOrderProcessesToDatabase(orderProcesses);

            foreach (var orderprocessId in IdsToDelete)
            {
                await App.Database.OrderProcesses.DeleteOrderProcessesById(orderprocessId);
            }
            IdsToDelete.Clear();
            //acknowledgement service call
            HttpResponseMessage ackResponse = await App.WarehouseService.Acknowledgement.VerifyAckAsync(orderProcesses.TerminalLogId.ToString(), orderProcesses.OrderProcesses.Count, ModulesConfig.SerialNo);

            string TerminalLogId = "";
            int ErrorCode = (int)ackResponse.StatusCode;
            bool IsSynced = false;
            if (ErrorCode == 200)
            {
                TerminalLogId = orderProcesses.TerminalLogId.ToString();
                IsSynced = true;
            }
            await App.Base.UpdateSyncLogAfterRequest(synclog, TerminalLogId, ErrorCode, IsSynced, orderProcesses.Count);
            isSyncingOrderProcesses = false;
            if (isOrderProcess)
            {
                if (orderProcesses.OrderProcesses.Any(x => x.OrderProcessDetails.Any(i => i.IsSerialised)))
                {
                    RefreshOrderItemsUi?.Invoke(true); // To Refresh OrderItemPage Ui
                }
                isOrderProcess = false;
            }

        }

        public async Task AddNotSyncedOrders(OrderProcessesSyncCollection orderProcesses)
        {
            var AlllLogs = await App.Database.SyncLog.GetAll();
            if (AlllLogs != null && AlllLogs.Count > 0)
            {
                var logsToPost = AlllLogs.FindAll((obj) => obj.TableName == Database.DatabaseConfig.Tables.OrderProcesses.ToString() && obj.IsPost && !obj.Synced);
                if (logsToPost != null && logsToPost.Count > 0)
                {
                    foreach (var syncLog in logsToPost)
                    {
                        var orderToPost = JsonConvert.DeserializeObject<OrderProcessesSyncCollection>(syncLog.request);
                        foreach (var order in orderToPost.OrderProcesses)
                        {
                            orderProcesses.OrderProcesses.Add(order);
                        }
                    }
                }

            }

        }


        public async Task AddOrderProcess(List<OrderProcessDetailSync> details, OrdersSync origOrder, bool IsCreate, string ShipmentReference, string ScanPickCodeContainer)
        {
            if (details != null && details.Count > 0)
            {
                OrderProcessesSyncCollection orderProcesses = new OrderProcessesSyncCollection();
                orderProcesses.SerialNo = ModulesConfig.SerialNo;
                orderProcesses.Count = 1;
                //Guid guid = Guid.Empty;
                //guid = Guid.Parse("{1700ed84-83cc-4386-af1b-9d55b0c73d06}");
                //orderProcesses.TransactionLogId = guid;
                orderProcesses.TransactionLogId = Guid.NewGuid();
                orderProcesses.OrderProcesses = new List<OrderProcessSync>();
                var terminalData = await App.Database.Vehicle.GetTerminalMetaData();

                var orderProcess = new OrderProcessSync()
                {
                    AccountID = origOrder.AccountID,
                    DeliveryNO = ShipmentReference,
                    OrderProcessDetails = details,
                    OrderID = origOrder.OrderID,
                    DateCreated = origOrder.DateCreated,
                    CreatedBy = origOrder.CreatedBy,
                    InventoryTransactionTypeId = origOrder.InventoryTransactionTypeId,
                    DateUpdated = DateTime.UtcNow,
                    UpdatedBy = App.Users.LoggedInUserId,
                    WarehouseId = ModulesConfig.WareHouseID,
                    PickContainerCode = ScanPickCodeContainer
                };
                if (terminalData != null)
                {
                    orderProcess.TenentId = terminalData.TenantId;
                }
                if (IsCreate)
                {
                    orderProcess.OrderToken = Guid.NewGuid();
                }
                orderProcesses.OrderProcesses.Add(orderProcess);

                // Add Order Process Method
                //await App.Database.OrderProcesses.AddOrderProcess(orderProcess);
                orderProcess.OrderProcessDetails[0].OrderProcessLocalId = orderProcess.OrderProcessID;
                var request = JsonConvert.SerializeObject(orderProcesses);


                var tableName = Database.DatabaseConfig.Tables.OrderProcesses.ToString();
                var synclog = await App.Base.AddSyncLogBeforeRequest(tableName, WebServiceConfig.SyncOrderProcesses, true, request);
                //await App.Database.OrderProcesses.AddOrderProcess(orderProcess);

                //bool Synced = false;
                isOrderProcess = true;
                if (CrossConnectivity.Current.IsConnected && await Util.Util.IsConnected())
                {
                    await Task.Factory.StartNew(async () =>
                    {
                        await App.Products.SyncProductSerials();
                        await App.OrderProcesses.SyncOrderProcesses();
                    });

                }
                else if (!CrossConnectivity.Current.IsConnected)
                {
                    "No Internet Connection".ToToast();
                }
            }
            else
                "No order process".ToToast();
        }
        private async Task<SyncLog> InitializeSyncLog()
        {
            SyncLog synclog = new SyncLog();
            synclog.TableName = Database.DatabaseConfig.Tables.OrderProcesses.ToString();
            synclog.RequestUrl = WebServiceConfig.SyncOrderProcesses;
            await App.Database.SyncLog.AddSyncLogItem(synclog);
            return synclog;
        }

        public async Task<List<OrderProcessSync>> GetCompletedSaleOrdersProcess(int inventoryTransactionTypeId)
        {
            var orderProcess = (await App.Database.OrderProcesses.GetAllOrderProcessesWithAccount(inventoryTransactionTypeId));
            return orderProcess;
        }

        private async Task AddOrderProcessesToDatabase(OrderProcessesSyncCollection orderProcesses)
        {
            if (orderProcesses != null && orderProcesses.Count > 0 && orderProcesses.OrderProcesses != null)
                await App.Database.OrderProcesses.AddUpdateOrderProcesses(orderProcesses.OrderProcesses);
        }

        public async Task<List<OrdersSync>> PostOrderProcesses(OrderProcessesSyncCollection orderProcessSync)
        {
            if (CrossConnectivity.Current.IsConnected && await Util.Util.IsConnected())
            {
                return await App.WarehouseService.OrderProcesses.PostOrderProcessesAsync(orderProcessSync);
            }
            else
            {
                return null;
            }
        }

        public void CreateUpdateOrderProcess(OrderAccount OrderAccount, List<OrderProcessDetailSync> OrderProcessDetails, OrderDetailsProduct item, List<PalleTrackingProcess> trackingProcesses, ObservableCollection<OrderDetailsProduct> orderDetailsProducts, decimal quantity = 1, string[] serials = null)
        {
            //setting up quantity for display
            if (!item.Product.Serialisable)
            {
                item.QuantityProcessed = item.QuantityProcessed + quantity;
                if (item.Product.ProductsPerCase != null)
                    item.BoxesRemaining = Math.Round(((item.OrderDetails.Qty - item.QuantityProcessed) / (decimal)item.Product.ProductsPerCase), 2);
            }
            var index = -1;

            if (!string.IsNullOrEmpty(item.LocationCode))
                index = OrderProcessDetails.FindIndex(x => x.OrderDetailID == item.OrderDetails.OrderDetailID && !string.IsNullOrEmpty(x.LocationCode) && x.LocationCode.Equals(item.LocationCode));
            else
                index = OrderProcessDetails.FindIndex(x => x.OrderDetailID == item.OrderDetails.OrderDetailID);

            if (index != -1)
            {
                if (!item.Product.Serialisable)
                {
                    OrderProcessDetails[index].QtyProcessed += quantity;

                    if (trackingProcesses != null)
                    {
                        if (OrderProcessDetails[index].PalleTrackingProcess == null)
                        {
                            OrderProcessDetails[index].PalleTrackingProcess = new List<PalleTrackingProcess>();
                        }
                    }
                }

                // Kit Products Items
                if (item.IsProductInKit)
                    AddProductKitVmToOrderProcessSync(OrderProcessDetails, OrderProcessDetails[index], item, orderDetailsProducts, quantity);
            }
            else
            {
                var orderProcessDetails = new OrderProcessDetailSync()
                {
                    OrderDetailID = item.OrderDetails.OrderDetailID,
                    ProductId = item.Product.ProductId,
                    TenentId = ModulesConfig.TenantID,
                    QtyProcessed = quantity,
                    CreatedBy = App.Users.LoggedInUserId,
                    InventoryTransactionTypeId = OrderAccount.Order.InventoryTransactionTypeId,
                    PalleTrackingProcess = trackingProcesses,
                    ExpiryDate = item.ExpiryDate,
                    BatchNumber = item.BatchNumber,
                    LocationCode = item.LocationCode,
                    ProductKitMapViewModel = new List<ProductKitMapViewModel>()
                };


                if (OrderAccount.Order.AccountID != null)
                {
                    orderProcessDetails.AccountID = OrderAccount.Order.AccountID.Value;
                }
                if (item.Product.Serialisable)
                {
                    orderProcessDetails.IsSerialised = true;
                    orderProcessDetails.Serials = serials;
                }

                // Kit Products Items
                if (item.IsProductInKit)
                    AddProductKitVmToOrderProcessSync(OrderProcessDetails, orderProcessDetails, item, orderDetailsProducts, quantity);

                OrderProcessDetails.Add(orderProcessDetails);
            }
        }


        private void AddProductKitVmToOrderProcessSync(List<OrderProcessDetailSync> OrderProcessDetails, OrderProcessDetailSync orderProcessDetailSync, OrderDetailsProduct item, ObservableCollection<OrderDetailsProduct> orderDetailsProducts, decimal quantity = 1)
        {
            orderProcessDetailSync.ProductId = item.KitOrderDetail.ProductId;
            orderProcessDetailSync.QtyProcessed = 0;

            var kitOrderDetailIndex = OrderProcessDetails.FindIndex(x => x.OrderDetailID.Equals(item.KitOrderDetail.OrderDetailID));
            var kitOrderDetailProductIndex = orderDetailsProducts.ToList().FindIndex(x => x.Product.ProductId.Equals(item.KitOrderDetail.ProductId));

            if (OrderProcessDetails != null && OrderProcessDetails.Any() && kitOrderDetailIndex >= 0 && OrderProcessDetails[kitOrderDetailIndex].ProductKitMapViewModel.Any(x => x.KitProductId == item.OrderDetails.ProductId))
            {
                OrderProcessDetails[kitOrderDetailIndex].ProductId = item.KitOrderDetail.ProductId;
                var index = OrderProcessDetails[kitOrderDetailIndex].ProductKitMapViewModel.FindIndex(x => x.KitProductId == item.OrderDetails.ProductId);

                OrderProcessDetails[kitOrderDetailIndex].ProductKitMapViewModel[index].Quantity += quantity;

                var orderDetailProducts = orderDetailsProducts.Where(x => x.KitOrderDetail != null && x.KitOrderDetail.ProductId.Equals(item.KitOrderDetail.ProductId) && x.IsProductInKit).ToList();
                if (IsBundleComplete(orderDetailProducts))
                {
                    //Update KitProductQuantity

                    // OrderDetailsSync Post Model
                    var totalkitSubProductQuantity = OrderProcessDetails[kitOrderDetailIndex].ProductKitMapViewModel.Where(x => x.ProductId.Equals(item.KitOrderDetail.ProductId)).Sum(x => x.Quantity);

                    // OrderDetailsProduct View Binded Model 
                    var totalSubProductOrderProductsQuantity = orderDetailProducts.Sum(x => x.QuantityProcessed);

                    // Total Kit Quantity for OrderDetailsProduct
                    var totalkitQuantity = orderDetailProducts.Sum(x => x.KitQuantity);

                    var kitProductQuantityOrderProducts = (int)(totalSubProductOrderProductsQuantity / totalkitQuantity);
                    var kitProductQuantityOrderProcessDetail = (int)(totalkitSubProductQuantity / totalkitQuantity);

                    orderDetailsProducts[kitOrderDetailProductIndex].QuantityProcessed = kitProductQuantityOrderProducts;
                    orderDetailsProducts[kitOrderDetailProductIndex].BoxesRemaining = Math.Round(((orderDetailsProducts[kitOrderDetailProductIndex].OrderDetails.Qty - kitProductQuantityOrderProducts) / (orderDetailsProducts[kitOrderDetailProductIndex].Product.ProductsPerCase ?? 1)), 2);
                    OrderProcessDetails[kitOrderDetailIndex].QtyProcessed = kitProductQuantityOrderProcessDetail;
                }
            }
            else
            {

                var productKit = new ProductKitMapViewModel
                {
                    KitProductId = item.Product.ProductId,
                    Quantity = quantity,
                    ProductId = item.KitOrderDetail.ProductId,
                    TenantId = ModulesConfig.TenantID,
                    ProductKitType = ProductKitTypeEnum.Kit,
                };
              
                orderProcessDetailSync.ProductKitMapViewModel.Add(productKit);

                if (OrderProcessDetails.Any() && kitOrderDetailIndex >= 0)
                {
                    var orderDetailProducts = orderDetailsProducts.Where(x => x.KitOrderDetail != null && x.KitOrderDetail.ProductId.Equals(item.KitOrderDetail.ProductId) && x.IsProductInKit).ToList();
                    if (IsBundleComplete(orderDetailProducts))
                    {
                        //Update KitProductQuantity

                        // OrderDetailsSync Post Model
                        var totalkitSubProductQuantity = OrderProcessDetails[kitOrderDetailIndex].ProductKitMapViewModel.Where(x => x.ProductId.Equals(item.KitOrderDetail.ProductId)).Sum(x => x.Quantity);

                        // OrderDetailsProduct View Binded Model 
                        var totalSubProductOrderProductsQuantity = orderDetailProducts.Sum(x => x.QuantityProcessed);

                        // Total Kit Quantity for OrderDetailsProduct
                        var totalkitQuantity = orderDetailProducts.Sum(x => x.KitQuantity);

                        var kitProductQuantityOrderProducts = (int)(totalSubProductOrderProductsQuantity / totalkitQuantity);
                        var kitProductQuantityOrderProcessDetail = (int)(totalkitSubProductQuantity / totalkitQuantity);

                        orderDetailsProducts[kitOrderDetailProductIndex].QuantityProcessed = kitProductQuantityOrderProducts;
                        orderDetailsProducts[kitOrderDetailProductIndex].BoxesRemaining = Math.Round(((orderDetailsProducts[kitOrderDetailProductIndex].OrderDetails.Qty - kitProductQuantityOrderProducts) / (orderDetailsProducts[kitOrderDetailProductIndex].Product.ProductsPerCase ?? 1)), 2);
                        OrderProcessDetails[kitOrderDetailIndex].QtyProcessed = kitProductQuantityOrderProcessDetail;

                        orderProcessDetailSync = OrderProcessDetails[kitOrderDetailIndex];
                    }
                }
            }
        }


        public bool IsBundleComplete(List<OrderDetailsProduct> orderDetailsProducts)
        {
            bool isComplete = false;
            if (orderDetailsProducts != null && orderDetailsProducts.Any(x => x.IsProductInKit))
            {
                foreach (var item in orderDetailsProducts)
                {
                    if (item.IsProductInKit)
                    {
                        var subOrderDetailProduct = orderDetailsProducts.FindAll(x => x.IsProductInKit);
                        foreach (var detailsProduct in subOrderDetailProduct)
                        {
                            if (detailsProduct.QuantityProcessed % detailsProduct.KitQuantity != 0)
                                return isComplete;
                        }
                        var kitSubProducts = orderDetailsProducts.FindAll(x => x.IsProductInKit && x.KitOrderDetail != null && item.KitOrderDetail != null && x.KitOrderDetail.OrderDetailID.Equals(item.KitOrderDetail.OrderDetailID));
                        if (kitSubProducts != null && kitSubProducts.Any())
                        {
                            var valueCheck = kitSubProducts.First().KitMeanQuantity;
                            isComplete = kitSubProducts.All(x => x.KitMeanQuantity.Equals(valueCheck)) ? true : false;
                            if (!isComplete)
                                return isComplete;
                        }
                    }
                }
                return isComplete;
            }
            return true;
        }



    }
}
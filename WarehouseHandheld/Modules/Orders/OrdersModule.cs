using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Models.Sync;
using WarehouseHandheld.Resources;
using WarehouseHandheld.Services;
using static WarehouseHandheld.Models.Orders.OrdersSync;
using WarehouseHandheld.Models.OrderProcesses;

namespace WarehouseHandheld.Modules.Orders
{
    public class OrdersModule : IOrdersModule
    {
        bool isSyncingOrders = false;

        public OrdersModule()
        {
        }

        public async Task SyncOrders()
        {
            if (!CrossConnectivity.Current.IsConnected || !await Util.Util.IsConnected())
            {
                if (!CrossConnectivity.Current.IsConnected)
                    AppStrings.NoInternet.ToToast();
                return;
            }
            if (isSyncingOrders)
            {
                "Already syncing orders".ToToast();
            }
            isSyncingOrders = true;
            DateTime date;
            SyncLog synclog = await App.Database.SyncLog.GetSyncLogByTableName(Database.DatabaseConfig.Tables.Orders.ToString());

            if (synclog != null && synclog.RequestedTime != DateTime.MinValue)
                date = synclog.RequestedTime;
            else
                date = ModulesConfig.SyncDate;

            string serialNo = ModulesConfig.SerialNo;

            if (synclog == null)
                synclog = InitializeSyncLog();

            //Update Sync Log before sending request
            synclog.SerialNo = serialNo;
            synclog.RequestUrl = WebServiceConfig.SyncOrders;
            synclog.RequestedTime = DateTime.UtcNow;

            //Service Call
            OrdersSyncCollection orders = await App.WarehouseService.Orders.GetOrdersAsync(date, serialNo);
            if (orders == null)
            {
                Constants.ApiErrorMsg.ToToast();
                isSyncingOrders = false;
                return;
            }

            //Adding fetched data to database
            await AddOrdersToDatabase(orders);

            //acknowledgement service call
            HttpResponseMessage ackResponse = await App.WarehouseService.Acknowledgement.VerifyAckAsync(orders.TerminalLogId.ToString(), orders.Orders.Count, serialNo);

            synclog.ResponseTime = DateTime.UtcNow;
            synclog.Synced = false;
            synclog.ErrorCode = (int)ackResponse.StatusCode;

            if ((int)ackResponse.StatusCode == 200)
            {
                synclog.TerminalLogId = orders.TerminalLogId.ToString();
                synclog.LastSynced = DateTime.UtcNow;
                synclog.Synced = true;
                synclog.ResultCount = orders.Count;
                synclog.ErrorCode = 0;
                await App.Database.SyncLog.UpdateSyncLogItem(synclog);
            }
            isSyncingOrders = false;

        }
        private SyncLog InitializeSyncLog()
        {
            SyncLog synclog = new SyncLog();
            synclog.TableName = Database.DatabaseConfig.Tables.Orders.ToString();
            synclog.RequestUrl = WebServiceConfig.SyncOrders;
            App.Database.SyncLog.AddSyncLogItem(synclog);
            return synclog;
        }

        private async Task AddOrdersToDatabase(OrdersSyncCollection orders)
        {
                if (orders != null && orders.Orders != null)
                await App.Database.Orders.AddUpdateOrders(orders.Orders);
        }

        public async Task<List<OrderAccount>> GetOrders(int inventoryTransactionTypeId)
        {
            var orders =
                await App.Database.Orders.GetAllOrdersWithAccount(inventoryTransactionTypeId, App.Users.LoggedInUserId);
            return orders.ToList();
        }


        //Not In Use
        public async Task<List<OrderAccount>> GetPurcahseOrdersWithinAccount(int inventoryTransactionTypeId,
            int AccountId)
        {
            return (await App.Database.Orders.GetAllOrdersWithAccount(inventoryTransactionTypeId,
                    App.Users.LoggedInUserId)).Where(x => x.Order.AccountID == AccountId)
                .OrderByDescending((arg) => arg.Order.DateCreated).ToList();
        }

        //Not In User
        public async Task<List<OrderAccount>> GetCompletedSaleOrders(int inventoryTransactionTypeId)
        {
            return (await App.Database.Orders.GetAllOrdersWithAccount(inventoryTransactionTypeId,
                    App.Users.LoggedInUserId)).Where(x => x.Order.OrderStatusID == (int) OrderStatusEnum.Complete)
                .OrderByDescending((arg) => arg.Order.DateCreated).ToList();
        }

        public async Task AddInventoryStockInOrder(List<OrderDetailsProduct> orders, int WarehouseId)
        {
            foreach (var order in orders)
            {
                order.InventoryStock =
                    await App.Database.InventoryStocks.GetInventoryStocksByWarehouseIdAndProductId(WarehouseId,
                        order.Product.ProductId);
            }
        }

        public async Task<List<OrderDetailsProduct>> GetOrderDetailsWithProduct(int orderId)
        {
            List<OrderProcessSync> orderProcessList = new List<OrderProcessSync>();
            orderProcessList = await App.Database.OrderProcesses.GetOrderProcessesFromSyncLog(orderId);


            var orderDetails = await App.Database.OrderDetails.GetOrderDetailsWithProductAndKit(orderId);
            foreach (var item in orderDetails)
            {
                var orderProcessDetails =
                    await App.Database.OrderProcessDetails.GetOrderProcessDetailByOrderDetailId(item.OrderDetails
                        .OrderDetailID);
                if (item.IsProductInKit)
                {
                    item.QuantityProcessed = orderProcessDetails.Sum(x => x.QtyProcessed) * item.KitQuantity;
                }
                else
                {
                    item.QuantityProcessed = orderProcessDetails.Sum(x => x.QtyProcessed);
                }

                // Checking for OrderProcessSync SyncLog processed quantity
                if (orderProcessList != null && orderProcessList.Count > 0)
                {
                    foreach (var orderProcess in orderProcessList)
                    {
                        foreach (var orderProcessDetail in orderProcess.OrderProcessDetails)
                        {
                            if (item.OrderDetails.OrderDetailID.Equals(orderProcessDetail.OrderDetailID))
                            {
                                if (item.IsProductInKit)
                                {
                                    item.QuantityProcessed += (orderProcessDetail.QtyProcessed * item.KitQuantity);
                                }
                                else
                                {
                                    item.QuantityProcessed += orderProcessDetail.QtyProcessed;
                                }
                            }
                        }
                    }
                }

                // To get quantity processed from db
                if (item.Product?.ProductsPerCase != null)
                    item.BoxesRemaining =
                        Math.Round(
                            ((item.OrderDetails.Qty - item.QuantityProcessed) / (decimal) item.Product.ProductsPerCase),
                            2);
            }

            return orderDetails;
        }

        public async Task<List<OrderDetailsProduct>> GetOrderDetailsForPallets(int orderId)
        {
            var orderDetails = await App.Database.OrderDetails.GetOrderDetailsWithProduct(orderId);
            var PalletProducts = await App.Database.Pallets.GetAllPalletProducts();
            foreach (var item in orderDetails)
            {
                var orderProcessDetails =
                    await App.Database.OrderProcessDetails.GetOrderProcessDetailByOrderDetailId(item.OrderDetails
                        .OrderDetailID);
                item.Quantity = orderProcessDetails.Sum(x => x.QtyProcessed);
                var Products = PalletProducts.FindAll((obj) =>
                    obj.OrderID == orderId && item.Product.ProductId == obj.ProductID);
                item.QuantityProcessed = Convert.ToDecimal(Products.Sum(x => x.PalletQuantity));
                if (item.Product.ProductsPerCase != null)
                    item.BoxesRemaining =
                        Math.Round(
                            ((item.OrderDetails.Qty - item.QuantityProcessed) / (decimal) item.Product.ProductsPerCase),
                            2);
            }

            return orderDetails;
        }

        public async Task<bool> UpdateOrderStatus(int orderId, OrderStatusEnum orderStatus)
        {
            var order = await App.WarehouseService.Orders.UpdateOrderStatusAsync(ModulesConfig.SerialNo, orderId,
                (int) orderStatus, App.Users.LoggedInUserId);
            if (order != null && order.OrderStatusID == (int) orderStatus)
            {
                await App.Database.Orders.AddUpdateOrder(order);
                return true;
            }

            return false;
        }

        public async Task<List<OrdersSync>> GetAllOrders()
        {
            return await App.Database.Orders.GetAllOrders();
        }

        public async Task<List<OrderAccount>> GetAllOrdersWithAccountForPickList()
        {
            var pickListOrders =
                (await App.Database.Orders.GetAllOrdersWithAccountForPickList(App.Users.LoggedInUserId));

            //SLA Priority
            var ordersBySla = pickListOrders.Where(x => (x.Order.SLAPriorityId != null && x.Order.SLAPriorityId > 0))
                .OrderBy((x => x.Order.SLAPriorityId)).ToList();
            var orderByOrderId = pickListOrders.Where(x => x.Order.SLAPriorityId == null || x.Order.SLAPriorityId == 0)
                .OrderBy(x => x.Order.OrderID).ToList();
            if (ordersBySla != null && ordersBySla.Any())
            {
                ordersBySla.AddRange(orderByOrderId);
                return ordersBySla;
            }
            else if (orderByOrderId != null && orderByOrderId.Any())
            {
                return orderByOrderId;
            }

            return pickListOrders;
        }

        private void GetProcessedQuantityForKitSubProducts(List<OrderDetailsProduct> orderDetailsProducts,
            List<OrderProcessSync> orderProcessList, List<OrderProcessDetailSync> orderProcessDetails)
        {
            foreach (var item in orderDetailsProducts)
            {
                if (item.IsProductInKit && item.KitOrderDetail != null)
                {
                    var quantityProcessed =
                        orderProcessDetails.Where(x => x.ProductId.Equals(item.KitOrderDetail.ProductId))
                            .Sum(x => x.QtyProcessed) * item.KitQuantity;
                    item.QuantityProcessed = quantityProcessed;
                }


                // Checking for OrderProcessSync SyncLog processed quantity
                if (orderProcessList != null && orderProcessList.Count > 0)
                {
                    foreach (var orderProcess in orderProcessList)
                    {
                        foreach (var orderProcessDetail in orderProcess.OrderProcessDetails)
                        {
                            if (item.OrderDetails.OrderDetailID.Equals(orderProcessDetail.OrderDetailID))
                            {
                                if (item.IsProductInKit)
                                {
                                    item.QuantityProcessed += orderProcessDetail.QtyProcessed * item.KitQuantity;
                                }
                            }
                        }
                    }
                }

                //// To get quantity processed from db
                //if (item.Product?.ProductsPerCase != null)
                //    item.BoxesRemaining = Math.Round(((item.OrderDetails.Qty - item.QuantityProcessed) / (decimal)item.Product.ProductsPerCase), 2);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using WarehouseHandheld.Database.DatabaseHandler;
using WarehouseHandheld.Models.OrderProcesses;
using System.Threading.Tasks;
using System.Linq;
using static WarehouseHandheld.Models.Orders.OrdersSync;
using WarehouseHandheld.Models.Sync;
using Newtonsoft.Json;
using WarehouseHandheld.Models.Enums;

namespace WarehouseHandheld.Database.OrderProcesses
{
    public class OrderProcessesTable : IOrderProcessesTable
    {
        public LocalDatabase Handler { get; private set; }
        public OrderProcessesTable(LocalDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("Database");
            this.Handler = database;
        }

        public async Task AddUpdateOrderProcesses(IList<OrderProcessSync> ordersProcessesSync)
        {
            foreach (var orderProcess in ordersProcessesSync)
            {
                var orderProcessItem = await GetOrderProcessById(orderProcess.OrderProcessID);
                if (orderProcessItem == null)
                {
                    if (orderProcess.IsDeleted == null || !(bool)orderProcess.IsDeleted)
                    {
                        var IsSave = await Handler.Database.InsertAsync(orderProcess);
                        if (orderProcess.OrderProcessDetails != null && orderProcess.OrderProcessDetails.Count > 0)
                            await Handler.OrderProcessDetails.AddUpdateOrderProcessesDetail(orderProcess.OrderProcessDetails);
                    }
                }
                else
                {
                    if (orderProcess.IsDeleted != null && (bool)orderProcess.IsDeleted)
                    {
                        await DeleteOrderProcess(orderProcessItem);
                    }
                    else
                    {

                        var IsSave = await Handler.Database.UpdateAsync(orderProcess);
                        if (orderProcess.OrderProcessDetails != null && orderProcess.OrderProcessDetails.Count > 0)
                            await Handler.OrderProcessDetails.AddUpdateOrderProcessesDetail(orderProcess.OrderProcessDetails);
                    }
                }


            }
        }

        public async Task DeleteOrderProcess(OrderProcessSync orderProcessSync)
        {
            var orderDetails = await Handler.OrderProcessDetails.GetOrderProcessDetailByOrderProcessId(orderProcessSync.OrderProcessID);
            if (orderDetails != null && orderDetails.Count > 0)
            {
                foreach (var orderDetail in orderDetails)
                {
                    await Handler.OrderProcessDetails.DeleteOrderProcessDetail(orderDetail);
                }
            }
            await Handler.Database.DeleteAsync(orderProcessSync);
        }


        //Adding Order Process Details in its table

        public async Task<int> AddOrderProcess(OrderProcessSync order)
        {
            if (order == null)
                return 0;
            var IsSave = await Handler.Database.InsertAsync(order);
            if (order.OrderProcessDetails != null)
            {
                foreach (var orderdetail in order.OrderProcessDetails)
                {
                    orderdetail.OrderProcessLocalId = order.OrderProcessID;
                }
                await Handler.OrderProcessDetails.AddUpdateOrderProcessesDetail(order.OrderProcessDetails);
            }
            return IsSave;
        }

        public async Task DeleteAllOrderProcesses()
        {
            await Handler.Database.DropTableAsync<OrderProcessDetailSync>();
            await Handler.Database.CreateTableAsync<OrderProcessDetailSync>();
            await Handler.Database.DropTableAsync<OrderProcessSync>();
            await Handler.Database.CreateTableAsync<OrderProcessSync>();
        }

        public async Task DeleteOrderProcessesById(int id)
        {
            if (id != 0)
            {
                var orderProcessItem = await GetOrderProcessById(id);
                if (orderProcessItem != null)
                {
                    var orderDetails = await Handler.Database.Table<OrderProcessDetailSync>().Where((obj) => obj.OrderProcessLocalId == id).ToListAsync();
                    if (orderDetails != null)
                    {
                        foreach (var orderdetail in orderDetails)
                        {
                            await Handler.Database.DeleteAsync(orderdetail);
                        }
                    }
                    await Handler.Database.DeleteAsync(orderProcessItem);
                }
            }

        }


        private async Task<OrderProcessSync> GetOrderProcessById(int OrderProcessId)
        {
            if (OrderProcessId != 0)
            {
                return await Handler.Database.Table<OrderProcessSync>().Where(x => x.OrderProcessID == OrderProcessId).FirstOrDefaultAsync();
            }
            return null;
        }

        public async Task<List<OrderProcessSync>> GetAllOrderProcessesWithAccount(int inventoryTransactionTypeId)
        {
            var orderprocesses = await Handler.Database.Table<OrderProcessSync>().Where(x => x.InventoryTransactionTypeId.Equals(inventoryTransactionTypeId) && x.OrderProcessStatusId == (int)OrderProcessStatusEnum.Complete
            && (x.DateUpdated >= DbConstants.StartDate || x.DateCreated >= DbConstants.StartDate)).OrderByDescending(x => x.OrderProcessID).Take(DbConstants.TakeLimitValue).ToListAsync();
            foreach (var orderprocess in orderprocesses)
            {
                if (orderprocess.AccountID != null && orderprocess.AccountID != 0)
                {
                    var account = await Handler.Accounts.GetAccountById((int)orderprocess.AccountID);
                    if (account != null)
                    {
                        orderprocess.Account = account;
                    }
                }

                if (orderprocess.OrderID != null && orderprocess.OrderID != 0)
                {
                    var order = await Handler.Orders.GetOrderById((int)orderprocess.OrderID);
                    if (order != null)
                    {
                        orderprocess.Order = order;
                    }
                }
                if (orderprocess.OrderProcessID != 0)
                {
                    var orderProcessDetails = await Handler.OrderProcessDetails.GetOrderProcessDetailByOrderProcessId(orderprocess.OrderProcessID);
                    if (orderProcessDetails != null)
                    {
                        orderprocess.OrderProcessDetails = new List<OrderProcessDetailSync>();
                        orderprocess.OrderProcessDetails = orderProcessDetails;
                    }
                }

            }


            //var joined =
            //from order in orderprocesses
            //join account in accounts on order.AccountID equals account.AccountID into ps
            //from account in ps.DefaultIfEmpty()
            //select new OrderProcessAccount() { Order = order, Account = account };
            return orderprocesses;
        }


        public async Task<OrderProcessSync> GetOrderProcessByOrderProcessId(int? OrderProcessId)
        {
            if (OrderProcessId != null || OrderProcessId != 0)
            {
                var orderProcess = await Handler.Database.Table<OrderProcessSync>().Where(x => x.OrderProcessID == OrderProcessId).FirstOrDefaultAsync();
                return orderProcess;
            }
            return null;
        }


        public async Task<List<OrderProcessSync>> GetOrderProcessesFromSyncLog(int orderId)
        {
            var orderProcessSyncCollectionList = new List<OrderProcessesSyncCollection>();
            var orderProcessSyncList = new List<OrderProcessSync>();

            if (orderId != 0)
            {
                var tableName = Database.DatabaseConfig.Tables.OrderProcesses.ToString();

                var synclogData = await Handler.Database.Table<SyncLog>().Where(x => x.TableName.Equals(tableName) && x.IsPost.Equals(true) && !x.Synced).OrderByDescending(x => x.Id).ToListAsync();
                if (synclogData != null)
                {

                    foreach (var syncLog in synclogData)
                    {
                        var orderProcessSyncCollection = JsonConvert.DeserializeObject<OrderProcessesSyncCollection>(syncLog.request);
                        if (orderProcessSyncCollection != null)
                        {
                            orderProcessSyncCollectionList.Add(orderProcessSyncCollection);
                        }
                    }

                    foreach (var orderProcessesSyncCollection in orderProcessSyncCollectionList)
                    {
                        foreach (var orderProcess in orderProcessesSyncCollection.OrderProcesses)
                        {
                            if (orderProcess.OrderID.Equals(orderId))
                            {
                                orderProcessSyncList.Add(orderProcess);
                            }
                        }
                    }

                    return orderProcessSyncList;
                }

            }
            return null;
        }
    }
}

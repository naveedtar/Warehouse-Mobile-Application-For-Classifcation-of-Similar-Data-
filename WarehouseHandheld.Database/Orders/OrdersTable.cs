using System;
using System.Collections.Generic;
using WarehouseHandheld.Database.DatabaseHandler;
using WarehouseHandheld.Models.Orders;
using System.Linq;
using WarehouseHandheld.Models.Accounts;
using System.Threading.Tasks;
using static WarehouseHandheld.Models.Orders.OrdersSync;
using System.Diagnostics;

namespace WarehouseHandheld.Database.Orders
{
    public class OrdersTable : IOrdersTable
    {
        public LocalDatabase Handler { get; private set; }
        public OrdersTable(LocalDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("Database");
            this.Handler = database;
        }


        public async Task AddUpdateOrders(IList<OrdersSync> ordersSync)
        {
            foreach (var order in ordersSync)
            {
                await AddUpdateOrder(order);
            }
            //todo:Need to improve this logic
            //var orders = await GetAllOrders();
            //foreach(var item in orders)
            //{
            //    if (ordersSync.FirstOrDefault(x=>x.OrderID == item.OrderID) == null)
            //        await Handler.Database.DeleteAsync(item);
            //}
        }

        public async Task DeleteOrder(OrdersSync order)
        {
            var orderInDb = await GetOrderById(order.OrderID);
            var orderDetails = await Handler.OrderDetails.GetOrderDetailsByOrderId(orderInDb.OrderID);
            foreach (var orderDetail in orderDetails)
            {
                await Handler.Database.DeleteAsync(orderDetail);
            }
            await Handler.Database.DeleteAsync(orderInDb);
        }

        public async Task AddUpdateOrder(OrdersSync order)
        {
            var orderItem = await GetOrderById(order.OrderID);
            if (orderItem == null)
            {
                if (order.IsDeleted == null || !(bool)order.IsDeleted)
                    await Handler.Database.InsertAsync(order);
            }
            else
                if (order.IsDeleted != null && order.IsDeleted == true)
            {
                await DeleteOrder(order);
            }
            else
            {
                await Handler.Database.UpdateAsync(order);
            }

            //Adding Order Detail in its table
            if ((order.IsDeleted == null || !(bool)order.IsDeleted) && order.OrderDetails != null)
            {
                await Handler.OrderDetails.AddUpdateOrderDetails(order.OrderDetails, order.OrderID);
            }
        }

        public async Task<OrdersSync> GetOrderById(int id)
        {
            return await Handler.Database.Table<OrdersSync>().Where(x => x.OrderID.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<List<OrdersSync>> GetAllOrders()
        {
            var orders = await Handler.Database.Table<OrdersSync>().ToListAsync();
            //foreach(var item in orders)
            //{
            //    item.OrderDetails = await Handler.OrderDetails.GetOrderDetailsByOrderId(item.OrderID);
            //}
            return orders;
        }

        public async Task<List<OrderAccount>> GetAllOrdersWithAccount(int inventoryTransactionTypeId, int loggedInUserId)
        {
            //var orders = await GetAllOrders();
            var orders = await GetAllOrdersByInventoryTransactionTypeId(inventoryTransactionTypeId, loggedInUserId);
            var accounts = await Handler.Accounts.GetAllAccounts();

            var joined =
                from order in orders
                join account in accounts on order.AccountID equals account.AccountID into ps
                from account in ps.DefaultIfEmpty()
                select new OrderAccount() { Order = order, Account = account };


            return joined.ToList();
        }

        public async Task<List<OrderAccount>> GetAllOrdersWithAccountForPickList(int loggedInUserId)
        {
            var orders = await GetAllOrders();
            var saleOrders = orders.Where(x => x.InventoryTransactionTypeId.Equals((int)InventoryTransactionTypeEnum.SaleOrder)).ToList();
            var sampleOrders = orders.Where(x => x.InventoryTransactionTypeId.Equals((int)InventoryTransactionTypeEnum.Samples)).ToList();
            var workOrders = orders.Where(x => x.InventoryTransactionTypeId.Equals((int)InventoryTransactionTypeEnum.WorkOrder)).ToList();
            var loanOrders = orders.Where(x => x.InventoryTransactionTypeId.Equals((int)InventoryTransactionTypeEnum.Loan)).ToList();

            var pickList = (saleOrders.Union(sampleOrders).Union(workOrders).Union(loanOrders)).Where(x => (x.OrderStatusID == (int)OrderStatusEnum.Active
                || x.OrderStatusID == (int)OrderStatusEnum.BeingPicked) && x.DirectShip != true && ((x.PickerId != null && x.PickerId.Equals(loggedInUserId))
                || x.PickerId == null || x.PickerId == 0)).ToList();
            var accounts = await Handler.Accounts.GetAllAccounts();

            var joined =
                from order in pickList
                join account in accounts on order.AccountID equals account.AccountID into ps
                from account in ps.DefaultIfEmpty()
                select new OrderAccount() { Order = order, Account = account };

            return joined.ToList();
        }

        public async Task<List<OrdersSync>> GetAllOrdersByInventoryTransactionTypeId(int inventoryTransactionTypeId, int loggedInUserId)
        {
            var orders = await Handler.Database.Table<OrdersSync>().Where(x => x.InventoryTransactionTypeId.Equals(inventoryTransactionTypeId) && (x.OrderStatusID == (int)OrderStatusEnum.Active
                || x.OrderStatusID == (int)OrderStatusEnum.BeingPicked) && (x.DirectShip != true || x.DirectShip == null) && ((x.PickerId != null && x.PickerId.Equals(loggedInUserId))
                || x.PickerId == null || x.PickerId == 0)).ToListAsync();

            // SLA Priority
            var ordersBySla = orders.Where(x => (x.SLAPriorityId != null && x.SLAPriorityId > 0)).OrderBy((x => x.SLAPriorityId)).ToList();
            var orderByOrderId = orders.Where(x => x.SLAPriorityId == null || x.SLAPriorityId == 0).OrderBy(x => x.OrderID).ToList();
            if (ordersBySla != null && ordersBySla.Any())
            {
                ordersBySla.AddRange(orderByOrderId);
                return ordersBySla;
            }
            else if (orderByOrderId != null && orderByOrderId.Any())
            {
                return orderByOrderId;
            }
            return orders;
        }
    }
}

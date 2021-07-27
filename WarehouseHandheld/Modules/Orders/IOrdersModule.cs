using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Orders;
using System.Threading;
using static WarehouseHandheld.Models.Orders.OrdersSync;

namespace WarehouseHandheld.Modules.Orders
{
    public interface IOrdersModule
    {
        Task SyncOrders();
        Task<List<OrderAccount>> GetOrders(int inventoryTransactionTypeId);
        Task<List<OrderAccount>> GetCompletedSaleOrders(int inventoryTransactionTypeId);
        Task<List<OrderAccount>> GetPurcahseOrdersWithinAccount(int inventoryTransactionTypeId, int AccountId);
        Task<List<OrderDetailsProduct>> GetOrderDetailsWithProduct(int orderId);
        Task<List<OrderDetailsProduct>> GetOrderDetailsForPallets(int orderId);
        Task<bool> UpdateOrderStatus(int orderId, OrderStatusEnum orderStatus);
        Task<List<OrderAccount>> GetAllOrdersWithAccountForPickList();
        Task AddInventoryStockInOrder(List<OrderDetailsProduct> orders, int WarehouseId);
        Task<List<OrdersSync>> GetAllOrders();
    }
}

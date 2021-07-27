using System;
using System.Collections.Generic;
using WarehouseHandheld.Models.Orders;
using System.Threading.Tasks;
namespace WarehouseHandheld.Database.Orders
{
    public interface IOrdersTable
    {
        Task AddUpdateOrders(IList<OrdersSync> orders);
        Task AddUpdateOrder(OrdersSync order);
        Task<List<OrdersSync>> GetAllOrders();
        Task<OrdersSync> GetOrderById(int id);
        Task<List<OrderAccount>> GetAllOrdersWithAccount(int inventoryTransactionTypeId, int loggedInUserId);
        Task<List<OrderAccount>> GetAllOrdersWithAccountForPickList(int loggedInUserId);
        Task DeleteOrder(OrdersSync order);
        Task<List<OrdersSync>> GetAllOrdersByInventoryTransactionTypeId(int inventoryTransactionTypeId, int loggedInUserId);
    }
}

using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Orders;
namespace WarehouseHandheld.Services.Orders
{
    public interface IOrdersService
    {
        Task<OrdersSyncCollection> GetOrdersAsync(DateTime dateUpdated, string serialNo);
        Task<OrdersSync> UpdateOrderStatusAsync(string serialNo, int orderId, int statusId, int userId);
        Task<OrderReceiveCountSyncCollection> PostReceiveCount(OrderReceiveCountSyncCollection receiveCount);
        bool HandleStatusConflict();
    }
}

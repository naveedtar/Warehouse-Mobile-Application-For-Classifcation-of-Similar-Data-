using System;
using System.Collections.Generic;
using WarehouseHandheld.Models.OrderProcesses;
using System.Threading.Tasks;

namespace WarehouseHandheld.Database.OrderProcesses
{
    public interface IOrderProcessesTable
    {
        Task AddUpdateOrderProcesses(IList<OrderProcessSync> orders);
        Task<int> AddOrderProcess(OrderProcessSync order);

        Task<List<OrderProcessSync>> GetAllOrderProcessesWithAccount(int inventoryTransactionTypeId);
        Task DeleteAllOrderProcesses();
        Task DeleteOrderProcessesById(int id);

        Task<OrderProcessSync> GetOrderProcessByOrderProcessId(int? OrderProcessId);
        Task<List<OrderProcessSync>> GetOrderProcessesFromSyncLog(int orderId);

    }
}

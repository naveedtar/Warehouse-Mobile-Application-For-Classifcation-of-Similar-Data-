using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.OrderProcesses;
using WarehouseHandheld.Models.Orders;
namespace WarehouseHandheld.Services.OrderProcesses
{
    public interface IOrderProcessesService
    {
        Task<OrderProcessesSyncCollection> GetOrderProcessesAsync(DateTime dateUpdated, string serialNo);
        Task<List<OrdersSync>> PostOrderProcessesAsync(OrderProcessesSyncCollection orderProcessSync);
        bool HandleStatusConflict();
    }
}

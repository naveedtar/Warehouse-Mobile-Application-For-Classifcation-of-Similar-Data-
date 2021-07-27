using System;
using System.Collections.Generic;
using WarehouseHandheld.Models.OrderProcesses;
using System.Threading.Tasks;
namespace WarehouseHandheld.Database.OrderProcesses
{
    public interface IOrderProcessesDetailTable
    {
        Task AddUpdateOrderProcessesDetail(IList<OrderProcessDetailSync> orders);
        Task<List<OrderProcessDetailSync>> GetOrderProcessDetailByOrderDetailId(int id);
        Task<List<OrderProcessDetailSync>> GetOrderProcessDetailByOrderProcessId(int id);
        Task DeleteOrderProcessDetail(OrderProcessDetailSync orderProcessDetailSync);
    }
}

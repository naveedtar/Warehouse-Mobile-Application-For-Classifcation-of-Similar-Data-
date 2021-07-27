using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Orders;
namespace WarehouseHandheld.Database.Orders
{
    public interface IOrderDetailsTable
    {
        Task AddUpdateOrderDetails(IList<OrderDetailSync> orderDetail, int orderId);
        Task<List<OrderDetailSync>> GetAllOrderDetails();
        Task<List<OrderDetailsProduct>> GetOrderDetailsWithProduct(int orderId);
        Task<OrderDetailSync> GetOrderDetailById(int id);
        Task<List<OrderDetailSync>> GetOrderDetailsByOrderId(int id);
        Task<List<OrderDetailsProduct>> GetOrderDetailsWithProductAndKit(int orderId);
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WarehouseHandheld.Models.OrderProcesses;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Models.Pallets;

namespace WarehouseHandheld.Modules.OrderProcesses
{
    public interface IOrderProcessesModule
    {
        Action<bool> RefreshOrderItemsUi { get; set; }
        Task SyncOrderProcesses();
        Task<List<OrdersSync>> PostOrderProcesses(OrderProcessesSyncCollection orderProcessSync);
        Task<List<OrderProcessSync>> GetCompletedSaleOrdersProcess(int inventoryTransactionTypeId);
        Task AddOrderProcess(List<OrderProcessDetailSync> details, OrdersSync origOrder, bool IsCreate, string ShipmentReference, string ScanPickContainerCode);
        void CreateUpdateOrderProcess(OrderAccount OrderAccount,List<OrderProcessDetailSync> OrderProcessDetails, OrderDetailsProduct item, List<PalleTrackingProcess> trackingProcesses, ObservableCollection<OrderDetailsProduct> orderDetailsProducts, decimal quantity = 1, string[] serials = null);
        Task AddNotSyncedOrders(OrderProcessesSyncCollection orderProcesses);
        bool IsBundleComplete(List<OrderDetailsProduct> orderDetailsProducts);
    }
}

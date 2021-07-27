using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Orders;
using static WarehouseHandheld.Models.Orders.OrdersSync;

namespace WarehouseHandheld.ViewModels.Orders.TransferOrders
{
    public class TransferOrdersViewModel : OrdersViewModel
    {
        public async Task Initialize()
        {
            IsBusy = true;
            var transferInOrders = await App.Orders.GetOrders((int)InventoryTransactionTypeEnum.TransferIn);
            var transferOutOrders = await App.Orders.GetOrders((int)InventoryTransactionTypeEnum.TransferOut);
            transferInOrders = transferInOrders.Union(transferOutOrders).ToList();
            Orders = new ObservableCollection<OrderAccount>(transferInOrders);
            IsBusy = false;
        }


    }
}

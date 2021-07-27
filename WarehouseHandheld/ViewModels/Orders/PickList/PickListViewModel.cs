using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Orders;
using static WarehouseHandheld.Models.Orders.OrdersSync;

namespace WarehouseHandheld.ViewModels.Orders.PickList
{
    public class PickListViewModel : OrdersViewModel
    {
        
        public async Task Initialize()
        {
            IsBusy = true;
            IsPickList = true;
            //var salesOrders = await App.Orders.GetOrders((int)InventoryTransactionTypeEnum.SaleOrder);
            //var sampleOrders = await App.Orders.GetOrders((int)InventoryTransactionTypeEnum.Samples);
            //var worksOrders = await App.Orders.GetOrders((int)InventoryTransactionTypeEnum.WorkOrder);
            //var loanOrders = await App.Orders.GetOrders((int)InventoryTransactionTypeEnum.Loan);

            //var pickList = (salesOrders.Union(sampleOrders).Union(worksOrders).Union(loanOrders)).ToList();
            var pickList = await App.Orders.GetAllOrdersWithAccountForPickList();
            Orders = new ObservableCollection<OrderAccount>(pickList);

            IsBusy = false;
        }
    }
}

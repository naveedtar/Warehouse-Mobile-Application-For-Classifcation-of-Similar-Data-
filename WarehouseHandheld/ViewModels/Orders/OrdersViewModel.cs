using Plugin.Connectivity;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Helpers;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Resources;
using WarehouseHandheld.Views.GoodsReceive;
using WarehouseHandheld.Views.OrderItems;
using Xamarin.Forms;
using static WarehouseHandheld.Models.Orders.OrdersSync;

namespace WarehouseHandheld.ViewModels.Orders
{
    public class OrdersViewModel : BaseViewModel
    {
        public InventoryTransactionTypeEnum OrderType { get; set; }
        public bool IsPickList;
        public bool IsGoodsReceive;
        private ObservableCollection<OrderAccount> orders;
        public ObservableCollection<OrderAccount> Orders
        {
            get { return orders; }
            set
            {
                orders = value;
                OnPropertyChanged();
            }
        }

        public ICommand MarkAsCompleteCommand { get; private set; }

        protected CommandLockerHelper SelectedCommandLocker => new CommandLockerHelper(async (e) => { await OnItemSelected(e); });
        public ICommand ItemSelectedCommand => new Command(SelectedCommandLocker.Execute);

        private async Task OnItemSelected(object e)
        {
            if (((SelectedItemChangedEventArgs)e).SelectedItem != null)

            {
                var order = ((OrderAccount)((SelectedItemChangedEventArgs)e).SelectedItem);
                if (IsGoodsReceive)
                {
                    await App.Current.MainPage.Navigation.PushAsync(new GoodsReceivePage(order));
                }
                else
                {
                    var orderItemPage = new OrderItemsPage(order, (InventoryTransactionTypeEnum)order.Order.InventoryTransactionTypeId);
                    orderItemPage.ViewModel.Completed += RefreshOrders;
                    orderItemPage.ViewModel.RefreshOrders += RefreshOrders;
                    await App.Current.MainPage.Navigation.PushAsync(orderItemPage);
                }
            }
        }

        async void RefreshOrders(bool obj)
        {
            IsBusy = true;
            await UpdateOrders();
            IsBusy = false;
        }


        async Task UpdateOrders()
        {
            if (IsPickList)
            {
                var salesOrders = await App.Orders.GetOrders((int)InventoryTransactionTypeEnum.SaleOrder);
                var sampleOrders = await App.Orders.GetOrders((int)InventoryTransactionTypeEnum.Samples);
                var worksOrders = await App.Orders.GetOrders((int)InventoryTransactionTypeEnum.WorkOrder);
                var loanOrders = await App.Orders.GetOrders((int)InventoryTransactionTypeEnum.Loan);

                var pickList = (salesOrders.Union(sampleOrders).Union(worksOrders).Union(loanOrders)).ToList();

                Orders = new ObservableCollection<OrderAccount>(pickList);
            }
            else
            {
                if (OrderType == InventoryTransactionTypeEnum.TransferIn)
                {
                    var transferInOrders = await App.Orders.GetOrders((int)InventoryTransactionTypeEnum.TransferIn);
                    var transferOutOrders = await App.Orders.GetOrders((int)InventoryTransactionTypeEnum.TransferOut);
                    transferInOrders = transferInOrders.Union(transferOutOrders).ToList();
                    Orders = new ObservableCollection<OrderAccount>(transferInOrders);
                }
                else
                {
                    Orders = new ObservableCollection<OrderAccount>(await App.Orders.GetOrders((int)OrderType));
                }
            }
        }



        public OrdersViewModel()
        {
            MarkAsCompleteCommand = new Command<OrderAccount>(OnMarkAsComplete);
        }

        public async Task<bool> IsNoRemainingItem(OrderAccount order)
        {
            bool IsRemainingItem = false;
            var OrderItems = await App.Orders.GetOrderDetailsWithProduct(order.Order.OrderID);
            foreach (var orderItem in OrderItems)
            {
                if (orderItem.QuantityProcessed < orderItem.OrderDetails.Qty)
                {
                    IsRemainingItem = true;
                }
            }
            if (IsRemainingItem)
            {
                return await App.Current.MainPage.DisplayAlert("Confirmation", "Some items are not processed. Do you really want to mark it as complete?", "Yes", "No");
            }
            return true;
        }

        private async void OnMarkAsComplete(OrderAccount obj)
        {
            if (CrossConnectivity.Current.IsConnected && await Util.Util.IsConnected())
            {
                var IsConfirmed = await App.Current.MainPage.DisplayAlert("Confirmation", "Do you really want to mark it as complete?", "Yes", "No");
                if (IsConfirmed && await IsNoRemainingItem(obj))
                {
                    try
                    {
                        IsBusy = true;
                        var order = obj;
                        var result = await App.Orders.UpdateOrderStatus(order.Order.OrderID, OrdersSync.OrderStatusEnum.Complete);
                        if (result)
                        {
                            await UpdateOrders();
                            "Order Completed Succuessfully.".ToToast();
                        }
                        else
                            "Error while completing order.".ToToast();
                    }
                    finally
                    {
                        IsBusy = false;
                    }
                }
            }
            else if(!CrossConnectivity.Current.IsConnected){
                AppStrings.NoInternet.ToToast();
            }
        }

        public void OnRowTap(OrderAccount order)
        {
            var orderItemPage = new OrderItemsPage(order, OrderType);
            orderItemPage.ViewModel.Completed += RefreshOrders;
            orderItemPage.ViewModel.RefreshOrders += RefreshOrders;
            App.Current.MainPage.Navigation.PushAsync(orderItemPage);
        }


        public async Task Initialize(InventoryTransactionTypeEnum ordersType)
        {
            IsBusy = true;
            Orders = new ObservableCollection<OrderAccount>(await App.Orders.GetOrders((int)ordersType));
            IsBusy = false;
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Models.OrderProcesses;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Resources;
using WarehouseHandheld.Views.Pallets.PalletOrder;
using Xamarin.Forms;

namespace WarehouseHandheld.ViewModels.Pallets.PalletOrder
{
    public class PalletOrdersViewModel : BaseViewModel
    {

        public ICommand AddQtyCommand { get; private set; }

        private bool isEnableAddQty;
        public bool IsEnableAddQty
        {
            get { return isEnableAddQty; }
            set
            {
                isEnableAddQty = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<OrderDetailsProduct> _orders;
        public ObservableCollection<OrderDetailsProduct> Orders
        {
            get { return _orders; }
            set
            {
                _orders = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<PalletProducts> _palletProducts;
        public ObservableCollection<PalletProducts> PalletProducts
        {
            get { return _palletProducts; }
            set
            {
                _palletProducts = value;
                OnPropertyChanged();
            }
        }

        private PalletSync _pallet;
        public PalletSync Pallet
        {
            get { return _pallet; }
            set
            {
                _pallet = value;
                OnPropertyChanged();
            }
        }

        private OrderProcessSync _orderProcessSync;
        public OrderProcessSync OrderProcess
        {
            get { return _orderProcessSync; }
            set
            {
                _orderProcessSync = value;
                OnPropertyChanged();
            }
        }

        public PalletOrdersViewModel()
        {
            AddQtyCommand = new Command<OrderDetailsProduct>(AddQty);
        }


        async void AddQty(OrderDetailsProduct orderProduct)
        {
            IsEnableAddQty = false;
            if (orderProduct.QuantityProcessed < orderProduct.Quantity)
            {
                var index = Orders.ToList().FindIndex((obj) => obj.Product.ProductId == orderProduct.Product.ProductId && obj.OrderDetails.OrderID == orderProduct.OrderDetails.OrderID);
                var PalletProductPopup = new PalletProductQuantityPopup(Convert.ToDouble(orderProduct.Quantity - orderProduct.QuantityProcessed));
                PalletProductPopup.SaveQty += async (qty) =>
                {
                    if (CrossConnectivity.Current.IsConnected && await Util.Util.IsConnected())
                    {
                        IsBusy = true;
                        var response = await App.Pallets.AddPalletItem(Pallet, orderProduct, qty, OrderProcess);
                        if (response != null)
                        {
                            "Item Added Successfully.".ToToast();
                            Orders[index].QuantityProcessed += Convert.ToDecimal(qty);
                            if (Orders[index].Product.ProductsPerCase != null)
                                Orders[index].BoxesRemaining = Math.Round(((Orders[index].OrderDetails.Qty - Orders[index].QuantityProcessed) / (decimal)Orders[index].Product.ProductsPerCase), 2);

                            if (Orders[index].Quantity == Orders[index].QuantityProcessed)
                            {
                                Orders.RemoveAt(index);
                            }
                            await App.Pallets.SyncPallets();
                            await SyncPalletProducts();
                        }
                        else
                        {
                            "There is some error in adding item.".ToToast();
                        }
                        IsBusy = false;
                    }
                    else if (!CrossConnectivity.Current.IsConnected)
                    {
                        AppStrings.NoInternet.ToToast();
                    }

                };
                await PopupNavigation.PushAsync(PalletProductPopup);
            }
            //await App.Current.MainPage.DisplayAlert("","Add Quantity Clicked", "OK");
            await Task.Delay(1000);
            IsEnableAddQty = true;
        }


        public async Task SetOrders()
        {
            IsBusy = true;
            Orders = new ObservableCollection<OrderDetailsProduct>();
            if (OrderProcess.OrderID != null)
            {
                var orderWithDetailList = await App.Orders.GetOrderDetailsForPallets((int)OrderProcess.OrderID);
                foreach (var orderWithDetail in orderWithDetailList)
                {
                    if (orderWithDetail.Quantity > 0 && orderWithDetail.Quantity > orderWithDetail.QuantityProcessed)
                        Orders.Add(orderWithDetail);
                }
                await SyncPalletProducts();
                IsBusy = false;
            }

        }

        public async Task SyncPalletProducts()
        {
            var palletProducts = await App.Pallets.GetPalletProductsWithPalletId(Pallet.PalletID, Pallet.DateCreated);
            PalletProducts = new ObservableCollection<PalletProducts>(palletProducts);
        }
    }
}

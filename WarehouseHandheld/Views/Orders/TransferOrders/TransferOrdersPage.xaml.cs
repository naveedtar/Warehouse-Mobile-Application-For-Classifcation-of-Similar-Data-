using System;
using System.Collections.Generic;
using WarehouseHandheld.Resources;
using WarehouseHandheld.ViewModels.Orders.TransferOrders;
using Xamarin.Forms;
using WarehouseHandheld.Views.Base.BaseContentPage;

namespace WarehouseHandheld.Views.Orders.TransferOrders
{
    public partial class TransferOrdersPage : BasePage
    {
        public TransferOrdersViewModel ViewModel => BindingContext as TransferOrdersViewModel;
        public TransferOrdersPage()
        {
            InitializeComponent();
            ViewModel.OrderType = Models.Orders.OrdersSync.InventoryTransactionTypeEnum.TransferIn;
            Constants.SetGridProperties(grid);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitializeViewModel();
        }

        async void InitializeViewModel()
        {
            await ViewModel.Initialize();
        }

        async void Add_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new CreateOrderPage());
        }
    }
}

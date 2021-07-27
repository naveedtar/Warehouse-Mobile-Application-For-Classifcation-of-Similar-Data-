using System;
using System.Collections.Generic;
using WarehouseHandheld.Models.OrderProcesses;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Resources;
using WarehouseHandheld.ViewModels.Pallets.PalletOrder;
using WarehouseHandheld.Views.Base.BaseContentPage;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.Pallets.PalletOrder
{
    public partial class PalletOrdersPage : BasePage
    {
        public PalletOrdersViewModel ViewModel => BindingContext as PalletOrdersViewModel;
        public PalletOrdersPage()
        {
            InitializeComponent();
        }

        public PalletOrdersPage(PalletSync Pallet, OrderProcessSync OrderProcess)
        {
            InitializeComponent();
            if (Pallet != null)
            {
                ViewModel.Pallet = Pallet;
                Title = Pallet.PalletNumber;

            }
            if (OrderProcess != null)
            {
                ViewModel.OrderProcess = OrderProcess;
            }
            AccountLabel.Text = "Account Name: "+ Pallet.RecipientAccount.CompanyName;
            Constants.SetGridProperties(grid);
            Constants.SetGridProperties(ProductsGrid);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.SetOrders();
        }
    }
}

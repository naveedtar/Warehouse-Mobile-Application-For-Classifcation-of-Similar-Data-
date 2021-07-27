using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using WarehouseHandheld.ViewModels.Pallets;
using WarehouseHandheld.Views.Accounts;
using WarehouseHandheld.Models.Accounts;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Views.Pallets.PalletOrder;
using WarehouseHandheld.Resources;
using System.Linq;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Models.OrderProcesses;
using WarehouseHandheld.Views.Base.BaseContentPage;

namespace WarehouseHandheld.Views.Pallets
{
    public partial class PalletsPage : BasePage
    {


        public PalletsOrderViewModel ViewModel => BindingContext as PalletsOrderViewModel;
        public PalletsPage()
        {
            InitializeComponent();
            Constants.SetGridProperties(grid);

        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.Initialize();
        }

        //async void Handle_Add_Clicked(object sender, System.EventArgs e)
        //{
        //    AccountsPopup account = new AccountsPopup();
        //    account.OnAccountSelected += OnAccountSelected;
        //    await PopupNavigation.PushAsync(account);
        //}

        //private async void OnAccountSelected(AccountSync account)
        //{
        //    var AddPalletPopup = new AddPalletPopup(account);
        //    AddPalletPopup.AddNewPallet+= (obj) => {
        //        ViewModel.Pallets.Insert(0,obj);
        //    };
        //    await PopupNavigation.PushAsync(AddPalletPopup);
        //}


        async void SearchOrder(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            var searchText = Search.Text;
            if (!string.IsNullOrEmpty(searchText))
            {
                var orders = new List<OrderProcessSync>(ViewModel.SaleOrders);
                ViewModel.SaleOrders.Clear();
                var ordersByAccount = orders.Where(c => c.Account.CompanyName.ToLower().Contains(searchText.ToLower()) || c.OrderProcessID.ToString().Contains(searchText.ToString()));
                if (ordersByAccount != null)
                {
                    foreach (var order in ordersByAccount)
                    {
                        if (order != null)
                        {
                            ViewModel.SaleOrders.Add(order);
                        }
                    }
                }
                if (ordersByAccount == null)
                    ViewModel.SaleOrders.Clear();

            }
            else
            {
                await ViewModel.Initialize();
            }
        }
    }
}

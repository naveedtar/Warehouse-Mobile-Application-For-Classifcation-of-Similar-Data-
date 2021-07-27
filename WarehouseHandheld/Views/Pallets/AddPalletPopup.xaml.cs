using System;
using System.Collections.Generic;

using Xamarin.Forms;
using WarehouseHandheld.Views.Base.Popup;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.ViewModels.Pallets;
using WarehouseHandheld.Models.Accounts;
using WarehouseHandheld.Extensions;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Views.Pallets.PalletOrder;
using Plugin.Connectivity;
using WarehouseHandheld.Resources;
using WarehouseHandheld.Models.OrderProcesses;

namespace WarehouseHandheld.Views.Pallets
{
    public partial class AddPalletPopup : PopupBase
    {
        public AddPalletViewModel ViewModel => BindingContext as AddPalletViewModel;
        public Action<PalletSync> AddNewPallet;
        public OrderProcessSync orderProcess;
        public AddPalletPopup(AccountSync account)
        {
            InitializeComponent();
            Constants.SetGridProperties(grid);
            ViewModel.Account = account;
            orderProcess = new OrderProcessSync();
            OnSaveClicked+= CreateNewClicked;
        }

        async void CreateNewClicked()
        {
            if (CrossConnectivity.Current.IsConnected && await Util.Util.IsConnected())
            {
                OrderProcessSync orderprocess = null;

                var pallet = await ViewModel.OnPalletAdd(orderprocess ?? null);
                if (pallet != null)
                {
                    "New Pallet Created Successfully.".ToToast();
                    await App.Current.MainPage.Navigation.PushAsync(new PalletOrdersPage(pallet, orderprocess));
                    await PopupNavigation.PopAsync(false);
                    ViewModel.Pallets.Insert(0,pallet);
                    AddNewPallet?.Invoke(pallet);
                }
            }
            else if(!CrossConnectivity.Current.IsConnected){
                AppStrings.NoInternet.ToToast();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.Initialize();
        }
    }
}

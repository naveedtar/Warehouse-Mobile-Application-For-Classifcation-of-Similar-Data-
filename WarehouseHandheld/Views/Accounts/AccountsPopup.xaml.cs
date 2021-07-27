using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Diagnostics;
using WarehouseHandheld.Models.Accounts;
using WarehouseHandheld.Views.Base.Popup;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.ViewModels.Accounts;

namespace WarehouseHandheld.Views.Accounts
{
    public partial class AccountsPopup : PopupBase
    {
        public Action<AccountSync> OnAccountSelected;
        public AccountsViewModel ViewModel => BindingContext as AccountsViewModel;
        public AccountsPopup()
        {
            InitializeComponent();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        void SearchEntry_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            ViewModel.FindAccount(e.NewTextValue);
        }

        async void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await PopupNavigation.PopAsync();
                OnAccountSelected?.Invoke((AccountSync)e.SelectedItem);
            }
        }
    }
}

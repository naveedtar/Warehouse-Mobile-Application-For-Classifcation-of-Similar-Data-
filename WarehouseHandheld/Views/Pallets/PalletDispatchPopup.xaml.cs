using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.ViewModels.Pallets;
using WarehouseHandheld.Views.Base.Popup;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.Pallets
{
    public partial class PalletDispatchPopup : PopupBase
    {
        public new PalletDispatchViewModel ViewModel => BindingContext as PalletDispatchViewModel;
        public PalletDispatchPopup()
        {
            InitializeComponent();
            OnSaveClicked += OnSave;
            OnCancelClicked += OnCancel;
        }

        private async void OnCancel()
        {
            await PopupNavigation.PopAsync();
        }

        private async void OnSave()
        {
            if (ViewModel.OnSave())
            {
                dispatchMethods.Unfocus();
                await PopupNavigation.PopAsync();
            }
            else{
                "Please enter all required information".ToToast();
            }
        }
    }
}

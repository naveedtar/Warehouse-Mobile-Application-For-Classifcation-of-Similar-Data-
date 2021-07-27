using System;
using System.Collections.Generic;
using WarehouseHandheld.Views.Base.Popup;
using WarehouseHandheld.Models.Products;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.ViewModels.StockTake;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.StockTake
{
    public partial class StockTakePopup : PopupBase
    {
        public StockTakePopupViewModel ViewModel => BindingContext as StockTakePopupViewModel;

        public StockTakePopup(ProductMasterSync product, string serial = "", decimal quantity = 1, string palletSerial = "", bool existingPallet = false)
        {
            InitializeComponent();

            ViewModel.Product = product;
            ViewModel.SerialNumber = serial;
            ViewModel.Quantity = quantity;
            ViewModel.ExistingPallet = existingPallet;
            ViewModel.PalletSerial = palletSerial;

            OnSaveClicked += OnSave;
            OnCancelClicked += OnCancel;

            if (product.RequiresBatchNumberOnReceipt ?? false)
            {
                BatchNumberLabel.IsVisible = true;
                BatchNumber.IsVisible = true;
            }

            if ((product.RequiresExpiryDateOnReceipt ?? false))
            {
                ExpiryDateLabel.IsVisible = true;
                ExpiryDatePicker.IsVisible = true;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void OnCancel()
        {
            if (await ViewModel.OnCancelClicked())
                await PopupNavigation.PopAsync();
        }

        private async void OnSave()
        {
            if (await ViewModel.OnSaveClicked())
            {
                await PopupNavigation.PopAsync();
            }
            SaveButtonEnabled = true;
        }

    }
}

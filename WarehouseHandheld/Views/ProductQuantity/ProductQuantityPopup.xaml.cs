using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using WarehouseHandheld.ViewModels.ProductQuantity;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Views.Base.Popup;
using System.Diagnostics;
using Rg.Plugins.Popup.Services;

namespace WarehouseHandheld.Views.ProductQuantity
{
    public partial class ProductQuantityPopup : PopupBase
    {

        public new ProductQuantityViewModel ViewModel => BindingContext as ProductQuantityViewModel;
        public ProductQuantityPopup(OrderDetailsProduct orderDetail)
        {
            InitializeComponent();
            ViewModel.OrderDetail = orderDetail;
            OnSaveClicked += OnSave;
            OnCancelClicked += OnCancel;
        }

        private async void OnCancel()
        {
            if (await ViewModel.OnCancelClicked())
                await PopupNavigation.PopAsync();
        }

        private void OnSave()
        {
            ViewModel.OnSaveClicked();
        }

        async void Handle_ScanTextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            await ViewModel.ScanCodeTextChanged(e.NewTextValue);
        }
    }
}

using System;
using System.Collections.Generic;

using Xamarin.Forms;
using WarehouseHandheld.Views.Base.Popup;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.ViewModels.Products;

namespace WarehouseHandheld.Views.Products
{
    public partial class ProductsPopup : PopupBase
    {
        public Action<ProductMasterSync> OnProductSelected;
        ProductsViewModel ViewModel => BindingContext as ProductsViewModel;
        public ProductsPopup()
        {
            InitializeComponent();
        }

        void SearchEntry_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            ViewModel.FindProducts(e.NewTextValue);
        }

        async void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await PopupNavigation.PopAsync();
                OnProductSelected?.Invoke((ProductMasterSync)e.SelectedItem);
            }
        }
    }
}

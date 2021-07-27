using System;
using System.Collections.Generic;

using Xamarin.Forms;
using WarehouseHandheld.Views.Base.Popup;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Views.Products;
using System.Diagnostics;
using WarehouseHandheld.ViewModels.OrderItems;
using WarehouseHandheld.Models.Orders;

namespace WarehouseHandheld.Views.OrderItems
{
    public partial class CreateOrderDetailsPopup : PopupBase
    {
        public new CreateOrderDetailsViewModel ViewModel => BindingContext as CreateOrderDetailsViewModel;
        async void Handle_Product_Tapped(object sender, System.EventArgs e)
        {
            ProductsPopup products = new ProductsPopup();
            products.OnProductSelected += (obj) => {
                ViewModel.OnProductSelected(obj);
            };
            await PopupNavigation.PushAsync(products);        
        }

        public CreateOrderDetailsPopup(OrderAccount order)
        {
            InitializeComponent();
            ViewModel.Order = order;
        }

        void Handle_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Debug.WriteLine("index changed");
        }

    }
}

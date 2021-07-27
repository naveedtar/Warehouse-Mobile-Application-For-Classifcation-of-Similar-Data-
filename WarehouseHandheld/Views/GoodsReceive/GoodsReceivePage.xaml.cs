using System;
using System.Collections.Generic;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Resources;
using WarehouseHandheld.ViewModels.GoodsReceive;
using Xamarin.Forms;
using WarehouseHandheld.Views.Base.BaseContentPage;

namespace WarehouseHandheld.Views.GoodsReceive
{
    public partial class GoodsReceivePage : BasePage
    {
        GoodsReceiveViewModel ViewModel => BindingContext as GoodsReceiveViewModel;
        public GoodsReceivePage(OrderAccount order)
        {
            InitializeComponent();
            ViewModel.order = order;
            ViewModel.SetgoodsReceive();
            Constants.SetGridProperties(grid);
            ViewModel.GoBack+= () => {
                Navigation.PopAsync();
            };
        }
    }
}

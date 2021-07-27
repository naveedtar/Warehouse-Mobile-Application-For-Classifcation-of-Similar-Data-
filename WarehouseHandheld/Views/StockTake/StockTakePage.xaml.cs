using System;
using System.Collections.Generic;
using WarehouseHandheld.Resources;
using WarehouseHandheld.ViewModels.StockTake;
using Xamarin.Forms;
using WarehouseHandheld.Views.Base.BaseContentPage;

namespace WarehouseHandheld.Views.StockTake
{
    public partial class StockTakePage : BasePage
    {
        public new StockTakeViewModel ViewModel => BindingContext as StockTakeViewModel;
        public StockTakePage()
        {
            InitializeComponent();
            Constants.SetGridProperties(grid);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.Initialize();
        }
    }
}

using System;
using System.Collections.Generic;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Resources;
using WarehouseHandheld.ViewModels.Pallets;
using Xamarin.Forms;
using Xamarin.Forms.DataGrid;
using WarehouseHandheld.Views.Base.BaseContentPage;
using System.Linq;

namespace WarehouseHandheld.Views.Pallets
{
    public partial class PalletDispatchPage : BasePage
    {
        private PalletDispatchSync palletDispatchObj;

        public PalletDispatchPodViewModel ViewModel => BindingContext as PalletDispatchPodViewModel;
        public PalletDispatchPage()
        {
            InitializeComponent();
            grid.HeightRequest = App.ScreenHeight;
            Constants.SetGridProperties(grid);
        }

        public PalletDispatchPage(PalletDispatchSync palletDispatchObj)
        {
            this.palletDispatchObj = palletDispatchObj;
        }

       

        protected override async void OnAppearing()
        {
            var dispatchExists = await ViewModel.Initialize();
            if (dispatchExists == false)
            {
                "No dispatches are exist.".ToToast();
            }
        }


        async void SearchOrder(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            var searchText = Search.Text;
            if (!string.IsNullOrEmpty(searchText))
            {
                var palletDispatches = new List<PalletDispatchSync>(ViewModel.BeLoaded);
                ViewModel.BeLoaded.Clear();
                var palletDispatchesByVehicleRegistration = palletDispatches.Where(c => c.MarketVehicle.VehicleIdentifier.ToLower().Contains(searchText.ToLower()) || c.MarketVehicle.Name.ToLower().Contains(searchText.ToLower()));
                if (palletDispatchesByVehicleRegistration != null)
                {
                    foreach (var palletDispatch in palletDispatchesByVehicleRegistration)
                    {
                        ViewModel.BeLoaded.Add(palletDispatch);
                    }
                }
                if (palletDispatchesByVehicleRegistration == null)
                    ViewModel.BeLoaded.Clear();

            }
            else
            {
                await ViewModel.Initialize();
            }
        }
    }
}

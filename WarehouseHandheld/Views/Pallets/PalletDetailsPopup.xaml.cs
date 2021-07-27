using System;
using System.Collections.Generic;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Resources;
using WarehouseHandheld.ViewModels.Pallets;
using WarehouseHandheld.Views.Base.Popup;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.Pallets
{
    public partial class PalletDetailsPopup : PopupBase
    {
        PalletDispatchDetailPodViewModel viewModel => BindingContext as PalletDispatchDetailPodViewModel;
        PalletSync palletSyncObj;
        public PalletDetailsPopup(PalletSync pallet)
        {
            InitializeComponent();
            Constants.SetGridProperties(grid);
            if (pallet != null)
            {
                palletSyncObj = pallet;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (palletSyncObj != null)
            {
                await viewModel.InitPalletDetail(palletSyncObj);
            }

        }
    }
}

using System;
using System.Collections.Generic;
using WarehouseHandheld.Models.Enums;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Resources;
using WarehouseHandheld.ViewModels.Pallets;
using Xamarin.Forms;
using WarehouseHandheld.Views.Base.BaseContentPage;

namespace WarehouseHandheld.Views.Pallets
{
    public partial class PalletDispatchDetailPodPage : BasePage
    {
        PalletDispatchSync palletDispatchObj;
        public PalletDispatchDetailPodViewModel ViewModel => BindingContext as PalletDispatchDetailPodViewModel;
        public PalletDispatchDetailPodPage(PalletDispatchSync palletDispatch)
        {
            InitializeComponent();
            this.palletDispatchObj = palletDispatch;
            Constants.SetGridProperties(grid);
            ViewModel.PalletDispatchObj = palletDispatch;
            if (palletDispatchObj.DispatchStatus == (int)PalletDispatchStatusEnum.Scheduled)
            {
                Title = "To be Loaded";
            }


        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.Initialize();
            palletSerialScan.Focus();

        }

        async void Keyboard_Tapped(object sender, System.EventArgs e)
        {
            palletSerialScan.ShowKeyboard = !palletSerialScan.ShowKeyboard;
            await System.Threading.Tasks.Task.Delay(200);
            palletSerialScan.Focus();
        }


        async void Handle_Completed(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(palletSerialScan.Text))
            {
                if (palletDispatchObj.DispatchStatus.Equals((int)PalletDispatchStatusEnum.Scheduled))
                {
                    await ViewModel.ScanTobeLoaded(palletSerialScan.Text);
                }

                palletSerialScan.Text = string.Empty;
                await System.Threading.Tasks.Task.Delay(200);
                palletSerialScan.Focus();
            }
        }

        async void SubmitButtonClicked(object sender, System.EventArgs e)
        {
            try
            {
                SubmitOrderButton.IsEnabled = false;
                await ViewModel.SubmitPalletDispatch();
            }
            finally
            {
                SubmitOrderButton.IsEnabled = true;
            }

        }

        void ClickedCancelled(object sender, System.EventArgs e)
        {
            CancelButton.IsEnabled = false;
            Navigation.PopAsync();

        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Models.Vehicles;
using WarehouseHandheld.ViewModels.StockTake;
using WarehouseHandheld.Views.Base.Popup;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.StockTake
{
    public partial class StockTakeScanPalletPopup : PopupBase
    {
        StockTakeScanPalletPopupViewModel ViewModel;
        TerminalMetadataSync Terminal;
        ProductMasterSync Product;
        PalleTrackingProcess newPallet;
        public Action<PalleTrackingProcess> PalletAdded;
        public Action CancelClicked;
        public Action SaveClicked;
        public StockTakeScanPalletPopup(ProductMasterSync product)
        {
            InitializeComponent();
            this.Product = product;
            ViewModel = new StockTakeScanPalletPopupViewModel(Product);
            OnSaveClicked += StockTakePalletScanPopup_OnSaveClicked;
            OnCancelClicked += StockTakeScanPalletPopup_OnCancelClicked;

            ViewModel.palletNotFound += ViewModel_PalletNotFound;
            ViewModel.palletScanned += ViewModel_PalletScanned;
        }

        void StockTakeScanPalletPopup_OnCancelClicked()
        {
            CancelClicked?.Invoke();
        }


        void ViewModel_PalletScanned(PalleTrackingProcess obj)
        {
            PalletAdded?.Invoke(obj);
        }


        async void ViewModel_PalletNotFound(PalleTrackingProcess pallet)
        {
            if (Terminal.AllowStocktakeAddNew)
            {
                await Util.Util.ShowErrorPopupWithBeep("Pallet not found in the system. please enter cases to submit as new pallet.");
                editQauntityLayout.IsVisible = true;
                newPallet = pallet;
            } 
            else
            {
                await Util.Util.ShowErrorPopupWithBeep("Pallet not found.");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Terminal = await App.Database.Vehicle.GetTerminalMetaData();
            //Terminal.AllowStocktakeAddNew = true;
            PopupAppeared();
        }

        public async void PopupAppeared()
        {
            SetScanEntryFocus();
        }

        void SetScanEntryFocus()
        {
            serialScanEntry.Unfocus();
            Task.Run(async () =>
            {
                await System.Threading.Tasks.Task.Delay(700);
                Device.BeginInvokeOnMainThread(() => {
                    serialScanEntry.Focus();
                });
            });
        }

        void StockTakePalletScanPopup_OnSaveClicked()
        {
            if (Terminal.AllowStocktakeAddNew && !string.IsNullOrEmpty(casesEntry.Text) && newPallet != null) 
            {
                newPallet.ProcessedQuantity = (decimal)(Convert.ToDecimal(casesEntry.Text));
                PalletAdded?.Invoke(newPallet);
            }
            if (string.IsNullOrEmpty(casesEntry.Text) && newPallet != null)
            {
                casesEntry.Focus();
                this.SaveButtonEnabled = true;
                Util.Util.ShowErrorPopupWithBeep("Please enter cases.");
                return;
            }
            SaveClicked?.Invoke();
            PopupNavigation.PopAsync();
        }

        async void Keyboard_Tapped(object sender, System.EventArgs e)
        {
            serialScanEntry.ShowKeyboard = !serialScanEntry.ShowKeyboard;
            await System.Threading.Tasks.Task.Delay(200);
            serialScanEntry.Focus();
        }

        async void Handle_Completed(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(serialScanEntry.Text))
            {
                await ViewModel.Scan(serialScanEntry.Text);
                serialScanEntry.Text = string.Empty;
                await Task.Delay(200);
                serialScanEntry.Focus();
            }
        }
    }
}

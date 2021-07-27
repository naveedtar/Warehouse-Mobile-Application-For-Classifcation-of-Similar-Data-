using System;
using System.Collections.Generic;
using System.Linq;
using Ganedata.Core.Barcoding;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Models.StockTakes;
using WarehouseHandheld.Models.Vehicles;
using WarehouseHandheld.Resources;
using WarehouseHandheld.ViewModels.StockTake;
using Xamarin.Forms;
using WarehouseHandheld.Views.Base.BaseContentPage;
using WarehouseHandheld.Models.StockMovement;

namespace WarehouseHandheld.Views.StockTake
{
    public partial class ScanStockTakeProducts : BasePage
    {
        bool IsProductsAdded;
        TerminalMetadataSync Terminal;
        ScanStockProductViewModel ViewModel => BindingContext as ScanStockProductViewModel;
        public ScanStockTakeProducts(StockTakeSync stocktake)
        {
            InitializeComponent();
            ViewModel.StockTake = stocktake;
            Constants.SetGridProperties(grid);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.Initialize();
            Terminal = await App.Database.Vehicle.GetTerminalMetaData();
            if (!IsProductsAdded)
            {
                IsProductsAdded = true;
                await ViewModel.GetProductsAndSerials(false);
            }
            if (Terminal.MandatoryLocationScan)
                productLocation.Focus();
            else
            {
                ScanEntry.IsEnabled = true;
                ScanEntry.Focus();
            }

            ViewModel.FocusScanEntry+= (obj) => {
                ScanEntry.Focus();
            };
            ViewModel.FocusScanSerialEntry+= async(obj) => {
                await System.Threading.Tasks.Task.Delay(200);
                ScanSerialEntry.Focus();
            };
            ViewModel.EmptyScannedProduct += async() => {
                await System.Threading.Tasks.Task.Delay(200);
                ScanEntry.Text = string.Empty;
                ScanEntry.Focus();
                ViewModel.SelectedProductIndex = -1;
                ViewModel.SelectedProduct = new Models.Products.ProductMasterSync { Name = "None" };
            };

        }

        async void Scan_Completed(object sender, System.EventArgs e)
        {
            if (Terminal.MandatoryLocationScan && string.IsNullOrEmpty(productLocation.Text))
            {
                await Util.Util.ShowErrorPopupWithBeep("You must scan a valid location code before scanning items.");
                return;
            }
            if (!string.IsNullOrEmpty(ViewModel.ProductCode) && await ViewModel.ScanProduct(ViewModel.ProductCode))
            {
                //"Product Added Successfully.".ToToast();
            }
            else if (Terminal.AllowStocktakeAddNew)
            {
                var barcode = new GS128Decoder();
                var code = barcode.GS128DecodeGTINOrDefault(ViewModel.ProductCode);

                await Util.Util.ShowErrorPopupWithBeep("No product/pallet with code: " + code + " found.");
                var identifyPopup = new IdentifyProductPopup(code);
                identifyPopup.Title = "Register Product/Pallet";
                identifyPopup.SendScannedProduct+= IdentifyPopup_SendScannedProduct;
                identifyPopup.SendScannedPallet += IdentifyPopup_SendScannedPallet;
                identifyPopup.CancelClicked += () => {
                    ScanEntry.Focus();
                };
                identifyPopup.SaveClicked += () => {
                    ScanEntry.Focus();
                };
                await PopupNavigation.PushAsync(identifyPopup);
            } else {
                await Util.Util.ShowErrorPopupWithBeep("No product/pallet with code: " + ViewModel.ProductCode + " found.");
            }
            if (!ViewModel.IsSerialEntryShowing)
            {
                await System.Threading.Tasks.Task.Delay(200);
                ScanEntry.Text = string.Empty;
                ScanEntry.Focus();
                ViewModel.SelectedProductIndex = -1;
                ViewModel.SelectedProduct = new Models.Products.ProductMasterSync() { Name = "None" };
            }
        }

        async void IdentifyPopup_SendScannedPallet(Models.Products.ProductMasterSync arg1, string arg2, decimal arg3, string arg4)
        {
            await ViewModel.SendScannedProduct(arg1, arg2, arg3, arg4, false);
        }


        async void ScanSerial_Completed(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewModel.SerialCode) && await ViewModel.ScanSerial(ViewModel.SerialCode))
            {
                //"Product Added Successfully.".ToToast();
            }
            else
            {
                await Util.Util.ShowErrorPopupWithBeep("Product Serial " + ViewModel.SerialCode + " not found.");
                var barcode = new GS128Decoder();
                var code = barcode.GS128DecodeGTINOrDefault(ViewModel.SerialCode);
                var identifyPopup = new IdentifyProductPopup(code);
                identifyPopup.SendScannedProduct += IdentifyPopup_SendScannedProduct;
                await PopupNavigation.PushAsync(identifyPopup);
            }
            await System.Threading.Tasks.Task.Delay(200);
            ScanSerialEntry.Text = string.Empty;
            ScanSerialEntry.Focus();
            ViewModel.SelectedProductIndex = -1;
            ViewModel.SelectedProduct = new Models.Products.ProductMasterSync() { Name = "None" };
        }

        async void IdentifyPopup_SendScannedProduct(Models.Products.ProductMasterSync arg1, string arg2, decimal arg3, string arg4)
        {
            await ViewModel.SendScannedProduct(arg1, arg2, arg3, arg4);
        }


        void Next_Clicked(object sender, System.EventArgs e)
        {
            ViewModel.IsSerialEntryShowing = false;
            ScanEntry.Text = string.Empty;
            ScanEntry.Focus();
            ViewModel.SelectedProductIndex = -1;
            ViewModel.SelectedProduct = new Models.Products.ProductMasterSync() { Name = "None" };
        }

        async void Keyboard_Tapped(object sender, System.EventArgs e)
        {
            ScanEntry.ShowKeyboard = !ScanEntry.ShowKeyboard;
            await System.Threading.Tasks.Task.Delay(200);
            ScanEntry.Focus();
        }

        async void Keyboard_Tapped_Serial(object sender, System.EventArgs e)
        {
            ScanSerialEntry.ShowKeyboard = !ScanSerialEntry.ShowKeyboard;
            await System.Threading.Tasks.Task.Delay(200);
            ScanSerialEntry.Focus();
        }

        async void productLocation_Completed(System.Object sender, System.EventArgs e)
        {
            LocationSync locationSync = await App.Database.StockMovements.GetStockLocationByLocationCode(productLocation.Text);
            if (locationSync == null)
            {
                await Util.Util.ShowErrorPopupWithBeep("Invalid location code scanned.");
                productLocation.Text = "";
                productLocation.Focus();
            }
            else
            {
                ScanEntry.IsEnabled = true;
                ScanEntry.Focus();
            }
        }
    }
}

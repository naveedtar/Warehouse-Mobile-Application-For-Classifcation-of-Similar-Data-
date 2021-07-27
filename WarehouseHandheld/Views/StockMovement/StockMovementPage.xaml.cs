using System;
using System.Collections.Generic;
using System.Linq;
using WarehouseHandheld.Resources;
using WarehouseHandheld.ViewModels.StockMovement;
using WarehouseHandheld.Views.Base.BaseContentPage;
using Xamarin.Forms;
using Xamarin.Forms.DataGridLocal;

namespace WarehouseHandheld.Views.StockMovement
{
    public partial class StockMovementPage : BasePage
    {
        public StockMoveViewModel ViewModel => BindingContext as StockMoveViewModel;
        bool IsAppearing;
        public StockMovementPage()
        {
            InitializeComponent();
            Constants.SetGridProperties(grid);
            ViewModel.ClearScanEntry += ViewModel_ClearScanEntry;
            ViewModel.SetFocus += (obj) =>
            {
                scanLocationEntry.Unfocus();
                scanLocationEntry.Focus();
                IsAppearing = true;
            };
            ViewModel.ShowPopup += (obj) =>
            {
                IsAppearing = false;
            };
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
            IsAppearing = true;
            await ViewModel.Initialize();
            scanLocationEntry.Focus();
            scanLocationEntry.ShowKeyboard = true;
        }


        async void Location_Entry_Completed(object sender, System.EventArgs e)
        {
            scanLocationEntry.Unfocus();
            scanLocationEntry.ShowKeyboard = false;
            if (!string.IsNullOrEmpty(ViewModel.ScanLocationCode))
            {
                await ViewModel.ScanLocationCodeTextChanged(ViewModel.ScanLocationCode);
            }
            scanLocationEntry.Text = string.Empty;
            ScanProduct.IsEnabled = true;
            await System.Threading.Tasks.Task.Delay(300);
            if (ViewModel.SelectedStockLocation != null)
            {
                scanProductEntry.Focus();
                scanProductEntry.ShowKeyboard = true;
            }
            else
            {
                scanLocationEntry.Focus();
                scanLocationEntry.ShowKeyboard = true;
            }

        }

        async void Scan_Product_Entry_Completed(object sender, System.EventArgs e)
        {
            scanProductEntry.Unfocus();
            scanProductEntry.ShowKeyboard = false;
            ViewModel.ScanProductCode = scanProductEntry.Text;
            if (!string.IsNullOrEmpty(ViewModel.ScanProductCode) && !await ViewModel.ScanProductCodeTextChanged(ViewModel.ScanProductCode))
            {
                //await Util.Util.ShowErrorPopupWithBeep("Item " + ViewModel.ScanProductCode + " not found.");
            }
            scanProductEntry.Text = string.Empty;
            await System.Threading.Tasks.Task.Delay(300);
            scanProductEntry.Focus();

        }


        async void Keyboard_Tapped_Location(object sender, System.EventArgs e)
        {
            scanProductEntry.ShowKeyboard = false;
            scanLocationEntry.ShowKeyboard = !scanLocationEntry.ShowKeyboard;
            await System.Threading.Tasks.Task.Delay(200);
            scanLocationEntry.Focus();
        }

        async void Keyboard_Tapped(object sender, System.EventArgs e)
        {
            scanLocationEntry.ShowKeyboard = false;
            scanProductEntry.ShowKeyboard = !scanProductEntry.ShowKeyboard;
            await System.Threading.Tasks.Task.Delay(200);
            scanProductEntry.Focus();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        async void Back_Clicked(object sender, System.EventArgs e)
        {
            if (ViewModel.StockMovements != null && ViewModel.StockMovements.Any())
            {
                var isSave = await Util.Util.ShowErrorPopupPromptWithBeep("Do you want to save stock movement progress?", "Yes", "No");
                if (!isSave)
                {
                    await App.Database.StockMovements.DeleteStockMovement();
                }
            }
            await Navigation.PopAsync();
        }

        async void ViewModel_ClearScanEntry()
        {
            scanProductEntry.Text = string.Empty;
            await System.Threading.Tasks.Task.Delay(200);
            scanProductEntry.Focus();
            if (scanLocationEntry.Focus())
            {
                scanLocationEntry.ShowKeyboard = true;
            }
        }
    }
}

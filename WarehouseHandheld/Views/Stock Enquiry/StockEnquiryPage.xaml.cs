using System;
using System.Collections.Generic;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Resources;
using WarehouseHandheld.ViewModels.StockEnquiry;
using Xamarin.Forms;
using WarehouseHandheld.Views.Base.BaseContentPage;

namespace WarehouseHandheld.Views.StockEnquiry
{
    public partial class StockEnquiryPage : BasePage
    {
        StockEnquiryViewModel ViewModel => BindingContext as StockEnquiryViewModel;
        bool IsProductsAdded;
        private bool keyboardImageTapped;

        public StockEnquiryPage()
        {
            InitializeComponent();
            Constants.SetGridProperties(grid);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (!IsProductsAdded)
            {
                IsProductsAdded = true;
                await ViewModel.InitializeStocks();
            }
            ScanEntry.Focus();
            ViewModel.FocusScanEntry += (obj) => {
                ScanEntry.Focus();
            };
        }

        async void Handle_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (ProductsPicker.SelectedIndex >= 0)
            {
                await ViewModel.SelectProduct(ProductsPicker.SelectedIndex);
            }
            if (!ViewModel.IsSerialEntryShowing)
            {
                await System.Threading.Tasks.Task.Delay(200);
                ScanEntry.Text = string.Empty;
                ScanEntry.Focus();
                ProductsPicker.SelectedIndex = -1;
            }
        }


        async void Scan_Completed(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewModel.ProductCode) &&
                await ViewModel.ScanProduct(ViewModel.ProductCode))
            {
                //"Product Added Successfully.".ToToast();
            }
            else
            {
                await Util.Util.ShowErrorPopupWithBeep("No Stock Found");
            }
            if (!ViewModel.IsSerialEntryShowing)
            {
                await System.Threading.Tasks.Task.Delay(200);
                ScanEntry.Text = string.Empty;
                ScanEntry.Focus();
                ProductsPicker.SelectedIndex = -1;
            }
            if (ViewModel.RemainingProductsinPallet != 0)
                remainingProductsStack.IsVisible = true;
            else
                remainingProductsStack.IsVisible = false;
            if (ViewModel.CasesinPallet != 0)
                remainingCasesStack.IsVisible = true;
            else
                remainingCasesStack.IsVisible = false;
        }

        void Picker_Unfocused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            ScanEntry.Focus();
        }

        async void Keyboard_Tapped(object sender, System.EventArgs e)
        {
            if (ScanEntry.ShowKeyboard == false)
            {
                keyboardImageTapped = true;
                ScanEntry.ShowKeyboard = !ScanEntry.ShowKeyboard;
                ScanEntry.ShowKeyboard = true;
                await System.Threading.Tasks.Task.Delay(200);
                ScanEntry.Focus();
            }
            else
            {
                ScanEntry.ShowKeyboard = false;
            }


        }
    }
}

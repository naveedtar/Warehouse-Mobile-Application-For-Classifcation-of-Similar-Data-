using System;
using System.Collections.Generic;
using Xamarin.Forms;
using WarehouseHandheld.ViewModels.Returns;
using WarehouseHandheld.Resources;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Views.Base.BaseContentPage;

namespace WarehouseHandheld.Views.Returns
{
    public partial class ReturnsPage : BasePage
    {
        public ReturnsViewModel ViewModel => BindingContext as ReturnsViewModel;
        private bool keyboardImageTapped;
        public ReturnsPage(bool IsWastage)
        {
            InitializeComponent();
            Constants.SetGridProperties(grid);
            ViewModel.IsWastages = IsWastage;
            if (IsWastage)
            {
                SellableCheckBox.IsVisible = false;
                SellableLabel.IsVisible = false;
                SearchLabel.IsVisible = false;
                scanEntry.IsVisible = false;
                OrderLabel.IsVisible = false;
                Title = AppStrings.Wastage;
            }
            ViewModel.OnFocus+= async(obj) => {
                scanEntry.Text = string.Empty;
                productScanEntry.Text = string.Empty;
                ViewModel.ProductIndex = -1;
                await System.Threading.Tasks.Task.Delay(200);
                productScanEntry.Focus();
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.InitializeProducts();
            scanEntry.Focus();
            productScanEntry.Focus();
            MessagingCenter.Send(this, "preventLandScape");

        }


        void Handle_Completed(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(scanEntry.Text) && !ViewModel.IsWastages)
                return;

            ScanText(scanEntry.Text);

           

        }

        async void ScanText(string text)
        {
            ViewModel.SetAllProducts();
            ViewModel.PalletTrackings = null;
            if (await ViewModel.ScanCodeTextChanged(text))
                scanEntry.Unfocus();
            else if (ViewModel.IsWastages)
            {
                await Util.Util.ShowErrorPopupWithBeep("Order " + text + " not found.");
                scanEntry.Text = string.Empty;
                productScanEntry.Text = string.Empty;
               
            }
            else
            {
                await Util.Util.ShowErrorPopupWithBeep("Order " + text + " not found.");
                ViewModel.Products.Clear();
                ViewModel.ProductIndex = -1;
                scanEntry.Text = string.Empty;
            }


        }


        async void ProductSearch_Completed(object sender, System.EventArgs e)
        {
            ViewModel.PalletTrackings = null;
            if (!string.IsNullOrEmpty(productScanEntry.Text))
            {
                var productFound = await ViewModel.ScanWastagesTextChanged(productScanEntry.Text);
                if (!productFound)
                {
                    productScanEntry.Text = string.Empty;
                    productScanEntry.Focus();
                }
            }
        }

        async void Keyboard_Tapped(object sender, System.EventArgs e)
        {
            if (productScanEntry.ShowKeyboard == false)
            {
                keyboardImageTapped = true;
                productScanEntry.ShowKeyboard = !productScanEntry.ShowKeyboard;
                productScanEntry.ShowKeyboard = true;
                await System.Threading.Tasks.Task.Delay(200);
                productScanEntry.Focus();
            }
            else
            {
                productScanEntry.ShowKeyboard = false;
            }

        }
        private void scanEntry_Focused(object sender, FocusEventArgs e)
        {
            if (keyboardImageTapped)
            {
                productScanEntry.ShowKeyboard = true;
                keyboardImageTapped = false;
            }
            else
            {
                productScanEntry.ShowKeyboard = false;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send(this, "allowLandScapePortrait");
        }
    }
}

using System;
using System.Collections.Generic;
using Ganedata.Core.Barcoding;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.ViewModels.GenerateLabels;
using WarehouseHandheld.Views.Base.BaseContentPage;
using WarehouseHandheld.Views.GenerateLabels.CaseLabels;
using WarehouseHandheld.Views.GenerateLabels.PalletLabels;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.GenerateLabels
{
    public partial class ScanProductGenerateLabel : BasePage
    {
        ScanProductLabelViewModel ViewModel => BindingContext as ScanProductLabelViewModel;
        private bool keyboardImageTapped;
        public ScanProductGenerateLabel()
        {
            InitializeComponent();
            BindingContext = ViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            scanEntry.Focus();
            scanEntry.Text = string.Empty;
        }

        async void Handle_Completed(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewModel.ScanCode) && !ViewModel.ScanCodeTextChanged(ViewModel.ScanCode))
            {
                await Util.Util.ShowErrorPopupWithBeep("Item " + ViewModel.ScanCode + " not found.");
                scanEntry.Text = string.Empty;
                await System.Threading.Tasks.Task.Delay(300);
                scanEntry.Focus();
            }
            else if (!string.IsNullOrEmpty(ViewModel.ScanCode))
            {
                if (ViewModel.SelectedProduct.ProcessByCase)
                {
                    await Navigation.PushAsync(new GenerateCaseLabelsPage(ViewModel.SelectedProduct));
                    "Product Selected".ToToast();
                }
                else if(ViewModel.SelectedProduct.ProcessByPallet)
                {
                    await Navigation.PushAsync(new GenerateLabelsPage(ViewModel.SelectedProduct));
                    "Product Selected".ToToast();
                }
                else
                {
                    await Util.Util.ShowErrorPopupWithBeep("Selected product is neither 'process by pallet' nor 'process by case'");
                }
            }
        }

        async void Keyboard_Tapped(object sender, System.EventArgs e)
        {
            if (scanEntry.ShowKeyboard == false)
            {
                keyboardImageTapped = true;
                scanEntry.ShowKeyboard = !scanEntry.ShowKeyboard;
                scanEntry.ShowKeyboard = true;
                await System.Threading.Tasks.Task.Delay(200);
                scanEntry.Focus();
            }
            else
            {
                scanEntry.ShowKeyboard = false;
            }

        }

        private void scanEntry_Focused(object sender, FocusEventArgs e)
        {
            if (keyboardImageTapped)
            {
                scanEntry.ShowKeyboard = true;
                keyboardImageTapped = false;
            }
            else
            {
                scanEntry.ShowKeyboard = false;
            }
        }

        async void Duplicate_Pallet_Labels(System.Object sender, System.EventArgs e)
        {
            var duplicatePage = new DuplicateLabelPage(GenerateLabelsPageMode.dupliatePalletLabel);
            duplicatePage.SetFocus += DuplicatePage_SetFocus;
            await PopupNavigation.PushAsync(duplicatePage);
            DuplicateButton.IsEnabled = true;
        }

        void DuplicatePage_SetFocus(bool obj)
        {
            scanEntry.Focus();
        }

        async void Pallets_Case_Labels_Clicked(System.Object sender, System.EventArgs e)
        {
            var duplicatePage = new DuplicateLabelPage(GenerateLabelsPageMode.generatePalletCaseLabels);
            duplicatePage.SetFocus += DuplicatePage_SetFocus;
            await PopupNavigation.PushAsync(duplicatePage);
            DuplicateButton.IsEnabled = true;
        }
    }
}

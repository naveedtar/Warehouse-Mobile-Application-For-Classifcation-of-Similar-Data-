using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Models.StockMovement;
using WarehouseHandheld.ViewModels.StockMovement;
using WarehouseHandheld.Views.Base.Popup;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.StockMovement
{
    public partial class ScanProductStockMovementPopup : PopupBase
    {
        public ScanStockMovementViewModel ViewModel => BindingContext as ScanStockMovementViewModel;
        public Action<bool> Back;

        public ScanProductStockMovementPopup(StockMovementViewModel stockMovementViewModel)
        {
            InitializeComponent();
            if (stockMovementViewModel != null)
            {
                ViewModel.Stock = stockMovementViewModel;
            }
            OnSaveClicked += OnSave;
            OnCancelClicked += OnCancel;
            ViewModel.SetFocus += ViewModel_SetFocus;
        }



        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (ViewModel.Stock != null)
            {
                ViewModel.Product = await App.Database.Products.GetProductById(ViewModel.Stock.ProductId);
                if (ViewModel.Product != null)
                {
                    if (ViewModel.Product.Serialisable)
                    {
                        await ViewModel.LoadSerials();
                        SerialProductStack.IsVisible = true;
                        NonSerialProductStack.IsVisible = false;
                    }
                    else
                    {
                        SerialProductStack.IsVisible = false;
                        NonSerialProductStack.IsVisible = true;
                    }
                }
            }
        }

        private async void OnCancel()
        {
            if (ViewModel.Product.Serialisable)
            {
                if (await ViewModel.OnSerialCancelClicked())
                    await PopupNavigation.Instance.PopAsync();
            }
            else
            {
                if (await ViewModel.OnCancelClicked())
                    await PopupNavigation.Instance.PopAsync();
            }

        }

        private async void OnSave()
        {
            SaveButtonEnabled = false;
            if (ViewModel.Product.Serialisable)
            {
                if (ViewModel.OnSaveSerialsClicked())
                {
                    await PopupNavigation.Instance.PopAsync();
                }
            }
            else
            {
                if (ViewModel.OnSaveClicked())
                {
                    await PopupNavigation.Instance.PopAsync();
                }
            }

            SaveButtonEnabled = true;
        }


        void Handle_ScanTextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                //ViewModel.ScanSerialChanged(e.NewTextValue);
            }
        }

        async void Handle_Completed(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewModel.ScanProductSerial) && !(await ViewModel.ScanSerialChanged(ViewModel.ScanProductSerial)))
            {
            }
            serialScanEntry.Text = string.Empty;
            await System.Threading.Tasks.Task.Delay(300);
            serialScanEntry.Focus();
        }

        async void Keyboard_Tapped(object sender, System.EventArgs e)
        {
            serialScanEntry.ShowKeyboard = !serialScanEntry.ShowKeyboard;
            await System.Threading.Tasks.Task.Delay(200);
            serialScanEntry.Focus();
        }


        async void Handle_Toggled(object sender, Xamarin.Forms.ToggledEventArgs e)
        {

            if (e.Value && ViewModel.Product != null)
            {
                if (ViewModel.Product.ProductsPerCase == null)
                {
                    await Util.Util.ShowErrorPopupWithBeep("Cannot add cases. Please contact admin");
                }
                CasesSwitch.IsToggled = false;
                return;
            }
            CasesLabel.IsVisible = e.Value;
            CasesStepper.IsVisible = e.Value;
            PerCaseLabel.IsVisible = e.Value;
            PerCaseText.IsVisible = e.Value;
            CasesProgress.IsVisible = e.Value;
            QuantityLabel.IsVisible = !e.Value;
            QuantityStepper.IsVisible = !e.Value;

        }

        void ViewModel_SetFocus(bool obj)
        {
            Task.Run(async () => {
                await Task.Delay(700);
                Device.BeginInvokeOnMainThread(() => {
                    serialScanEntry.Focus();
                });
            });
        }

    }
}

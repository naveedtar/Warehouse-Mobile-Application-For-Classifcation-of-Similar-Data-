using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.ViewModels.StockTake;
using WarehouseHandheld.Views.Base.Popup;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.StockTake
{
    public partial class IdentifyProductPopup : PopupBase
    {
        IdentifyProductViewModel ViewModel;
        public Action<ProductMasterSync, string, decimal, string> SendScannedProduct;
        public Action<ProductMasterSync, string, decimal, string> SendScannedPallet;
        public Action CancelClicked;
        public Action SaveClicked;
        public IdentifyProductPopup(string code)
        {
            InitializeComponent();
            ViewModel = new IdentifyProductViewModel();
            ViewModel.code = code;
            BindingContext = ViewModel;
            OnSaveClicked += IdentifyProductPopup_OnSaveClicked;
            OnCancelClicked += () =>
            {
                CancelClicked?.Invoke();
                PopupNavigation.PopAsync();
            };
        }

        void IdentifyProductPopup_OnSaveClicked()
        {
            if (OptionPicker.SelectedIndex == 0 || OptionPicker.SelectedIndex == 3)
            {
                ProductMasterSync product = new ProductMasterSync();
                if (OptionPicker.SelectedIndex != 0 && string.IsNullOrEmpty(ViewModel.SKU))
                {
                    "Enter Product Code".ToToast();
                    this.SaveButtonEnabled = true;
                    return;
                }

                if (ViewModel.Quantity == 0)
                {
                    "Enter Quantity".ToToast();
                    this.SaveButtonEnabled = true;
                    return;
                }


                product.Name = ViewModel.Name;
                product.SKUCode = ViewModel.SKU;
                product.BarCode = ViewModel.Barcode;
                product.BarCode2 = ViewModel.Barcode2;
                if (OptionPicker.SelectedIndex == 0)
                {
                    product.SKUCode = ViewModel.code;
                    SendScannedProduct?.Invoke(product, "", ViewModel.Quantity, "");
                }
                else if (OptionPicker.SelectedIndex == 3)
                {
                    product.BarCode = ViewModel.code;
                    SendScannedProduct?.Invoke(product, ViewModel.code, ViewModel.Quantity, "");
                }
                else if (OptionPicker.SelectedIndex == 4)
                {
                    product.BarCode2 = ViewModel.Barcode2;
                    SendScannedProduct?.Invoke(product, "", ViewModel.Quantity, ViewModel.code);
                }
            }

            else if (OptionPicker.SelectedIndex == 1 || OptionPicker.SelectedIndex == 2)
            {
                if (ViewModel.Product.Name != "None")
                {
                    if (OptionPicker.SelectedIndex == 1)
                        SendScannedProduct?.Invoke(ViewModel.Product, ViewModel.code, 1, "");
                    else
                    {
                        if (ViewModel.Quantity == 0)
                        {
                            "Enter Quantity".ToToast();
                            this.SaveButtonEnabled = true;
                            return;
                        }

                        SendScannedProduct?.Invoke(ViewModel.Product, "", ViewModel.Quantity, ViewModel.code);
                    }
                }
                else
                {
                    "Select Product".ToToast();
                    this.SaveButtonEnabled = true;
                    return;
                }
            }
            else if (OptionPicker.SelectedIndex == 4)
            {
                if (ViewModel.Product.Name != "None" && ViewModel.Quantity > 0)
                {
                    ViewModel.Product.SKUCode = ViewModel.code;
                    SendScannedPallet?.Invoke(ViewModel.Product, "", ViewModel.Quantity, ViewModel.code);
                }
                else if (ViewModel.Product.Name == "None")
                {
                    Util.Util.ShowErrorPopupWithBeep("Please select Product.");
                    this.SaveButtonEnabled = true;
                    return;
                }
                else if (ViewModel.Quantity == 0)
                {
                    Util.Util.ShowErrorPopupWithBeep("Please enter quantity.");
                    this.SaveButtonEnabled = true;
                    return;
                }

            }
            SaveClicked?.Invoke();
            PopupNavigation.PopAsync();

        }


        void Handle_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (OptionPicker.SelectedIndex == 0 || OptionPicker.SelectedIndex == 3)
            {
                ProductGrid.IsVisible = true;
                ProductSerialGrid.IsVisible = false;
                PalletSerialGrid.IsVisible = false;
                SKULabel.IsVisible = OptionPicker.SelectedIndex != 0;
                SKUEntry.IsVisible = OptionPicker.SelectedIndex != 0;
                BarcodeLabel.IsVisible = OptionPicker.SelectedIndex != 3;
                BarcodeEntry.IsVisible = OptionPicker.SelectedIndex != 3;
                Barcode2Label.IsVisible = OptionPicker.SelectedIndex != 4;
                Barcode2Entry.IsVisible = OptionPicker.SelectedIndex != 4;

            }
            else if (OptionPicker.SelectedIndex == 1)
            {
                ProductGrid.IsVisible = false;
                ProductSerialGrid.IsVisible = true;
                PalletSerialGrid.IsVisible = false;
            }
            else if (OptionPicker.SelectedIndex == 2)
            {
                ProductGrid.IsVisible = false;
                ProductSerialGrid.IsVisible = false;
                PalletSerialGrid.IsVisible = true;
            }
            else if (OptionPicker.SelectedIndex == 4)
            {
                qauntityLabel.Text = "Cases";
                ProductGrid.IsVisible = false;
                ProductSerialGrid.IsVisible = false;
                PalletSerialGrid.IsVisible = true;
            }
        }
    }
}

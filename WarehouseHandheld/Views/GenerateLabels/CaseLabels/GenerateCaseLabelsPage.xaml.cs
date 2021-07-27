using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Helpers;
using WarehouseHandheld.Models.DeviceSettings;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Modules;
using WarehouseHandheld.Resources;
using WarehouseHandheld.ViewModels.GenerateLabels;
using WarehouseHandheld.Views.Base.BaseContentPage;
using WarehouseHandheld.Views.DeviceSettings;
using WarehouseHandheld.Views.GenerateLabels.PalletLabels;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.GenerateLabels.CaseLabels
{
    public partial class GenerateCaseLabelsPage : BasePage
    {
        GenerateLabelsViewModel ViewModel => BindingContext as GenerateLabelsViewModel;
        PalletTrackingSyncCollection palletTrackingSyncCollection = new PalletTrackingSyncCollection();
        private bool keyboardImageTapped;

        public GenerateCaseLabelsPage(ProductMasterSync selectedProduct)
        {
            InitializeComponent();
            palletTrackingSyncCollection.PalletTrackingSync = new List<PalletTrackingSync>();
            var todayDate = DateTime.Today;
            var lastDayOfMonth = DateTime.DaysInMonth(todayDate.Year, todayDate.Month);
            var lastDateOfMonth = todayDate.AddDays(lastDayOfMonth - todayDate.Day);
            expiryDate.Date = DateTime.Today;
            expiryDate.Format = "dd/MM/yy";
            GetSelectedProduct(selectedProduct);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            scanEntry.Focus();
        }

        async void Handle_Completed(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewModel.ScanCode) && !ViewModel.ScanCodeTextChangedCases(ViewModel.ScanCode))
            {

                ProductNameLabel.Text = string.Empty;
                ProductNameLabel.IsVisible = false;
                if (ViewModel.SelectedProduct != null && !ViewModel.SelectedProduct.ProcessByCase)
                {
                    await Util.Util.ShowErrorPopupWithBeep(ViewModel.SelectedProduct.Name + " is not process by case.");
                }
                else
                {
                    await Util.Util.ShowErrorPopupWithBeep("Item " + ViewModel.ScanCode + " not found.");
                }
                scanEntry.Text = string.Empty;
                await System.Threading.Tasks.Task.Delay(300);
                scanEntry.Focus();

            }
            else if (!string.IsNullOrEmpty(ViewModel.ScanCode))
            {
                ProductNameLabel.Text = ViewModel.SelectedProduct.Name;
                ProductNameLabel.IsVisible = true;
                if (ViewModel.SelectedProduct.CasesPerPallet != null)
                {
                    //Cases.Text = ViewModel.SelectedProduct.CasesPerPallet.ToString();
                }

                "Product Selected".ToToast();
            }


        }


        async void Handle_SettingsClicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new DeviceSettingsPage());
        }
        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            bool IsPrint = false;

            if (casesQuantity.Value == 0 || labelNo.Value == 0 || string.IsNullOrEmpty(poNumber.Text))
            {
                "Enter required values".ToToast();
            }

            else if (ViewModel.SelectedProduct == null)
            {
                "Please select product".ToToast();
            }

            else
            {
                var printerHelper = DependencyService.Get<IPrinterHelper>();

                for (int i = 0; i < labelNo.Value; i++)
                {
                    // check printer status
                    if (await printerHelper.ConnectionScheck() == false)
                    {
                        "Printer is not connected. Please check paired devices.".ToToast();
                        break;
                    }

                    List<PrintLine> PrintList = new List<PrintLine>();
                    var TxtHeaderHeight = new Ref<int>();
                    int posXRightForProducts = 280;
                    var TxtHeight = await PrintLineHelper.GetConnectedDeviceTxtHeight(TxtHeaderHeight);
                    if (TxtHeight != 0)
                    {
                        int PosY = 20;

                        PrintList.Add(await PrintLineHelper.GetLine("Name :", PosY, posXRightForProducts, PrintTextType.TextTrueType));
                        PosY += printerHelper.HeightForPosY(TxtHeight);

                        PrintList.Add(await PrintLineHelper.GetLine(ViewModel.SelectedProduct.Name.PadRight(20), PosY, posXRightForProducts, PrintTextType.TextTrueType, true));
                        PosY += printerHelper.HeightForPosY(TxtHeight);

                        PrintList.Add(await PrintLineHelper.GetSeparatorLine(PosY));
                        PosY += printerHelper.HeightForPosY(TxtHeight);

                        PrintList.Add(await PrintLineHelper.GetLine("SKU :", PosY, posXRightForProducts, PrintTextType.TextTrueType));
                        PosY += printerHelper.HeightForPosY(TxtHeight);

                        PrintList.Add(await PrintLineHelper.GetLine(ViewModel.SelectedProduct.SKUCode, PosY, 100, PrintTextType.OneDBarcode, true, null, false));
                        PosY += printerHelper.HeightForPosY(TxtHeight);

                        PrintList.Add(await PrintLineHelper.GetLine(ViewModel.SelectedProduct.SKUCode, PosY, posXRightForProducts, PrintTextType.TextTrueType, true));
                        PosY += printerHelper.HeightForPosY(TxtHeight);

                        PrintList.Add(await PrintLineHelper.GetSeparatorLine(PosY));
                        PosY += printerHelper.HeightForPosY(TxtHeight);

                        PrintList.Add(await PrintLineHelper.GetLine("QTY :".PadRight(20) + PrintLineHelper.ArrayJoiner(Constants.PrintLineSeparator) + "CTN :", PosY, posXRightForProducts, PrintTextType.TextTrueType));
                        PosY += printerHelper.HeightForPosY(TxtHeight);

                        PrintList.Add(await PrintLineHelper.GetLine(casesQuantity.Value.ToString(), PosY, 100, PrintTextType.OneDBarcode, false, null, false));
                        PosY += printerHelper.HeightForPosY(TxtHeight);

                        PrintList.Add(await PrintLineHelper.GetLine(casesQuantity.Value.ToString().PadRight(20) + PrintLineHelper.ArrayJoiner(Constants.PrintLineSeparator) + ctn.Text, PosY, posXRightForProducts, PrintTextType.TextTrueType, true));
                        PosY += printerHelper.HeightForPosY(TxtHeight);

                        PrintList.Add(await PrintLineHelper.GetSeparatorLine(PosY));
                        PosY += printerHelper.HeightForPosY(TxtHeight);

                        PrintList.Add(await PrintLineHelper.GetLine("Date : ".PadRight(20) + PrintLineHelper.ArrayJoiner(Constants.PrintLineSeparator) + "PO No", PosY, posXRightForProducts, PrintTextType.TextTrueType));
                        PosY += printerHelper.HeightForPosY(TxtHeight);

                        PrintList.Add(await PrintLineHelper.GetLine(expiryDate.Date.ToString("dd/MM/yy").PadRight(20) + PrintLineHelper.ArrayJoiner(Constants.PrintLineSeparator) + poNumber.Text, PosY, posXRightForProducts, PrintTextType.TextTrueType, true));
                        PosY += printerHelper.HeightForPosY(TxtHeight);

                        PrintList.Add(await PrintLineHelper.GetSeparatorLine(PosY));
                        PosY += printerHelper.HeightForPosY(TxtHeight);


                        if (string.IsNullOrEmpty(await DependencyService.Get<IPrinterHelper>().Print(PrintList)))
                        {
                            "Lost Printer connection. Please check Printer".ToToast();
                            break;
                        }

                        IsPrint = true;
                        var printerStatus = DependencyService.Get<IPrinterHelper>().GetPrinterStatus();
                        int retry = 0;
                        while (!(printerStatus == "00\r\n") && retry < 20)
                        {
                            printerStatus = DependencyService.Get<IPrinterHelper>().GetPrinterStatus();
                            retry++;
                        }

                    }
                    else
                    {
                        "Printer is not connected. Please check paired devices.".ToToast();
                        break;
                    }
                    await Task.Delay(300);
                }
                if (IsPrint)
                {
                    ViewModel.SelectedProduct = null;
                    casesQuantity.Value = 0;
                    labelNo.Value = 0;
                    ViewModel.ScanCode = string.Empty;
                    poNumber.Text = string.Empty;
                    ctn.Text = string.Empty;
                    ProductNameLabel.Text = string.Empty;
                    ProductNameLabel.IsVisible = false;
                    expiryDate.Date = DateTime.Today;
                    await Task.Delay(300);
                    scanEntry.Focus();
                    bool doesPageExists = Navigation.NavigationStack.Any(p => p is GenerateCaseLabelsPage);
                    if (doesPageExists)
                    {
                        await Navigation.PopAsync();
                    }
                }
            }
            ((Button)sender).IsEnabled = true;

        }

        public string GenerateRandomNo()
        {
            Random _random = new Random();
            return _random.Next(0, 9999).ToString("D4");
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

        void GetSelectedProduct(ProductMasterSync productMasterSync)
        {
            ProductNameLabel.Text = productMasterSync.Name;
            ProductNameLabel.IsVisible = true;
            ViewModel.SelectedProduct = productMasterSync;
            if (ViewModel.SelectedProduct.ProductsPerCase != null)
            {
                casesQuantity.Value = Convert.ToDouble(ViewModel.SelectedProduct.ProductsPerCase ?? 1);
            }

            "Product Selected".ToToast();
        }

    }
}

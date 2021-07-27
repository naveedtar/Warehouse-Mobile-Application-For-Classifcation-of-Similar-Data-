using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Helpers;
using WarehouseHandheld.Models.DeviceSettings;
using WarehouseHandheld.Models.Enums;
using WarehouseHandheld.ViewModels.PrintBarCode;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WarehouseHandheld.Views.Base.BaseContentPage;

namespace WarehouseHandheld.Views.PrintBarCode
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrintBarCodePage : BasePage
    {
        public PrintBarCodeViewModel ViewModel => BindingContext as PrintBarCodeViewModel;
        bool IsAppearing;
        private string PrintCode;

        public PrintBarCodePage()
        {
            InitializeComponent();
        }

        private void Handle_ScanTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
                return;
        }

        private async void Handle_Completed(object sender, EventArgs e)
        {
            scanEntry.Unfocus();
            scanEntry.ShowKeyboard = false;

            if (!string.IsNullOrEmpty(ViewModel.ScanCode) && !await ViewModel.ScanCodeTextChanged(ViewModel.ScanCode))
            {
                await Util.Util.ShowErrorPopupWithBeep("Item " + ViewModel.ScanCode + " not found.");
            }

            scanEntry.Text = string.Empty;
            await System.Threading.Tasks.Task.Delay(300);
            if (IsAppearing)
                scanEntry.Focus();
        }


        private async void Keyboard_Tapped(object sender, EventArgs e)
        {
            scanEntry.ShowKeyboard = !scanEntry.ShowKeyboard;
            await System.Threading.Tasks.Task.Delay(200);
            scanEntry.Focus();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            scanEntry.Focus();
            await ViewModel.GetAllProducts();
        }

        private async void Print(object sender, EventArgs e)
        {
            int numberOfPrints = 1;
            numberOfPrints = ViewModel.PrintsNumber;
            var printerHelper = DependencyService.Get<IPrinterHelper>();

            if (await printerHelper.ConnectionScheck() == false)
            {
                "Printer is not connected. Please check paired devices.".ToToast();
            }

            List<PrintLine> PrintList = new List<PrintLine>();
            var TxtHeaderHeight = new Ref<int>();
            var TxtHeight = await PrintLineHelper.GetConnectedDeviceTxtHeight(TxtHeaderHeight);
            if (TxtHeight != 0)
            {
                int PosY = 60;
                PrintList.Add(await PrintLineHelper.GetLine(ViewModel.BarCode, PosY, 320, PrintTextType.OneDBarcode));
                PosY += printerHelper.HeightForPosY(TxtHeight);
                PrintList.Add(await PrintLineHelper.GetLine(ViewModel.BarCode, PosY));
                PosY += printerHelper.HeightForPosY(TxtHeight);
                PrintList.Add(await PrintLineHelper.GetLine("", PosY));
                PosY += printerHelper.HeightForPosY(TxtHeight);

                if (ViewModel.ProductName.Length > 24)
                {
                    string nameLine1 = ViewModel.ProductName.Substring(0, 24);
                    string nameLine2 = ViewModel.ProductName.Substring(24);

                    PrintList.Add(await PrintLineHelper.GetLine(nameLine1, PosY));
                    PosY += printerHelper.HeightForPosY(TxtHeight);
                    PrintList.Add(await PrintLineHelper.GetLine(nameLine2, PosY));
                }
                else
                {
                    PrintList.Add(await PrintLineHelper.GetLine(ViewModel.ProductName, PosY));
                }


                for (int j = 0; j < numberOfPrints; j++)
                {
                    if (string.IsNullOrEmpty(await DependencyService.Get<IPrinterHelper>().Print(PrintList)))
                    {
                        "Lost Printer connection. Please check Printer".ToToast();
                        break;
                    }
                    var printerStatus = DependencyService.Get<IPrinterHelper>().GetPrinterStatus();
                    int retry = 0;
                    while (!(printerStatus == "00\r\n") && retry < 20)
                    {
                        printerStatus = DependencyService.Get<IPrinterHelper>().GetPrinterStatus();
                        retry++;
                    }
                }
            }
            else
            {
                "Printer is not connected. Please check paired devices.".ToToast();
            }
            scanEntry.Focus();
        }

        private void BarCodeTypeSelected(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(ViewModel.BarCode))
            {
                "Enter BarCode Please".ToToast();
                return;
            }
            var modePicker = (Picker)sender;
            var mode = modePicker.SelectedIndex;
            if (mode == 0)
            {
                PrintCode = ViewModel.Product.SKUCode;
                ViewModel.BarCode = PrintCode;
                ViewModel.SelectedIndex = 0;
            }
            else if (mode == 1)
            {
                PrintCode = ViewModel.Product.BarCode;
                ViewModel.BarCode = PrintCode;
                ViewModel.SelectedIndex = 1;
            }
            else if (mode == 2)
            {
                PrintCode = ViewModel.Product.BarCode2;
                ViewModel.BarCode = PrintCode;
                ViewModel.SelectedIndex = 2;
            }

        }
    }
}
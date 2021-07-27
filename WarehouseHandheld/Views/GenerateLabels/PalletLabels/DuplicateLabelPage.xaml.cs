using System;
using System.Collections.Generic;
using WarehouseHandheld.Views.Base.Popup;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using System.Threading.Tasks;
using WarehouseHandheld.Helpers;
using WarehouseHandheld.Models.DeviceSettings;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Models.Products;
using System.Diagnostics;

namespace WarehouseHandheld.Views.GenerateLabels.PalletLabels
{
    public enum GenerateLabelsPageMode
    {
        dupliatePalletLabel = 0,
        generatePalletCaseLabels = 1
    }
    public partial class DuplicateLabelPage : PopupBase
    {
        private PalletTrackingSync palletTrackingSyncObj;
        private ProductMasterSync productMasterSyncObj;
        public Action SaveQuantity;
        private bool keyboardImageTapped;

        public Action<bool> SetFocus;
        public Action Cancel;
        public GenerateLabelsPageMode pageMode;

        public DuplicateLabelPage(GenerateLabelsPageMode pMode)
        {
            InitializeComponent();
            OnSaveClicked += OnSave;
            OnCancelClicked += OnCancel;
            palletsNo.Value = 1;
            pageMode = pMode;
            Cases.IsVisible = false;
            CasesEntry.IsVisible = false;
            TotalPrintableLbl.IsVisible = false;
            TotalCount.IsVisible = false;

            if (pMode == GenerateLabelsPageMode.generatePalletCaseLabels)
            {
                Stepper.Text = "No of Copies";
                PrintButton.Text = "Print Case Labels";
                Cases.Text = "No of Cases";
                TotalPrintableLbl.Text = "Total Case Labels";
                TotalCount.Text = "";

            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Device.BeginInvokeOnMainThread(() =>
            {
                PalletEntry.Focus();
            });


        }

        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(PalletEntry.Text))
            {
                await Util.Util.ShowErrorPopupWithBeep("Enter pallet serial.");
            }

            var palletTrackingSync = await App.Pallets.GetPalletTrackingWithSerial(PalletEntry.Text);
            if (palletTrackingSync == null)
            {
                await Util.Util.ShowErrorPopupWithBeep("No pallets found match this serial.");
                return;
            }
            palletTrackingSyncObj = palletTrackingSync;

            var productMasterSync = await App.Products.GetProductById(palletTrackingSyncObj.ProductId);
            if (productMasterSync == null)
            {
                await Util.Util.ShowErrorPopupWithBeep("No product found matching pallet for this serial.");
                return;
            }
            productMasterSyncObj = productMasterSync;

            FoundPalletSerial.Text = palletTrackingSync.PalletSerial;
            FoundProductName.Text = productMasterSync.Name;

            //cases entry and label
            CasesEntry.IsVisible = true;
            CasesEntry.IsEnabled = true;
            Cases.IsVisible = true;
            CasesEntry.Text = palletTrackingSync.TotalCases.ToString();

            TotalPrintableLbl.IsVisible = true;
            TotalCount.IsVisible = true;

            TotalPrintableLbl.Text = "Total Case Labels";
            TotalCount.Text = CasesEntry.Text;


            PalletSerialLabel.IsVisible = true;
            FoundPalletSerial.IsVisible = true;
            ProductNameLabel.IsVisible = true;
            FoundProductName.IsVisible = true;
            PrintButton.IsEnabled = true;
            PalletEntry.Text = string.Empty;
            PalletEntry.Focus();
        }

        async void PrintLabel(object sender, System.EventArgs e)
        {
            try
            {
                ((Button)sender).IsEnabled = false;

                var printerHelper = DependencyService.Get<IPrinterHelper>();
                if (palletsNo.Value <= 0)
                {
                    palletsNo.Value = 1;
                }

                //check printer status
                if (await printerHelper.ConnectionScheck() == false)
                {
                    "Printer is not connected. Please check paired devices.".ToToast();
                    return;
                }

                List<PrintLine> PrintList = new List<PrintLine>();
                var TxtHeaderHeight = new Ref<int>();
                var TxtHeight = await PrintLineHelper.GetConnectedDeviceTxtHeight(TxtHeaderHeight);

                if (TxtHeight != 0)
                {
                    int PosY = 20;
                    if (pageMode == GenerateLabelsPageMode.dupliatePalletLabel)
                    {
                        PrintList.Add(await PrintLineHelper.GetLine(productMasterSyncObj.BarCode, PosY, 320, PrintTextType.OneDBarcode));
                        PosY += printerHelper.HeightForPosY(TxtHeight);
                        PrintList.Add(await PrintLineHelper.GetLine(productMasterSyncObj.Name, PosY));
                        PosY += printerHelper.HeightForPosY(TxtHeight);
                        PrintList.Add(await PrintLineHelper.GetLine("Cases: " + palletTrackingSyncObj.TotalCases, PosY));
                        PosY += printerHelper.HeightForPosY(TxtHeight);
                        PrintList.Add(await PrintLineHelper.GetLine("Batch: " + palletTrackingSyncObj.BatchNo, PosY));

                        PosY += printerHelper.HeightForPosY(TxtHeight);
                        PrintList.Add(await PrintLineHelper.GetLine(string.Format("Exp: {0:dd/MM/yy}", palletTrackingSyncObj.ExpiryDate), PosY, 320, PrintTextType.TextInternal, true));

                        PosY += printerHelper.HeightForPosY(TxtHeaderHeight);

                        PrintList.Add(await PrintLineHelper.GetLine(palletTrackingSyncObj.PalletSerial, PosY, 320, PrintTextType.OneDBarcode, false, null, true));

                        for (int i = 0; i < palletsNo.Value; i++)
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
                    else if (pageMode == GenerateLabelsPageMode.generatePalletCaseLabels)
                    {
                        if (productMasterSyncObj.CasesPerPallet == null || productMasterSyncObj.CasesPerPallet <= 0)
                        {
                            "Invalid value for cases per pallet.".ToToast();
                            return;
                        }
                        for (int i = 0; i < palletsNo.Value; i++)//No of copies of case labels
                        {
                            for (int j = 1; j <= productMasterSyncObj.CasesPerPallet; j++)//generating label for number of case times
                            {
                                PrintList.Add(await PrintLineHelper.GetLine(productMasterSyncObj.Name, PosY));
                                PosY += printerHelper.HeightForPosY(TxtHeight);
                                PrintList.Add(await PrintLineHelper.GetLine(palletTrackingSyncObj.PalletSerial + "-" + j, PosY, 320, PrintTextType.OneDBarcode, false, null, true));

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
                                PrintList.Clear();
                                PosY -= printerHelper.HeightForPosY(TxtHeight);
                            }
                        }
                    }
                }

                else
                {
                    "Printer is not connected. Please check paired devices.".ToToast();
                    return;
                }
                await Task.Delay(300);
            }
            finally
            {
                ((Button)sender).IsEnabled = true;
                FoundProductName.IsVisible = false;
                FoundProductName.Text = string.Empty;
                FoundPalletSerial.IsVisible = false;
                FoundPalletSerial.Text = string.Empty;
                ProductNameLabel.IsVisible = false;
                PalletSerialLabel.IsVisible = false;
                CasesEntry.Text = null;
                TotalCount.Text = null;
                PalletEntry.Focus();
            }
        }

        void OnSave()
        {
            SetFocus?.Invoke(true);
            PopupNavigation.PopAsync();
        }

        void OnCancel()
        {
            SetFocus?.Invoke(true);
            PopupNavigation.PopAsync();
        }

        async void Keyboard_Tapped(object sender, System.EventArgs e)
        {
            if (PalletEntry.ShowKeyboard == false)
            {
                keyboardImageTapped = true;
                PalletEntry.ShowKeyboard = !PalletEntry.ShowKeyboard;
                PalletEntry.ShowKeyboard = true;
                await System.Threading.Tasks.Task.Delay(200);
                PalletEntry.Focus();
            }
            else
            {
                PalletEntry.ShowKeyboard = false;
            }

        }
        private void scanEntry_Focused(object sender, FocusEventArgs e)
        {
            if (keyboardImageTapped)
            {
                PalletEntry.ShowKeyboard = true;
                keyboardImageTapped = false;
            }
            else
            {
                PalletEntry.ShowKeyboard = false;
            }
        }

        void CasesEntry_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            updatePrintCount();
        }

        void palletsNo_ValueChanged(System.Object sender, Xamarin.Forms.ValueChangedEventArgs e)
        {
            updatePrintCount();
        }

        void updatePrintCount()
        {
            if (productMasterSyncObj == null)
                return;
            if (!string.IsNullOrEmpty(CasesEntry.Text))
            {
                int caseTextEntry = int.Parse(CasesEntry.Text);
                productMasterSyncObj.CasesPerPallet = caseTextEntry;
                int copies = (int)(palletsNo.Value);
                TotalCount.Text = (copies * caseTextEntry).ToString();
            }
        }
    }
}

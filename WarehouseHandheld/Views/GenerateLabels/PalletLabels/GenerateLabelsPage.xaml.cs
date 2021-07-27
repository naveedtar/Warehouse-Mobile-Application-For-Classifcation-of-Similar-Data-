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
using WarehouseHandheld.ViewModels.GenerateLabels;
using WarehouseHandheld.Views.Base.BaseContentPage;
using WarehouseHandheld.Views.DeviceSettings;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.GenerateLabels.PalletLabels
{
    public partial class GenerateLabelsPage : BasePage
    {
        GenerateLabelsViewModel ViewModel => BindingContext as GenerateLabelsViewModel;
        PalletTrackingSyncCollection palletTrackingSyncCollection = new PalletTrackingSyncCollection();
        private bool keyboardImageTapped;

        public GenerateLabelsPage(ProductMasterSync product)
        {
            InitializeComponent();
            palletTrackingSyncCollection.PalletTrackingSync = new List<PalletTrackingSync>();
            var todayDate = DateTime.Today;
            var lastDayOfMonth = DateTime.DaysInMonth(todayDate.Year, todayDate.Month);
            var lastDateOfMonth = todayDate.AddDays(lastDayOfMonth - todayDate.Day);
            expiryDate.Date = DateTime.Today;
            expiryDate.Format = "dd/MM/yy";
            GetSelectedProduct(product);

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            scanEntry.Focus();
        }

        async void Handle_Completed(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewModel.ScanCode) && !ViewModel.ScanCodeTextChanged(ViewModel.ScanCode))
            {
                ProductNameLabel.Text = string.Empty;
                ProductNameLabel.IsVisible = false;
                if (ViewModel.SelectedProduct != null && !ViewModel.SelectedProduct.ProcessByPallet)
                {
                    await Util.Util.ShowErrorPopupWithBeep(ViewModel.SelectedProduct.Name + " is not process by pallet.");
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
                    Cases.Text = ViewModel.SelectedProduct.CasesPerPallet.ToString();
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
            if (string.IsNullOrEmpty(CommentText.Text))
                CommentText.Text = " ";

            if (expiryDate.Date <= DateTime.Today)
            {
                "Please select expiry date".ToToast();
            }
            else if (palletsNo.Value == 0 || labelNo.Value == 0 || expiryDate.Date < DateTime.Today || string.IsNullOrEmpty(Cases.Text))
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

                for (int i = 0; i < palletsNo.Value; i++)
                {
                    // check printer status
                    if (await printerHelper.ConnectionScheck() == false)
                    {
                        "Printer is not connected. Please check paired devices.".ToToast();
                        break;
                    }

                    List<PrintLine> PrintList = new List<PrintLine>();
                    var TxtHeaderHeight = new Ref<int>();
                    var TxtHeight = await PrintLineHelper.GetConnectedDeviceTxtHeight(TxtHeaderHeight);
                    if (TxtHeight != 0)
                    {
                        int PosY = 20;
                        PrintList.Add(await PrintLineHelper.GetLine(ViewModel.SelectedProduct.BarCode, PosY, 320, PrintTextType.OneDBarcode));
                        PosY += printerHelper.HeightForPosY(TxtHeight);
                        PrintList.Add(await PrintLineHelper.GetLine(ViewModel.SelectedProduct.Name, PosY));
                        PosY += printerHelper.HeightForPosY(TxtHeight);
                        PrintList.Add(await PrintLineHelper.GetLine("Cases: " + Cases.Text, PosY));
                        PosY += printerHelper.HeightForPosY(TxtHeight);
                        PrintList.Add(await PrintLineHelper.GetLine("Batch: " + BatchNo.Text, PosY));

                        PosY += printerHelper.HeightForPosY(TxtHeight);
                        PrintList.Add(await PrintLineHelper.GetLine("Exp: " + expiryDate.Date.ToString("dd/MM/yy"), PosY, 320, PrintTextType.TextTrueType, true));

                        PosY += printerHelper.HeightForPosY(TxtHeaderHeight);

                        var date = DateTime.Now.Date.ToString("ddMMyy");
                        date += DateTime.Now.ToString("HHmmss");
                        date += GenerateRandomNo();
                        PrintList.Add(await PrintLineHelper.GetLine(date, PosY, 320, PrintTextType.OneDBarcode, false, null, true));


                        bool IsLabelPrinted = false;
                        for (int j = 0; j < labelNo.Value; j++)
                        {
                            if (string.IsNullOrEmpty(await DependencyService.Get<IPrinterHelper>().Print(PrintList)))
                            {
                                "Lost Printer connection. Please check Printer".ToToast();
                                break;
                            }

                            IsLabelPrinted = true;
                            IsPrint = true;
                            var printerStatus = DependencyService.Get<IPrinterHelper>().GetPrinterStatus();
                            int retry = 0;
                            while (!(printerStatus == "00\r\n") && retry < 20)
                            {
                                printerStatus = DependencyService.Get<IPrinterHelper>().GetPrinterStatus();
                                retry++;
                            }

                        }
                        if (IsLabelPrinted)
                            AddtoPalletTrackingSyncCollection(date);
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
                    await AddPalletTrackingSyncCollectionToSyncLog();
                    ViewModel.SelectedProduct = null;
                    palletsNo.Value = 0;
                    labelNo.Value = 0;
                    ViewModel.ScanCode = string.Empty;
                    Cases.Text = string.Empty;
                    BatchNo.Text = string.Empty;
                    ProductNameLabel.Text = string.Empty;
                    ProductNameLabel.IsVisible = false;
                    expiryDate.Date = DateTime.Today;
                    await Task.Delay(300);
                    scanEntry.Focus();
                    bool doesPageExists = Navigation.NavigationStack.Any(p => p is GenerateLabelsPage);
                    if (doesPageExists)
                    {
                        await Navigation.PopAsync();
                    }
                }
            }
            ((Button)sender).IsEnabled = true;
        }

        void AddtoPalletTrackingSyncCollection(string serial)
        {
            PalletTrackingSync palletTracking = new PalletTrackingSync();
            palletTracking.BatchNo = BatchNo.Text;
            palletTracking.DateCreated = DateTime.Now;
            palletTracking.ExpiryDate = expiryDate.Date;
            palletTracking.PalletSerial = serial;
            palletTracking.TotalCases = Convert.ToDecimal(Cases.Text);
            palletTracking.RemainingCases = Convert.ToDecimal(Cases.Text);
            palletTracking.ProductId = ViewModel.SelectedProduct.ProductId;
            palletTracking.Status = PalletTrackingStatusEnum.Created;
            palletTracking.TenantId = ModulesConfig.TenantID;
            palletTracking.WarehouseId = ModulesConfig.WareHouseID;
            palletTracking.Comments = CommentText.Text;
            palletTrackingSyncCollection.PalletTrackingSync.Add(palletTracking);
        }

        async Task AddPalletTrackingSyncCollectionToSyncLog()
        {
            palletTrackingSyncCollection.TerminalLogId = new Guid();
            palletTrackingSyncCollection.TransactionLogId = Guid.NewGuid();
            palletTrackingSyncCollection.SerialNo = ModulesConfig.SerialNo;
            palletTrackingSyncCollection.Count = palletTrackingSyncCollection.PalletTrackingSync.Count;
            await ViewModel.AddSyncLog(palletTrackingSyncCollection);
            palletTrackingSyncCollection.PalletTrackingSync.Clear();
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

        async void DuplicateLabel(object sender, System.EventArgs e)
        {
            DuplicateButton.IsEnabled = false;
            var duplicatePage = new DuplicateLabelPage(GenerateLabelsPageMode.dupliatePalletLabel);
            duplicatePage.SetFocus += DuplicatePage_SetFocus;
            await PopupNavigation.PushAsync(duplicatePage);
            DuplicateButton.IsEnabled = true;
        }

        void DuplicatePage_SetFocus(bool obj)
        {
            scanEntry.Focus();
        }

        void GetSelectedProduct(ProductMasterSync productMasterSync)
        {
            ProductNameLabel.Text = productMasterSync.Name;
            ProductNameLabel.IsVisible = true;
            ViewModel.SelectedProduct = productMasterSync;
            if (ViewModel.SelectedProduct.CasesPerPallet != null)
            {
                Cases.Text = ViewModel.SelectedProduct.CasesPerPallet.ToString();
            }

            "Product Selected".ToToast();
        }

    }
}

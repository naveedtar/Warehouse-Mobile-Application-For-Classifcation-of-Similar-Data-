using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Models.OrderProcesses;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Models.StockMovement;
using WarehouseHandheld.Models.Users;
using WarehouseHandheld.Models.Vehicles;
using WarehouseHandheld.Resources;
using WarehouseHandheld.ViewModels.OrderItems;
using WarehouseHandheld.Views.Base.BaseContentPage;
using WarehouseHandheld.Views.Sync;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace WarehouseHandheld.Views.OrderItems
{
    public partial class PalletTrackingScanPage : BasePage
    {
        public PalletTrackingScanViewModel ViewModel;
        TerminalMetadataSync Terminal;

        public Action<List<PalleTrackingProcess>, decimal> PalletListAdded;
        public Action<StockMovementViewModel> StockAdded;

        public Action CancelClicked;
        public Action<bool> SetShouldAllowOverridPerm;
        bool isAppearingFirstTime = true;
        bool isPurchase = false;
        bool isStockTake = false;
        bool isReturnOrWastage = false;
        public bool mandatoryLocationScan;

        int palletSerialTrackingProductId;

        public PalletTrackingScanPage(ProductMasterSync product, bool IsPurchase, OrderDetailsProduct orderDetail, UserSync User, OrderProcessDetailSync details, List<OrderProcessDetailSync> processDetailSyncList, bool shouldAllowOverrid, string palletSerial = null, bool isStkTake = false)
        {
            InitializeComponent();
            isPurchase = IsPurchase;
            isStockTake = isStkTake;
            Constants.SetGridProperties(grid);
            ViewModel = new PalletTrackingScanViewModel(IsPurchase, product, shouldAllowOverrid, isStockTake);
            ViewModel.OrderDetails = orderDetail;
            BindingContext = ViewModel;


            if (orderDetail != null)
            {
                ViewModel.Progress = orderDetail.QuantityProcessed + "/" + orderDetail.OrderDetails.Qty;
            }
            if (ViewModel.PalletTrackingProcessInOrderProcessesOfSameProduct == null)
            {
                ViewModel.PalletTrackingProcessInOrderProcessesOfSameProduct = new List<PalleTrackingProcess>();
            }
            if (processDetailSyncList != null && processDetailSyncList.Count != 0)
            {
                foreach (var processDetailSync in processDetailSyncList)
                {
                    if (processDetailSync != null && processDetailSync.PalleTrackingProcess?.Count > 0)
                    {
                        foreach (var palleTracking in processDetailSync.PalleTrackingProcess)
                        {
                            ViewModel.PalletTrackingProcessInOrderProcessesOfSameProduct.Add(palleTracking);
                        }
                    }
                }
            }
            if (details != null && details.PalleTrackingProcess?.Count > 0)
            {
                ViewModel.PalletTrackingProcesses = details.PalleTrackingProcess;
            }
            else
            {
                ViewModel.PalletTrackingProcesses = new List<PalleTrackingProcess>();
            }
            if (!IsPurchase)
            {
                ActivePalletLayout.IsVisible = true;
                NextButton.IsVisible = true;

            }

            if (palletSerial != null)
            {
                serialScanEntry.Text = palletSerial;
                AutoPalletScanning();
            }

            ViewModel.SetFocus += SetScanEntryFocus;
            ViewModel.SetOverridPerm += (bool perm) =>
            {
                SetShouldAllowOverridPerm?.Invoke(perm);
            };

            //To check for Mandatory Location..
            GetMadatoryLocationDetails();
        }

        public async void GetMadatoryLocationDetails()
        {
            try
            {
                Terminal = await App.Database.Vehicle.GetTerminalMetaData();
                if (Terminal != null)
                {
                    if (Terminal.MandatoryLocationScan)
                    {
                        StkLocation.IsVisible = true;
                        serialScanEntry.IsEnabled = false;
                        LocationEntry.Focus();
                    }
                    else
                    {
                        StkLocation.IsVisible = false;
                        serialScanEntry.IsEnabled = true;
                    }
                    mandatoryLocationScan = Terminal.MandatoryLocationScan;
                }
            }
            catch (Exception ex)
            {
                var exMsg = ex.Message;
            }
        }
        
        //Used In Case of Returns / Wastages
        public PalletTrackingScanPage(ProductMasterSync product, OrderDetailsProduct orderDetail, bool returnOrWastage, bool isWastages)
        {
            InitializeComponent();

            Constants.SetGridProperties(grid);
            isReturnOrWastage = returnOrWastage;
            ViewModel = new PalletTrackingScanViewModel(product); ViewModel.OrderDetails = orderDetail;
            ViewModel.IsWastages = isWastages;
            BindingContext = ViewModel;

            if (orderDetail != null)
            {
                ViewModel.Progress = orderDetail.QuantityProcessed + "/" + orderDetail.OrderDetails.Qty;
            }
            if (ViewModel.PalletTrackingProcessInOrderProcessesOfSameProduct == null)
            {
                ViewModel.PalletTrackingProcessInOrderProcessesOfSameProduct = new List<PalleTrackingProcess>();
            }

            ViewModel.PalletTrackingProcesses = new List<PalleTrackingProcess>();

            ViewModel.SetFocus += SetScanEntryFocus;
            ViewModel.SetOverridPerm += (bool perm) =>
            {
                SetShouldAllowOverridPerm?.Invoke(perm);
            };
        }

        
        //Constructor used for Stock Movement
        public PalletTrackingScanPage(ProductMasterSync product, StockMovementViewModel stockMovementViewModel)
        {
            InitializeComponent();

            Constants.SetGridProperties(grid);
            ViewModel = new PalletTrackingScanViewModel(product);
            ViewModel.IsStockMovement = true;
            ViewModel.StockMovement = stockMovementViewModel;
            BindingContext = ViewModel;
            ViewModel.SetFocus += SetScanEntryFocus;

            if (ViewModel.PalletTrackingProcessInOrderProcessesOfSameProduct == null)
            {
                ViewModel.PalletTrackingProcessInOrderProcessesOfSameProduct = new List<PalleTrackingProcess>();
            }

            ViewModel.PalletTrackingProcesses = new List<PalleTrackingProcess>();

        }

        void SetScanEntryFocus()
        {
            //serialScanEntry.Unfocus();
            Task.Run(async () =>
            {
                await System.Threading.Tasks.Task.Delay(700);
                Device.BeginInvokeOnMainThread(() =>
                {
                    serialScanEntry.Focus();
                });
            });
        }

        void PalletTrackingScanPopup_OnSaveClicked(object sender, EventArgs e)
        {
            SaveToolbarItem.IsEnabled = false;

            if (ViewModel.OrderDetails != null && ViewModel.OrderDetails.IsProductInKit && (ViewModel.ScannedPallets.Sum(x => x.Quantity) % ViewModel.OrderDetails.KitQuantity != 0))
            {
                SaveToolbarItem.IsEnabled = true;
                Util.Util.ShowErrorPopupWithBeep("Complete bundle to process");
                return;
            }
            if (ViewModel.IsStockMovement)
            {
                    StockAdded?.Invoke(ViewModel.StockMovement);
            }
            else
            {
                if (ViewModel.ScannedPallets.Count > 0)
                {
                    ViewModel.PalletTrackingProcessInOrderProcessesOfSameProduct.ToList().ForEach(x => x.IsIncluded = true);
                    ViewModel.PalletTrackingProcesses.ToList().ForEach(x => x.IsIncluded = true);
                    PalletListAdded?.Invoke(ViewModel.PalletTrackingProcesses, ViewModel.ScannedPallets.Sum((x) => x.Quantity));
                }

            }

            //PalletListAdded?.Invoke(ViewModel.PalletTrackingProcesses, ViewModel.ScannedPallets.Sum((x) => x.Cases));
            this.Navigation.PopModalAsync();
        }

        private async void OnSave()
        {
            SaveToolbarItem.IsEnabled = false;

            if (ViewModel.OrderDetails != null && ViewModel.OrderDetails.IsProductInKit && (ViewModel.ScannedPallets.Sum(x => x.Quantity) % ViewModel.OrderDetails.KitQuantity != 0))
            {
                SaveToolbarItem.IsEnabled = true;
                await Util.Util.ShowErrorPopupWithBeep("Complete bundle to process");
                return;
            }
            if (ViewModel.IsStockMovement)
            {
                    StockAdded?.Invoke(ViewModel.StockMovement);
            }
            else
            {
                if (ViewModel.ScannedPallets.Count > 0)
                {
                    ViewModel.PalletTrackingProcessInOrderProcessesOfSameProduct.ToList().ForEach(x => x.IsIncluded = true);
                    ViewModel.PalletTrackingProcesses.ToList().ForEach(x => x.IsIncluded = true);
                    PalletListAdded?.Invoke(ViewModel.PalletTrackingProcesses, ViewModel.ScannedPallets.Sum((x) => x.Quantity));
                }

            }

            //PalletListAdded?.Invoke(ViewModel.PalletTrackingProcesses, ViewModel.ScannedPallets.Sum((x) => x.Cases));
            await this.Navigation.PopModalAsync();
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (ViewModel.OrderDetails != null)
            {
                var order = await App.Database.Orders.GetOrderById(ViewModel.OrderDetails.OrderDetails.OrderID);
                if (order != null)
                    ViewModel.Account = await App.Database.Accounts.GetAccountById((int)order.AccountID);
            }

            if (isAppearingFirstTime)
            {
                if (!isPurchase)
                {
                    await ViewModel.GetPalletTrackingsForGoodsOut();
                }
                isAppearingFirstTime = false;
            }
            var syncAlertPopupPage = new SyncAlertPopupPage("Syncing Pallet Trackings, Please wait.", "Pallets-Tracking sync failed, Please check your connection or contact support. Do you wish to proceed?", Constants.PalletTrackingSync, "Pallet tracking sync completed successfully.");
            //await PopupNavigation.Instance.PushAsync(new SyncAlertPopupPage("Syncing Pallet Trackings, Please wait.", "Pallets-Tracking sync failed, Please check your connection or contact support. Do you wish to proceed?", Constants.PalletTrackingSync, "Pallet tracking sync completed successfully."));
            await PopupNavigation.Instance.PushAsync(syncAlertPopupPage);
            syncAlertPopupPage.SetFocus += () =>
            {
                if (mandatoryLocationScan)
                {
                    LocationEntry.Focus();
                }
                else
                {
                    PopupAppeared();
                }
            };
        }

        async void LocationEntry_Unfocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            if (!string.IsNullOrEmpty(LocationEntry.Text))
            {
                LocationSync locationSync = await App.Database.StockMovements.GetStockLocationByLocationCode(LocationEntry.Text);
                if (locationSync != null)
                {
                    serialScanEntry.IsEnabled = true;
                }
            }
        }
        async void Handle_Location_Completed(System.Object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(LocationEntry.Text))
            {
                LocationSync locationSync = await App.Database.StockMovements.GetStockLocationByLocationCode(LocationEntry.Text);
                if (locationSync == null)
                {
                    await Util.Util.ShowErrorPopupWithBeep("Invalid location code scanned.");
                    LocationEntry.Text = "";
                    LocationEntry.Focus();
                }
                else if (ViewModel.OrderDetails.QuantityProcessed + 1 == ViewModel.OrderDetails.OrderDetails.Qty && mandatoryLocationScan)
                {

                    OnSave();
                }
                else
                {
                    ViewModel.OrderDetails.LocationCode = locationSync.LocationCode;
                    serialScanEntry.Focus();
                    serialScanEntry.IsEnabled = true;
                    // QuantityStepper.IsEnabled = true;
                }
            }
        }
        public async void PopupAppeared()
        {
            SetScanEntryFocus();
        }

        async void Keyboard_Tapped(object sender, System.EventArgs e)
        {
            serialScanEntry.ShowKeyboard = !serialScanEntry.ShowKeyboard;
            await System.Threading.Tasks.Task.Delay(200);
            serialScanEntry.Focus();
        }

        async void Handle_Completed(object sender, System.EventArgs e)
        {

            if (!string.IsNullOrEmpty(serialScanEntry.Text))
            {
                if (isReturnOrWastage)
                {
                    await ViewModel.ScanWastageReturnPallet(serialScanEntry.Text);
                    serialScanEntry.Text = string.Empty;
                    await System.Threading.Tasks.Task.Delay(200);
                    serialScanEntry.Focus();
                    return;
                }

                if (ViewModel.IsStockMovement)
                {
                    await ViewModel.ScanStockMovePallet(serialScanEntry.Text);
                    serialScanEntry.Text = string.Empty;
                    await System.Threading.Tasks.Task.Delay(200);
                    serialScanEntry.Focus();
                    return;
                }

                if (ViewModel.IsPurchase)
                {
                    await ViewModel.Scan(serialScanEntry.Text);
                }
                else
                {
                    await ViewModel.ScanActivePallet(serialScanEntry.Text);
                }

                OnSave();

                serialScanEntry.Text = string.Empty;
                await System.Threading.Tasks.Task.Delay(200);
                serialScanEntry.Focus();
            }
        }

        private async void AutoPalletScanning()
        {
            await Task.Delay(1000);

            if (!string.IsNullOrEmpty(serialScanEntry.Text))
            {
                if (isReturnOrWastage)
                {
                    await ViewModel.ScanWastageReturnPallet(serialScanEntry.Text);
                    serialScanEntry.Text = string.Empty;
                    await System.Threading.Tasks.Task.Delay(200);
                    serialScanEntry.Focus();
                    return;
                }

                if (ViewModel.IsStockMovement)
                {
                    await ViewModel.ScanStockMovePallet(serialScanEntry.Text);
                    serialScanEntry.Text = string.Empty;
                    await System.Threading.Tasks.Task.Delay(200);
                    serialScanEntry.Focus();
                    return;
                }

                if (ViewModel.IsPurchase)
                {
                    await ViewModel.Scan(serialScanEntry.Text);
                }
                else
                {
                    await ViewModel.ScanActivePallet(serialScanEntry.Text);
                }

                OnSave();

                serialScanEntry.Text = string.Empty;
                await System.Threading.Tasks.Task.Delay(200);
                serialScanEntry.Focus();
            }
        }

        async void Next_Clicked(object sender, System.EventArgs e)
        {
            serialScanEntry.Text = string.Empty;
            await ViewModel.NextActivePallet();
        }

        async void Back_Clicked(object sender, System.EventArgs e)
        {
            if (await ViewModel.CancelPopupClicked())
            {
                await this.Navigation.PopModalAsync();
                CancelClicked?.Invoke();
            }
        }
    }
}

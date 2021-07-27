using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Models.OrderProcesses;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Models.Users;
using WarehouseHandheld.Resources;
using WarehouseHandheld.ViewModels.OrderItems;
using WarehouseHandheld.Views.Base.Popup;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.OrderItems
{
    public partial class PalletTrackingScanPopup : PopupBase
    {
        PalletTrackingScanViewModel ViewModel;
        public Action<List<PalleTrackingProcess>, decimal> PalletListAdded;
        public Action CancelClicked;
        public Action<bool> SetShouldAllowOverridPerm;
        bool isAppearingFirstTime = true;
        bool isPurchase = false;
        bool isStockTake = false;
        bool isReturnOrWastage = false;
        public PalletTrackingScanPopup(ProductMasterSync product, bool IsPurchase, OrderDetailsProduct orderDetail, UserSync User, OrderProcessDetailSync details, List<OrderProcessDetailSync> processDetailSyncList, bool shouldAllowOverrid, bool isStkTake = false)
        {
            InitializeComponent();
            isPurchase = IsPurchase;
            isStockTake = isStkTake;
            Constants.SetGridProperties(grid);
            ViewModel = new PalletTrackingScanViewModel(IsPurchase, product, shouldAllowOverrid, isStockTake);
            ViewModel.OrderDetails = orderDetail;
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
            BindingContext = ViewModel;
            if (!IsPurchase)
            {
                ActivePalletLayout.IsVisible = true;
                NextButton.IsVisible = true;

            }
            DisableCancelPop = true;
            OnCancelClicked += async () => { 
                
                if (await ViewModel.CancelPopupClicked())
                {
                    await PopupNavigation.PopAsync();
                    CancelClicked?.Invoke();
                }
            };
            OnSaveClicked += PalletTrackingScanPopup_OnSaveClicked;
            ViewModel.SetFocus += SetScanEntryFocus;
            ViewModel.SetOverridPerm += (bool perm) => {
                SetShouldAllowOverridPerm?.Invoke(perm);  
            };
        }


        public PalletTrackingScanPopup(ProductMasterSync product, OrderDetailsProduct orderDetail, bool returnOrWastage, bool isWastages)
        {
            InitializeComponent();

            Constants.SetGridProperties(grid);
            isReturnOrWastage = returnOrWastage;
            ViewModel = new PalletTrackingScanViewModel(product);
            ViewModel.OrderDetails = orderDetail;
            ViewModel.IsWastages = isWastages;
            if (orderDetail != null)
            {
                ViewModel.Progress = orderDetail.QuantityProcessed + "/" + orderDetail.OrderDetails.Qty;
            }
            if (ViewModel.PalletTrackingProcessInOrderProcessesOfSameProduct == null)
            {
                ViewModel.PalletTrackingProcessInOrderProcessesOfSameProduct = new List<PalleTrackingProcess>();
            }

            ViewModel.PalletTrackingProcesses = new List<PalleTrackingProcess>();
            BindingContext = ViewModel;

            DisableCancelPop = true;
            OnCancelClicked += async () => {

                if (await ViewModel.CancelPopupClicked())
                {
                    await PopupNavigation.PopAsync();
                    CancelClicked?.Invoke();
                }
            };
            OnSaveClicked += PalletTrackingScanPopup_OnSaveClicked;
            ViewModel.SetFocus += SetScanEntryFocus;
            ViewModel.SetOverridPerm += (bool perm) => {
                SetShouldAllowOverridPerm?.Invoke(perm);
            };
        }

        void SetScanEntryFocus()
        {
            serialScanEntry.Unfocus();
            Task.Run(async() =>
            {

                await System.Threading.Tasks.Task.Delay(700);
                Device.BeginInvokeOnMainThread(() => { 
                serialScanEntry.Focus();
                });
            });
        }

        void PalletTrackingScanPopup_OnSaveClicked()
        {
            if (ViewModel.ScannedPallets.Count > 0)
                PalletListAdded?.Invoke(ViewModel.PalletTrackingProcesses, ViewModel.ScannedPallets.Sum((x) => x.Quantity));
            
            //PalletListAdded?.Invoke(ViewModel.PalletTrackingProcesses, ViewModel.ScannedPallets.Sum((x) => x.Cases));
            PopupNavigation.PopAsync();
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (isAppearingFirstTime)
            {
                if (!isPurchase)
                {
                    await ViewModel.GetPalletTrackingsForGoodsOut();   
                }
                isAppearingFirstTime = false;
            }
            PopupAppeared();
        }

        public async void PopupAppeared ()
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
                if (ViewModel.IsPurchase)
                {
                    await ViewModel.Scan(serialScanEntry.Text);
                }
                else
                {
                    await ViewModel.ScanActivePallet(serialScanEntry.Text);
                }
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
    }
}

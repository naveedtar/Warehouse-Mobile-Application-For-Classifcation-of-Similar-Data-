using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using WarehouseHandheld.ViewModels.ProductQuantity;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Views.Base.Popup;
using System.Diagnostics;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.ViewModels.ScanItems;
using static WarehouseHandheld.Models.Orders.OrdersSync;
using WarehouseHandheld.Extensions;
using System.Linq;
using WarehouseHandheld.Models.StockMovement;

namespace WarehouseHandheld.Views.ScanItems
{
    public partial class ScanSerialItemsPopup : PopupBase
    {

        public bool mandatoryLocationScan;

        public new ScanItemsViewModel ViewModel => BindingContext as ScanItemsViewModel;

        public ScanSerialItemsPopup(OrderDetailsProduct orderDetail, OrderAccount orderAccount, bool mandatoryLocationSelection)
        {
            InitializeComponent();
            ViewModel.OrderDetail = orderDetail;
            ViewModel.Order = orderAccount;
            ViewModel.Initialize(orderDetail);
            mandatoryLocationScan = mandatoryLocationSelection;
            ViewModel.OrderType = (InventoryTransactionTypeEnum)orderAccount.Order.InventoryTransactionTypeId;
            ViewModel.SetFocus+= ViewModel_SetFocus;
            ViewModel.IsSerialized = true;
            OnSaveClicked += OnSave;
            OnCancelClicked += OnCancel;
            if (mandatoryLocationScan)
            {
                LocationEntry.Focus();
                serialScanEntry.IsEnabled = false;
            }
            else
            {
                serialScanEntry.Focus();
            }
            ViewModel.Quantity = 0;

        }

        // Initialized for Returns
        public ScanSerialItemsPopup(OrderDetailsProduct orderDetail,bool isWastages)
        {
            InitializeComponent();
            ViewModel.OrderDetail = orderDetail;
            ViewModel.Initialize(orderDetail);
            ViewModel.IsWastages = isWastages;
            ViewModel.IsReturn = true;
            ViewModel.SetFocus += ViewModel_SetFocus;
            ViewModel.OrderType = InventoryTransactionTypeEnum.Returns;
            ViewModel.IsSerialized = true;
            OnSaveClicked += OnSave;
            OnCancelClicked += OnCancel;
            serialScanEntry.Focus();
            ViewModel.Quantity = 0;
        }

        async void ViewModel_SetFocus(bool obj)
        {
            serialScanEntry.Unfocus();
            await System.Threading.Tasks.Task.Delay(400);
            serialScanEntry.Focus();
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.LoadSerials();
            serialScanEntry.Unfocus();
            await System.Threading.Tasks.Task.Delay(400);
            if (mandatoryLocationScan)
            {
                LocationEntry.Focus();
                serialScanEntry.IsEnabled = false;
            }
            else
            {
                serialScanEntry.Focus();
            }
        }

        private async void OnCancel()
        {
            if (await ViewModel.OnSerialCancelClicked())
                await PopupNavigation.PopAsync();


        }

        private async void OnSave()
        {
            LocationSync locationSync = await App.Database.StockMovements.GetStockLocationByLocationCode(LocationEntry.Text);
            if (mandatoryLocationScan && locationSync != null)
            {
                if (ViewModel.OrderDetail.IsProductInKit && (ViewModel.SerialsAdded.Count() % ViewModel.OrderDetail.KitQuantity != 0))
                {
                    await Util.Util.ShowErrorPopupWithBeep("Complete bundle to process");
                    SaveButtonEnabled = true;
                    return;
                }
                ViewModel.OnSaveSerialsClicked();
                await PopupNavigation.PopAsync();
            }
            else if (mandatoryLocationScan && locationSync == null)
            {
                await Util.Util.ShowErrorPopupWithBeep("Your location code is invalid!");
                return;
            }
            else if (!mandatoryLocationScan && locationSync == null)
            {
                if (ViewModel.OrderDetail.IsProductInKit && (ViewModel.SerialsAdded.Count() % ViewModel.OrderDetail.KitQuantity != 0))
                {
                    await Util.Util.ShowErrorPopupWithBeep("Complete bundle to process");
                    SaveButtonEnabled = true;
                    return;
                }
                ViewModel.OnSaveSerialsClicked();
                await PopupNavigation.PopAsync();
            }
          
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
            if (!string.IsNullOrEmpty(ViewModel.ProductCode) && !(await ViewModel.ScanSerialChanged(ViewModel.ProductCode)))
            {
                await Util.Util.ShowErrorPopupWithBeep("Item " + ViewModel.ProductCode + " not found.");
            }
            serialScanEntry.Text = string.Empty;
            await System.Threading.Tasks.Task.Delay(300);
            serialScanEntry.Focus();

            if(ViewModel.Quantity == ViewModel.MaxQuantity)
            {
                ViewModel.OnSaveSerialsClicked();
                await PopupNavigation.PopAsync();
            }
        }

        async void Keyboard_Tapped(object sender, System.EventArgs e)
        {
            serialScanEntry.ShowKeyboard = !serialScanEntry.ShowKeyboard;
            await System.Threading.Tasks.Task.Delay(200);
            serialScanEntry.Focus();
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
                else if (ViewModel.OrderDetail.QuantityProcessed + 1 == ViewModel.OrderDetail.OrderDetails.Qty && mandatoryLocationScan)
                {
                    OnSave();
                }
                else
                {
                    ViewModel.OrderDetail.LocationCode = locationSync.LocationCode;
                    serialScanEntry.Focus();
                }
            }
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


    }
}

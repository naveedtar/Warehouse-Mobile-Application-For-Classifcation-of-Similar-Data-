using System;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Views.Base.Popup;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.ViewModels.ScanItems;
using WarehouseHandheld.Models.StockMovement;

namespace WarehouseHandheld.Views.ScanItems
{
    public partial class ScanNonSerialItemsPopup : PopupBase
    {
        public bool IsWastage = false;
        public Action<bool> Back;
        public bool mandatoryLocationScan;
        OrderDetailsProduct oDetail;
        
        public new ScanItemsViewModel ViewModel => BindingContext as ScanItemsViewModel;
        public ScanNonSerialItemsPopup(OrderDetailsProduct orderDetail, OrderAccount orderAccount, bool mandatoryLocationScaning)
        {
            InitializeComponent();
           
            ViewModel.Initialize(orderDetail);
            ViewModel.Order = orderAccount;
            mandatoryLocationScan = mandatoryLocationScaning;
            OnSaveClicked += OnSave;
            OnCancelClicked += OnCancel;
            ViewModel.SetFocus+= ViewModel_SetFocus;
            oDetail = orderDetail;
            if (orderDetail.IsProductInKit)
            {
                ViewModel.IncrementValue = orderDetail.KitQuantity;
                ViewModel.IsDisableEntry = true;
                AddCasesLabel.IsVisible = false;
                CasesSwitch.IsVisible = false;
            }
            if (orderDetail.Product.RequiresBatchNumberOnReceipt ?? false)
            {
                    BatchNumberLabel.IsVisible = true;
                    BatchNumber.IsVisible = true;
            }

            if (orderDetail.Product.RequiresExpiryDateOnReceipt ?? false)
            {
                ExpiryDateLabel.IsVisible = true;
                ExpiryDatePicker.IsVisible = true;
            }
            if (mandatoryLocationScan)
            {
                LocationEntry.Focus();
                QuantityStepper.IsEnabled = false;
                scanEntry.IsEnabled = false;
            }
            else
            {
                scanEntry.Focus();
            }
        }

        //No Reference for this constructor, dont know what its for
        public ScanNonSerialItemsPopup(OrderDetailsProduct orderDetail)
        {
            InitializeComponent();
            oDetail = orderDetail;
            ViewModel.Initialize(orderDetail);
            if (orderDetail == null)
            {
                LocationLabel.IsVisible = false;
                LocationEntry.IsVisible = false;
                ProdCodeLabel.IsVisible = false;
                scanEntry.IsVisible = false;
                ItemScanLabel.IsVisible = false;
            }
            //OnSaveClicked += OnSave;
            OnCancelClicked += OnCancel;
            ViewModel.SetFocus += ViewModel_SetFocus;
            if (mandatoryLocationScan)
            {
                LocationEntry.Focus();
                scanEntry.IsEnabled = false;
                QuantityStepper.IsEnabled = false;
            }

        }


        //Initialized from Returns
        public ScanNonSerialItemsPopup(OrderDetailsProduct orderDetail, ProductMasterSync product)
        {
            InitializeComponent();
            oDetail = orderDetail;
            ViewModel.InitializeReturn(orderDetail);
            IsWastage = true;
            if (orderDetail.OrderDetails == null)
            {
                LocationLabel.IsVisible = false;
                LocationEntry.IsVisible = false;
                ProdCodeLabel.IsVisible = false;
                scanEntry.IsVisible = false;
                ItemScanLabel.IsVisible = false;
            }
            //OnSaveClicked += OnSave;
            OnCancelClicked += OnCancel;
            ViewModel.SetFocus += ViewModel_SetFocus;
            scanEntry.Focus();

        }

        async void Handle_Toggled(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            if (!IsWastage)
            {
                if (e.Value && ViewModel.OrderDetail != null && (ViewModel.OrderDetail.Product.ProductsPerCase == null || (ViewModel.OrderDetail.OrderDetails.Qty - ViewModel.OrderDetail.QuantityProcessed) < ViewModel.OrderDetail.Product.ProductsPerCase))
                {
                    if (ViewModel.OrderDetail.Product.ProductsPerCase == null)
                    {
                        await Util.Util.ShowErrorPopupWithBeep("Cannot add cases. Please contact admin");
                    }
                    else
                    {
                        await Util.Util.ShowErrorPopupWithBeep("Required Quantity is less than cases quantity");
                    }
                    CasesSwitch.IsToggled = false;
                    return;
                }
            }
            CasesLabel.IsVisible = e.Value;
            CasesStepper.IsVisible = e.Value;
            PerCaseLabel.IsVisible = e.Value;
            PerCaseText.IsVisible = e.Value;
            CasesProgress.IsVisible = e.Value;
            QuantityLabel.IsVisible = !e.Value;
            QuantityStepper.IsVisible = !e.Value;


        }

        protected override void OnAppearingAnimationEnd()
        {
            base.OnAppearingAnimationEnd();
            if (mandatoryLocationScan)
            {
                LocationEntry.Focus();
            }
            else
            {
                scanEntry.IsEnabled = true;
                scanEntry.Focus();
            }
        }

        void ViewModel_SetFocus(bool obj)
        {
            scanEntry.Unfocus();
            scanEntry.Focus();
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            scanEntry.Focus();
        }

        private async void OnCancel()
        {
            if (await ViewModel.OnCancelClicked())
                await PopupNavigation.Instance.PopAsync();
        }

        
        private async void OnSave()
        {
            LocationSync locationSync = await App.Database.StockMovements.GetStockLocationByLocationCode(LocationEntry.Text);

            //SaveButtonEnabled = false;

            //if (mandatoryLocationScan && ViewModel.Quantity == ViewModel.MaxQuantity)
            if (mandatoryLocationScan && ViewModel.Quantity <= ViewModel.MaxQuantity)
            {
                if (ViewModel.OnSaveClicked() && locationSync != null)
                {
                    await PopupNavigation.Instance.PopAsync();
                }
                //else
                //{
                //    await Util.Util.ShowErrorPopupWithBeep("You must scan a valid location code before updating the order.");
                //    LocationEntry.Focus();
                //    return;
                //}
            }

            else if (mandatoryLocationScan && string.IsNullOrEmpty(LocationEntry.Text))
            {
                await Util.Util.ShowErrorPopupWithBeep("You must enter location code before updating the order!");
                LocationEntry.Focus();
            }

            else if (mandatoryLocationScan && locationSync == null)
            {
                await Util.Util.ShowErrorPopupWithBeep("Your location code is invalid!");
                //SaveButtonEnabled = true;
                LocationEntry.Text = "";
                LocationEntry.Focus();
                return;
            }

            else if (!mandatoryLocationScan && ViewModel.OnSaveClicked())
            {
                await PopupNavigation.Instance.PopAsync();
            }
            //SaveButtonEnabled = true;
        }


        async void Handle_ScanTextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            
           
            //if(!string.IsNullOrEmpty(e.NewTextValue) && await ViewModel.ScanCodeTextChanged(e.NewTextValue))
            //{
            //    scanEntry.Unfocus();
            //        scanEntry.Text = string.Empty;
            //         scanEntry.Focus();
                    
            //}
        }

       
        async void Handle_Completed(object sender, System.EventArgs e)
        {
            if (!await ViewModel.ScanCodeTextChanged(ViewModel.ProductCode))
            {
                await Util.Util.ShowErrorPopupWithBeep("Item " + ViewModel.ProductCode + " not found.");
                scanEntry.Text = string.Empty;
                await System.Threading.Tasks.Task.Delay(200);
                scanEntry.Focus();
            }
            else
            {
                if (ViewModel.OnSaveClicked())
                {
                    await PopupNavigation.Instance.PopAsync();
                }
                //ViewModel.Quantity = 0;
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
                else if (ViewModel.OrderDetail.QuantityProcessed + 1 == ViewModel.OrderDetail.OrderDetails.Qty && mandatoryLocationScan)
                {
                    OnSave();
                }
                else
                {
                    oDetail.LocationCode = locationSync.LocationCode;
                    scanEntry.Focus();
                    QuantityStepper.IsEnabled = true;
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
                    scanEntry.IsEnabled = true;
                }
            }
        }

        void StepperIncrementedAutoSave(System.Object sender, Xamarin.Forms.ValueChangedEventArgs e)
        {
            if (e.NewValue == decimal.ToDouble(ViewModel.MaxCases) && mandatoryLocationScan && e.NewValue != 1)
            {
                OnSave();
            }
        }
    }
}

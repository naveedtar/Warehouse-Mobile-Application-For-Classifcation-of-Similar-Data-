using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Ganedata.Core.Barcoding;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Helpers;
using WarehouseHandheld.Models.Enums;
using WarehouseHandheld.Models.OrderProcesses;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Models.Users;
using WarehouseHandheld.Models.Vehicles;
using WarehouseHandheld.Resources;
using WarehouseHandheld.Views.OrderItems;
using WarehouseHandheld.Views.ScanItems;
using Xamarin.Forms;
using static WarehouseHandheld.Models.Orders.OrdersSync;

namespace WarehouseHandheld.ViewModels.OrderItems
{
    public class OrderItemsViewModel : BaseViewModel
    {
        public Action<bool> Completed;
        public Action<bool> SetFocus;
        public Action<bool> ShowPopup;
        public Action<bool> RefreshOrders;
        public Action OrderProcessFinished;
        public Action OrderProcessStarted;
        public Action GoBack;

        bool IsCompleteClicked;
        bool IsGoBackClicked;
        public bool IsShowLocationColumn;
        UserSync User;
        TerminalMetadataSync Terminal;
        public InventoryTransactionTypeEnum OrderType { get; set; }

        public PalletTrackingScanPage serialPopup = null;
        bool shouldAllowOverrider = false;
        private string scanCode = string.Empty;
        private string scanPickContainerCode = string.Empty;
        ScanNonSerialItemsPopup scanPopup;
        ScanSerialItemsPopup serialItemPopup;
        public string ScanCode
        {
            get { return scanCode; }
            set
            {
                scanCode = value;
                OnPropertyChanged();
            }
        }

        public string ScanPickContainerCode
        {
            get { return scanPickContainerCode; }
            set
            {
                scanPickContainerCode = value;
                OnPropertyChanged();
            }
        }

        private OrderAccount order;
        public OrderAccount OrderAccount
        {
            get { return order; }
            set
            {
                order = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<OrderDetailsProduct> orderItems;
        public ObservableCollection<OrderDetailsProduct> OrderItems
        {
            get { return orderItems; }
            set
            {
                orderItems = value;
                OnPropertyChanged();
            }
        }

        private List<OrderProcessDetailSync> orderProcessDetails;
        public List<OrderProcessDetailSync> OrderProcessDetails
        {
            get { return orderProcessDetails; }
            set
            {
                orderProcessDetails = value;
                OnPropertyChanged();
            }
        }

        private decimal itemsToPick;
        public decimal ItemsToPick
        {
            get { return itemsToPick; }
            set
            {
                itemsToPick = value;
                OnPropertyChanged();
            }
        }

        public string shippmentReference;
        public string ShippmentReference
        {
            get { return shippmentReference; }
            set
            {
                shippmentReference = value;
                OnPropertyChanged();
            }
        }

        private string accountInfo = string.Empty;
        public string AccountInfo
        {
            get { return accountInfo; }
            set
            {
                accountInfo = value;
                OnPropertyChanged();
            }
        }

        public ICommand LocationsPopupCommand { get; private set; }
        protected CommandLockerHelper LocationPopupCommandLocker => new CommandLockerHelper((e) => { OpenLocationsPopup(e); });

       
        public ICommand CompleteCommand { get; private set; }
        protected CommandLockerHelper CompleteCommandLocker => new CommandLockerHelper((e) => { OnComplete(e); });

        public ICommand FinishCommand { get; private set; }
        protected CommandLockerHelper FinishCommandLocker => new CommandLockerHelper((e) => { OnFinish(e); });

        public ICommand ItemSelectedCommand => new Command(SelectedCommandLocker.Execute);
        protected CommandLockerHelper SelectedCommandLocker => new CommandLockerHelper(async (e) => { await OnItemSelected(e); });



        public OrderItemsViewModel()
        {
            CompleteCommand = new Command(CompleteCommandLocker.Execute);
            FinishCommand = new Command(FinishCommandLocker.Execute);
            LocationsPopupCommand = new Command(LocationPopupCommandLocker.Execute);
            OrderProcessDetails = new List<OrderProcessDetailSync>();
            string guid = Guid.NewGuid().ToString();
            string[] shippmentRefs = guid.Split('-');
            ShippmentReference = shippmentRefs[0].ToUpper();

        }

        public async void AutoSave()
        {
            OnFinish(null);
            IsCompleteClicked = false;
            await App.Orders.UpdateOrderStatus(OrderAccount.Order.OrderID, OrderStatusEnum.Active);
            OrderAccount.Order.OrderStatusID = (int)OrderStatusEnum.Active;
        }

        public async void AutoComplete()
        {
            var orders = OrderItems.ToList();

            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].QuantityProcessed != orders[i].OrderDetails.Qty)
                {
                    break;
                }
                if (i == orders.Count - 1)
                {
                    if (orders[i].QuantityProcessed <= orders[i].OrderDetails.Qty)
                    {
                        await MarkOrderAsComplete();
                    }
                }
            }
        }

        public async void AutoCompleteForPallet()
        {
            var orders = OrderItems.ToList();

            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].QuantityProcessed < orders[i].OrderDetails.Qty)
                {
                    break;
                }
                if (i == orders.Count - 1)
                {
                    if (orders[i].QuantityProcessed >= orders[i].OrderDetails.Qty)
                    {
                        await MarkOrderAsComplete();
                    }
                }
            }
        }



        public async Task CheckOrdeStatus()
        {
            if (OrderAccount.Order.OrderStatusID == (int)OrderStatusEnum.BeingPicked)
            {
                IsGoBackClicked = true;
                "This order is already picked by someone else".ToToast();
                GoBack?.Invoke();
            }
            if (OrderAccount.Order.OrderStatusID != (int)OrderStatusEnum.BeingPicked)
            {
                //var IsPick = await Application.Current.MainPage.DisplayAlert("", "Do you want to pick this order?", "Yes", "No");
                //if (IsPick)
                //{
                //App.Orders.UpdateOrderStatus(OrderAccount.Order.OrderID, OrderStatusEnum.BeingPicked);
                //OrderAccount.Order.OrderStatusID = (int)OrderStatusEnum.BeingPicked;
                //}
            }
        }

        public async Task MakeOrderActive()
        {
            if (!IsGoBackClicked && OrderAccount.Order.OrderStatusID == (int)OrderStatusEnum.BeingPicked)
            {
                IsCompleteClicked = true;
                OnFinish(null);
                IsCompleteClicked = false;
                await App.Orders.UpdateOrderStatus(OrderAccount.Order.OrderID, OrderStatusEnum.Active);
                OrderAccount.Order.OrderStatusID = (int)OrderStatusEnum.Active;
                RefreshOrders?.Invoke(true);
            }
        }


        public async Task Initialize()
        {
            Terminal = await App.Database.Vehicle.GetTerminalMetaData();
            OrderType = (InventoryTransactionTypeEnum)order.Order.InventoryTransactionTypeId;
            var orders = await App.Orders.GetOrderDetailsWithProduct(OrderAccount.Order.OrderID);
            if (OrderAccount != null && OrderAccount.Order.TransferWarehouseId != null)
                await App.Orders.AddInventoryStockInOrder(orders, (int)OrderAccount.Order.TransferWarehouseId);
            OrderItems = new ObservableCollection<OrderDetailsProduct>(orders);

            if (orders.Any(x => x.ProductLocation != null))
            {
                IsShowLocationColumn = true;
            }
            else
            {
                IsShowLocationColumn = false;
            }
            SetItemsToPick();
            if (OrderAccount != null && OrderAccount.Account != null)
                AccountInfo = string.Format("{0}, {1}", OrderAccount.Account.CompanyName, OrderAccount.Account.CountryName);

            User = (await App.Database.Users.GetAllUsers()).Find((x) => x.UserId == App.Users.LoggedInUserId);
        }

        public async Task<bool> CheckOrder()
        {
            try
            {
                if (this.scanPopup != null)
                {
                    this.scanPopup = null;
                    await PopupNavigation.Instance.PopAsync();
                    return true;
                }
                if (this.serialItemPopup != null)
                {
                    this.serialItemPopup = null;
                    await PopupNavigation.Instance.PopAsync();
                    return true;
                }

                OrderProcessStarted?.Invoke();
                var result = await Application.Current.MainPage.DisplayActionSheet("Finished Being Picked?", "Cancel", "", "Save", "Complete", "Cancel Being Picked");
                if (result == "Complete")
                {
                    if (await MarkOrderAsComplete())
                    {
                        await App.Orders.UpdateOrderStatus(OrderAccount.Order.OrderID, OrderStatusEnum.Complete);
                        OrderAccount.Order.OrderStatusID = (int)OrderStatusEnum.Complete;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else if (result == "Save")
                {
                    OnFinish(null);
                    IsCompleteClicked = false;
                    await App.Orders.UpdateOrderStatus(OrderAccount.Order.OrderID, OrderStatusEnum.Active);
                    OrderAccount.Order.OrderStatusID = (int)OrderStatusEnum.Active;
                }
                else if (result == "Cancel Being Picked")
                {
                    await App.Orders.UpdateOrderStatus(OrderAccount.Order.OrderID, OrderStatusEnum.Active);
                    OrderAccount.Order.OrderStatusID = (int)OrderStatusEnum.Active;

                }
                else
                {
                    OrderProcessFinished?.Invoke();
                    return false;
                }
                RefreshOrders?.Invoke(true);
                OrderProcessFinished?.Invoke();
                GoBack?.Invoke();
                return true;
            }
            catch { }
            return true;
        }


        void SetItemsToPick()
        {
            var sumTotal = OrderItems.Sum(x => x.OrderDetails.Qty);
            var sumProcessed = OrderItems.Sum(x => x.QuantityProcessed);
            ItemsToPick = sumTotal - sumProcessed;
        }

        private async Task OnItemSelected(object e)
        {

            var item = (OrderDetailsProduct)((SelectedItemChangedEventArgs)e).SelectedItem;
            if (item == null || item.OrderDetails == null)
                return;

            if (item.Product.ProductType == Models.Products.ProductKitTypeEnum.Kit)
            {
                await Util.Util.ShowErrorPopupWithBeep("Selected product is bundle. Please scan bundle items");
                return;
            }



            if (item.QuantityProcessed < item.OrderDetails.Qty)
            {
                //Terminal.GlobalProcessByPallet = true;  
                if (item.Product != null && item.Product.Serialisable)
                {
                    //Terminal.MandatoryLocationScan = true;
                    this.serialItemPopup = new ScanSerialItemsPopup(item, OrderAccount, Terminal.MandatoryLocationScan);
                    serialItemPopup.ViewModel.OnSaveSerial += OnSaveSerial;
                    serialItemPopup.ViewModel.Back += (obj) =>
                    {
                        serialItemPopup = null;
                        SetFocus?.Invoke(true);
                    };
                    await PopupNavigation.PushAsync(serialItemPopup);
                    ShowPopup?.Invoke(true);

                }
                else
                {
                    Terminal.GlobalProcessByPallet = true;
                    if (Terminal.GlobalProcessByPallet && item.Product.ProcessByPallet)
                    {
                        await HandleProcessByPallet(item, 1);
                    }
                    else
                    {
                        //this.showNonSerialScanPopupWithItme(item).AutoSaveAction += AutoSave;
                        this.showNonSerialScanPopupWithItme(item);
                        //this.scanPopup = new ScanNonSerialItemsPopup(item, OrderAccount);
                        //scanPopup.ViewModel.OnSave += OnPopupSave;
                        //scanPopup.OnCancelClicked += ScanPopup_OnCancelClicked;
                        //scanPopup.ViewModel.Back += (obj) =>
                        //{
                        //    SetFocus?.Invoke(true);
                        //};
                        //await PopupNavigation.PushAsync(scanPopup);
                        //ShowPopup?.Invoke(true);


                        //if (item.OrderDetails.Qty == item.QuantityProcessed)
                        //{
                        //    AutoSave();
                        //}

                    }
                }
            }
            else
            {
                AudioHelper.PlayBeep();
                await Application.Current.MainPage.DisplayAlert("", "Already reached maximum quantity", "OK");
                SetFocus?.Invoke(true);
            }


        }

        async void OnSaveSerial(OrderDetailsProduct orderDetail, string[] serials, decimal quantity)
        {
            serialItemPopup = null;
            if (Terminal.GlobalProcessByPallet && orderDetail.Product.ProcessByPallet && (OrderType == InventoryTransactionTypeEnum.PurchaseOrder || OrderType == InventoryTransactionTypeEnum.SaleOrder))
            {
                await HandleProcessByPallet(orderDetail, quantity);
            }
            else
            {
                App.OrderProcesses.CreateUpdateOrderProcess(OrderAccount, OrderProcessDetails, orderDetail, null, OrderItems, serials.Count(), serials);
            }
            SetItemsToPick();
            SetFocus?.Invoke(true);

            if (orderDetail.QuantityProcessed == orderDetail.OrderDetails.Qty)
            {
                AutoSave();
            }
            if (ItemsToPick == 0)
            {
                AutoComplete();
            }

        }

        void ScanPopup_OnCancelClicked()
        {
            scanPopup = null;
        }


        private async void OnPopupSave(OrderDetailsProduct orderDetail, Decimal quantity)
        {
            scanPopup = null;
            if (quantity > 0)
            {
                if (Terminal.GlobalProcessByPallet && orderDetail.Product.ProcessByPallet && (OrderType == InventoryTransactionTypeEnum.PurchaseOrder || OrderType == InventoryTransactionTypeEnum.SaleOrder))
                {
                    await HandleProcessByPallet(orderDetail, quantity);
                }
                else
                {
                    if (orderDetail.QuantityProcessed < orderDetail.OrderDetails.Qty)
                        App.OrderProcesses.CreateUpdateOrderProcess(OrderAccount, OrderProcessDetails, orderDetail, null, OrderItems, quantity);
                }

            }

            SetItemsToPick();
            SetFocus?.Invoke(true);

            if (orderDetail.QuantityProcessed == orderDetail.OrderDetails.Qty && OrderItems.Count > 1 && ItemsToPick != 0)
            {
                AutoSave();
            }
            if (ItemsToPick == 0)
            {
                AutoComplete();
            }
        }

        private async Task<bool> verifyPickContainerCode()
        {
            var terminalMetadataSync = await App.Database.Vehicle.GetTerminalMetaData();
            if (terminalMetadataSync != null && terminalMetadataSync.MandatoryPickByContainer && string.IsNullOrEmpty(ScanPickContainerCode) && this.order.Order.InventoryTransactionTypeId.Equals((int)InventoryTransactionTypeEnum.SaleOrder))
            {
                await Util.Util.ShowErrorPopupWithBeep("Must scan pick container code");
                return false;
            }
            return true;
        }

        public async void OnFinish(object obj)
        {
            if (!await verifyPickContainerCode())
            {
                return;
            }

            if (App.OrderProcesses.IsBundleComplete(OrderItems.ToList()))
            {
                OrderProcessStarted?.Invoke();
                if (IsCompleteClicked && (OrderProcessDetails == null || OrderProcessDetails.Count() == 0))
                {
                    OrderProcessFinished?.Invoke();
                    return;
                }
                try
                {
                    IsBusy = true;
                    await App.OrderProcesses.AddOrderProcess(OrderProcessDetails, OrderAccount.Order, false, ShippmentReference, ScanPickContainerCode);
                    await UpdatePallets();
                    App.OrderProcesses.RefreshOrderItemsUi += async (shouldRefresh) =>
                    {
                        if (shouldRefresh)
                        {
                            await Initialize();
                        }
                    };

                    OrderProcessDetails.Clear();
                }
                finally
                {
                    IsBusy = false;
                }
                OrderProcessFinished?.Invoke();
            }
            else
            {
                await Util.Util.ShowErrorPopupWithBeep("Complete a bundle to submit order");
            }

        }

        async Task UpdatePallets()
        {
            foreach (var orderProcess in orderProcessDetails.ToList())
            {
                if (orderProcess.PalleTrackingProcess != null && orderProcess.PalleTrackingProcess.Count > 0)
                {
                    foreach (var palletTracking in orderProcess.PalleTrackingProcess)
                    {
                        var palletFound = await App.Pallets.GetPalletTrackingWithTrackingId(palletTracking.PalletTrackingId);
                        if (palletFound != null)
                        {
                            if (OrderType == InventoryTransactionTypeEnum.SaleOrder)
                            {
                                palletFound.RemainingCases -= palletTracking.ProcessedQuantity;
                            }
                            else if (OrderType == InventoryTransactionTypeEnum.PurchaseOrder)
                            {
                                palletFound.Status = PalletTrackingStatusEnum.Active;
                            }
                            await App.Database.Pallets.AddUpdatePalletTracking(palletFound);
                        }
                    }
                }
            }
        }

        private async Task<bool> IsNoRemainingItem()
        {
            bool IsRemainingItem = false;
            foreach (var orderItem in OrderItems)
            {
                if (orderItem.OrderDetails.OrderID == OrderAccount.Order.OrderID && orderItem.QuantityProcessed < orderItem.OrderDetails.Qty)
                {
                    IsRemainingItem = true;
                }
            }
            if (IsRemainingItem)
            {
                return await App.Current.MainPage.DisplayAlert("Confirmation", "Some items are not processed. Do you really want to mark it as complete?", "Yes", "No");

            }
            return true;
        }

        async Task<bool> MarkOrderAsComplete()
        {
            if (!await verifyPickContainerCode())
            {
                return false;
            }
            if (App.OrderProcesses.IsBundleComplete(OrderItems.ToList()))
            {
                IsCompleteClicked = true;
                OnFinish(null);
                IsCompleteClicked = false;
                if (CrossConnectivity.Current.IsConnected && await Util.Util.IsConnected())
                {
                    //var IsConfirmed = await App.Current.MainPage.DisplayAlert("Confirmation", "Do you really want to mark it as complete?", "Yes", "No");
                    if (await IsNoRemainingItem())
                    {
                        try
                        {
                            IsBusy = true;
                            var result = await App.Orders.UpdateOrderStatus(OrderAccount.Order.OrderID, OrdersSync.OrderStatusEnum.Complete);
                            if (result)
                            {
                                OrderAccount.Order.OrderStatusID = (int)OrderStatusEnum.Complete;
                                "Order Completed Succuessfully.".ToToast();
                                Completed?.Invoke(true);
                                await App.Current.MainPage.Navigation.PopAsync();
                                return true;
                            }
                            else
                                "Error while completing order.".ToToast();
                        }
                        finally
                        {
                            IsBusy = false;
                        }
                    }
                }
                else if (!CrossConnectivity.Current.IsConnected)
                {
                    AppStrings.NoInternet.ToToast();
                }
                return false;
            }
            else
            {
                await Util.Util.ShowErrorPopupWithBeep("Complete a bundle to submit order");
                return false;
            }
        }

        private async void OnComplete(object obj)
        {
            await MarkOrderAsComplete();
        }

        public async Task<bool> ScanCodeTextChanged(string code)
        {
            string palletSerialWithoutCaseLabel = "";
            var palletSerialTrackingProductId = 0;


            if (string.IsNullOrEmpty(code))
                return false;

            if (code.Contains("-"))
            {
                var indexOfDashInSerialKey = code.IndexOf("-");
                palletSerialWithoutCaseLabel = code.Substring(0, indexOfDashInSerialKey);

            }
            else
            {
                palletSerialWithoutCaseLabel = code;
            }

            if (code.Contains("\t"))
            {
                palletSerialWithoutCaseLabel = palletSerialWithoutCaseLabel.Substring(0, palletSerialWithoutCaseLabel.Length - 1);
            }

            var productWithPalletSerial = await App.Pallets.GetPalletTrackingWithSerial(palletSerialWithoutCaseLabel);
            if (productWithPalletSerial != null)
            {
                palletSerialTrackingProductId = productWithPalletSerial.ProductId;
            }

            foreach (var item in OrderItems)
            {
                if (item != null)
                {

                    var barcode = new GS128Decoder();
                    code = barcode.GS128DecodeGTINOrDefault(code);

                    if ((!string.IsNullOrEmpty(item.Product.SKUCode) && item.Product.SKUCode.ToLower().Equals(code.ToLower())) || (!string.IsNullOrEmpty(item.Product.BarCode) && item.Product.BarCode.ToLower().Equals(code.ToLower())) || (!string.IsNullOrEmpty(item.Product.BarCode2) && item.Product.BarCode2.ToLower().Equals(code.ToLower())) || (item.Product.ProductId == palletSerialTrackingProductId))
                    {
                        if (item.Product.Serialisable)
                        {
                            if (item.QuantityProcessed >= item.OrderDetails.Qty)
                            {
                                Helpers.AudioHelper.PlayBeep();
                                await App.Current.MainPage.DisplayAlert("Warning", "Already processed maximum quantity.", "ok");
                                return true;
                            }
                            else
                            {
                                //Terminal.MandatoryLocationScan = true;
                                var serialPopup = new ScanSerialItemsPopup(item, OrderAccount, Terminal.MandatoryLocationScan);
                                serialPopup.ViewModel.OnSaveSerial += OnSaveSerial;
                                serialPopup.ViewModel.Back += (obj) =>
                                {
                                    SetFocus?.Invoke(true);
                                };
                                await PopupNavigation.Instance.PushAsync(serialPopup);
                                ShowPopup?.Invoke(true);
                                return true;
                            }
                        }
                        else
                        {
                            ScanCode = string.Empty;
                            decimal quantity = 1;
                            if (!string.IsNullOrEmpty(item.Product.BarCode2) && item.Product.BarCode2.ToLower().Equals(code.ToLower()) && item.Product.ProductsPerCase != null)
                            {
                                quantity = (decimal)item.Product.ProductsPerCase;
                            }
                            if (item.QuantityProcessed >= item.OrderDetails.Qty)
                            {
                                AudioHelper.PlayBeep();
                                await Application.Current.MainPage.DisplayAlert("", "Already processed maximum quantity.", "OK");
                                SetFocus?.Invoke(true);
                                return true;
                            }
                            if (!string.IsNullOrEmpty(item.Product.BarCode2) && item.Product.BarCode2.ToLower().Equals(code.ToLower()) && item.QuantityProcessed + quantity > item.OrderDetails.Qty)
                            {
                                await Util.Util.ShowErrorPopupWithBeep("Scanned Quantity is more than required");
                                SetFocus?.Invoke(true);
                                return true;
                            }

                            if (item.Product.ProductType == Models.Products.ProductKitTypeEnum.Kit)
                            {
                                await Util.Util.ShowErrorPopupWithBeep("Selected product is bundle. Please scan bundle items");
                                return true;
                            }


                            if (Terminal.GlobalProcessByPallet && item.Product.ProcessByPallet && (OrderType == InventoryTransactionTypeEnum.PurchaseOrder || OrderType == InventoryTransactionTypeEnum.SaleOrder))
                            {
                                //Executed if product scan by serial.
                                if (item.Product.ProductId == palletSerialTrackingProductId)
                                {
                                    return await HandleProcessByPallet(item, quantity, palletSerialWithoutCaseLabel);
                                }
                                //Executes if product scan by SKUcode
                                else if (item.Product.SKUCode == code)
                                {
                                    return await HandleProcessByPallet(item, quantity);
                                }
                                else
                                {
                                    continue;
                                }

                            }
                            else
                            {
                                if (item.Product.SKUCode == code)
                                {
                                    if ((item.Product.RequiresBatchNumberOnReceipt ?? false) || (item.Product.RequiresExpiryDateOnReceipt ?? false) || Terminal.MandatoryLocationScan)
                                    {
                                        this.showNonSerialScanPopupWithItme(item);
                                    }
                                    else
                                    {
                                        if (item.IsProductInKit)
                                            quantity = quantity * item.KitQuantity;

                                        App.OrderProcesses.CreateUpdateOrderProcess(OrderAccount, OrderProcessDetails, item, null, OrderItems, quantity);
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                            }
                            SetItemsToPick();
                            "Item added.".ToToast();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private async void showNonSerialScanPopupWithItme(OrderDetailsProduct item)
        {
            //Terminal.MandatoryLocationScan = true;
            this.scanPopup = new ScanNonSerialItemsPopup(item, OrderAccount, Terminal.MandatoryLocationScan);
            scanPopup.ViewModel.OnSave += OnPopupSave;
            scanPopup.OnCancelClicked += ScanPopup_OnCancelClicked;
            scanPopup.ViewModel.Back += (obj) =>
            {
                SetFocus?.Invoke(true);
            };
            await PopupNavigation.Instance.PushAsync(scanPopup);
            ShowPopup?.Invoke(true);
        }

        private async Task<bool> HandleProcessByPallet(OrderDetailsProduct item, decimal quantity, string palletSerial = null)
        {
            var OdrProDetail = OrderProcessDetails.Find(x => x.OrderDetailID == item.OrderDetails.OrderDetailID);
            var OrderProcessDetailProduct = OrderProcessDetails.FindAll(x => x.ProductId == item.OrderDetails.ProductId);


            if (palletSerial != null)
            {
                if (OrderType == InventoryTransactionTypeEnum.PurchaseOrder || OrderType == InventoryTransactionTypeEnum.TransferIn)
                {
                    serialPopup = new PalletTrackingScanPage(item.Product, true, item, User, OdrProDetail, OrderProcessDetailProduct, shouldAllowOverrider, palletSerial);
                }
                else if (OrderType == InventoryTransactionTypeEnum.SaleOrder || OrderType == InventoryTransactionTypeEnum.TransferOut)
                {
                    serialPopup = new PalletTrackingScanPage(item.Product, false, item, User, OdrProDetail, OrderProcessDetailProduct, shouldAllowOverrider, palletSerial);
                }
            }
            else
            {
                if (OrderType == InventoryTransactionTypeEnum.PurchaseOrder || OrderType == InventoryTransactionTypeEnum.TransferIn)
                {
                    serialPopup = new PalletTrackingScanPage(item.Product, true, item, User, OdrProDetail, OrderProcessDetailProduct, shouldAllowOverrider);
                }
                else if (OrderType == InventoryTransactionTypeEnum.SaleOrder || OrderType == InventoryTransactionTypeEnum.TransferOut)
                {
                    serialPopup = new PalletTrackingScanPage(item.Product, false, item, User, OdrProDetail, OrderProcessDetailProduct, shouldAllowOverrider);
                }
            }


            serialPopup.PalletListAdded += (obj, quantityProcessed) =>
            {
                quantityProcessed = Math.Round(quantityProcessed, 2);
                if ((quantityProcessed > 0 && quantityProcessed < 1) || quantityProcessed > quantity)
                {
                    quantity = quantityProcessed;
                }
                if (quantityProcessed != 0)
                    App.OrderProcesses.CreateUpdateOrderProcess(OrderAccount, OrderProcessDetails, item, obj, OrderItems, quantity);

                SetItemsToPick();
                SetFocus?.Invoke(true);
                serialPopup = null;

                if (quantityProcessed == quantity && OrderItems.Count > 1 && ItemsToPick != 0)
                {
                    AutoSave();
                }
                if (ItemsToPick == 0)
                {
                    AutoComplete();
                }
                else if (ItemsToPick <= 0)
                {
                    AutoCompleteForPallet();
                }
            };

            serialPopup.CancelClicked += () =>
            {
                SetFocus?.Invoke(true);
                serialPopup = null;
            };
            serialPopup.SetShouldAllowOverridPerm += (bool perm) =>
            {
                shouldAllowOverrider = perm;
            };
            await App.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(serialPopup));
            ShowPopup?.Invoke(true);
            return true;
        }
        
        
        private async void OpenLocationsPopup(object productId)
        {
            if (productId != null)
            {
                await PopupNavigation.Instance.PushAsync(new ProductLocationsPopup((int)productId));
            }
        }
    }
}
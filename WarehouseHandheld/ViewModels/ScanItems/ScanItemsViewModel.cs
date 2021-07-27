using System;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Models.Orders;
using System.Collections.Generic;
using WarehouseHandheld.Models.OrderProcesses;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using WarehouseHandheld.Models.Products;
using System.Windows.Input;
using WarehouseHandheld.Modules;
using static WarehouseHandheld.Models.Orders.OrdersSync;
using Newtonsoft.Json;
using Ganedata.Core.Barcoding;
using WarehouseHandheld.Helpers;
using System.Linq;

namespace WarehouseHandheld.ViewModels.ScanItems
{
    public class ScanItemsViewModel : BaseViewModel
    {
        public Action<bool> Back;
        public Action<bool> SetFocus;
        public Action<OrderDetailsProduct, Decimal> OnSave;
        public Action<OrderDetailsProduct, string[], decimal> OnSaveSerial;
        List<ProductSerialSync> AllSerials { get; set; }
        public bool IsSerialized { get; set; }
        
        public InventoryTransactionTypeEnum OrderType { get; set; }
        public ICommand RemoveSerialCommand { get; private set; }


        List<string> SerialsRejected = new List<string>();
        ObservableCollection<string> serialsAdded = new ObservableCollection<string>();
        public ObservableCollection<string> SerialsAdded
        {
            get { return serialsAdded; }
            set
            {
                serialsAdded = value;
                OnPropertyChanged();
            }
        }

        private OrderDetailsProduct orderDetail;
        public OrderDetailsProduct OrderDetail
        {
            get { return orderDetail; }
            set
            {
                orderDetail = value;
                OnPropertyChanged();
            }
        }

        private OrderAccount order;
        public OrderAccount Order
        {
            get { return order; }
            set
            {
                order = value;
                OnPropertyChanged();
            }
        }

        private bool isReturn = false;
        public bool IsReturn
        {
            get { return isReturn; }
            set
            {
                isReturn = value;
                OnPropertyChanged();
            }
        }

        private bool isDisableEntry;
        public bool IsDisableEntry
        {
            get { return isDisableEntry; }
            set
            {
                isDisableEntry = value;
                OnPropertyChanged();
            }
        }

        private string productCode = string.Empty;
        public string ProductCode
        {
            get { return productCode; }
            set
            {
                productCode = value;
                OnPropertyChanged();
            }
        }

        private string progress = string.Empty;
        public string Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                OnPropertyChanged();
            }
        }

        private string casesProgress = string.Empty;
        public string CasesProgress
        {
            get { return casesProgress; }
            set
            {
                casesProgress = value;
                OnPropertyChanged();
            }
        }

        private Decimal quantity;
        public Decimal Quantity
        {
            get { return quantity; }
            set
            {

                quantity = value;
                if (orderDetail != null && orderDetail.Product.ProductsPerCase != null)
                {
                    cases = Math.Round(value / (decimal)orderDetail.Product.ProductsPerCase, 2);
                    OnPropertyChanged(nameof(Cases));
                }
                CasesProgress = "Cases " + Cases + " , Products " + value;
                OnPropertyChanged();
            }
        }

        private Decimal cases = 0;
        public Decimal Cases
        {
            get { return cases; }
            set
            {
                CasesProgress = "Cases " + value + " , Products " + Quantity;
                if (orderDetail != null)
                    Quantity += Math.Round(Math.Ceiling(((value - cases) * (decimal)orderDetail.Product.ProductsPerCase)), 2);
                else
                    Quantity++;
                cases = value;
                OnPropertyChanged();
            }
        }

        private Decimal maxCases = 1;
        public Decimal MaxCases
        {
            get { return maxCases; }
            set
            {
                maxCases = value;
                OnPropertyChanged();
            }
        }

        private Decimal maxQuantity = 100;
        public Decimal MaxQuantity
        {
            get { return maxQuantity; }
            set
            {
                maxQuantity = value;
                OnPropertyChanged();
            }
        }

        private string batchNumber;
        public string BatchNumber
        {
            get { return batchNumber; }
            set
            {
                batchNumber = value;
                OnPropertyChanged();
            }
        }

        private DateTime expiryDate = DateTime.Today.Date;
        public DateTime ExpiryDate
        {
            get { return expiryDate; }
            set
            {
                expiryDate = value;
                OnPropertyChanged();
            }
        }

        private bool isWastages;
        public bool IsWastages
        {
            get { return isWastages; }
            set
            {
                isWastages = value;
                OnPropertyChanged();
            }
        }

        private decimal incrementValue = 1;
        public decimal IncrementValue
        {
            get { return incrementValue; }
            set
            {
                incrementValue = value;
                OnPropertyChanged();
            }
        }

        private bool isFirstTimePopupOpen = true;
        public bool IsFirstTimePopupOpen
        {
            get { return isFirstTimePopupOpen; }
            set
            {
                isFirstTimePopupOpen = value;
                OnPropertyChanged();
            }
        }

        public ScanItemsViewModel()
        {
            RemoveSerialCommand = new Command<string>(RemoveSerial);
        }

        async void RemoveSerial(string serial)
        {
            var IsRemove = await App.Current.MainPage.DisplayAlert("", "Do you really want to remove this serial?", "Yes", "No");
            if (IsRemove)
            {
                SerialsAdded.RemoveAt(SerialsAdded.IndexOf(serial));
                SerialsRejected.Add(serial);
                Quantity -= 1;
                if (OrderDetail != null)
                {
                    Progress = (OrderDetail.QuantityProcessed + Quantity) + "/" + OrderDetail.OrderDetails.Qty;
                }
            }
            SetFocus?.Invoke(true);
        }


        public void Initialize(OrderDetailsProduct orderDetail)
        {
            Quantity = 1;
            if (orderDetail != null)
            {
                OrderDetail = orderDetail;

                MaxQuantity = OrderDetail.OrderDetails.Qty - OrderDetail.QuantityProcessed;
                if (OrderDetail.Product.ProductsPerCase != null)
                {
                    var RequiredCases = Math.Round(MaxQuantity / (decimal)OrderDetail.Product.ProductsPerCase, 2);
                    //var RequiredCases = Math.Truncate(MaxQuantity / (decimal)OrderDetail.Product.ProductsPerCase);
                    if (RequiredCases > 0)
                    {
                        MaxCases = RequiredCases;
                    }
                }

                Progress = (OrderDetail.QuantityProcessed + Quantity) + "/" + OrderDetail.OrderDetails.Qty;
            }
            else
            {
                MaxCases = 100000;
                MaxQuantity = 100000;
            }
        }

        public void InitializeReturn(OrderDetailsProduct orderDetails)
        {
            IsReturn = true;
            orderDetail = orderDetails;
            MaxCases = 100000;
            MaxQuantity = 100000;
        }


        public async Task LoadSerials()
        {
            if (OrderDetail != null)
            {
                AllSerials = await App.Products.GetProductSerialByProductId(OrderDetail.Product.ProductId);
                AddNotSyncedSerials(OrderDetail.Product.ProductId);
            }
            else
            {
                AllSerials = await App.Products.GetAllProductSerials();
                AddNotSyncedSerials(0);
            }
        }

        public async void AddNotSyncedSerials(int ProductId)
        {
            var AlllLogs = await App.Database.SyncLog.GetAll();
            var logsToPost = AlllLogs.FindAll((obj) => obj.TableName == Database.DatabaseConfig.Tables.OrderProcesses.ToString() && obj.IsPost && !obj.Synced && !obj.IsPending);
            foreach (var syncLog in logsToPost)
            {
                var orderToPost = JsonConvert.DeserializeObject<OrderProcessesSyncCollection>(syncLog.request);
                foreach (var orderProcess in orderToPost.OrderProcesses)
                {
                    if (orderProcess.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.PurchaseOrder && orderProcess.OrderProcessDetails != null && orderProcess.OrderProcessDetails.Count > 0)
                    {
                        foreach (var orderProcessDetail in orderProcess.OrderProcessDetails)
                        {
                            if ((ProductId == 0 || ProductId == orderProcessDetail.ProductId) && orderProcessDetail.Serials != null && orderProcessDetail.Serials.Length > 0)
                            {
                                foreach (var serial in orderProcessDetail.Serials)
                                {
                                    var serialsync = new ProductSerialSync();
                                    serialsync.SerialNo = serial;
                                    serialsync.ProductId = ProductId;
                                    serialsync.CurrentStatus = 1;
                                    AllSerials.Add(serialsync);
                                }
                            }
                        }
                    }
                }
            }

        }

        public bool OnSaveClicked()
        {
            if (!IsReturn)
            {
                if (!string.IsNullOrEmpty(BatchNumber))
                {
                    OrderDetail.BatchNumber = BatchNumber;
                }
                if (OrderDetail.Product.RequiresExpiryDateOnReceipt ?? false)
                {
                    if (ExpiryDate > DateTime.Today.Date)
                    {
                        OrderDetail.ExpiryDate = ExpiryDate;
                    }
                    else
                    {
                        Util.Util.ShowErrorPopupWithBeep("Date must be after Today");
                        return false;
                    }
                  
                }   
            }
            else
            {
                OrderDetail.ExpiryDate = null;
            }
            OnSave?.Invoke(OrderDetail, Quantity);
            Back?.Invoke(true);
            return true;
        }

        public void OnSaveSerialsClicked()
        {
            if (OrderDetail != null)
            {
                OrderDetail.QuantityProcessed += Quantity;
                if (orderDetail.Product.ProductsPerCase != null)
                    orderDetail.BoxesRemaining = Math.Round(((orderDetail.OrderDetails.Qty - orderDetail.QuantityProcessed) / (decimal)orderDetail.Product.ProductsPerCase), 2);

            }

            string[] serials = new string[SerialsAdded.Count];
            for (int i = 0; i < SerialsAdded.Count; i++)
            {
                serials[i] = SerialsAdded[i];
            }

            OnSaveSerial?.Invoke(OrderDetail, serials, quantity);
            Back?.Invoke(true);
        }

        public async Task<bool> OnCancelClicked()
        {
            Back?.Invoke(true);
            if (Quantity > 0)
                return await App.Current.MainPage.DisplayAlert("Alert", "You have unsaved changes, still want to leave ?", "yes", "no");
            else
                return true;
        }

        public async Task<bool> OnSerialCancelClicked()
        {
            Back?.Invoke(true);
            if (SerialsAdded.Count > 0)
                return await App.Current.MainPage.DisplayAlert("Alert", "You have unsaved changes, still want to leave ?", "yes", "no");
            else
                return true;
        }

        public async Task<bool> ScanSerialChanged(string code)
        {

            InventoryTransactionTypeEnum[] outOfStockStatuses = { InventoryTransactionTypeEnum.SaleOrder, InventoryTransactionTypeEnum.TransferOut, InventoryTransactionTypeEnum.AdjustmentOut, InventoryTransactionTypeEnum.Loan,
                InventoryTransactionTypeEnum.WorkOrder, InventoryTransactionTypeEnum.Wastage };


            if (OrderDetail == null)
            {
                ProductSerialSync serialSync = null;
                if (IsReturn)
                {
                    if (IsWastages)
                    {
                        //var serial = AllSerials.Find((obj) => obj.SerialNo == code);
                        var serial = AllSerials.Where(m => !outOfStockStatuses.Contains((InventoryTransactionTypeEnum)m.CurrentStatus)).FirstOrDefault((obj) => obj.SerialNo == code);
                        serialSync = serial;
                    }
                    else
                    {
                        var serial = AllSerials.Where(m => outOfStockStatuses.Contains((InventoryTransactionTypeEnum)m.CurrentStatus)).FirstOrDefault((obj) => obj.SerialNo == code);
                        serialSync = serial;
                    }

                }
                else
                {
                    var serial = AllSerials.Find((obj) => obj.SerialNo == code);
                    serialSync = serial;
                }
                if (serialSync != null && !SerialsAdded.Contains(code))
                {
                    SerialsAdded.Insert(0, code);
                    "Item added.".ToToast();
                    ProductCode = string.Empty;
                    Quantity += 1;

                    return true;
                }
                return false;
            }
            if (serialsAdded.Count < MaxQuantity)
            {
                if (OrderType == InventoryTransactionTypeEnum.PurchaseOrder || OrderType == InventoryTransactionTypeEnum.TransferIn)
                {
                    var serial = AllSerials.Find((obj) => obj.SerialNo == code);
                    bool Check = false;
                    Check = OrderType == InventoryTransactionTypeEnum.PurchaseOrder ? serial == null : serial != null && serial.CurrentStatus != (int)InventoryTransactionTypeEnum.PurchaseOrder && serial.CurrentStatus != (int)InventoryTransactionTypeEnum.TransferIn && serial.CurrentStatus != (int)InventoryTransactionTypeEnum.Allocated
                        && serial.CurrentStatus != (int)InventoryTransactionTypeEnum.AdjustmentIn && serial.CurrentStatus != (int)InventoryTransactionTypeEnum.Returns;
                    if (Check)
                    {
                        if (SerialsAdded.Contains(code))
                        {
                            "Serial Already Added".ToToast();
                            return true;

                        }
                        SerialsAdded.Insert(0, code);
                        "Item added.".ToToast();
                        ProductCode = string.Empty;
                        Quantity += 1;
                        if (OrderDetail != null)
                        {
                            Progress = (OrderDetail.QuantityProcessed + Quantity) + "/" + OrderDetail.OrderDetails.Qty;
                        }
                        return true;
                    }
                }
                else if (OrderType == InventoryTransactionTypeEnum.SaleOrder || OrderType == InventoryTransactionTypeEnum.TransferOut || OrderType == InventoryTransactionTypeEnum.Returns)
                {
                    ProductSerialSync serial = null;
                    if (OrderType == InventoryTransactionTypeEnum.Returns)
                    {

                        if (!IsWastages)
                        {
                            serial = AllSerials.Where(m => outOfStockStatuses.Contains((InventoryTransactionTypeEnum)m.CurrentStatus)).FirstOrDefault((obj) => obj.SerialNo == code);
                            //serial = AllSerials.Find((obj) => obj.SerialNo.ToLower().Equals(code.ToLower()) && !serialsAdded.Contains(obj.SerialNo));
                            //return true;
                        }

                    }
                    else
                    {
                        serial = AllSerials.Find((obj) => obj.SerialNo.ToLower().Equals(code.ToLower()) && !serialsAdded.Contains(obj.SerialNo) && (obj.CurrentStatus == (int)InventoryTransactionTypeEnum.PurchaseOrder || obj.CurrentStatus == (int)InventoryTransactionTypeEnum.TransferIn || obj.CurrentStatus == (int)InventoryTransactionTypeEnum.Allocated || obj.CurrentStatus == (int)InventoryTransactionTypeEnum.AdjustmentIn || obj.CurrentStatus == (int)InventoryTransactionTypeEnum.Returns));
                    }
                    if (serial != null)
                    {
                        SerialsAdded.Insert(0, serial.SerialNo);
                        "Item added.".ToToast();
                        ProductCode = string.Empty;
                        Quantity += 1;
                        if (OrderDetail != null)
                        {
                            Progress = (OrderDetail.QuantityProcessed + Quantity) + "/" + OrderDetail.OrderDetails.Qty;
                        }
                        return true;
                    }
                }
            }
            else
            {
                AudioHelper.PlayBeep();
                await Application.Current.MainPage.DisplayAlert("", "Already reached maximum quantity", "OK");

            }
            return false;
        }

        public async Task<bool> ScanCodeTextChanged(string code)
        {
            if (string.IsNullOrEmpty(code))
                return false;
            decimal quantityToAdd = 1;

            var barcode = new GS128Decoder();
            code = barcode.GS128DecodeGTINOrDefault(code);
            if ((!string.IsNullOrEmpty(OrderDetail.Product.SKUCode) && OrderDetail.Product.SKUCode.ToLower().Equals(code.ToLower())) || (!string.IsNullOrEmpty(OrderDetail.Product.BarCode) && OrderDetail.Product.BarCode.ToLower().Equals(code.ToLower())) || (!string.IsNullOrEmpty(OrderDetail.Product.BarCode2) && OrderDetail.Product.BarCode2.ToLower().Equals(code.ToLower())))
            {
                ProductCode = string.Empty;
                if (OrderDetail.Product.BarCode2 != null && OrderDetail.Product.BarCode2.ToLower().Equals(code.ToLower()) && OrderDetail.Product.ProductsPerCase != null)
                {
                    quantityToAdd = (decimal)OrderDetail.Product.ProductsPerCase;
                }
                else if (OrderDetail.Product.BarCode2 != null && OrderDetail.Product.BarCode2.ToLower().Equals(code.ToLower()))
                {
                    "Could not add due to some error".ToToast();
                    SetFocus?.Invoke(true);
                    return false;
                }

                if (OrderDetail.QuantityProcessed + Quantity == OrderDetail.OrderDetails.Qty)
                {
                    //await Util.Util.ShowErrorPopupWithBeep("Already Processed Maximum Quantity");
                    SetFocus?.Invoke(true);
                    return true;
                }
                if (OrderDetail.QuantityProcessed + Quantity + quantityToAdd > OrderDetail.OrderDetails.Qty)
                {
                    await Util.Util.ShowErrorPopupWithBeep("Scanned Quantity is More than Required");
                    SetFocus.Invoke(true);
                    return false;
                }
                Quantity += quantityToAdd;

                "Item added.".ToToast();
                return true;
            }

            return false;
        }
    }
}

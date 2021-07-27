using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Ganedata.Core.Barcoding;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Models.StockMovement;
using Xamarin.Forms;
using static WarehouseHandheld.Models.Orders.OrdersSync;

namespace WarehouseHandheld.ViewModels.StockMovement
{
    public class ScanStockMovementViewModel : BaseViewModel
    {
        public Action<bool> Back;
        public Action<bool> SetFocus;
        public Action<StockMovementViewModel> OnSave;
        public Action<StockMovementViewModel> OnSaveSerial;
        List<ProductSerialSync> SerialsRejected = new List<ProductSerialSync>();


        public ScanStockMovementViewModel()
        {
            RemoveSerialCommand = new Command<ProductSerialSync>(RemoveSerial);
        }

        List<ProductSerialSync> AllSerials { get; set; }
        public bool IsSerialized { get; set; }
        public ICommand RemoveSerialCommand { get; private set; }

        ObservableCollection<ProductSerialSync> serialsAdded = new ObservableCollection<ProductSerialSync>();
        public ObservableCollection<ProductSerialSync> SerialsAdded
        {
            get { return serialsAdded; }
            set
            {
                serialsAdded = value;
                OnPropertyChanged();
            }
        }



        private StockMovementViewModel _stock;
        public StockMovementViewModel Stock
        {
            get { return _stock; }
            set
            {
                _stock = value;
                OnPropertyChanged();
            }
        }

        private ProductMasterSync product;
        public ProductMasterSync Product
        {
            get { return product; }
            set
            {
                product = value;
                OnPropertyChanged();
            }
        }

        private string scanProductSerial = string.Empty;
        public string ScanProductSerial
        {
            get { return scanProductSerial; }
            set
            {
                scanProductSerial = value;
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

        private decimal quantity = 0;
        public decimal Quantity
        {
            get { return quantity; }
            set
            {

                quantity = value;
                if (Product != null && Product.ProductsPerCase != null)
                {
                    cases = Math.Round(value / (decimal)Product.ProductsPerCase, 2);
                    OnPropertyChanged(nameof(Cases));
                }
                CasesProgress = "Cases " + Cases + " , Products " + value;
                OnPropertyChanged();
            }
        }

        private decimal cases = 0;
        public decimal Cases
        {
            get { return cases; }
            set
            {
                CasesProgress = "Cases " + value + " , Products " + Quantity;
                if (Product != null)
                    Quantity += Math.Round(Math.Ceiling(((value - cases) * (Product.ProductsPerCase ?? 1))), 2);
                else
                    Quantity++;
                cases = value;
                OnPropertyChanged();
            }
        }

        private decimal maxCases = 100000;
        public decimal MaxCases
        {
            get { return maxCases; }
            set
            {
                maxCases = value;
                OnPropertyChanged();
            }
        }

        private decimal maxQuantity = 100000;
        public decimal MaxQuantity
        {
            get { return maxQuantity; }
            set
            {
                maxQuantity = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadSerials()
        {
            if (Product != null)
            {
                AllSerials = await App.Products.GetProductSerialByProductId(Product.ProductId);
            }
            else
            {
                AllSerials = await App.Products.GetAllProductSerials();
            }
        }


        public bool OnSaveClicked()
        {
            if (Stock.IsFromLocationComplete)
            {
                if (Stock.QtyProcessed + Quantity <= Stock.Qty)
                {
                    Stock.QtyProcessed += Quantity;
                    OnSave?.Invoke(Stock);
                }
                else
                {
                    Util.Util.ShowErrorPopupWithBeep("Can't process more than required");
                    return false;
                }
            }
            else
            {
                if (Quantity > 0 && Quantity <= Stock.QuantityAvailableOnLocation)
                {
                    Stock.Qty += Quantity;
                    OnSave?.Invoke(Stock);
                }
                else if(Quantity <= 0)
                {
                    Util.Util.ShowErrorPopupWithBeep("Add quantity to proceeed");
                    return false;
                }                    
                else
                {
                     Util.Util.ShowErrorPopupWithBeep("Maximum available quantity is "+Stock.QuantityAvailableOnLocation);
                     return false;
                }
            }
            Back?.Invoke(true);
            return true;
        }

        public bool OnSaveSerialsClicked()
        {
            if (Stock.IsFromLocationComplete)
            {
                if (Stock.QtyProcessed + Quantity <= Stock.Qty)
                {
                    Stock.QtyProcessed += Quantity;
                    OnSaveSerial?.Invoke(Stock);
                }
                else
                {
                    Util.Util.ShowErrorPopupWithBeep("Can't process more than required");
                    return false;
                }
            }
            else
            {
                if (Quantity > 0 && Quantity <= Stock.QuantityAvailableOnLocation)
                {
                    Stock.Qty += Quantity;
                    var serialsToAdd = SerialsAdded.Select(x => x.SerialID).ToList();
                    if (Stock.PalletSerials == null)
                    {
                        foreach (var item in SerialsAdded)
                        {
                            if (Stock.PalletSerials == null)
                            {
                                Stock.PalletSerials = new List<StockMovementPalletSerialsViewModel>
                                {
                                    new StockMovementPalletSerialsViewModel
                                    {
                                        PalletSerialId = item.SerialID,
                                        IsSerialised = true,
                                        ProductId = Stock.ProductId
                                    }
                                };
                            }
                            else
                            {
                                var stockSerial = new StockMovementPalletSerialsViewModel
                                {
                                    PalletSerialId = item.SerialID,
                                    IsSerialised = true,
                                    ProductId = Stock.ProductId
                                };
                                Stock.PalletSerials.Add(stockSerial);
                            }
                        }
                    }
                    // Stock.SerialIds.AddRange(serialsToAdd);
                    OnSaveSerial?.Invoke(Stock);
                }
                else if(Quantity <= 0)
                {
                    Util.Util.ShowErrorPopupWithBeep("Add quantity to proceeed");
                    return false;
                }         
                else
                {
                    Util.Util.ShowErrorPopupWithBeep("Maximum available quantity is "+Stock.QuantityAvailableOnLocation);
                    return false;
                }
            }
            Back?.Invoke(true);
            return true;
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


        async void RemoveSerial(ProductSerialSync serial)
        {
            if (!Stock.IsFromLocationComplete)
            {
                var IsRemove = await App.Current.MainPage.DisplayAlert("", "Do you really want to remove this serial?", "Yes", "No");
                if (IsRemove)
                {
                    var scannedItem = Stock.PalletSerials.FirstOrDefault(x =>
                        x.IsSerialised && x.PalletSerialId.Equals(serial.SerialID));
                    if (scannedItem != null)
                    {
                        Stock.PalletSerials.Remove(scannedItem);
                    }
                    SerialsAdded.RemoveAt(SerialsAdded.IndexOf(serial));
                    SerialsRejected.Add(serial);
                    Quantity -= 1;
                }
                SetFocus?.Invoke(true);
            }
        }


        public async Task<bool> ScanSerialChanged(string code)
        {
            InventoryTransactionTypeEnum[] outOfStockStatuses = { InventoryTransactionTypeEnum.SaleOrder, InventoryTransactionTypeEnum.TransferOut, InventoryTransactionTypeEnum.AdjustmentOut, InventoryTransactionTypeEnum.Loan,
                InventoryTransactionTypeEnum.WorkOrder, InventoryTransactionTypeEnum.Wastage };
            ProductSerialSync serialSync = null;

            var serial = AllSerials.Where(m => outOfStockStatuses.Contains((InventoryTransactionTypeEnum)m.CurrentStatus)).FirstOrDefault((obj) => obj.SerialNo == code);
            serialSync = serial;

            if (serialSync != null)
            {
                    if (Stock.IsFromLocationComplete)
                    {
                        if (Stock.PalletSerials != null &&!Stock.PalletSerials.Any(x => x.PalletSerialId.Equals(serialSync.SerialID) && x.IsSerialised))
                        {
                            await Util.Util.ShowErrorPopupWithBeep("Scanned serial is not part of stock movement");
                            return false;
                        }

                        if (Stock.PalletSerials != null && Stock.PalletSerials.Any(x => x.PalletSerialId.Equals(serialSync.SerialID) && x.IsSerialised && x.IsFromComplete))
                        {
                            await Util.Util.ShowErrorPopupWithBeep("Serial already scanned");
                            return false;
                        }
                        if(Stock.PalletSerials != null && Stock.PalletSerials.Any(x => x.IsSerialised))
                        {
                            var stockMovementPalletSerialIndex = Stock.PalletSerials.FindIndex(x => x.PalletSerialId.Equals(serial.SerialID) && x.IsSerialised);
                            if (stockMovementPalletSerialIndex != -1)
                            {
                                Stock.PalletSerials[stockMovementPalletSerialIndex].IsFromComplete = true;
                            }
                        }
                    }
                    else
                    {
                        if (Stock.PalletSerials != null && Stock.PalletSerials.Any(x => x.PalletSerialId.Equals(serialSync.SerialID) && x.IsSerialised))
                        {
                            await Util.Util.ShowErrorPopupWithBeep("Serial already scanned");
                            return false;
                        }
                        var stockMovementPalletSerial = new StockMovementPalletSerialsViewModel()
                        {
                            IsSerialised = true,
                            ProductId = Stock.ProductId,
                            PalletSerialId = serialSync.SerialID,
                            IsFromComplete = false
                        };
                        if (Stock.PalletSerials != null) Stock.PalletSerials.Insert(0, stockMovementPalletSerial);
                    }
                    SerialsAdded.Insert(0, serialSync);
                    "Item added.".ToToast();
                    ScanProductSerial = string.Empty;
                    Quantity += 1;
                    return true;
            }
            await Util.Util.ShowErrorPopupWithBeep("Item " + ScanProductSerial + " not found.");
            return false;
        }
    }
}

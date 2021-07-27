using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Helpers;
using WarehouseHandheld.Models.Accounts;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Models.StockMovement;
using WarehouseHandheld.Models.Vehicles;
using WarehouseHandheld.Modules;
using WarehouseHandheld.Views.OrderItems;
using Xamarin.Forms;

namespace WarehouseHandheld.ViewModels.OrderItems
{
    public class PalletTrackingScanViewModel : BaseViewModel
    {
        public List<PalleTrackingProcess> PalletTrackingProcesses;
        public List<PalleTrackingProcess> PalletTrackingProcessInOrderProcessesOfSameProduct;
        List<PalletTrackingSync> SkippedPallets = new List<PalletTrackingSync>();
        public Action SetFocus;
        public Action<bool> SetOverridPerm;
        bool shouldAllowOverrideWithoutPassword;
        bool isStockTake = false;
        bool isSaleOrder = false;
        public bool IsStockMovement;
        public PalletTrackingScanViewModel(bool IsPurchase, ProductMasterSync product, bool shouldAllowOverrid, bool isStkToke = false)
        {
            this.isStockTake = isStkToke;
            this.IsPurchase = IsPurchase;
            this.Product = product;
            shouldAllowOverrideWithoutPassword = shouldAllowOverrid;
        }
        
        //Used for StockTake and Stock Movement
        public PalletTrackingScanViewModel(ProductMasterSync product, bool isStkToke = false)
        {
            this.isStockTake = isStkToke;
            this.Product = product;
        }
        


        public async Task<bool> GetPalletTrackingsForGoodsOut()
        {
            ActivePallet = await App.Pallets.GetPalletTrackingForGoodsOut(Product.ProductId, PalletTrackingProcesses,PalletTrackingProcessInOrderProcessesOfSameProduct, SkippedPallets, ModulesConfig.TrackingScheme);
            return true;
        }

        public async Task<bool> NextActivePallet()
        {
            if (shouldAllowOverrideWithoutPassword)
            {
                await FindAndAssignNextActivePallet();
                return true;
            }
            else
            {
                var authPopup = new SkippedPalletAuthPopup();
                authPopup.VerifyUser += async (obj) =>
                {
                    if (await App.Users.VerifyUserPass(obj))
                    {
                        shouldAllowOverrideWithoutPassword = true;
                        SetOverridPerm?.Invoke(true);
                        await FindAndAssignNextActivePallet();
                    }
                    else
                    {
                        await Util.Util.ShowErrorPopupWithBeep("User authorization failed. Can't skip pallet.");
                    }
                    SetFocus?.Invoke();
                };
                authPopup.OnCancelClicked += () => { SetFocus?.Invoke(); };
                await PopupNavigation.PushAsync(authPopup);
                return true;
            }

        }

        public async Task<bool> FindAndAssignNextActivePallet()
        {
            SkippedPallets.Add(ActivePallet);
            ActivePallet = await App.Pallets.GetPalletTrackingForGoodsOut(Product.ProductId, PalletTrackingProcesses, PalletTrackingProcessInOrderProcessesOfSameProduct, SkippedPallets, ModulesConfig.TrackingScheme);
            if (ActivePallet != null)
            {
                await Util.Util.ShowErrorPopupWithBeep("Pallet is Skipped.");
            }
            else
            {
                await Util.Util.ShowErrorPopupWithBeep("Next pallet is not found.");
            }
            return true;

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

        public OrderDetailsProduct OrderDetails;

        private bool isPurchase;
        public bool IsPurchase
        {
            get { return isPurchase; }
            set
            {
                isPurchase = value;
                OnPropertyChanged();
            }
        }

        private string progress;
        public string Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                OnPropertyChanged();
            }
        }

        private decimal orderQuantity;
        public decimal OrderQuantity
        {
            get { return orderQuantity; }
            set
            {
                orderQuantity = value;
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

        private string serialToScan;
        public string SerialToScan
        {
            get { return serialToScan; }
            set
            {
                serialToScan = value;
                OnPropertyChanged();
            }
        }

        private AccountSync account;
        public AccountSync Account
        {
            get { return account; }
            set
            {
                account = value;
                OnPropertyChanged();
            }
        }

        private StockMovementViewModel _stockMovement;
        public StockMovementViewModel StockMovement
        {
            get { return _stockMovement; }
            set
            {
                _stockMovement = value;
                OnPropertyChanged();
            }
        }

        private string itemToScanText = "Item To Scan:";
        public string ItemToScanText
        {
            get { return itemToScanText; }
            set
            {
                itemToScanText = value;
                OnPropertyChanged();
            }
        }

        private PalletTrackingSync activePallet;
        public PalletTrackingSync ActivePallet
        {
            get { return activePallet; }
            set
            {
                activePallet = value;
                if (value != null)
                {
                    if (ModulesConfig.TrackingScheme == PalletTrackingSchemeEnum.ByExpiryMonth)
                    {
                        SerialToScan = ((DateTime)value.ExpiryDate).ToString("MM/yyyy");
                        ItemToScanText = "Scan item with expiry month:";
                    }
                    else if (ModulesConfig.TrackingScheme == PalletTrackingSchemeEnum.ByExpiryDate)
                    {
                        SerialToScan = ((DateTime)value.ExpiryDate).ToString("dd/MM/yyyy");
                        ItemToScanText = "Scan item with Expirty date:";
                    }
                    else if (ModulesConfig.TrackingScheme == PalletTrackingSchemeEnum.DontEnforce)
                    {
                        SerialToScan = "Anyone";
                    }
                    else
                    {
                        SerialToScan = value.PalletSerial;
                    }
                }
                OnPropertyChanged();
            }
        }

        private ObservableCollection<PalletSerial> scannedPallets = new ObservableCollection<PalletSerial>();
        public ObservableCollection<PalletSerial> ScannedPallets
        {
            get { return scannedPallets; }
            set
            {
                scannedPallets = value;
                OnPropertyChanged();
            }
        }


        public async Task<bool> Scan(string code)
        {
            
            if (OrderDetails != null && OrderDetails.OrderDetails.Qty == scannedPallets.Sum((x) => x.Quantity))
            {
                await Util.Util.ShowErrorPopupWithBeep("Already reached maximum quantity");
                return true;
            }
            if (OrderDetails != null && OrderDetails.OrderDetails.Qty <= OrderDetails.QuantityProcessed + scannedPallets.Sum((x) => x.Quantity))
            {
                await Util.Util.ShowErrorPopupWithBeep("Already reached maximum quantity");
                return true;
            }

           
            var pallet = await App.Pallets.GetPalletTrackingBySerial(code, product.ProductId);
            if (pallet != null && !isStockTake)
            {
                if (scannedPallets != null && scannedPallets.Count > 0)
                {
                    var existingPallet = scannedPallets.FirstOrDefault(x => x.Serial == code && x.RemainingCases > 0.0M);
                    if (existingPallet != null)
                    {
                        await AddCases(existingPallet);
                        return false;
                    }
                }

                if (IsShelfDateExceeded(pallet))
                {
                    await Util.Util.ShowErrorPopupWithBeep("Shelf date has exceeded");
                    return true;
                }
                if (PalletTrackingProcessInOrderProcessesOfSameProduct.Find(x => x.PalletTrackingId == pallet.PalletTrackingId)  != null)
                {
                    await Util.Util.ShowErrorPopupWithBeep("Pallet is already scanned.");
                    return true;
                }
                if (PalletTrackingProcesses.Find((x) => x.PalletTrackingId == pallet.PalletTrackingId) != null)
                {
                    await Util.Util.ShowErrorPopupWithBeep("Pallet is already scanned.");
                    return true;
                }

                if (pallet.Status == PalletTrackingStatusEnum.Created)
                {
                    var palletFound = PalletTrackingProcesses.Find((x) => x.PalletTrackingId == pallet.PalletTrackingId);
                    if (palletFound == null)
                    {
                        PalleTrackingProcess process = new PalleTrackingProcess();
                        process.PalletTrackingId = pallet.PalletTrackingId;
                        process.ProcessedQuantity = pallet.RemainingCases;
                        decimal CasesAdded = process.ProcessedQuantity;
                        if (Product.ProductsPerCase != null)
                        {
                            process.ProcessedQuantity = pallet.RemainingCases * (decimal)Product.ProductsPerCase;
                            CasesAdded = pallet.RemainingCases;
                        }
                        var TotalQuantity = process.ProcessedQuantity;
                        var TotalCases = CasesAdded;
                        if (OrderDetails != null && (OrderDetails.QuantityProcessed + scannedPallets.Sum((x) => x.Quantity) + process.ProcessedQuantity) > OrderDetails.OrderDetails.Qty)
                        {
                            process.ProcessedQuantity = OrderDetails.OrderDetails.Qty - (OrderDetails.QuantityProcessed + scannedPallets.Sum((x) => x.Quantity));

                            CasesAdded = process.ProcessedQuantity;
                            if (Product.ProductsPerCase != null)
                            {
                                CasesAdded = Math.Round((process.ProcessedQuantity / (decimal)Product.ProductsPerCase),2);
                            }

                            if (TotalCases - CasesAdded > 0)
                            {
                                var IsGetAll = await Util.Util.ShowErrorPopupPromptWithBeep("There are " + (pallet.RemainingCases.ToString("F")) + " cases in pallet and required cases are " + CasesAdded.ToString("F") + ". Do you want to get all cases?", "Yes", "No");
                                if (IsGetAll)
                                {
                                    process.ProcessedQuantity = TotalQuantity;
                                    CasesAdded = pallet.RemainingCases;
                                }
                                else
                                {
                                    await Util.Util.ShowErrorPopupWithBeep("Only " + CasesAdded.ToString("F") + " cases are being taken from this pallet");
                                }
                            }
                        }
                        pallet.RemainingCases = TotalCases - CasesAdded;
                        process.RemainingCasesAfterProcessing = pallet.RemainingCases;
                        OrderQuantity = process.ProcessedQuantity;
                        process.ProcessedQuantity = CasesAdded;
                        //OrderQuantity = CasesAdded;
                        PalletTrackingProcesses.Add(process);
                        pallet.Status = PalletTrackingStatusEnum.Active;
                        ScannedPallets.Insert(0, new PalletSerial() { PalletTrackingId = pallet.PalletTrackingId, Serial = pallet.PalletSerial, Cases = CasesAdded, Quantity = OrderQuantity, OrderQuantity= OrderDetails.OrderDetails.Qty, RemainingCases = pallet.RemainingCases });
                        "Pallet Added".ToToast();
                        if (OrderDetails != null && OrderDetails.OrderDetails.Qty == (OrderDetails.QuantityProcessed + scannedPallets.Sum((x) => x.Quantity)))
                        {
                            await Util.Util.ShowErrorPopupWithBeep("Reached maximum quantity");
                        }
                        return true;
                    }
                    else
                    {
                        await Util.Util.ShowErrorPopupWithBeep("Pallet is already in scanned.");
                    }
                }
                else
                {
                    await Util.Util.ShowErrorPopupWithBeep("Pallet already booked in.");
                    return false;
                }
            }
            else if (pallet != null && isStockTake)
            {
                var palletSerial = new PalletSerial() { Serial = code, Cases = pallet.RemainingCases };
                var process = new PalleTrackingProcess() { ProcessedQuantity = (decimal)(pallet.RemainingCases * product.ProductsPerCase ?? 1), palletSerial = code, PalletTrackingId = pallet.PalletTrackingId };
                PalletTrackingProcesses.Add(process);
                scannedPallets.Insert(0,palletSerial);
                return true;
            }

            else if (pallet == null && isStockTake)
            {
                var palletSerial = new PalletSerial() { Serial = code };
                await this.AddCases(palletSerial);
                scannedPallets.Insert(0,palletSerial);
                return false;
            }

            await Util.Util.ShowErrorPopupWithBeep("Either pallet is already booked in or does not exist in the system.");
            return false;
        }

      
        public async Task<bool> ScanActivePallet(string code)
        {
            isSaleOrder = true;
            decimal sum = 0;

            if (scannedPallets != null && scannedPallets.Count > 0)
            {
                var existingPallet = scannedPallets.FirstOrDefault(x => x.Serial == code && x.RemainingCases > 0.0M);

                if (existingPallet != null)
                {
                    await AddCases(existingPallet);
                    return false;
                }
            }

            if (PalletTrackingProcesses.Count > 0)
            {
                sum = scannedPallets.Sum((x) => x.Quantity);
            }
            if (OrderDetails.OrderDetails.Qty - OrderDetails.QuantityProcessed - sum > 0)
            {
                if (!await GetPalletWithSerialAndTrackingScheme(code))
                    return false;


                if (ActivePallet != null && (ActivePallet.PalletSerial == code) && ActivePallet.ProductId == Product.ProductId && (ActivePallet.Status == PalletTrackingStatusEnum.Active))
                {
                    var includedPalletTrackingProcess = PalletTrackingProcesses.LastOrDefault(x => x.PalletTrackingId == ActivePallet.PalletTrackingId && x.IsIncluded == true);
                    var includedPalletTrackingProcessSameProduct = PalletTrackingProcessInOrderProcessesOfSameProduct.LastOrDefault(x => x.PalletTrackingId == ActivePallet.PalletTrackingId && x.IsIncluded == true);

                    if (IsShelfDateExceeded(ActivePallet))
                    {
                        await Util.Util.ShowErrorPopupWithBeep("Shelf date has exceeded");
                        return true;
                    }

                    if (includedPalletTrackingProcess != null)
                    {
                        ActivePallet.RemainingCases = includedPalletTrackingProcess.RemainingCasesAfterProcessing;
                    }

                    if (includedPalletTrackingProcessSameProduct != null)
                    {
                        ActivePallet.RemainingCases = includedPalletTrackingProcessSameProduct.RemainingCasesAfterProcessing;
                    }

                    PalleTrackingProcess process = new PalleTrackingProcess();
                    process.PalletTrackingId = ActivePallet.PalletTrackingId;
                    process.ProcessedQuantity = 1;
                    process.ProcessedQuantity = ActivePallet.RemainingCases;
                    decimal CasesAdded = process.ProcessedQuantity;

                    if (Product.ProductsPerCase != null)
                    {
                        process.ProcessedQuantity = ActivePallet.RemainingCases * (decimal)Product.ProductsPerCase;

                    }
                    if (OrderDetails.QuantityProcessed + scannedPallets.Sum((x) => x.Quantity) + process.ProcessedQuantity > OrderDetails.OrderDetails.Qty)
                    {
                        process.ProcessedQuantity = OrderDetails.OrderDetails.Qty - (OrderDetails.QuantityProcessed + scannedPallets.Sum((x) => x.Quantity));
                        if (Product.ProductsPerCase == null)
                        {
                            ActivePallet.RemainingCases = ActivePallet.RemainingCases - process.ProcessedQuantity;
                            CasesAdded = process.ProcessedQuantity;
                        }
                        else
                        {
                            CasesAdded = Math.Round((process.ProcessedQuantity / (decimal)Product.ProductsPerCase),2);
                            ActivePallet.RemainingCases = ActivePallet.RemainingCases - CasesAdded;
                        }
                    }
                    else
                    {
                        ActivePallet.RemainingCases = 0;
                    }
                    OrderQuantity = process.ProcessedQuantity;
                    process.ProcessedQuantity = CasesAdded;
                    process.RemainingCasesAfterProcessing = activePallet.RemainingCases;
                    PalletTrackingProcesses.Add(process);

                    //var prg = OrderDetails.QuantityProcessed + PalletTrackingProcesses.Sum((x) => x.ProcessedQuantity * Product.ProductsPerCase ?? 1);
                    var prg = OrderDetails.QuantityProcessed + PalletTrackingProcesses.FindAll(x => x.IsIncluded == false).Sum((x) => x.ProcessedQuantity * Product.ProductsPerCase ?? 1);

                    Progress = prg + "/" + OrderDetails.OrderDetails.Qty;

                    ScannedPallets.Insert(0, new PalletSerial { PalletTrackingId = ActivePallet.PalletTrackingId, Serial = ActivePallet.PalletSerial, Cases = CasesAdded, Quantity = OrderQuantity, OrderQuantity = OrderDetails.OrderDetails.Qty, RemainingCases = ActivePallet.RemainingCases, IsSaleOrder = true, OrderQuantityProcessed = prg, prdPerCase= Product.ProductsPerCase ?? 1 });
                    "Pallet Added".ToToast();
                    //ScannedPallets.ToList().ForEach(x => x.OrderQuantityProcessed = prg);

                    if (ActivePallet.RemainingCases > 0)
                        await Util.Util.ShowErrorPopupWithBeep("Only " + CasesAdded.ToString("F") + " cases are being taken from scanned pallet.");

                    if (OrderDetails != null && OrderDetails.OrderDetails.Qty == (OrderDetails.QuantityProcessed + scannedPallets.Sum((x) => x.Quantity)))
                        await Util.Util.ShowErrorPopupWithBeep("Reached maximum quantity");
                    else
                        await GetPalletTrackingsForGoodsOut();
                    return true;
                }
                else
                {
                    await Util.Util.ShowErrorPopupWithBeep("No active pallet found in the system against scanned serial "+code+".");
                }
            }
            else
            {
                await Util.Util.ShowErrorPopupWithBeep("Already reached maximum quantity");
            }
            return false;
        }


        public async Task<bool> ScanWastageReturnPallet(string code)
        {
            var pallet = await App.Pallets.GetPalletTrackingBySerial(code, product.ProductId);

            if (pallet != null)
            {
                if (pallet.Status != PalletTrackingStatusEnum.Active)
                {
                    await Util.Util.ShowErrorPopupWithBeep("Pallet is not active.");
                    return true;
                }
                if (PalletTrackingProcessInOrderProcessesOfSameProduct.Find(x => x.PalletTrackingId == pallet.PalletTrackingId) != null)
                {
                    await Util.Util.ShowErrorPopupWithBeep("Pallet is already scanned.");
                    return true;
                }
                if (PalletTrackingProcesses.Find((x) => x.PalletTrackingId == pallet.PalletTrackingId) != null)
                {
                    await Util.Util.ShowErrorPopupWithBeep("Pallet is already scanned.");
                    return true;
                }

                var palletFound = PalletTrackingProcesses.Find((x) => x.PalletTrackingId == pallet.PalletTrackingId);
                if (palletFound == null)
                {
                    PalleTrackingProcess process = new PalleTrackingProcess();
                    process.PalletTrackingId = pallet.PalletTrackingId;
                    process.palletSerial = pallet.PalletSerial;
                    process.ProcessedQuantity = pallet.RemainingCases;
                    process.isExistingPallet = true;
                    decimal CasesAdded = process.ProcessedQuantity;

                    if (Product.ProductsPerCase != null)
                    {
                        process.ProcessedQuantity = pallet.RemainingCases * (decimal)Product.ProductsPerCase;
                        CasesAdded = Math.Round((process.ProcessedQuantity / (decimal)Product.ProductsPerCase), 2);
                        CasesAdded = pallet.RemainingCases;
                    }
                    else
                    {
                        CasesAdded = process.ProcessedQuantity;
                    }

                    if (!IsWastages)
                    {
                        CasesAdded = pallet.TotalCases - pallet.RemainingCases;
                    }
                    if(IsWastages)
                    {
                        CasesAdded = pallet.RemainingCases;
                    }

                    if (CasesAdded <= 0)
                    {
                        await Util.Util.ShowErrorPopupWithBeep("There are no cases to process.");
                        return false;
                    }
                    OrderQuantity = process.ProcessedQuantity;
                    process.ProcessedQuantity = pallet.RemainingCases;
                    var TotalQuantity = process.ProcessedQuantity;
                    var TotalCases = CasesAdded;
                    OrderQuantity = pallet.RemainingCases;
                    process.RemainingCasesAfterProcessing = pallet.RemainingCases;
                    PalletTrackingProcesses.Add(process);
                    ScannedPallets.Insert(0,new PalletSerial() { PalletTrackingId = pallet.PalletTrackingId, Serial = pallet.PalletSerial, Cases = CasesAdded,Quantity = OrderQuantity });
                    await AddCasesWastageReturn(ScannedPallets.FirstOrDefault(x => x.PalletTrackingId.Equals(pallet.PalletTrackingId)));
                    "Pallet Added".ToToast();
                    return true;
                }
                else
                {
                    await Util.Util.ShowErrorPopupWithBeep("Pallet not found.");
                    return false;
                }
            }
            await Util.Util.ShowErrorPopupWithBeep("Either pallet is already booked in or does not exist in the system.");
            return false;
        }




        public async Task<bool> ScanStockMovePallet(string code)
        {
            var pallet = await App.Pallets.GetPalletTrackingBySerial(code, product.ProductId);

            if (pallet != null)
            {
                if (pallet.Status != PalletTrackingStatusEnum.Active)
                {
                    await Util.Util.ShowErrorPopupWithBeep("Pallet is not active.");
                    return true;
                }
                if (!StockMovement.IsFromLocationComplete && StockMovement.PalletSerials.Any(x => x.PalletSerialId.Equals(pallet.PalletTrackingId)))
                {
                    await Util.Util.ShowErrorPopupWithBeep("Pallet is already scanned.");
                    return true;
                }
                if (StockMovement.IsFromLocationComplete)
                {
                    if (!StockMovement.PalletSerials.Any(x => x.PalletSerialId.Equals(pallet.PalletTrackingId)))
                    {
                        await Util.Util.ShowErrorPopupWithBeep("Scanned pallet is not part of stock movement");
                        return true;
                    }
                }
                
                var palletFound = PalletTrackingProcesses.Find((x) => x.PalletTrackingId == pallet.PalletTrackingId);
                if (palletFound == null)
                {
                    PalleTrackingProcess process = new PalleTrackingProcess();
                    process.PalletTrackingId = pallet.PalletTrackingId;
                    process.palletSerial = pallet.PalletSerial;
                    process.ProcessedQuantity = pallet.RemainingCases;
                    process.isExistingPallet = true;
                    decimal CasesAdded = process.ProcessedQuantity;
                    if (!StockMovement.IsFromLocationComplete)
                    {
                        var stockMovementPalletSerial = new StockMovementPalletSerialsViewModel
                        {
                            PalletSerialId = pallet.PalletTrackingId,
                            ProductId = StockMovement.ProductId,
                        };
                        if(StockMovement.QuantityAvailableOnLocation > 0)
                            StockMovement.PalletSerials.Insert(0, stockMovementPalletSerial);
                        else
                        {
                             await Util.Util.ShowErrorPopupWithBeep("Maximum available quantity is "+StockMovement.QuantityAvailableOnLocation);
                            return false;
                        }

                    }
                    else
                    {
                        if (StockMovement.PalletSerials != null && StockMovement.PalletSerials.Any())
                        {
                            var stockMovmentPalletSerialIndex = StockMovement.PalletSerials.FindIndex(x => x.PalletSerialId.Equals(pallet.PalletTrackingId));
                            if (stockMovmentPalletSerialIndex != -1)
                            {
                                StockMovement.PalletSerials[stockMovmentPalletSerialIndex].IsFromComplete = true;
                            }
                        }
                    }
                    
                    if (Product.ProductsPerCase != null)
                    {
                        process.ProcessedQuantity = pallet.RemainingCases * (decimal)Product.ProductsPerCase;
                        CasesAdded = Math.Round((process.ProcessedQuantity / (decimal)Product.ProductsPerCase), 2);
                        CasesAdded = pallet.RemainingCases;
                    }
                    else
                    {
                        CasesAdded = process.ProcessedQuantity;
                    }

                    if (CasesAdded <= 0)
                    {
                        await Util.Util.ShowErrorPopupWithBeep("There are no cases to process.");
                        return false;
                    }
                    OrderQuantity = process.ProcessedQuantity;
                    process.ProcessedQuantity = pallet.RemainingCases;
                    var TotalQuantity = process.ProcessedQuantity;
                    var TotalCases = CasesAdded;
                    OrderQuantity = pallet.RemainingCases;
                    process.RemainingCasesAfterProcessing = pallet.RemainingCases;
                    PalletTrackingProcesses.Add(process);
                    ScannedPallets.Insert(0, new PalletSerial() { PalletTrackingId = pallet.PalletTrackingId, Serial = pallet.PalletSerial, Cases = CasesAdded, Quantity = OrderQuantity });
                    await AddCasesStockMovement(ScannedPallets.FirstOrDefault(x => x.PalletTrackingId.Equals(pallet.PalletTrackingId)));
                    "Pallet Added".ToToast();
                    return true;
                }
                else
                {
                    await Util.Util.ShowErrorPopupWithBeep("Pallet not found.");
                    return false;
                }
            }
            await Util.Util.ShowErrorPopupWithBeep("Either pallet is already booked in or does not exist in the system.");
            return false;
        }


        public async Task<bool> GetPalletWithSerialAndTrackingScheme(string serial)
        {
            var pallet = ActivePallet;
            switch (ModulesConfig.TrackingScheme)
            {
                case PalletTrackingSchemeEnum.FirstInFirstOut:
                case PalletTrackingSchemeEnum.FirstInLastOut:
                    return true;
                case PalletTrackingSchemeEnum.ByExpiryDate:
                case PalletTrackingSchemeEnum.ByExpiryMonth:
                case PalletTrackingSchemeEnum.DontEnforce:
                    pallet = await App.Pallets.GetPalletTrackingForGoodsOutWithSerial(product.ProductId, PalletTrackingProcesses, PalletTrackingProcessInOrderProcessesOfSameProduct, SkippedPallets, ModulesConfig.TrackingScheme, serial);
                    break;
            }

            if (pallet != null)
            {
                if (ModulesConfig.TrackingScheme == PalletTrackingSchemeEnum.ByExpiryMonth)
                {
                    var expiryDate = Convert.ToDateTime(pallet.ExpiryDate.Value.ToString("MM/yyyy"));
                    var allowedExpiryDate = Convert.ToDateTime(serialToScan);
                    if ((expiryDate.Month <= allowedExpiryDate.Month) && (expiryDate.Year <= allowedExpiryDate.Year))
                    {
                        ActivePallet = pallet;
                    }
                    else
                    {
                        await Util.Util.ShowErrorPopupWithBeep("Expiry month of scanned pallet is greater than allowed expiry month. " + serialToScan);
                        return false;
                    }
                }
                else if (ModulesConfig.TrackingScheme == PalletTrackingSchemeEnum.ByExpiryDate)
                {
                    var expiryDate = Convert.ToDateTime(pallet.ExpiryDate.Value.ToString("dd/MM/yyyy"));
                    var allowedExpiryDate = Convert.ToDateTime(serialToScan);
                    if ((expiryDate.Date <= allowedExpiryDate.Date) && (expiryDate.Month <= allowedExpiryDate.Month) && (expiryDate.Year <= allowedExpiryDate.Year))
                    {
                        ActivePallet = pallet;
                    }
                    else
                    {
                        await Util.Util.ShowErrorPopupWithBeep("Expiry Date of scanned pallet is greater than allowed expiry month. " + allowedExpiryDate);
                        return false;
                    }
                }
                ActivePallet = pallet;
            }
            else
            {
                await Util.Util.ShowErrorPopupWithBeep("Either pallet is not active or already processed  or does not exist in system.");
                return false;
            }
            return true;
        }
        
        public async Task UpdatePallet(PalletTrackingSync pallet)
        {
            List<PalletTrackingSync> palletTrackings = new List<PalletTrackingSync>();
            palletTrackings.Add(pallet);
            await App.Database.Pallets.AddUpdatePalletTracking(palletTrackings);

        }


        protected CommandLockerHelper SelectedCommandLocker => new CommandLockerHelper(async (e) => { await OnItemSelected(e); });
        public ICommand ItemSelectedCommand => new Command(SelectedCommandLocker.Execute);

        private async Task OnItemSelected(object e)
        {
            try
            {
                if (IsStockMovement)
                {
                    await this.AddCasesStockMovement((PalletSerial)((SelectedItemChangedEventArgs)e).SelectedItem);
                }
                else
                {
                    await this.AddCases((PalletSerial)((SelectedItemChangedEventArgs)e).SelectedItem);
                }
            }
            catch { }
        }


        private async Task AddCasesWastageReturn(PalletSerial pallet)
        {
            var casesPopup = new CasesEditPopup(pallet.Cases,IsWastages);
            //casesPopup.SaveButtonEnabled = true;
            casesPopup.SaveCases += (decimal obj) =>
            {
                var index = ScannedPallets.ToList().FindIndex((x) => x.Serial == pallet.Serial);
                if (product.ProductsPerCase != null)
                {
                    OrderQuantity = (decimal)(obj * product.ProductsPerCase);
                }
                else
                {
                    OrderQuantity = obj;
                }
                var itemToAdd = new PalletSerial { PalletTrackingId = pallet.PalletTrackingId, Serial = pallet.Serial, Cases = obj, Quantity = OrderQuantity };
                ScannedPallets.RemoveAt(index);
                ScannedPallets.Insert(index, itemToAdd);
                var process = new PalleTrackingProcess() { ProcessedQuantity = (decimal)(obj * product.ProductsPerCase ?? 1), palletSerial = pallet.Serial, RemainingCasesAfterProcessing = pallet.RemainingCases };
                PalletTrackingProcesses.Add(process);
                UpdateQuantity(pallet.Cases, obj, pallet.PalletTrackingId, itemToAdd);
            };
            await PopupNavigation.PushAsync(casesPopup);
        }


        private async Task AddCases(PalletSerial pallet)
        {
            CasesEditPopup casesPopup;
            PalletSerial itemToAdd;

            if (pallet != null && pallet.IsSaleOrder)
            {
                casesPopup = new CasesEditPopup(pallet);
            }
            else
            {
                casesPopup = new CasesEditPopup(pallet);
            }

            //casesPopup.SaveButtonEnabled = true;
            casesPopup.SaveCases += (decimal obj) =>
            {
                var index = ScannedPallets.ToList().FindIndex((x) => x.Serial == pallet.Serial);
                if(product.ProductsPerCase != null)
                {
                    OrderQuantity = (decimal)(obj * product.ProductsPerCase);
                }
                else
                {
                    OrderQuantity = obj;
                }

                if (pallet.IsSaleOrder)
                {
                    itemToAdd = new PalletSerial { PalletTrackingId = pallet.PalletTrackingId, Serial = pallet.Serial, Cases = obj, Quantity = OrderQuantity, OrderQuantity = OrderDetails.OrderDetails.Qty, RemainingCases = ActivePallet.RemainingCases, IsSaleOrder = isSaleOrder, OrderQuantityProcessed = 0.0M, prdPerCase = Product.ProductsPerCase ?? 1 };
                }
                else
                {
                    itemToAdd = new PalletSerial { PalletTrackingId = pallet.PalletTrackingId, Serial = pallet.Serial, Cases = obj, Quantity = OrderQuantity, OrderQuantity = OrderDetails.OrderDetails.Qty, RemainingCases = pallet.RemainingCases, IsSaleOrder = isSaleOrder, OrderQuantityProcessed = 0.0M, prdPerCase = Product.ProductsPerCase ?? 1 };
                }

                if (pallet != null && pallet.IsSaleOrder)
                {
                    itemToAdd.RemainingCases = (ScannedPallets[index].Cases + ScannedPallets[index].RemainingCases) - itemToAdd.Cases;
                    itemToAdd.OrderQuantity = ScannedPallets[index].OrderQuantity;
                    itemToAdd.IsSaleOrder = true;
                }
                else
                {
                    itemToAdd.RemainingCases = (ScannedPallets[index].Cases + ScannedPallets[index].RemainingCases) - itemToAdd.Cases;
                    itemToAdd.OrderQuantity = ScannedPallets[index].OrderQuantity;
                    itemToAdd.IsSaleOrder = false;
                }

                ScannedPallets.RemoveAt(index);
                ScannedPallets.Insert(index, itemToAdd);
                //var palletTrackingProcess = PalletTrackingProcesses.FirstOrDefault(x => x.PalletTrackingId == pallet.PalletTrackingId);
                var palletTrackingProcess = PalletTrackingProcesses.FirstOrDefault(x => x.PalletTrackingId == pallet.PalletTrackingId && x.IsIncluded == false);
                if (palletTrackingProcess != null)
                {
                    //palletTrackingProcess.RemainingCasesAfterProcessing = ActivePallet.RemainingCases - obj;
                    palletTrackingProcess.RemainingCasesAfterProcessing = (ScannedPallets[index].Cases + ScannedPallets[index].RemainingCases) - itemToAdd.Cases;

                    palletTrackingProcess.ProcessedQuantity = (decimal)(obj * product.ProductsPerCase ?? 1);
                }
                else
                {
                    var process = new PalleTrackingProcess() { PalletTrackingId = pallet.PalletTrackingId, ProcessedQuantity = (decimal)(obj * product.ProductsPerCase ?? 1), palletSerial = pallet.Serial };
                    //process.RemainingCasesAfterProcessing = ActivePallet.RemainingCases - obj;
                    process.RemainingCasesAfterProcessing = (ScannedPallets[index].Cases + ScannedPallets[index].RemainingCases) - itemToAdd.Cases;

                    PalletTrackingProcesses.Add(process);
                }
                UpdateQuantity(pallet.Cases, obj, pallet.PalletTrackingId, itemToAdd);
            };
            await PopupNavigation.PushAsync(casesPopup);
        }

        void UpdateQuantity(decimal OldCases, decimal NewCases, int PalletTrackingId, PalletSerial pSrl)
        {
            var quantity = NewCases;
            if (Product.ProductsPerCase != null)
            {
                quantity = NewCases * (decimal)Product.ProductsPerCase;
            }
            if (!isStockTake)
            {
                //Commenting out this to fix Progress Label, processed quantity was being overridden 25 Sep 2019
                //var PalletTrackingProcessIndex = PalletTrackingProcesses.FindIndex((x) => x.PalletTrackingId == PalletTrackingId);
                var PalletTrackingProcessIndex = PalletTrackingProcesses.FindIndex((x) => x.IsIncluded == false && x.PalletTrackingId == PalletTrackingId);
                PalletTrackingProcesses[PalletTrackingProcessIndex].ProcessedQuantity = NewCases;
                if (OrderDetails != null)
                {
                    //var prg = OrderDetails.QuantityProcessed + PalletTrackingProcesses.Sum((x) => x.ProcessedQuantity * Product.ProductsPerCase ?? 1);
                    var prg = OrderDetails.QuantityProcessed + PalletTrackingProcesses.FindAll(x => x.IsIncluded == false).Sum((x) => x.ProcessedQuantity * Product.ProductsPerCase ?? 1);
                    pSrl.OrderQuantityProcessed = prg;
                    Progress = prg + "/" + OrderDetails.OrderDetails.Qty;
                }
            }

            //var PalletTrackingIndex = PalletsTracking.FindIndex((x) => x.PalletTrackingId == PalletTrackingId);
            //PalletsTracking[PalletTrackingIndex].RemainingCases = PalletsTracking[PalletTrackingIndex].RemainingCases + OldCases - NewCase

        }

        public async Task<bool> CancelPopupClicked()
        {
            if (PalletTrackingProcesses.Count > 0)
            {
                var cancelPopup = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to cancel? Scanned pallets won't be processed", "Yes", "No");
                if (cancelPopup)
                {

                    var removePalletsProductList = PalletTrackingProcesses.FindAll(x => x.IsIncluded.Equals(false));
                    foreach (var palleTracking in removePalletsProductList)
                    {
                        PalletTrackingProcesses.Remove(palleTracking);
                    }

                    var removePalletsSameProductList = PalletTrackingProcessInOrderProcessesOfSameProduct.FindAll(x => x.IsIncluded.Equals(false));
                    foreach (var palleTracking in removePalletsSameProductList)
                    {
                        PalletTrackingProcessInOrderProcessesOfSameProduct.Remove(palleTracking);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }



        private bool IsShelfDateExceeded(PalletTrackingSync palletTracking)
        {
            if (OrderDetails != null && Account != null && palletTracking != null)
            {
                var dateTimeNow = DateTime.Now;
                var palletExpiry = palletTracking.ExpiryDate;
                if (palletExpiry != null  && dateTimeNow > palletExpiry)
                {
                    var numberOfDaysExceeded = (int)(dateTimeNow - (DateTime)palletExpiry).TotalDays;
                    if(numberOfDaysExceeded > Account.AcceptedShelfLife)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        private async Task AddCasesStockMovement(PalletSerial pallet)
        {
            decimal cases = 0;
            if (StockMovement.IsFromLocationComplete)
            {
                var stockMovementPalletSerialsViewModel =
                    StockMovement.PalletSerials.FirstOrDefault(x => x.PalletSerialId.Equals(pallet.PalletTrackingId));
                if (stockMovementPalletSerialsViewModel != null) cases = stockMovementPalletSerialsViewModel.Cases;
            }
            else
                cases = pallet.Cases;
            
            var casesPopup = new CasesEditPopup(pallet.Cases, cases , IsStockMovement);
            //casesPopup.SaveButtonEnabled = true;
            casesPopup.SaveCases += (decimal obj) =>
            {
                var index = ScannedPallets.ToList().FindIndex((x) => x.Serial == pallet.Serial);
                if (product.ProductsPerCase != null)
                {
                    OrderQuantity = (decimal)(obj * product.ProductsPerCase);
                }
                else
                {
                    OrderQuantity = obj;
                }
                var itemToAdd = new PalletSerial { PalletTrackingId = pallet.PalletTrackingId, Serial = pallet.Serial, Cases = obj, Quantity = OrderQuantity, RemainingCases = pallet.RemainingCases};
                var palletTrackingProcess = PalletTrackingProcesses.FirstOrDefault(x => x.PalletTrackingId == pallet.PalletTrackingId && x.IsIncluded == false);
                if (palletTrackingProcess != null)
                {
                    itemToAdd.RemainingCases = (ScannedPallets[index].Cases + ScannedPallets[index].RemainingCases) - itemToAdd.Cases;

                    palletTrackingProcess.RemainingCasesAfterProcessing = (ScannedPallets[index].Cases + ScannedPallets[index].RemainingCases) - itemToAdd.Cases;
                    palletTrackingProcess.ProcessedQuantity = (decimal)(obj * product.ProductsPerCase ?? 1);
                }
                else
                {
                    var process = new PalleTrackingProcess() { ProcessedQuantity = (decimal)(obj * product.ProductsPerCase ?? 1), palletSerial = pallet.Serial, RemainingCasesAfterProcessing = pallet.RemainingCases };
                    PalletTrackingProcesses.Add(process);
                }
                ScannedPallets.RemoveAt(index);
                ScannedPallets.Insert(index, itemToAdd);
                
                if (StockMovement.IsFromLocationComplete)
                {
                    if (StockMovement.QtyProcessed + OrderQuantity <= StockMovement.Qty)
                    {
                        var palletSerialIndex = StockMovement.PalletSerials.FindIndex(
                            x => x.PalletSerialId.Equals(pallet.PalletTrackingId));
                        StockMovement.PalletSerials[palletSerialIndex].Cases = itemToAdd.Cases;
                        StockMovement.QtyProcessed = ScannedPallets.Sum(x => x.Quantity);
                    }
                    else
                    {
                         Util.Util.ShowErrorPopupWithBeep("Cannot process more the required");
                    }
                }
                else
                {
                    var palletSerialIndex = StockMovement.PalletSerials.FindIndex(
                        x => x.PalletSerialId.Equals(pallet.PalletTrackingId));
                    var quantityProcessed = ScannedPallets.Sum(x => x.Quantity);
                    if (quantityProcessed <= StockMovement.QuantityAvailableOnLocation && StockMovement.QuantityAvailableOnLocation > 0)
                    {
                        StockMovement.Qty = ScannedPallets.Sum(x => x.Quantity);
                        StockMovement.PalletSerials[palletSerialIndex].Cases = itemToAdd.Cases;
                    }
                    else if(StockMovement.QuantityAvailableOnLocation <= 0)
                    {
                        StockMovement.PalletSerials.RemoveAt(palletSerialIndex);
                    }
                    else
                    {
                        StockMovement.PalletSerials.RemoveAt(palletSerialIndex);
                        Util.Util.ShowErrorPopupWithBeep("Maximum available quantity is "+StockMovement.QuantityAvailableOnLocation);
                    }
                }
                UpdateQuantity(pallet.Cases, obj, pallet.PalletTrackingId, itemToAdd);
            };
            await PopupNavigation.PushAsync(casesPopup);
        }
    }
}

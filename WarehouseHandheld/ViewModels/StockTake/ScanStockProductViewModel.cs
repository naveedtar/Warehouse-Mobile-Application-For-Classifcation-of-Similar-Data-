using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Ganedata.Core.Barcoding;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Models.StockTakes;
using WarehouseHandheld.Models.Vehicles;
using WarehouseHandheld.Modules;
using WarehouseHandheld.Views.OrderItems;
using WarehouseHandheld.Views.ScanItems;
using WarehouseHandheld.Views.Products;
using WarehouseHandheld.Views.StockTake;
using Xamarin.Forms;
using Plugin.Connectivity;

namespace WarehouseHandheld.ViewModels.StockTake
{
    public class ScanStockProductViewModel : BaseViewModel
    {
        public List<ProductMasterSync> Products;
        List<ProductSerialSync> Serials;
        public StockTakeSync StockTake;
        public Action<bool> FocusScanEntry;
        public Action<bool> FocusScanSerialEntry;
        public Action EmptyScannedProduct;
        public ICommand EditCommand { get; private set; }
        public bool ShouldShowEdit { get; set; }
        public ICommand SelectProductCommand { get; private set; }
        public int SelectedProductIndex = -1;
        TerminalMetadataSync Terminal;
        StockTakePopup stockTakePopup;
        private ObservableCollection<StockTakeScanProduct> stockProducts = new ObservableCollection<StockTakeScanProduct>();
        public ObservableCollection<StockTakeScanProduct> StockProducts
        {
            get { return stockProducts; }
            set
            {
                stockProducts = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> allProducts = new ObservableCollection<string>();
        public ObservableCollection<string> AllProducts
        {
            get { return allProducts; }
            set
            {
                allProducts = value;
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

        private bool isActionButtonEnable;
        public bool IsActionButtonEnable
        {
            get { return isActionButtonEnable; }
            set
            {
                isActionButtonEnable = value;
                OnPropertyChanged();
            }
        }

        private ProductMasterSync selectedProduct;
        public ProductMasterSync SelectedProduct
        {
            get { return selectedProduct; }
            set
            {
                selectedProduct = value;
                OnPropertyChanged();
            }
        }

        private string serialCode = string.Empty;
        public string SerialCode
        {
            get { return serialCode; }
            set
            {
                serialCode = value;
                OnPropertyChanged();
            }
        }

        private bool isSerialEntryShowing;
        public bool IsSerialEntryShowing
        {
            get { return isSerialEntryShowing; }
            set
            {
                isSerialEntryShowing = value;
                OnPropertyChanged();
            }
        }

        private string locationCode = string.Empty;
        public string LocationCode
        {
            get { return locationCode; }
            set
            {
                locationCode = value;
                OnPropertyChanged();
            }
        }

        public ScanStockProductViewModel()
        {
            EditCommand = new Command<StockTakeScanProduct>(EditQuantity);
            SelectProductCommand = new Command(OpenProductsList);
            SelectedProduct = new ProductMasterSync() { Name = "None" };
        }

        public async Task Initialize()
        {
            Terminal = await App.Database.Vehicle.GetTerminalMetaData();
            if (Terminal.AllowStocktakeEdit)
            {
                ShouldShowEdit = true;
            }
        }

        void OpenProductsList(object obj)
        {
            ProductsPopup popup = new ProductsPopup();
            popup.OnProductSelected += (async (x) =>
            {
                SelectedProduct = x;
                SelectedProductIndex = Products.FindIndex((y) => y.ProductId == x.ProductId);
                await SelectProduct(SelectedProductIndex);
                if (!IsSerialEntryShowing)
                {
                    EmptyScannedProduct?.Invoke();
                }
            });
            popup.OnCancelClicked += () =>
            {
                FocusScanEntry?.Invoke(true);
            };
            PopupNavigation.PushAsync(popup);
        }

        async void EditQuantity(StockTakeScanProduct product)
        {
            IsActionButtonEnable = false;
            var qty = product.Product.ProcessByCase ? (product.Quantity / product.Product.ProductsPerCase ?? 1) : product.Quantity;
            var StockTakeQuantityPopup = new StockTakeQuantityPopup(Convert.ToDouble(qty), product);
            StockTakeQuantityPopup.SaveQuantity += async (obj, batchNumber, expiryDate) =>
            {
                var quantity = product.Product.ProcessByCase ? (Convert.ToDecimal(obj) * product.Product.ProductsPerCase ?? 1) : Convert.ToDecimal(obj);
                product.Quantity = quantity;
                product.Cases = Math.Round((product.Quantity / (decimal)(product.Product.ProductsPerCase ?? 1)), 2);
                await App.StockTakes.UpdateQuantity(product.Product.ProcessByCase ? product.Cases : product.Quantity, product.StockDetailId, batchNumber, expiryDate);
                FocusScanEntry?.Invoke(true);
                IsActionButtonEnable = true;

            };

            StockTakeQuantityPopup.Cancel += (obj) =>
            {
                FocusScanEntry.Invoke(true);
                IsActionButtonEnable = true;
            };
            await PopupNavigation.PushAsync(StockTakeQuantityPopup);
        }

        public async Task GetProductsAndSerials(bool IsStockEnquiry)
        {
            await Task.Run(async () =>
            {
                Products = await App.Products.GetAllProducts();
                if (!IsStockEnquiry)
                {
                    Serials = await App.Products.GetAllProductSerials();
                    //await GetAllScanProductRequest();
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    foreach (var product in Products)
                    {
                        AllProducts.Add(product.Name);
                    }
                });


            }).ConfigureAwait(false);
        }

        public async Task GetAllScanProductRequest()
        {

            var AllRequests = (await App.Database.StockTakes.GetAllStockTakeProductScanrequest()).FindAll((arg) => arg.StockTakeId == StockTake.StockTakeId);
            Device.BeginInvokeOnMainThread(() =>
            {
                foreach (var stockTakeRequest in AllRequests)
                {
                    var stockTakeScanProduct = new StockTakeScanProduct();
                    var product = Products.Find((obj) => obj.SKUCode == stockTakeRequest.ProductCode);
                    stockTakeScanProduct.Product = product;
                    stockTakeScanProduct.Quantity = stockTakeRequest.ScannedQuantity;
                    stockTakeScanProduct.Serial = stockTakeRequest.ProductSerial;


                    if (!string.IsNullOrEmpty(stockTakeRequest.PalletSerial))
                    {
                        stockTakeScanProduct.Serial = stockTakeRequest.PalletSerial;
                    }
                    stockTakeScanProduct.StockTakeRequestId = stockTakeRequest.Id;
                    stockTakeScanProduct.StockDetailId = stockTakeRequest.StockTakeId;
                    StockProducts.Insert(0, stockTakeScanProduct);
                }
            });
        }


        public async Task<bool> ScanProduct(string code)
        {
            var barcode = new GS128Decoder();
            code = barcode.GS128DecodeGTINOrDefault(code);
            var product = Products.Find((obj) => (!string.IsNullOrEmpty(obj.SKUCode) && obj.SKUCode.ToLower() == code.ToLower()) || (!string.IsNullOrEmpty(obj.BarCode) && obj.BarCode.ToLower() == code.ToLower()) || (!string.IsNullOrEmpty(obj.BarCode2) && obj.BarCode2.ToLower() == code.ToLower()));
            if (product != null && !product.Serialisable)
            {
                bool IsProceed = true;
                decimal quantity = 1;

                if (product.BarCode2 == code)
                {
                    quantity = product.ProcessByCase ? 1 : (decimal)(product.ProductsPerCase ?? 1);
                }
                //else if(product.BarCode2 == code)
                //{
                //    IsProceed = false;
                //    "Could not add due to some error".ToToast();
                //}
                if (IsProceed)
                {
                    if (product.ProcessByPallet)
                    {
                        var serialPopup = new StockTakeScanPalletPopup(product);
                        serialPopup.PalletAdded += async (obj) =>
                        {
                            quantity = obj.ProcessedQuantity;
                            await SendScannedProduct(product, "", quantity, obj.palletSerial, obj.isExistingPallet);
                        };
                        serialPopup.SaveClicked += () =>
                        {
                            FocusScanEntry?.Invoke(true);
                        };
                        serialPopup.CancelClicked += () =>
                        {
                            FocusScanEntry?.Invoke(true);
                        };
                        await PopupNavigation.PushAsync(serialPopup);
                    }
                    else
                    {
                        await SendScannedProduct(product, "", quantity);
                    }
                }
                return true;
            }
            else if (product != null)
            {
                IsSerialEntryShowing = true;
                FocusScanSerialEntry?.Invoke(true);
                return true;
            }
            else if (product == null)
            {
                var pallet = await App.Pallets.GetPalletTrackingWithSerial(code);
                if (pallet != null)
                {
                    var pdt = await App.Products.GetProductById(pallet.ProductId);
                    await SendScannedProduct(pdt, "", pallet.RemainingCases, pallet.PalletSerial, true);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> ScanSerial(string code)
        {
            var serial = Serials.Find((obj) => obj.SerialNo == code);
            if (serial != null)
            {
                if (StockProducts.ToList().Find((x) => x.Serial == code) != null)
                {
                    "Serial Already Added".ToToast();
                    return true;
                }
                var productInSerial = Products.Find((arg) => arg.ProductId == serial.ProductId);
                {
                    await SendScannedProduct(productInSerial, code, 1, "", true);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> SelectProduct(int index)
        {
            var product = Products[index];
            if (product != null)
            {
                if (product.Serialisable)
                {
                    IsSerialEntryShowing = true;
                    FocusScanSerialEntry?.Invoke(true);
                    //"This Product is Serialilised please scan serial.".ToToast();
                }
                else
                {
                    if (product.ProcessByPallet)
                    {
                        decimal quantity = 1;
                        var serialPopup = new PalletTrackingScanPage(product, true, null, null, null, null, false);
                        serialPopup.PalletListAdded += async (obj, quantityProcessed) =>
                        {
                            foreach (var pallet in ((List<PalleTrackingProcess>)obj))
                            {
                                quantity = pallet.ProcessedQuantity;
                                await SendScannedProduct(product, "", quantity);
                            }

                        };
                        await App.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(serialPopup));
                    }
                    else
                    {
                        await SendScannedProduct(product);
                    }
                    return true;
                }
            }
            return false;
        }


        public async Task SendScannedProduct(ProductMasterSync product, string serial = "", decimal quantity = 1, string palletSerial = "", bool existingItem = false)
        {
            if (product.Serialisable)
            {
                var serials = await App.Database.ProductSerials.GetProductSerialByProductId(product.ProductId);
            }

            if (!product.Serialisable && !product.ProcessByPallet && (product.RequiresBatchNumberOnReceipt ?? false) || (product.RequiresExpiryDateOnReceipt ?? false))
            {
                this.stockTakePopup = new StockTakePopup(product, serial, quantity, palletSerial, existingItem);
                stockTakePopup.ViewModel.OnSave += OnPopupSave;
                stockTakePopup.OnCancelClicked += OnCancelClicked;
                stockTakePopup.ViewModel.Back += (obj) =>
                {
                    stockTakePopup = null;
                };
                await PopupNavigation.PushAsync(stockTakePopup);
            }
            else
            {
                string batchNumber = null;
                DateTime? expiryDate = null;
                await StockTakeScan(product, batchNumber, expiryDate, serial, quantity, palletSerial, existingItem);
            }
        }


        private async void OnPopupSave(ProductMasterSync product, decimal quantity, string serial, string palletSerial, string batchNumber, DateTime? expiryDate, bool existingItem)
        {
            await StockTakeScan(product, batchNumber, expiryDate, serial, quantity, palletSerial, existingItem);
            stockTakePopup = null;
        }

        private void OnCancelClicked()
        {
            stockTakePopup = null;
        }

        public async Task StockTakeScan(ProductMasterSync product, string batchNumber, DateTime? expiryDate, string serial = "", decimal quantity = 1, string palletSerial = "", bool existingItem = false)
        {
            if (CrossConnectivity.Current.IsConnected && await Util.Util.IsConnected())
            {

                if (product.ProductId == 0)
                {
                    var existingProduct = Products.Find((obj) => (!string.IsNullOrEmpty(obj.SKUCode) && obj.SKUCode.ToLower() == product?.SKUCode?.ToLower()) || (!string.IsNullOrEmpty(obj.BarCode) && obj.BarCode.ToLower() == product.BarCode?.ToLower()) || (!string.IsNullOrEmpty(obj.BarCode2) && obj.BarCode2.ToLower() == product?.BarCode2?.ToLower()));
                    if (existingProduct != null) { product.ProductId = existingProduct.ProductId; }
                }


                var stockTakeProductScanResponse = await App.StockTakes.PostScanStockTakeProduct(LocationCode, product, StockTake.StockTakeId, StockTake.WarehouseId, batchNumber, expiryDate, serial, palletSerial, quantity, existingItem);
                if (stockTakeProductScanResponse != null && stockTakeProductScanResponse.Response.Success)
                {
                    var stockTakeScanProduct = new StockTakeScanProduct();
                    stockTakeScanProduct.Product = product;
                    stockTakeScanProduct.BatchNumber = batchNumber;
                    stockTakeScanProduct.ExpiryDate = expiryDate;
                    stockTakeScanProduct.Quantity = quantity;
                    if (product.ProcessByCase)
                        stockTakeScanProduct.Quantity = (decimal)(quantity * (product.ProductsPerCase ?? 1));
                    if (product.ProcessByPallet)
                        stockTakeScanProduct.Quantity = !(string.IsNullOrEmpty(palletSerial)) ? (decimal)(quantity * (product.ProductsPerCase ?? 1)) : quantity;
                    stockTakeScanProduct.Serial = stockTakeProductScanResponse.ProductSerial;
                    if (!string.IsNullOrEmpty(palletSerial))
                    {
                        stockTakeScanProduct.Serial = palletSerial;
                    }
                    stockTakeScanProduct.StockTakeRequestId = stockTakeProductScanResponse.Id;
                    stockTakeScanProduct.StockDetailId = stockTakeProductScanResponse.StockTakeDetailId;
                    if (product.ProductsPerCase == null)
                    {
                        product.ProductsPerCase = 1;
                    }

                    stockTakeScanProduct.Cases = Math.Round((stockTakeScanProduct.Quantity / (decimal)(product.ProductsPerCase ?? 1)), 2);
                    StockProducts.Insert(0, stockTakeScanProduct);
                }
                else if (stockTakeProductScanResponse == null)
                {
                    await Util.Util.ShowErrorPopupWithBeep("Unexpected error, please contact support.");
                }
            }
            else
            {
                await Util.Util.ShowErrorPopupWithBeep("No Internet Connection");
            }
        }

    }
}

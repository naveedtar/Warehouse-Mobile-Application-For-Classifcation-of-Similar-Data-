using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Ganedata.Core.Barcoding;
using Plugin.Connectivity;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Models.InventoryStocks;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Models.StockEnquiry;
using WarehouseHandheld.Modules;
using WarehouseHandheld.ViewModels.StockTake;

namespace WarehouseHandheld.ViewModels.StockEnquiry
{
    public class StockEnquiryViewModel : ScanStockProductViewModel
    {
        public List<InventoryStockSync> Stocks { get; set; }
         
        public async Task InitializeStocks()
        {
            await GetProductsAndSerials(true);
            Stocks = await App.InventoryStocks.GetAllInventoryStock();

        }

        ObservableCollection<ProductLocationsDetailResponse> productDetails;
        public ObservableCollection<ProductLocationsDetailResponse> ProductDetails
        {
            get { return productDetails; }
            set
            {
                productDetails = value;
                OnPropertyChanged();
            } 
        }

        private decimal stockQuantity;
        public decimal StockQuantity
        {
            get { return stockQuantity; }
            set
            {
                stockQuantity = value;
                OnPropertyChanged();
            }
        }

        private decimal casesinPallet;
        public decimal CasesinPallet
        {
            get { return casesinPallet; }
            set
            {
                casesinPallet = value;
                OnPropertyChanged();
            }
        }

        private decimal remainingProductsinPallet;
        public decimal RemainingProductsinPallet
        {
            get { return remainingProductsinPallet; }
            set
            {
                remainingProductsinPallet = value;
                OnPropertyChanged();
            }
        }

        private string productName;
        public string ProductName
        {
            get { return productName; }
            set
            {
                productName = value;
                OnPropertyChanged();
            }
        }

        public async new Task SelectProduct(int productIndex)
        {
            var product = Products[productIndex];
            if (product != null)
            {
                if(!await GetStock(product))
                {
                    "Stock Not Found".ToToast();
                }
            }

        }

        public async new Task<bool> ScanProduct(string code)
        {
            var palletTracking = await App.Pallets.GetPalletTrackingWithSerial(code);
            if (palletTracking != null)
            {
                var productPalletTracking = await App.Products.GetProductById(palletTracking.ProductId);
                if (productPalletTracking != null)
                {
                    "Scanned Pallet".ToToast();
                    CasesinPallet = palletTracking.RemainingCases;
                    if(productPalletTracking.ProductsPerCase != null && productPalletTracking.ProductsPerCase > 1)
                    {
                        RemainingProductsinPallet = (decimal)(casesinPallet * productPalletTracking.ProductsPerCase);
                    }

                    return await GetStock(productPalletTracking);
                }
            }

            var productSerial = await App.Products.GetProductSerialBySerialNo(code);
            if (productSerial != null)
            {
                var productBySerial = await App.Products.GetProductById(productSerial.ProductId);
                if (productBySerial != null)
                {
                     CasesinPallet = 0;
                     RemainingProductsinPallet = 0;
                    "Scanned Product By Serial".ToToast();
                     StockQuantity = 1;
                     return true;
                }
            }

            var barcode = new GS128Decoder();
            code = barcode.GS128DecodeGTINOrDefault(code);
            var product = Products.Find((obj)=>obj.SKUCode?.ToLower()==code.ToLower() || obj.BarCode?.ToLower()==code.ToLower() || obj.BarCode2?.ToLower()== code.ToLower());
            //var productSerial = await App.Products.GetProductSerialByProductId(product.ProductId);
            
            if (product != null)
            {
                CasesinPallet = 0;
                RemainingProductsinPallet = 0;
                return await GetStock(product);
            }
        
            ProductName = string.Empty;
            StockQuantity = 0;
            ProductDetails?.Clear();
            return false;
        }

        async Task<bool> GetStock(ProductMasterSync product)
        {
            ProductDetails?.Clear();
            ProductName = product.Name;
            InventoryStockSync stock = null;
            var warehouseId = 0;
            var terminalData = await App.Database.Vehicle.GetTerminalMetaData();
            if(terminalData!=null)
            {
                warehouseId = terminalData.ParentWarehouseId;
            }
            if (CrossConnectivity.Current.IsConnected && await Util.Util.IsConnected())
            {
                stock = await App.WarehouseService.StockEnquiry.GetStockTakesAsync(product.ProductId, ModulesConfig.SerialNo,warehouseId);

            }
            else
            {
                stock = Stocks.Find((obj) => obj.ProductId == product.ProductId);

            }

            if (stock != null)
            {
                if (stock.InStock >= 0)
                {
                    StockQuantity = stock.InStock;
                }
                else
                {
                    StockQuantity = 0;
                }

                if (stock.LocationDetails != null && stock.LocationDetails.ProductDetails != null)
                {
                    ProductDetails = new ObservableCollection<ProductLocationsDetailResponse>(stock.LocationDetails.ProductDetails);
                }

                if (stock.DateCreated != DateTime.MinValue)
                {
                    "Stock Found".ToToast();
                    return true;
                }
            }
            return false;
        }
    }


}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ganedata.Core.Barcoding;
using Newtonsoft.Json;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Models.Sync;
using WarehouseHandheld.Services;

namespace WarehouseHandheld.ViewModels.GenerateLabels
{
    public class GenerateLabelsViewModel : BaseViewModel
    {
        private string scanCode = string.Empty;
        public string ScanCode
        {
            get { return scanCode; }
            set
            {
                scanCode = value;
                OnPropertyChanged();
            }
        }

        private Decimal quantity = 0;
        public Decimal Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                OnPropertyChanged();
            }
        }

        List<ProductMasterSync> Products;
        public ProductMasterSync SelectedProduct;
        public GenerateLabelsViewModel()
        {
            GetAllProducts();
        }

        async void GetAllProducts()
        {
            Products = await App.Database.Products.GetAllProducts();

        }

        public bool ScanCodeTextChanged(string code)
        {
            if (string.IsNullOrEmpty(code))
                return false;

            var barcode = new GS128Decoder();
            code = barcode.GS128DecodeGTINOrDefault(code);
            SelectedProduct = Products.Find((obj) => (obj.BarCode != null && obj.BarCode.ToLower()==code.ToLower()) || (obj.BarCode2 != null && obj.BarCode2.ToLower()==code.ToLower()) ||( obj.SKUCode != null && obj.SKUCode.ToLower()==code.ToLower()));
            if (SelectedProduct != null && SelectedProduct.ProcessByPallet)
            {
                return true;
            }

            return false;
        }


        public bool ScanCodeTextChangedCases(string code)
        {
            if (string.IsNullOrEmpty(code))
                return false;

            var barcode = new GS128Decoder();
            code = barcode.GS128DecodeGTINOrDefault(code);
            SelectedProduct = Products.Find((obj) => (obj.BarCode != null && obj.BarCode.ToLower() == code.ToLower()) || (obj.BarCode2 != null && obj.BarCode2.ToLower() == code.ToLower()) || (obj.SKUCode != null && obj.SKUCode.ToLower() == code.ToLower()));
            if (SelectedProduct != null && SelectedProduct.ProcessByCase)
            {
                return true;
            }

            return false;
        }


        public async Task AddSyncLog(PalletTrackingSyncCollection palletTracking)
        {
            //await App.Pallets.AddPalletTrackingToDatabase(palletTracking);
            var syncLog = new SyncLog();
            syncLog.IsPost = true;
            syncLog.request = JsonConvert.SerializeObject(palletTracking);
            syncLog.RequestUrl = WebServiceConfig.PostPalletTracking;
            syncLog.TableName = Database.DatabaseConfig.Tables.PalletTracking.ToString();
            syncLog.TableName = Database.DatabaseConfig.Tables.PalletTracking.ToString();
            await App.Database.SyncLog.AddSyncLogItem(syncLog);
            await App.Pallets.SyncPalletTracking();
        }
    }
}

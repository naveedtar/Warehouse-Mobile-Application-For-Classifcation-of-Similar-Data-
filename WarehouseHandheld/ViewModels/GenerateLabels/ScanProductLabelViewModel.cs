using System.Collections.Generic;
using Ganedata.Core.Barcoding;
using WarehouseHandheld.Models.Products;

namespace WarehouseHandheld.ViewModels.GenerateLabels
{
    public class ScanProductLabelViewModel : BaseViewModel
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

        List<ProductMasterSync> Products;
        public ProductMasterSync SelectedProduct;
        public ScanProductLabelViewModel()
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
            SelectedProduct = Products.Find((obj) => (obj.BarCode != null && obj.BarCode.ToLower() == code.ToLower()) || (obj.BarCode2 != null && obj.BarCode2.ToLower() == code.ToLower()) || (obj.SKUCode != null && obj.SKUCode.ToLower() == code.ToLower()));
            if (SelectedProduct != null)
            {
                return true;
            }

            return false;
        }

    }
}


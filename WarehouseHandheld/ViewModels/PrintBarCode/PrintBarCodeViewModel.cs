using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ganedata.Core.Barcoding;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Helpers;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Models.StockEnquiry;
using WarehouseHandheld.Views.ScanItems;
using Xamarin.Forms;

namespace WarehouseHandheld.ViewModels.PrintBarCode
{
    public class PrintBarCodeViewModel : BaseViewModel
    {
        public int SelectedIndex = 0;
        public  List<ProductMasterSync> ProductList { get; set; }
        public async Task GetAllProducts()
        {
            ProductList = await App.Products.GetAllProducts();
        }

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

        private int printsNumber = 1;
        public int PrintsNumber
        {
            get { return printsNumber; }
            set
            {
                printsNumber = value;
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

        private string barCode;
        public string BarCode
        {
            get { return barCode; }
            set
            {
                barCode = value;
                OnPropertyChanged();
            }
        }


        private ProductMasterSync products;
        public ProductMasterSync Product
        {
            get { return products; }
            set
            {
                products = value;
                OnPropertyChanged();
            }
        }

        public async Task<bool> ScanCodeTextChanged(string code)
        {
            var barcode = new GS128Decoder();
            code = barcode.GS128DecodeGTINOrDefault(code);
            var product = ProductList.Find((obj) => obj.SKUCode?.ToLower() == code.ToLower() || obj.BarCode?.ToLower() == code.ToLower() || obj.BarCode2?.ToLower() == code.ToLower());
            if (product != null)
            {
               Product = product;
               ProductName = product.Name;
                if (SelectedIndex == 0)
                {
                    BarCode = product.SKUCode;
                }
                else if(SelectedIndex == 1)
                {
                    BarCode = product.BarCode;
                }
                else if (SelectedIndex == 2)
                {
                    BarCode = product.BarCode2;
                }
               return true;
            }
          
            return false;

        }
    }
}
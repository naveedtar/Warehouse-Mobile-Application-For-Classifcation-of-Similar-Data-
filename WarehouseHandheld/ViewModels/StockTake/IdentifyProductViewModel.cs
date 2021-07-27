using System;
using System.Windows.Input;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Views.Products;
using Xamarin.Forms;

namespace WarehouseHandheld.ViewModels.StockTake
{
    public class IdentifyProductViewModel : BaseViewModel
    {
        public ICommand SelectProduct { get; private set; }
        public string code;
        private ProductMasterSync product = new ProductMasterSync(){Name="None"};
        public ProductMasterSync Product
        {
            get { return product; }
            set
            {
                product = value;
                OnPropertyChanged();
            }
        }


        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        private string sku;
        public string SKU
        {
            get { return sku; }
            set
            {
                sku = value;
                OnPropertyChanged();
            }
        }

        private string barcode;
        public string Barcode
        {
            get { return barcode; }
            set
            {
                barcode = value;
                OnPropertyChanged();
            }
        }

        private string barcode2;
        public string Barcode2
        {
            get { return barcode2; }
            set
            {
                barcode2 = value;
                OnPropertyChanged();
            }
        }

        private decimal qunatity;
        public decimal Quantity
        {
            get { return qunatity; }
            set
            {
                qunatity = value;
                OnPropertyChanged();
            }
        }


        public IdentifyProductViewModel()
        {
            SelectProduct = new Command(OpenProductsList);
        }

        void OpenProductsList(object obj)
        {
            ProductsPopup popup = new ProductsPopup();
            popup.OnProductSelected+= ((x) => {
                Product = x;
            });
            PopupNavigation.PushAsync(popup);
        }

    }
}

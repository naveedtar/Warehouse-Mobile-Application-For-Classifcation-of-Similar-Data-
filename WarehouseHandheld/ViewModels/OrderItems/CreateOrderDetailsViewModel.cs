using System;
using System.Windows.Input;
using Xamarin.Forms;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Views.Products;
using WarehouseHandheld.Models.Products;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WarehouseHandheld.Models.OrderProcesses;
using System.Threading.Tasks;
using WarehouseHandheld.Modules;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Models.Orders;

namespace WarehouseHandheld.ViewModels.OrderItems
{
    public class CreateOrderDetailsViewModel : BaseViewModel
    {

        public ICommand SaveCommand { get; private set; }
        public OrderAccount Order { get; set; }

        public void OnProductSelected(ProductMasterSync product)
        {
            if(product != null)
            {
                SelectedProduct = product;
                SelectedProductName = product.Name;
            }
        }

        private string selectedProductName = "Select Product";
        public string SelectedProductName
        {
            get { return selectedProductName; }
            set
            {
                selectedProductName = value;
                OnPropertyChanged();
            }
        }

        private decimal quantity = 1;
        public decimal Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                OnPropertyChanged();
            }
        }

        private ProductMasterSync selectedProduct;
        public ProductMasterSync SelectedProduct
        {
            get { return selectedProduct; }
            set
            {
                selectedProduct= value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> processByValues;
        public ObservableCollection<string> ProcessByValues
        {
            get { return processByValues; }
            set
            {
                processByValues = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> warrantyValues;
        public ObservableCollection<string> WarrantyValues
        {
            get { return warrantyValues; }
            set
            {
                warrantyValues = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> taxValues;
        public ObservableCollection<string> TaxValues
        {
            get { return taxValues; }
            set
            {
                taxValues = value;
                OnPropertyChanged();
            }
        }

        public CreateOrderDetailsViewModel()
        {
            SaveCommand = new Command(SaveProduct);
            ProcessByValues = new ObservableCollection<string>();
            ProcessByValues.Add("By Item");
            ProcessByValues.Add("By Case");
            ProcessByValues.Add("By Pallet");

            WarrantyValues = new ObservableCollection<string>();
            WarrantyValues.Add("None");
            WarrantyValues.Add("Standard");
            WarrantyValues.Add("Extended");
            WarrantyValues.Add("Extended 2 years");

            TaxValues = new ObservableCollection<string>();
            TaxValues.Add("VAT Standard - 20%");
            TaxValues.Add("VAT Reduced - 5%");
            TaxValues.Add("VAT Zero - 0%");
        }

        async void SaveProduct(object obj)
        {
            await PopupNavigation.PopAsync();
            if(SelectedProduct!=null)
            {
                List<OrderProcessDetailSync> details = new List<OrderProcessDetailSync>();
                details.Add(new OrderProcessDetailSync()
                {
                    ProductId = selectedProduct.ProductId,
                    QtyProcessed = 1
                });
                await App.OrderProcesses.AddOrderProcess(details, Order.Order, true,"","");
            }
        }

    }
}

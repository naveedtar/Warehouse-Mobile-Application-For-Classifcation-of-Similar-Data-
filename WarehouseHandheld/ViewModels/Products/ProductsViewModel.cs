using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WarehouseHandheld.Models.Products;
namespace WarehouseHandheld.ViewModels.Products
{
    public class ProductsViewModel : BaseViewModel
    {
        List<ProductMasterSync> AllProducts;
        private ObservableCollection<ProductMasterSync> products;
        public ObservableCollection<ProductMasterSync> Products
        {
            get { return products; }
            set
            {
                products = value;
                OnPropertyChanged();
            }
        }

        public ProductsViewModel()
        {
            Initialize();
        }

        public async void Initialize()
        {
            AllProducts = await App.Products.GetAllProducts();
            Products = new ObservableCollection<ProductMasterSync>();
        }

        public void FindProducts(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Products = new ObservableCollection<ProductMasterSync>();
            }
            else
            {

                List<ProductMasterSync> FoundProducts = AllProducts.FindAll((obj) => obj.Name.ToLower().Contains(text.ToLower()) || obj.SKUCode.Contains(text));
                Products = new ObservableCollection<ProductMasterSync>(FoundProducts);
            }
        }
    }
}

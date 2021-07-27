using System;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Models.Orders;
using System.Collections.Generic;
using WarehouseHandheld.Models.OrderProcesses;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace WarehouseHandheld.ViewModels.ProductQuantity
{
    public class ProductQuantityViewModel : BaseViewModel
    {

        private OrderDetailsProduct orderDetail;
        public OrderDetailsProduct OrderDetail
        {
            get { return orderDetail; }
            set
            {
                orderDetail = value;
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

        private List<OrderProcessDetailSync> orderProcessDetails;
        public List<OrderProcessDetailSync> OrderProcessDetails
        {
            get { return orderProcessDetails; }
            set
            {
                orderProcessDetails = value;
                OnPropertyChanged();
            }
        }

        public ProductQuantityViewModel()
        {
            OrderProcessDetails = new List<OrderProcessDetailSync>();
        }

        public void OnSaveClicked()
        {
            MessagingCenter.Send<ProductQuantityViewModel, List<OrderProcessDetailSync>>(this, "SendOrderProcesses", OrderProcessDetails);
        }

        public async Task<bool> OnCancelClicked()
        {
            if (OrderProcessDetails.Count > 0)
                return await App.Current.MainPage.DisplayAlert("Alert", "You have unsaved changes, still want to leave ?", "yes", "no");
            else
                return true;
        }

        public async Task<bool> ScanCodeTextChanged(string code)
        {
            if (string.IsNullOrEmpty(code))
                return false;
            var products = await App.Products.GetAllProducts();

            foreach (var item in products)
            {
                if (item.SKUCode.ToLower().Equals(code.ToLower()))
                {
                    //creating order process detail for each product being added
                    OrderProcessDetails.Add(new OrderProcessDetailSync() { ProductId = item.ProductId });
                    (item.Name + " added").ToToast();
                    return true;
                }
            }
            return false;
        }
    }
}

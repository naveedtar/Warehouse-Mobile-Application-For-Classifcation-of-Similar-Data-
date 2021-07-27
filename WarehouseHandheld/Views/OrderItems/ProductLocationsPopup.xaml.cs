using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Models.ProductStockLocation;
using WarehouseHandheld.Models.StockMovement;
using WarehouseHandheld.Resources;
using WarehouseHandheld.Views.Base.Popup;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WarehouseHandheld.Views.OrderItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProductLocationsPopup : PopupBase
    {
        private int _productId;
        private ObservableCollection<ProductLocationStocksSync> productlocations;
        public ObservableCollection<ProductLocationStocksSync> ProductLocations
        {
            get { return productlocations; }
            set
            {
                productlocations = value;
                OnPropertyChanged(nameof(ProductLocations));
            }
        }
        public ProductLocationsPopup(int productId)
        {
            InitializeComponent();
            _productId = productId;
            Constants.SetGridProperties(grid);
        }

        protected override async void OnAppearing()
        {
            var locations = await App.Database.ProductLocationStock.GetProductStockLocationsSortedByProductId(_productId);
            if (locations != null && locations.Any())
            {
                ProductLocations = new ObservableCollection<ProductLocationStocksSync>(locations);
            }
        }
    }
}
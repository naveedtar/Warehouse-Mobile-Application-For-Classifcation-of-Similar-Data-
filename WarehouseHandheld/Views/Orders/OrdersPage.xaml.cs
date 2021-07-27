using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Resources;
using WarehouseHandheld.ViewModels.Orders;
using Xamarin.Forms;
using static WarehouseHandheld.Models.Orders.OrdersSync;
using WarehouseHandheld.Views.Base.BaseContentPage;

namespace WarehouseHandheld.Views.Orders
{
    public partial class OrdersPage : BasePage
    {
        public OrdersViewModel ViewModel => BindingContext as OrdersViewModel;
        public OrdersPage(InventoryTransactionTypeEnum orderType, bool IsGoodsReceive = false)
        {
            if(IsGoodsReceive)
            {
                ToolbarItems.Clear();
            }
            InitializeComponent();
            ViewModel.IsGoodsReceive = IsGoodsReceive;
            if (IsGoodsReceive)
            {
                grid.Columns[2].Title = "Date";
                grid.Columns.RemoveAt(2);
                grid.Columns.Add(new Xamarin.Forms.DataGrid.DataGridColumn() { PropertyName = "Order.DateCreated" });

            }

            Title = orderType.ToString();
            ViewModel.OrderType = orderType;
            InitializeViewMode();
            Constants.SetGridProperties(grid);
        }

        async void InitializeViewMode()
        {
            await ViewModel.Initialize(ViewModel.OrderType);
        }
        

        async void Add_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new CreateOrderPage());
        }


        async void SearchOrder(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            var searchText = Search.Text;
            if (!string.IsNullOrEmpty(searchText))
            {
                var orders = new List<OrderAccount>(ViewModel.Orders);
                ViewModel.Orders.Clear();
                var ordersByAccount = orders.Where(c => c.Account.CompanyName.ToLower().Contains(searchText.ToLower()) || c.Order.OrderNumber.ToLower().Contains(searchText.ToLower()));       
                if (ordersByAccount != null)
                {
                    foreach (var order in ordersByAccount)
                    {
                        ViewModel.Orders.Add(order);
                    }
                }
                if (ordersByAccount == null)
                    ViewModel.Orders.Clear();

            }
            else
            {
                await ViewModel.Initialize(ViewModel.OrderType);
            }
        }
    }
}

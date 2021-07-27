using System;
using System.Collections.Generic;
using System.Linq;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.ViewModels.Orders.PickList;
using Xamarin.Forms;
using Xamarin.Forms.DataGrid;
using static WarehouseHandheld.Models.Orders.OrdersSync;
using WarehouseHandheld.Views.Base.BaseContentPage;

namespace WarehouseHandheld.Views.Orders.PickList
{
    public partial class PickListPage : BasePage
    {
        public PickListViewModel ViewModel => BindingContext as PickListViewModel;

        public PickListPage(InventoryTransactionTypeEnum orderType)
        {
            InitializeComponent();
            ViewModel.OrderType = orderType;
            grid.RowsBackgroundColorPalette = new ColorProvider();

        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitializeViewModel();

        }

        async void InitializeViewModel()
        {
            await ViewModel.Initialize();
        }


        async void SearchOrder(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            var searchText = Search.Text;
            if (!string.IsNullOrEmpty(searchText))
            {
                var orders = new List<OrderAccount>(ViewModel.Orders);
                ViewModel.Orders.Clear();
                var ordersByAccount = orders.Where(c => c.Account != null && c.Account.CompanyName.ToLower().Contains(searchText.ToLower()) || c.Order != null && c.Order.OrderNumber.ToLower().Contains(searchText.ToLower()));
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

    public class ColorProvider : IColorProvider
    {
        public Color GetColor (int a, object o)
        {
            var order = (OrderAccount)o;
            switch (order.Order.InventoryTransactionTypeId)
            {
                case (int)InventoryTransactionTypeEnum.SaleOrder:
                    return Color.LightBlue;
                case (int)InventoryTransactionTypeEnum.WorkOrder:
                    return Color.LightGreen;
                case (int)InventoryTransactionTypeEnum.Samples:
                    return Color.LightPink;
                case (int)InventoryTransactionTypeEnum.Loan:
                    return Color.LightCoral;
                default:
                    return Color.LightSeaGreen;
            }
                        
        }
    }
}

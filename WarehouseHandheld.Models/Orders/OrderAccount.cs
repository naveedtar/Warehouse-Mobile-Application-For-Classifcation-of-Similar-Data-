using System;
using System.ComponentModel;
using WarehouseHandheld.Models.Accounts;
using WarehouseHandheld.Models.InventoryStocks;
using Xamarin.Forms;

namespace WarehouseHandheld.Models.Orders
{
    public class OrderAccount
    {
        public OrdersSync Order { get; set; }
        public AccountSync Account { get; set; }
        public Color RowColor
        {
            get
            {
                // Order.SLAPriorityId = 2;
                if (Order != null && Order.SLAPriorityId != null && Order.SLAPriorityId > 0)
                {
                    if (Order.SLAPriorityId == 1)
                    {
                        return Color.FromHex("#e36639");
                    }
                    if (Order.SLAPriorityId == 2)
                    {
                        return Color.FromHex("#ef9d38");
                    }
                    if (Order.SLAPriorityId == 3)
                    {
                        return Color.FromHex("#bbd4ab");
                    }
                }
                return Color.Transparent;
            }
        }
    }
}

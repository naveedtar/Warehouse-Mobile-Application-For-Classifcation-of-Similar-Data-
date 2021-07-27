using System;
using System.Globalization;
using Xamarin.Forms;
using static WarehouseHandheld.Models.Orders.OrdersSync;

namespace WarehouseHandheld.ValueConverters
{
    public class OrderTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch((InventoryTransactionTypeEnum)((int)value))
            {
                case InventoryTransactionTypeEnum.SaleOrder:
                    return "SO";
                case InventoryTransactionTypeEnum.PurchaseOrder:
                    return "PO";
                case InventoryTransactionTypeEnum.WorkOrder:
                    return "WO";
                default:
                    return ((InventoryTransactionTypeEnum)((int)value)).ToString();
                
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
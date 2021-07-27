using System;
using System.Globalization;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.Orders.TransferOrders.Converters
{
    public class TransferInOutConverter : IValueConverter
    {
        public static string In = "In";
        public static string Out = "Out";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return In;

            if(value.ToString() == "3")
            {
                return In;
            }
            return Out;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

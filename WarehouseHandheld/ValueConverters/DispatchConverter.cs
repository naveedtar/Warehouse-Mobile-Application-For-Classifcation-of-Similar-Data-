using System;
using System.Globalization;
using WarehouseHandheld.Resources;
using Xamarin.Forms;
namespace WarehouseHandheld.ValueConverters
{
    public class DispatchConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((bool)value)
            {
                return AppStrings.Dispatched;
            }
            else{
                return AppStrings.Dispatch;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    
    }
}

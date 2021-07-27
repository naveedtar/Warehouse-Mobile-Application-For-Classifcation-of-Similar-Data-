using System;
using Xamarin.Forms;
using WarehouseHandheld.Providers.ToastNotifier;

namespace WarehouseHandheld.Extensions
{
    public static partial class StringExtensions
    {
        public static void ToToast(this string message, string title = null, bool showOnTop = false)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var toaster = DependencyService.Get<IToastNotifier>();
                toaster?.Notify(title, message, TimeSpan.FromMilliseconds(300), showOnTop: showOnTop);
            });
        }
    }
}

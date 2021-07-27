using System;
using System.Threading.Tasks;
using Android.Widget;
using WarehouseHandheld.Droid.Providers.ToastNotifier;
using WarehouseHandheld.Providers.ToastNotifier;
using Xamarin.Forms;
using Android.Views;

[assembly: Dependency(typeof(ToastNotifierProvider))]

namespace WarehouseHandheld.Droid.Providers.ToastNotifier
{
    public class ToastNotifierProvider : IToastNotifier
    {
        public Task<bool> Notify( string title, string description, TimeSpan duration, object context = null, bool showOnTop = true)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            var toast = Toast.MakeText(Forms.Context, description, ToastLength.Short);
            if(showOnTop)
                toast.SetGravity(GravityFlags.Top, 0, 0);
            toast.Show();
            return taskCompletionSource.Task;
        }

        public void HideAll()
        {
        }
    }
}

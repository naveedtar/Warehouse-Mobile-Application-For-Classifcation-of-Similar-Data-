using System;
using WarehouseHandheld.Droid.Helpers;
using WarehouseHandheld.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(CrashLogHelper))]
namespace WarehouseHandheld.Droid.Helpers
{
    public class CrashLogHelper : ICrashLogHelper
    {
        public void CrashLogs(string LogText)
        {
            //Crashlytics.Instance.Log(LogText);
        }
    }
}

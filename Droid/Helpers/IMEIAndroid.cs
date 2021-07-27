using System;
using Android.Net.Wifi;
using Android.Telephony;
using WarehouseVanSales.Droid.Helpers;
using WarehouseVanSales.Helpers;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(IMEIAndroid))]
namespace WarehouseVanSales.Droid.Helpers
{
    public class IMEIAndroid : IDeviceIMEI
    {
        public string GetIdentifier()
        {
            //TelephonyManager telephonyManager = (TelephonyManager)Forms.Context.GetSystemService(Android.Content.Context.TelephonyService);
            var _deviceAndroidId = Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            if (!string.IsNullOrEmpty(_deviceAndroidId))
            {
                return _deviceAndroidId;
            }
            else if (!string.IsNullOrEmpty(Android.OS.Build.Serial) && Android.OS.Build.Serial != "unknown")
            {
                return Android.OS.Build.Serial;
            }
            else
            {
                WifiManager wifiMan = (WifiManager)Forms.Context.GetSystemService(
                    Android.Content.Context.WifiService);
                WifiInfo wifiInf = wifiMan.ConnectionInfo;
                String macAddr = wifiInf.MacAddress;
                return macAddr;
            }
        }

        public string ApplicationsPublicVersion { get; set; }
        public string ApplicationsPrivateVersion { get; set; }

        public IMEIAndroid()
        {
            var context = Android.App.Application.Context;
            var info = context.PackageManager.GetPackageInfo(context.PackageName, 0);

            ApplicationsPublicVersion = info.VersionName;
            ApplicationsPrivateVersion = info.VersionCode.ToString();
        }


    }
}

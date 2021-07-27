using System;
using WarehouseHandheld.Models.Vehicles;
using WarehouseVanSales.Helpers;
using Xamarin.Forms;

namespace WarehouseHandheld.Modules
{
    public static class ModulesConfig
    {
        public static string deviceIMEI => DependencyService.Get<IDeviceIMEI>().GetIdentifier();
        public static string SerialNo = "6647164900009";
        public static DateTime SyncDate = new DateTime(2000, 01, 01);
        public static int SyncTime = 2; //2 minutes.
        public static int SyncInterval = 2; // Sync Interval
        public static int TenantID = 0; //This value will be updated on getting response of Terminal data.
        public static int WareHouseID = 2;
        public static PalletTrackingSchemeEnum TrackingScheme = PalletTrackingSchemeEnum.ByExpiryMonth;

    }
}

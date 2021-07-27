using System;
namespace WarehouseVanSales.Helpers
{
    public interface IDeviceIMEI
    {
        string GetIdentifier(); 
        string ApplicationsPublicVersion { get; set; }
        string ApplicationsPrivateVersion { get; set; }
    }
}

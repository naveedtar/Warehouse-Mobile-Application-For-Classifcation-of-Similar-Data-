using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.GeoLocation;


namespace WarehouseHandheld.Services.GeoLocation
{
    public interface IPostGeoLocationService
    {
        Task<string> PostUserLocationAsync(TerminalGeoLocationViewModel request);
    }
}

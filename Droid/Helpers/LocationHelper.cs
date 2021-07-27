using System;
using Plugin.Geolocator;
using WarehouseHandheld.Helpers;
using WarehouseHandheld.Droid.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocationHelper))]
namespace WarehouseHandheld.Droid.Helpers
{
    public class LocationHelper : ILocationHelper
    {
        GeoLocation LastKnowLocation;
        public async System.Threading.Tasks.Task<GeoLocation> GetLocation()
        {
            try
            {
                var canceltoken = new System.Threading.CancellationTokenSource();
                var locator = CrossGeolocator.Current;
                var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(15000), canceltoken.Token, false);
                if (position != null)
                {
                    var location = new GeoLocation();

                    location.Latitude = position.Latitude;
                    location.Longitude = position.Longitude;
                    LastKnowLocation = location;
                    return location;
                }
                return LastKnowLocation;
            }
            catch (Exception ex)
            {
                return LastKnowLocation;

            }
        }
    }
}

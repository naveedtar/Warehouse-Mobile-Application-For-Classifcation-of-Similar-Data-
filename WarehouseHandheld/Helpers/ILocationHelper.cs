using System;
namespace WarehouseHandheld.Helpers
{
    public interface ILocationHelper
    {
        System.Threading.Tasks.Task<GeoLocation> GetLocation();
    }
    public class GeoLocation{
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}

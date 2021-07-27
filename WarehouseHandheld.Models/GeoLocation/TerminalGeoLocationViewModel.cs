using System;
namespace WarehouseHandheld.Models.GeoLocation
{
    public class TerminalGeoLocationViewModel
    {
        public Guid Id { get; set; }
        public int TerminalId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime Date { get; set; }
        public int? LoggedInUserId { get; set; }
        public int TenantId { get; set; }
        public string SerialNo { get; set; }
    }
}

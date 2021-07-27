using System;
using SQLite;

namespace WarehouseHandheld.Models.Vehicles
{
    public class MarketVehiclesSync
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string VehicleIdentifier { get; set; }
        public int? MarketId { get; set; }
        public int TenantId { get; set; }
        public string MarketName { get; set; }
        public bool? IsDeleted { get; set; }
    }
}

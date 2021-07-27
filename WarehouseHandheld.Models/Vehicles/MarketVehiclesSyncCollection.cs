using System;
using System.Collections.Generic;

namespace WarehouseHandheld.Models.Vehicles
{
    public class MarketVehiclesSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public List<MarketVehiclesSync> Vehicles { get; set; }
    }
}
   
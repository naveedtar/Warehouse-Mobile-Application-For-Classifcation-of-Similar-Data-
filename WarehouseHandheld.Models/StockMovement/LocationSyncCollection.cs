using System;
using System.Collections.Generic;
using SQLite;

namespace WarehouseHandheld.Models.StockMovement
{
    public class LocationSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }

        [IgnoreAttribute]
        public List<LocationSync> LocationSync { get; set; }

    }
}

using System;
using System.Collections.Generic;
using SQLite;

namespace WarehouseHandheld.Models.StockTakes
{
    public class StockTakeSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }

        [IgnoreAttribute]
        public List<StockTakeSync> StockTakes { get; set; }
    }
}

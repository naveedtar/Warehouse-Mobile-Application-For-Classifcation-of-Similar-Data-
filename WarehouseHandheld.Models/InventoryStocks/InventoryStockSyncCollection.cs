using System;
using System.Collections.Generic;
using SQLite;

namespace WarehouseHandheld.Models.InventoryStocks
{
    public class InventoryStockSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }

        [IgnoreAttribute]
        public List<InventoryStockSync> InventoryStocks { get; set; }
    }
}

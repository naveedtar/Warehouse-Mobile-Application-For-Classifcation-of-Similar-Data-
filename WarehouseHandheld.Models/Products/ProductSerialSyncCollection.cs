using System;
using System.Collections.Generic;
using SQLite;

namespace WarehouseHandheld.Models.Products
{
    public class ProductSerialSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }

        [IgnoreAttribute]
        public List<ProductSerialSync> ProductSerials { get; set; }
    }
}

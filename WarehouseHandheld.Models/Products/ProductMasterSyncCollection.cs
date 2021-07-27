using System;
using System.Collections.Generic;
using SQLite;

namespace WarehouseHandheld.Models.Products
{
    public class ProductMasterSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }

        [IgnoreAttribute]
        public List<ProductMasterSync> Products { get; set; }
    }
}

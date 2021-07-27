using System;
using System.Collections.Generic;
using SQLite;

namespace WarehouseHandheld.Models.Orders
{
    public class OrdersSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }

        [IgnoreAttribute]
        public List<OrdersSync> Orders { get; set; }
    }

}

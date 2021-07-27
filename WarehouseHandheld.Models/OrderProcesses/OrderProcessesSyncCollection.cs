using System;
using System.Collections.Generic;
using SQLite;

namespace WarehouseHandheld.Models.OrderProcesses
{
    public class OrderProcessesSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public string SerialNo { get; set; }
        public Guid TransactionLogId { get; set; }

        [IgnoreAttribute]
        public List<OrderProcessSync> OrderProcesses { get; set; } = new List<OrderProcessSync>();
    }
}

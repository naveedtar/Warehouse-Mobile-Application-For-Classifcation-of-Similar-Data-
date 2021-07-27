using System;
using System.Collections.Generic;

namespace WarehouseHandheld.Models.Orders
{
    public class OrderReceiveCountSyncCollection
    {
        public OrderReceiveCountSyncCollection()
        {
            OrderReceiveCountSync = new List<OrderReceiveCountSync>();
        }
        public Guid TerminalLogId { get; set; }
        public Guid TransactionLogId { get; set; }
        public string SerialNo { get; set; }
        public int Count { get; set; }
        public List<OrderReceiveCountSync> OrderReceiveCountSync { get; set; }
    }
}

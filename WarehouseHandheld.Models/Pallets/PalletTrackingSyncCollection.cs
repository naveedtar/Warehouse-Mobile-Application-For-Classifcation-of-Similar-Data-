using System;
using System.Collections.Generic;

namespace WarehouseHandheld.Models.Pallets
{
    public class PalletTrackingSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public Guid TransactionLogId { get; set; }
        public string SerialNo { get; set; }
        public int Count { get; set; }
        public List<PalletTrackingSync> PalletTrackingSync { get; set; }
    }
}

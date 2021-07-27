using System;
using System.Collections.Generic;

namespace WarehouseHandheld.Models.Pallets
{
    public class PalletsDispatchSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public string SerialNo { get; set; }
        public int Count { get; set; }
        public List<PalletDispatchSync> PalletDispatchSync { get; set; }
    }
}

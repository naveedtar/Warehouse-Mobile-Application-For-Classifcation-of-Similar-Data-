using System;
using System.Collections.Generic;
using SQLite;

namespace WarehouseHandheld.Models.Pallets
{
    public class PalletDispatchMethodSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public string SerialNo { get; set; }
        public int Count { get; set; }
        [IgnoreAttribute]
        public List<PalletDispatchMethodSync> PalletDispatchMethods { get; set; }
    }
}

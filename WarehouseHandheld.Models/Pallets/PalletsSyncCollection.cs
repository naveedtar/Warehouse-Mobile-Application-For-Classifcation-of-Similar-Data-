using System;
using System.Collections.Generic;
using SQLite;

namespace WarehouseHandheld.Models.Pallets
{
    public class PalletsSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public string SerialNo { get; set; }
        public int Count { get; set; }

        [IgnoreAttribute]
        public List<PalletSync> Pallets { get; set; }
    }
}

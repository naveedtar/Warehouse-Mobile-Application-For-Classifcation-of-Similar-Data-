using System;
using System.Collections.Generic;
using SQLite;

namespace WarehouseHandheld.Models.Pallets
{
    public class PalletProductsSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public string SerialNo { get; set; }
        public int Count { get; set; }
        public Guid TransactionLogId { get; set; }

        [IgnoreAttribute]
        public List<PalletProductsSync> PalletProducts { get; set; }

    }
}

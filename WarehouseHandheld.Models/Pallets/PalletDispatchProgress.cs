using System;
using System.Collections.Generic;

namespace WarehouseHandheld.Models.Pallets
{
    public class PalletDispatchProgress
    {
        public Guid TerminalLogId { get; set; }
        public Guid TransactionLogId { get; set; }
        public string SerialNo { get; set; }
        public int DispatchId { get; set; }
        public int DispatchStatus { get; set; }
        public string Comments { get; set; }
        public List<int> ScannedPalletSerials { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public string ReceiverName { get; set; }
        public byte[] ReceiverSign { get; set; }
    }
}

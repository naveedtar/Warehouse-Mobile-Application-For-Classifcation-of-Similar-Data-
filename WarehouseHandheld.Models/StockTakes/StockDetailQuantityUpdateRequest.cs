using System;
using SQLite;

namespace WarehouseHandheld.Models.StockTakes
{
    public class StockDetailQuantityUpdateRequest
    {
        [PrimaryKey]
        public int StockTakeDetailId { get; set; }
        public bool IsSynced { get; set; }
        public int StockTakeRequestId { get; set; }
        public decimal NewQuantity { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public Guid TransactionLogId { get; set; }
    }
}

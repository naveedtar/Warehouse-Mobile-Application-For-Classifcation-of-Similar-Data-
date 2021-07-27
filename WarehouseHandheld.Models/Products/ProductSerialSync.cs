using System;
using SQLite;
using static WarehouseHandheld.Models.Orders.OrdersSync;

namespace WarehouseHandheld.Models.Products
{
    public class ProductSerialSync
    {
        [PrimaryKey]
        public int SerialID { get; set; }
        public string SerialNo { get; set; }
        public int CurrentStatus { get; set; } // Inventory TransactionType Enum
        public string Batch { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public decimal BuyPrice { get; set; }
        public int ProductId { get; set; }
    }
}

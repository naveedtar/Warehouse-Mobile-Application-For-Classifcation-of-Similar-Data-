using System;
namespace WarehouseHandheld.Models.StockEnquiry
{
    public class ProductLocationBatchResponse
    {
        public int LocationId { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string ExpiryDateString { get; set; }
        public decimal Quantity { get; set; }
    }
}

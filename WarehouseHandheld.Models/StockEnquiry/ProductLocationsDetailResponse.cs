using System;
using System.Collections.Generic;

namespace WarehouseHandheld.Models.StockEnquiry
{
    public class ProductLocationsDetailResponse
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public string LocationCode { get; set; }
        public string Location { get; set; }
        public string Serial { get; set; }
        public string ProductName { get; set; }
        public List<ProductLocationBatchResponse> Batches { get; set; }
        public int ProductId { get; set; }
        public string IsSerializable { get; set; }
    }
}

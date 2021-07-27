using System;
using System.Collections.Generic;

namespace WarehouseHandheld.Models.StockEnquiry
{
    public class ProductLocationsResponse
    {
        public ProductLocationsResponse()
        {
            ProductDetails = new List<ProductLocationsDetailResponse>();
        }
        public List<ProductLocationsDetailResponse> ProductDetails { get; set; }
        public bool ContainsBatches { get; set; }
        public bool ContainsExpiryDate { get; set; }
        public bool Serialised { get; set; }
    }
}

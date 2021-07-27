using System;
namespace WarehouseHandheld.Models.StockTakes
{
    public class StockTakeProductCodeScanResponse : StockTakeProductCodeScanModel
    {
        public StockTakeProductCodeScanResponse()
        {
            Response = new ResponseObject();
        }

        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public ResponseObject Response { get; set; }
        public decimal InStock { get; set; }
        public decimal Allocated { get; set; }
        public decimal Available { get; set; }
        public bool SerialRequired { get; set; }
        public bool PalletSerialRequired { get; set; }
        public int StockTakeDetailId { get; set; }
        public int StockDetailQuantity { get; set; }

    }
}

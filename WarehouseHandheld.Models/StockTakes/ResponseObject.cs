using System;
namespace WarehouseHandheld.Models.StockTakes
{
    public class ResponseObject
    {
        public ResponseObject()
        {
            Success = true;
            ResponseTime = DateTime.UtcNow;
        }
        public bool Success { get; set; }

        public string FailureMessage { get; set; }

        public DateTime ResponseTime { get; set; }

        public bool HasWarning { get; set; }

        public string WarningMessage { get; set; }

        public bool SerialRequired { get; set; }

        public int ProductId { get; set; }
        public bool MoveToNextProduct { get; set; }
        public bool ProductDontExist { get; set; }
        public bool SerialInsteadProduct { get; set; }

        public string ProductCode { get; set; }
    }
}

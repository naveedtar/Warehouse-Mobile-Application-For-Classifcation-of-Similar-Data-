using System;
namespace WarehouseHandheld.Models.Returns
{
    public class GoodsReturnResponse: GoodsReturnRequestSync
    {
        public bool IsSuccess { get; set; }
        public bool CanProceed { get; set; }
        public string FailureMessage { get; set; }
        public int orderId { get; set; }
    }
}

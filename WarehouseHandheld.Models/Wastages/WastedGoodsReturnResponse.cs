using System;
namespace WarehouseHandheld.Models.Wastages
{
    public class WastedGoodsReturnResponse: WastedGoodsReturnRequestSync
    {
        public bool IsSuccess { get; set; }
        public bool CanProceed { get; set; }
        public string FailureMessage { get; set; }
    }
}

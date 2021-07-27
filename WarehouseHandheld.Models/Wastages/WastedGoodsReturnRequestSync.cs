using System;
namespace WarehouseHandheld.Models.Wastages
{
    public class WastedGoodsReturnRequestSync
    {
        public string SerialNo { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int LocationId { get; set; }
        public int Quantity { get; set; }
        public string[] Serials { get; set; }
        public string ReturnReason { get; set; }
    }
}

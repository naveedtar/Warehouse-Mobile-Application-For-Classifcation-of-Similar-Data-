using System;
namespace WarehouseHandheld.Models.Orders
{
    public class OrderReceiveCountDetailSync
    {
        public Guid ReceiveCountDetailId { get; set; }
        public Guid ReceiveCountId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Counted { get; set; }
        public decimal Demaged { get; set; }
        public int OrderDetailID { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int TenentId { get; set; }
        public bool? IsDeleted { get; set; }
    }
}

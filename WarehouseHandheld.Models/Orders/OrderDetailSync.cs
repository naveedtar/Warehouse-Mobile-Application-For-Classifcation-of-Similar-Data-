using System;
using SQLite;

namespace WarehouseHandheld.Models.Orders
{
    public class OrderDetailSync
    {
        [PrimaryKey]
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int WarehouseId { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string Notes { get; set; }
        public int ProductId { get; set; }
        public int? ProdAccCodeID { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public int? WarrantyID { get; set; }
        public decimal WarrantyAmount { get; set; }
        public int? TaxID { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public int TenentId { get; set; }
        public bool? IsDeleted { get; set; }
        public int? OrderDetailStatusId { get; set; }
        public int SortOrder { get; set; }
    }
}

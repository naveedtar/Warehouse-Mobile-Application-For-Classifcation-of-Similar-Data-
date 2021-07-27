using System;
using SQLite;

namespace WarehouseHandheld.Models.Products
{
    public class ProductKitMapViewModel
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int KitProductId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public bool? IsDeleted { get; set; }
        public int TenantId { get; set; }
        public decimal Quantity { get; set; }
        public ProductKitTypeEnum ProductKitType { get; set; }
    }

    public enum ProductKitTypeEnum
    {
        Kit = 1,
        Grouped = 2,
        Recipe = 3,
    }
}

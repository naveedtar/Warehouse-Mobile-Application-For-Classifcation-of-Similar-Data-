using System;
using System.Collections.Generic;

namespace WarehouseHandheld.Models.Orders
{
    public class OrderReceiveCountSync
    {
        public Guid ReceiveCountId { get; set; }
        public int OrderID { get; set; }
        public string ReferenceNo { get; set; }
        public string Notes { get; set; }
        public int WarehouseId { get; set; }
        public int TenantId { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public bool? IsDeleted { get; set; }
        public virtual ICollection<OrderReceiveCountDetailSync> OrderReceiveCountDetail { get; set; }
    }
}

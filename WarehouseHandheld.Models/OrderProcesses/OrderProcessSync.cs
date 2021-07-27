using System;
using System.Collections.Generic;
using SQLite;
using WarehouseHandheld.Models.Accounts;
using WarehouseHandheld.Models.Orders;

namespace WarehouseHandheld.Models.OrderProcesses
{
    public class OrderProcessSync
    {
        public OrderProcessSync()
        {
            OrderProcessDetails = new List<OrderProcessDetailSync>();
        }

        /*Create a token on every new sales session, 
         * then create order process details for every product you sell and assign this token against it. 
         * This will be posted to web portal, which we will just convert to an Order and return back as order list.*/

        public Guid? OrderToken { get; set; }

        [PrimaryKey]
        public int OrderProcessID { get; set; }
        public string DeliveryNO { get; set; }
        public int? ConsignmentTypeId { get; set; }
        public int? OrderID { get; set; }
        public int WarehouseId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int TenentId { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public int? AccountID { get; set; }
        public string OrderNotes { get; set; }
        public decimal OrderProcessTotal { get; set; }
        public int InventoryTransactionTypeId { get; set; }
        public int? TransferToWarehouseId { get; set; }
        public string TransferToWarehouseName { get; set; }

        public int? OrderProcessStatusId { get; set; }

        public string PickContainerCode { get; set; }

        [IgnoreAttribute]
        public List<OrderProcessDetailSync> OrderProcessDetails { get; set; }

        [Ignore]
        public AccountSync Account { get; set; }

        [IgnoreAttribute]
        public OrdersSync Order { get; set; }

    }
}

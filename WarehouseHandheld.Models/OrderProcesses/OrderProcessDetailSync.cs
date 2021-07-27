using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SQLite;
using WarehouseHandheld.Models.Products;

namespace WarehouseHandheld.Models.OrderProcesses
{
    public class OrderProcessDetailSync
    {
        [PrimaryKey]
        public int OrderProcessDetailID { get; set; }
        public int OrderProcessId { get; set; }
        public int ProductId { get; set; }
        public decimal QtyProcessed { get; set; }
        public int? OrderDetailID { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public int TenentId { get; set; }
        public bool? IsDeleted { get; set; }
        public int OrderProcessLocalId { get; set; }

        public bool IsSerialised { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string LocationCode { get; set; }


        [IgnoreAttribute]
        public string[] Serials { get; set; }

        public int AccountID { get; set; }
        public string Notes { get; set; }
        public int InventoryTransactionTypeId { get; set; }

        [IgnoreAttribute]
        public List<Pallets.PalleTrackingProcess> PalleTrackingProcess { get; set; }
        [IgnoreAttribute]
        public List<ProductKitMapViewModel> ProductKitMapViewModel { get; set; }

    }
}

using System;
using SQLite;

namespace WarehouseHandheld.Models.Pallets
{
    public class PalletTrackingSync
    {
        [PrimaryKey]
        public int PalletTrackingId { get; set; }
        public int ProductId { get; set; }
        public string Comments { get; set; }
        public string PalletSerial { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal RemainingCases { get; set; }
        public string BatchNo { get; set; }
        public PalletTrackingStatusEnum Status { get; set; }
        public int TenantId { get; set; }
        public int WarehouseId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public decimal TotalCases { get; set; }
    }

    public enum PalletStatusEnum
    {
        Active = 1,
        Completed = 2
    }

    public enum PalletTrackingStatusEnum
    {
        Created = 1,
        Active = 2,
        Completed = 3,
        Hold = 4,
        Archived = 5
    }

}

using System;
using SQLite;
using WarehouseHandheld.Models.OrderProcesses;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Models.Vehicles;

namespace WarehouseHandheld.Models.Pallets
{
    public class PalletDispatchSync
    {
        [PrimaryKey]
        public int PalletsDispatchID { get; set; }
        public string DispatchReference { get; set; }
        public string VehicleIdentifier { get; set; }
        public string VehicleDescription { get; set; }
        public DateTime? DateCompleted { get; set; }
        public int? CompletedBy { get; set; }
        public string TrackingReference { get; set; }
        public string CustomVehicleNumber { get; set; }
        public string CustomVehicleModel { get; set; }
        public string CustomDriverDetails { get; set; }
        public string ProofOfDeliveryImageFilenames { get; set; }
        public string DispatchNotes { get; set; }
        public int? MarketVehicleID { get; set; }
        public int? SentMethodID { get; set; }
        public int? MarketVehicleDriverID { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int? UpdatedBy { get; set; }
        public int? OrderProcessID { get; set; }

        public Guid TransactionLogId { get; set; }
        public int DispatchStatus { get; set; }

        [IgnoreAttribute]
        public OrderProcessSync OrderProcess { get; set; }
        [IgnoreAttribute]
        public OrdersSync Order { get; set; }
        [IgnoreAttribute]
        public MarketVehiclesSync MarketVehicle { get; set; }
    }
}

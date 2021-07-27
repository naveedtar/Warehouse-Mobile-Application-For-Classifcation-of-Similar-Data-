using System;
using SQLite;

namespace WarehouseHandheld.Models.StockTakes
{
    public class StockTakeProductCodeScanRequest : StockTakeProductCodeScanModel
    {
        public DateTime RequestTime { get; set; }
        public bool ForceSerialCreate { get; set; }
        public string TerminalSerial { get; set; }
        public Guid TransactionLogId { get; set; }
    }

    public class StockTakeProductCodeScanModel
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }
        public int CurrentTenantId { get; set; }
        public int WarehouseId { get; set; }
        public int AuthUserId { get; set; }
        public int StockTakeId { get; set; }
        public string LocationCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductSerial { get; set; }
        public int ScannedQuantity { get; set; }
        public string PalletSerial { get; set; }
        public bool NotExistingItem { get; set; }
        public string NewProductBarcode { get; set; }
        public string NewProductBarcode2 { get; set; }
        public string NewProductName { get; set; }
        public bool IsSerialised { get; set; }
        public bool IsProcessByPallet { get; set; }
        public int ProductId { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool BatchRequired { get; set; }


    }
}

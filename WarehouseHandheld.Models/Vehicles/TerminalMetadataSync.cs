using System;
using SQLite;

namespace WarehouseHandheld.Models.Vehicles
{
    public class TerminalMetadataSync
    {
        public string Serial { get; set; }
        public string TerminalName { get; set; }
        [PrimaryKey]
        public int TerminalId { get; set; }
        public int ParentWarehouseId { get; set; }
        public string ParentWarehouseName { get; set; }
        public int? MobileWarehouseId { get; set; }
        public string MobileWarehouseName { get; set; }
        public int? MarketVehicleId { get; set; }
        public string VehicleRegistration { get; set; }
        public int? SalesManUserId { get; set; }
        public string SalesManResourceName { get; set; }
        public int TenantId { get; set; }
        public PalletTrackingSchemeEnum PalletTrackingScheme { get; set; }
        public bool GlobalProcessByPallet { get; set; }
        public string TenantName { get; set; }
        public string TenantReceiptPrintHeaderLine1 { get; set; }
        public string TenantReceiptPrintHeaderLine2 { get; set; }
        public string TenantReceiptPrintHeaderLine3 { get; set; }
        public string TenantReceiptPrintHeaderLine4 { get; set; }
        public byte[] TenantLogo { get; set; }
        public string LogoPath { get; set; }
        public bool PrintLogoForReceipts { get; set; }
        public short SessionTimeoutHours { get; set; }
        public bool AllowStocktakeAddNew { get; set; }
        public bool AllowStocktakeEdit { get; set; }
        public bool VehicleChecksAtStart { get; set; }
        public bool PostGeoLocation { get; set; }
        public bool ShowFullBalanceOnPayment { get; set; }
        public bool AllowSaleWithoutAccount { get; set; }
        public bool AllowExportDatabase { get; set; }
        public bool ShowCasePrices { get; set; }
        public bool ScanVehicleLicensePlate { get; set; }
        public bool PickByContainer { get; set; }
        public bool MandatoryPickByContainer { get; set; }
        public bool MandatoryLocationScan { get; set; }
    }
    public enum PalletTrackingSchemeEnum
    {
        FirstInFirstOut = 1,
        FirstInLastOut = 2,
        ByExpiryDate = 3,
        ByExpiryMonth = 4,
        DontEnforce = 5

    }
}

using System;
namespace WarehouseHandheld.Services
{
    public static class WebServiceConfig
    {
        //Need to change this to allow easy setup for different clients
        public const string BaseUrl = "http://192.168.20.222:8082";


        public const string VerifyAck = "api/sync/verify-acks";

        public const string SyncUsers = "api/sync/users";
        public const string SyncAccounts = "api/sync/accounts";
        public const string SyncProducts = "api/sync/products";
        public const string SyncProductSerials = "api/sync/product-serials";
        public const string SyncInventoryStocks = "api/sync/inventory-stocks";
        public const string SyncStockTakes = "api/sync/stocktakes";
        public const string SyncOrders = "api/sync/orders";
        public const string SyncOrderProcesses = "api/sync/order-processes";
        public const string SyncTerminalMetaData = "api/sync/terminal-data";
        public const string SyncPallets = "api/sync/pallets";
        public const string SyncPalletDispatchMethods = "api/sync/pallet-dispatchmethods";
        public const string SyncPalletProducts = "api/sync/pallet-products";

        public const string SyncPalletStatus = "api/sync/pallet-status";
        public const string SyncPalletImage = "api/sync/pallet-images";
        public const string SyncOrderStatus = "api/sync/order-status";
        public const string PostOrderProcesses = "api/sync/post-order-processes";
        public const string PostPallet = "Pallets/_GetNewPallet";
        public const string PalletProduct = "api/sync/pallet-products-processes/";
        public const string PalletDispatch = "api/sync/pallet-dispatch/";
        public const string SyncVehicles = "api/sync/vehicles/";
        public const string RecordStockScan = "api/stocktake/record-stockscan";
        public const string UpdateStockQuantity = "api/stocktake/stockdetail-updatequantity";

        public const string PostGoodsReturn = "api/sync/goods-return";
        public const string PostWastages = "api/sync/wastage-return";
        public const string ProductStock = "api/sync/product-stock";
        public const string PortCheck = "api/sync/connection-check";
        public const string PostGeoLocation = "api/sync/post-geo-location";
        public const string PostPalletTracking = "api/sync/post-pallet-tracking";
        public const string GetPalletTracking = "api/sync/get-pallet-tracking";

        public const string PostOrderReceiveCount = "api/sync/post-order-receive-count";

        public const string SyncPalletDispatches = "api/sync/all-pallet-dispatches";

        public const string PostPalletDispatchProgress = "api/sync/post-dispatch-progress";


        public const string SyncStockLocationMovement = "api/sync/Locations";
        public const string PostStockMovement = "api/sync/post-stockmovement-detail";

        public const string SyncProductLocationStocksSync = "api/sync/locations-stocks";


    }
}

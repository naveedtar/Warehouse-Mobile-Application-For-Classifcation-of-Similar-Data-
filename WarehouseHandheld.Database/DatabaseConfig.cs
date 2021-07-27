using System;
using System.Collections.Generic;
namespace WarehouseHandheld.Database
{
    public static class DatabaseConfig
    {
        public static string DatabaseName = "WarehouseHandheld.db3";

        public enum Tables 
        {
            Users,
            Accounts,
            Products,
            ProductSerials,
            InventoryStocks,
            StockTakes,
            Orders,
            OrderProcesses,
            Pallets,
            PalletMethods,
            PalletProducts,
            PalletTracking,
            Vehicles,
            PalletDispatch,
            StockMovementViewModel,
            StockLocationMovement,
            ProductLocationStock
        }
    }
}

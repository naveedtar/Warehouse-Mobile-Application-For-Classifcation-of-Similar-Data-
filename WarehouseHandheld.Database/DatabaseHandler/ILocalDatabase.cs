using System;
using System.Collections.Generic;
using WarehouseHandheld.Models.Users;
using WarehouseHandheld.Models.Sync;
using WarehouseHandheld.Database.Users;
using WarehouseHandheld.Database.Sync;
using WarehouseHandheld.Database.Accounts;
using WarehouseHandheld.Database.Products;
using WarehouseHandheld.Database.InventoryStocks;
using WarehouseHandheld.Database.StockTakes;
using WarehouseHandheld.Database.Orders;
using WarehouseHandheld.Database.Pallets;
using WarehouseHandheld.Database.OrderProcesses;
using WarehouseHandheld.Database.DeviceSettings;
using WarehouseHandheld.Database.ProductLocationStock;
using WarehouseHandheld.Database.StockMovement;

namespace WarehouseHandheld.Database.DatabaseHandler
{
    public interface ILocalDatabase
    {
        IUsersTable Users { get; }
        IRequestLogTable RequestLog { get; }
        ISyncLogTable SyncLog { get; }
        IAccountsTable Accounts { get; }
        IProductsTable Products { get; }
        IProductSerialsTable ProductSerials { get; }
        IInventoryStocksTable InventoryStocks { get; }
        IStockTakesTable StockTakes { get; }
        IOrdersTable Orders { get; }
        IOrderDetailsTable OrderDetails { get; }
        IOrderProcessesTable OrderProcesses { get; }
        IOrderProcessesDetailTable OrderProcessDetails { get; }
        IPalletsTable Pallets { get; }
        IPalletDispatchTable PalletDispatch { get; }
        IVehiclesTable Vehicle { get; }
        IDeviceSettingsTable DeviceSettings { get; }
        IStockMovementTable StockMovements { get; }
        IProductLocationStockTable ProductLocationStock { get; }

        void OpenConnection();
        void CloseConnection();

    }
}

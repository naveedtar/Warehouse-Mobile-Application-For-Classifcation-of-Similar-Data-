using System;
using SQLite;
using WarehouseHandheld.Models.Users;
using WarehouseHandheld.Models.Sync;
using Xamarin.Forms;
using WarehouseHandheld.Database.Users;
using WarehouseHandheld.Database.Sync;
using WarehouseHandheld.Database.Accounts;
using WarehouseHandheld.Models.Accounts;
using WarehouseHandheld.Database.Products;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Database.InventoryStocks;
using WarehouseHandheld.Database.StockTakes;
using WarehouseHandheld.Models.InventoryStocks;
using WarehouseHandheld.Models.StockTakes;
using WarehouseHandheld.Database.Orders;
using WarehouseHandheld.Database.Pallets;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Database.OrderProcesses;
using WarehouseHandheld.Models.OrderProcesses;
using WarehouseHandheld.Models.Vehicles;
using WarehouseHandheld.Database.DeviceSettings;
using WarehouseHandheld.Database.ProductLocationStock;
using WarehouseVanSales.Database.DeviceSettings;
using WarehouseHandheld.Models.DeviceSettings;
using WarehouseHandheld.Database.StockMovement;
using WarehouseHandheld.Models.ProductStockLocation;
using WarehouseHandheld.Models.StockMovement;

namespace WarehouseHandheld.Database.DatabaseHandler
{
    public class LocalDatabase : ILocalDatabase
    {
        private readonly string _databasePath;
        private SQLiteAsyncConnection _database;
        public SQLiteAsyncConnection Database
        {
            get
            {
                if (_database == null)
                    OpenConnection();

                return _database;
            }
        }


        public virtual IUsersTable Users { get; private set; }
        public virtual ISyncLogTable SyncLog { get; private set; }
        public virtual IRequestLogTable RequestLog { get; private set; }
        public virtual IAccountsTable Accounts { get; private set; }
        public virtual IProductsTable Products { get; private set; }
        public virtual IProductSerialsTable ProductSerials { get; private set; }
        public virtual IInventoryStocksTable InventoryStocks { get; private set; }
        public virtual IStockTakesTable StockTakes { get; private set; }
        public virtual IOrdersTable Orders { get; private set; }
        public virtual IOrderDetailsTable OrderDetails { get; private set; }
        public virtual IOrderProcessesTable OrderProcesses { get; private set; }
        public virtual IOrderProcessesDetailTable OrderProcessDetails { get; private set; }
        public virtual IPalletsTable Pallets { get; private set; }
        public virtual IPalletDispatchTable PalletDispatch { get; private set; }
        public virtual IVehiclesTable Vehicle { get; private set; }
        public virtual IDeviceSettingsTable DeviceSettings { get; private set; }
        public virtual IStockMovementTable StockMovements { get; private set; }
        public virtual IProductLocationStockTable ProductLocationStock { get; private set; }

        public LocalDatabase()
        {
            _databasePath = DependencyService.Get<IDatabaseConnection>().GetDatabasePath(DatabaseConfig.DatabaseName);
            try
            {
                Initialize();
            }
            catch (Exception ex)
            {
                //_exceptionService.Handle(ex);
            }
            finally
            {
                CloseConnection();
            }
        }

        private void Initialize()
        {
            OpenConnection();
            //Initializing handlers
            SyncLog = new SyncLogTable(this);
            RequestLog = new RequestLogTable(this);
            Users = new UsersTable(this);
            Accounts = new AccountsTable(this);
            Products = new ProductsTable(this);
            ProductSerials = new ProductSerialsTable(this);
            InventoryStocks = new InventoryStocksTable(this);
            StockTakes = new StockTakesTable(this);
            Orders = new OrdersTable(this);
            OrderDetails = new OrderDetailsTable(this);
            OrderProcesses = new OrderProcessesTable(this);
            OrderProcessDetails = new OrderProcessesDetailTable(this);
            Pallets = new PalletsTable(this);
            PalletDispatch = new PalletDispatchTable(this);
            Vehicle = new VehiclesTable(this);
            DeviceSettings = new DeviceSettingsTable(this);
            StockMovements = new StockMovementTable(this);
            ProductLocationStock = new ProductLocationStockTable(this);
        }

        public void OpenConnection()
        {
            _database = new SQLiteAsyncConnection(_databasePath);
            CreateTables();
        }

        private void CreateTables()
        {
            //Creating tables from Models
            _database.CreateTableAsync<SyncLog>();
            _database.CreateTableAsync<RequestLog>();
            _database.CreateTableAsync<UserSync>();
            _database.CreateTableAsync<AccountSync>();
            _database.CreateTableAsync<ProductMasterSync>();
            _database.CreateTableAsync<ProductSerialSync>();
            _database.CreateTableAsync<InventoryStockSync>();
            _database.CreateTableAsync<StockTakeSync>();
            _database.CreateTableAsync<OrdersSync>();
            _database.CreateTableAsync<OrderDetailSync>();
            _database.CreateTableAsync<OrderProcessSync>();
            _database.CreateTableAsync<OrderProcessDetailSync>();
            _database.CreateTableAsync<PalletSync>();
            _database.CreateTableAsync<PalletDispatchSync>();
            _database.CreateTableAsync<PalletProductsSync>();
            _database.CreateTableAsync<MarketVehiclesSync>();
            _database.CreateTableAsync<PalletDispatchMethodSync>();
            _database.CreateTableAsync<StockTakeProductCodeScanRequest>();
            _database.CreateTableAsync<StockDetailQuantityUpdateRequest>();
            _database.CreateTableAsync<TerminalMetadataSync>();
            _database.CreateTableAsync<DeviceModel>();
            _database.CreateTableAsync<PalletTrackingSync>();
            _database.CreateTableAsync<ProductKitMapViewModel>();
            _database.CreateTableAsync<StockMovementViewModel>();
            _database.CreateTableAsync<StockMovementPalletSerialsViewModel>();
            _database.CreateTableAsync<LocationSync>();
            _database.CreateTableAsync<ProductLocationStocksSync>();

        }

        public void CloseConnection()
        {
            if (_database != null)
            {
                //_database.GetConnection().Close();
                _database = null;
            }
        }
    }
}

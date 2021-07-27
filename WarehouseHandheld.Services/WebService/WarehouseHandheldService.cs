using System;
using WarehouseHandheld.Services.WebService.RestService;
using WarehouseHandheld.Services.Users;
using WarehouseHandheld.Services.Acknowledgements;
using WarehouseHandheld.Services.Accounts;
using WarehouseHandheld.Services.Products;
using WarehouseHandheld.Services.InventoryStocks;
using WarehouseHandheld.Services.StockTakes;
using WarehouseHandheld.Services.Orders;
using WarehouseHandheld.Services.Pallets;
using WarehouseHandheld.Services.OrderProcesses;
using WarehouseHandheld.Services.Vehicles;
using WarehouseHandheld.Services.Returns;
using WarehouseHandheld.Services.Wastages;
using WarehouseHandheld.Services.StockEnquiry;
using WarehouseHandheld.Services.ServerPing;
using WarehouseHandheld.Services.GeoLocation;
using WarehouseHandheld.Services.ProductStockLocation;
using WarehouseHandheld.Services.StockMovement;

namespace WarehouseHandheld.Services.WebService
{
    public class WarehouseHandheldService : IWarehouseHandheldService
    {
        public WarehouseHandheldService()
        {
            HttpClient = new HttpClientExtended();
            Initialize();
        }

        public HttpClientExtended HttpClient { get; private set; }

        public Uri BaseUri { get; set; }

        public virtual IAcknowledgementService Acknowledgement { get; private set; }
        public virtual IUsersService Users { get; private set; }
        public virtual IAccountsService Accounts { get; private set; }
        public virtual IProductsService Products { get; private set; }
        public virtual IInventoryStockService InventoryStock { get; private set; }
        public virtual IStockTakesService StockTakes { get; private set; }
        public virtual IOrdersService Orders { get; private set; }
        public virtual IOrderProcessesService OrderProcesses { get; private set; }
        public virtual IPalletsService Pallets { get; private set; }
        public virtual IVehiclesService Vehicles { get; private set; }
        public virtual IReturnsService Returns { get; private set; }
        public virtual IWastagesService Wastages { get; private set; }
        public virtual IStockEnquiryService StockEnquiry { get; private set; }
        public virtual IServerPingService ServerPing { get; private set; }
        public virtual IPostGeoLocationService PostGeoLocation { get; private set; }
        public virtual IStockMovementService StockMovement { get; set; }
        public virtual IProductStockLocationService ProductStockLocation { get; set; }


        private void Initialize()
        {
            this.BaseUri = new Uri(WebServiceConfig.BaseUrl);
            this.Acknowledgement = new AcknowledgementService(this);
            this.Users = new UsersService(this);
            this.Accounts = new AccountsService(this);
            this.Products = new ProductsService(this);
            this.InventoryStock = new InventoryStockService(this);
            this.StockTakes = new StockTakesService(this);
            this.Orders = new OrdersService(this);
            this.OrderProcesses = new OrderProcessesService(this);
            this.Pallets = new PalletsService(this);
            this.Vehicles = new VehiclesService(this);
            this.Returns = new ReturnsService(this);
            this.Wastages = new WastagesService(this);
            this.StockEnquiry = new StockEnquiryService(this);
            this.ServerPing = new ServerPingService(this);
            this.PostGeoLocation = new PostGeoLocationService(this);
            this.StockMovement = new StockMovementService(this);
            this.ProductStockLocation = new ProductStockLocationService(this);

        }
    }
}

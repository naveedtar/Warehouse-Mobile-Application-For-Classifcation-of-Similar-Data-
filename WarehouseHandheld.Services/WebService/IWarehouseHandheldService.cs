using System;
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
    public interface IWarehouseHandheldService
    {
        Uri BaseUri { get; set; }

        IAcknowledgementService Acknowledgement { get; }
        IUsersService Users { get; }
        IAccountsService Accounts { get; }
        IProductsService Products { get; }
        IInventoryStockService InventoryStock { get; }
        IStockTakesService StockTakes { get; }
        IOrdersService Orders { get; }
        IOrderProcessesService OrderProcesses { get; }
        IPalletsService Pallets { get; }
        IVehiclesService Vehicles { get; }
        IReturnsService Returns {get;}
        IWastagesService Wastages { get; }
        IStockEnquiryService StockEnquiry { get; }
        IServerPingService ServerPing { get; }
        IPostGeoLocationService PostGeoLocation { get; }
        IStockMovementService StockMovement { get; }
        IProductStockLocationService ProductStockLocation { get; }
    }
}

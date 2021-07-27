using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Vehicles;

namespace WarehouseHandheld.Services.Vehicles
{
    public interface IVehiclesService
    {
        Task<TerminalMetadataSync> GetTerminalMetadataAsync(DateTime dateUpdated, string serialNo);
        Task<MarketVehiclesSyncCollection> GetVehiclesAsync(DateTime dateUpdated, string serialNo);
    }
}
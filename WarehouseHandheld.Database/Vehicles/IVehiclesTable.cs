using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Vehicles;

namespace WarehouseHandheld.Database.Accounts
{
    public interface IVehiclesTable
    {
        Task AddUpdateTerminalMetaData(TerminalMetadataSync TerminalMetadataSync);
        Task<List<MarketVehiclesSync>> GetAllVehicles();
        Task<MarketVehiclesSync> GetVehicleById(int id);
        Task<TerminalMetadataSync> GetTerminalMetaData();
        Task AddUpdateVehicles(IList<MarketVehiclesSync> VehiclesSync);
    }
}

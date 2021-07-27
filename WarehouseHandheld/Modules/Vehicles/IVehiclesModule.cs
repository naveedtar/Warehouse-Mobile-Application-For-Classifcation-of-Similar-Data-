using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Vehicles;

namespace WarehouseHandheld.Modules.Accounts
{
    public interface IVehiclesModule
    {
        Task SyncVehicles();
        Task<List<MarketVehiclesSync>> GetAllVehicles();
        Task<TerminalMetadataSync> SyncTerminalMetaData();

    }
}

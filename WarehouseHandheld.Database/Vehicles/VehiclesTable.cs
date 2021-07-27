using System;
using System.Collections.Generic;
using WarehouseHandheld.Database.DatabaseHandler;
using System.Linq;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Vehicles;

namespace WarehouseHandheld.Database.Accounts
{
    public class VehiclesTable : IVehiclesTable
    {
        public LocalDatabase Handler { get; private set; }
        public VehiclesTable(LocalDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("Database");
            this.Handler = database;
        }


        public async Task<MarketVehiclesSync> GetVehicleById(int id)
        {
            return await Handler.Database.Table<MarketVehiclesSync>().Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<TerminalMetadataSync> GetTerminalById(int id)
        {
            return await Handler.Database.Table<TerminalMetadataSync>().Where(x => x.TerminalId.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<List<MarketVehiclesSync>> GetAllVehicles()
        {
            return await Handler.Database.Table<MarketVehiclesSync>().ToListAsync();
        }

        public async Task AddUpdateVehicles(IList<MarketVehiclesSync> VehiclesSync)
        {
            foreach (var vehicle in VehiclesSync)
            {
                var userItem = await GetVehicleById(vehicle.Id);
                if (userItem == null)
                {
                    if (vehicle.IsDeleted==null || !(bool)vehicle.IsDeleted)
                        await Handler.Database.InsertAsync(vehicle);
                }
                else
                {
                    if (vehicle.IsDeleted == null || !(bool)vehicle.IsDeleted)
                        await Handler.Database.UpdateAsync(vehicle);
                    else
                        await Handler.Database.DeleteAsync(userItem);
                }
            }
        }

        public async Task AddUpdateTerminalMetaData(TerminalMetadataSync TerminalMetadataSync)
        {
            var terminalMetadataItem = await GetTerminalById(TerminalMetadataSync.TerminalId);
            if (terminalMetadataItem == null)
                await Handler.Database.InsertAsync(TerminalMetadataSync);
            else
                await Handler.Database.UpdateAsync(TerminalMetadataSync);

        }
        public async Task<TerminalMetadataSync> GetTerminalMetaData()
        {
            var metadataList = await Handler.Database.Table<TerminalMetadataSync>().ToListAsync();
            if (metadataList.Count() > 0)
            {
                return metadataList[0];
            }
            return null;
        }

    }
}

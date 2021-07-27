using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Database.DatabaseHandler;
using WarehouseHandheld.Models.Enums;
using WarehouseHandheld.Models.Pallets;

namespace WarehouseHandheld.Database.Pallets
{
    public class PalletDispatchTable : IPalletDispatchTable
    {
        public LocalDatabase Handler { get; private set; }
        public PalletDispatchTable(LocalDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("Database");
            this.Handler = database;
        }


        public async Task AddUpdatePalletDispatch(PalletDispatchSync palletDispatch)
        {
            var palletDispatchItem = await GetPalletDispatchById(palletDispatch.PalletsDispatchID);
            if (palletDispatchItem == null)
                await Handler.Database.InsertAsync(palletDispatch);
            else
                await Handler.Database.UpdateAsync(palletDispatch);

        }

        public async Task<PalletDispatchSync> GetPalletDispatchById(int palletDispatchId)
        {
            if (palletDispatchId != 0)
            {
                var palletDispatch = await Handler.Database.Table<PalletDispatchSync>().Where(x => x.PalletsDispatchID.Equals(palletDispatchId)).FirstOrDefaultAsync();
                return palletDispatch;
            }
            return null;
        }


        public async Task AddUpdatePalletDispatchProgress(IList<PalletDispatchProgress> palletDispatch)
        {
            foreach (var pallet in palletDispatch)
            {
                var palletItem = await GetPalletById(pallet.DispatchId);
                if (palletItem == null)
                    await Handler.Database.InsertAsync(pallet);
                else
                    await Handler.Database.UpdateAsync(pallet);
            }
        }

        private async Task<PalletSync> GetPalletById(int id)
        {
            return await Handler.Database.Table<PalletSync>().Where(x => x.PalletID.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<List<PalletDispatchProgress>> GetAllPalletDispatchProgress()
        {
            var palletsDispatchProgress = await Handler.Database.Table<PalletDispatchProgress>().ToListAsync();
            return palletsDispatchProgress;
        }


        public async Task<bool> AddUpdatePalletDispatchProgress(PalletDispatchProgress palletDispatchProgress)
        {
            var palletDispatchProgressInDb = await GetPalletDispatchById(palletDispatchProgress.DispatchId);
            if (palletDispatchProgressInDb == null)
            {
                if (palletDispatchProgress != null)
                {
                    await Handler.Database.InsertAsync(palletDispatchProgress);
                    return true;
                }
            }
            if (palletDispatchProgressInDb != null)
            {
                if (palletDispatchProgress != null)
                {
                    await Handler.Database.UpdateAsync(palletDispatchProgress);
                    return true;
                }
            }
            return false;
        }

        public async Task<PalletDispatchProgress> GetPalletDispatchProgressById(int id)
        {
            if (id != 0)
            {
                var palletDispatchProgressDb = await Handler.Database.Table<PalletDispatchProgress>().Where(x => x.DispatchId.Equals(id)).FirstOrDefaultAsync();
                return palletDispatchProgressDb;
            }
            return null;
        }

        public async Task AddUpdatePalletDispatch(IList<PalletDispatchSync> palletDispatches)
        {
            foreach (var palletDispatch in palletDispatches)
            {
                var palletDispatchInDb = await GetPalletDispatchById(palletDispatch.PalletsDispatchID);
                if (palletDispatchInDb == null)
                    await Handler.Database.InsertAsync(palletDispatch);
                else
                    await Handler.Database.UpdateAsync(palletDispatch);
            }

        }

        public async Task<List<PalletDispatchSync>> GetAllPalletDispatches()
        {
            var palletDispatches = await Handler.Database.Table<PalletDispatchSync>().Where(x => x.DispatchStatus.Equals((int)PalletDispatchStatusEnum.Scheduled)
            && (x.DateUpdated >= DbConstants.StartDate || x.DateCreated >= DbConstants.StartDate)).OrderByDescending(x => x.PalletsDispatchID).Take(DbConstants.TakeLimitValue).ToListAsync();
            if (palletDispatches != null)
            {
                foreach (var palletDispatch in palletDispatches)
                {

                    var orderProcess = await Handler.OrderProcesses.GetOrderProcessByOrderProcessId(palletDispatch.OrderProcessID);
                    if (orderProcess != null)
                    {
                        palletDispatch.OrderProcess = orderProcess;
                        if (orderProcess.OrderID != null && orderProcess.OrderID != 0)
                        {
                            var order = await Handler.Orders.GetOrderById((int)orderProcess.OrderID);
                            if (order != null)
                            {
                                palletDispatch.Order = order;
                            }
                        }


                    }
                    if (palletDispatch.MarketVehicleID != null && palletDispatch.MarketVehicleID != 0)
                    {
                        var marketVehicle = await Handler.Vehicle.GetVehicleById((int)palletDispatch.MarketVehicleID);
                        if (marketVehicle != null)
                        {
                            palletDispatch.MarketVehicle = marketVehicle;
                        }
                    }
                }

                return palletDispatches;
            }
            return null;
        }


    }
}

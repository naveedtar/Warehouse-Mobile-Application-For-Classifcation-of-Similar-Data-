using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseHandheld.Database.DatabaseHandler;
using WarehouseHandheld.Models.StockMovement;

namespace WarehouseHandheld.Database.StockMovement
{
    public class StockMovementTable : IStockMovementTable
    {
        public LocalDatabase Handler { get; private set; }
        public StockMovementTable(LocalDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("Database");
            this.Handler = database;
        }


        public async Task AddUpdateStockMovement(StockMovementViewModel stockMovementViewModel)
        {
            var stockMovementViewModelInDb = await GetStockMovementViewModelById(stockMovementViewModel.Id);
            if (stockMovementViewModelInDb == null)
            {
                await Handler.Database.InsertAsync(stockMovementViewModel);
                if (stockMovementViewModel.PalletSerials != null && stockMovementViewModel.PalletSerials.Any())
                {
                    await AddPalletSerial(stockMovementViewModel.PalletSerials);
                }
            }
            else
            {
                await Handler.Database.UpdateAsync(stockMovementViewModel);
                if (stockMovementViewModel.PalletSerials != null && stockMovementViewModel.PalletSerials.Any())
                {
                    await AddPalletSerial(stockMovementViewModel.PalletSerials);
                }
            }
        }

        public async Task AddUpdateStockMovementList(List<StockMovementViewModel> stockMovementViewModels)
        {
            if (stockMovementViewModels != null && stockMovementViewModels.Any())
            {
                foreach (var stockMovementViewModel in stockMovementViewModels)
                {
                    var stockMovementViewModelInDb = await GetStockMovementViewModelById(stockMovementViewModel.Id);
                    if (stockMovementViewModelInDb == null)
                    {
                        await Handler.Database.InsertAsync(stockMovementViewModel);
                        if (stockMovementViewModel.PalletSerials != null && stockMovementViewModel.PalletSerials.Any())
                        {
                            await AddPalletSerial(stockMovementViewModel.PalletSerials);
                        }
                    }
                    else
                    {
                        await Handler.Database.UpdateAsync(stockMovementViewModel);
                        if (stockMovementViewModel.PalletSerials != null && stockMovementViewModel.PalletSerials.Any())
                        {
                            await AddPalletSerial(stockMovementViewModel.PalletSerials);
                        }
                    }
                }
            }
        }

        public async Task AddUpdateStockLocations(List<LocationSync> stockLocations)
        {
            foreach (var stockLocation in stockLocations)
            {
                var stockLocationInDb = await GetStockLocationByLocationId(stockLocation.LocationId);
                if (stockLocationInDb == null)
                {
                    if (stockLocation.IsDeleted == null || !(bool)stockLocation.IsDeleted)
                        await Handler.Database.InsertAsync(stockLocation);
                }
                else if(stockLocation.IsDeleted != null && (bool)stockLocation.IsDeleted)
                {
                    await DeleteStockLocation(stockLocation);
                }
                else
                {
                    await Handler.Database.UpdateAsync(stockLocation);
                }
            }
        }

        private async Task DeleteStockLocation(LocationSync stockLocation)
        {
            await Handler.Database.DeleteAsync(stockLocation.LocationId);
        }

        // Local Model to store stock location scanned pallets and serials
        public async Task AddPalletSerial(List<StockMovementPalletSerialsViewModel> stockMovementSerialsPallets)
        {
            foreach (var stockMovementSerialPallet in stockMovementSerialsPallets)
            {
                var stockMovementSerialPalletInDb = await Handler.Database.Table<StockMovementPalletSerialsViewModel>().Where(x => x.Id.Equals(stockMovementSerialPallet.Id)).FirstOrDefaultAsync();
                if (stockMovementSerialPalletInDb != null)
                {
                    await Handler.Database.UpdateAsync(stockMovementSerialPallet);

                }
                else
                {
                    await Handler.Database.InsertAsync(stockMovementSerialPallet);
                }
            }
        }

        public async Task<LocationSync> GetStockLocationByLocationId(int locationId)
        {
            return await Handler.Database.Table<LocationSync>().Where(x => x.LocationId.Equals(locationId)).FirstOrDefaultAsync();
        }

        public async Task<List<StockMovementViewModel>> GetStockMovements()
        {
            var stockMovements = await Handler.Database.Table<StockMovementViewModel>().ToListAsync();
            if (stockMovements != null && stockMovements.Any())
            {
                foreach (var stockMovement in stockMovements)
                {
                    var stockMovementSerialsPallets = await Handler.Database.Table<StockMovementPalletSerialsViewModel>().Where(x => x.ProductId.Equals(stockMovement.ProductId)).ToListAsync();
                    stockMovement.PalletSerials = stockMovementSerialsPallets;
                }
                return stockMovements;
            }

            return null;
        }

        public async Task<StockMovementViewModel> GetStockMovementViewModelById(int id)
        {
            return await Handler.Database.Table<StockMovementViewModel>().Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<List<StockMovementPalletSerialsViewModel>> GetSerialPalletsByProductId(int productId)
        {
            return await Handler.Database.Table<StockMovementPalletSerialsViewModel>().Where(x => x.ProductId.Equals(productId)).ToListAsync();
        }

        public async Task<LocationSync> GetStockLocationByLocationCode(string locationCode)
        {
            if (string.IsNullOrEmpty(locationCode))
                return null;

            return await Handler.Database.Table<LocationSync>().Where(x => x.LocationCode.ToLower().Equals(locationCode.ToLower())).FirstOrDefaultAsync();
        }

        public async Task DeleteStockMovement()
        {
            await Handler.Database.DropTableAsync<StockMovementPalletSerialsViewModel>();
            await Handler.Database.CreateTableAsync<StockMovementPalletSerialsViewModel>();
            await Handler.Database.DropTableAsync<StockMovementViewModel>();
            await Handler.Database.CreateTableAsync<StockMovementViewModel>();
        }
    }
}

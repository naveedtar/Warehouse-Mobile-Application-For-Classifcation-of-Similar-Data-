using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Database.DatabaseHandler;
using WarehouseHandheld.Models.InventoryStocks;
namespace WarehouseHandheld.Database.InventoryStocks
{
    public class InventoryStocksTable : IInventoryStocksTable
    {
        public LocalDatabase Handler { get; private set; }
        public InventoryStocksTable(LocalDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("Database");
            this.Handler = database;
        }

        public async Task AddUpdateInventoryStocks(IList<InventoryStockSync> inventoryStockSync)
        {
            foreach (var inventory in inventoryStockSync)
            {
                var inventoryItem = await GetInventoryStockById(inventory.InventorystockId);
                if (inventoryItem == null)
                    await Handler.Database.InsertAsync(inventory);
                else
                    await Handler.Database.UpdateAsync(inventory);
            }
        }

        private async Task<InventoryStockSync> GetInventoryStockById(int id)
        {
            return await Handler.Database.Table<InventoryStockSync>().Where(x => x.InventorystockId.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<List<InventoryStockSync>> GetAllInventoryStocks()
        {
            return await Handler.Database.Table<InventoryStockSync>().ToListAsync();
        }

        public async Task<InventoryStockSync> GetInventoryStocksByWarehouseIdAndProductId(int id, int productId)
        {
            return await Handler.Database.Table<InventoryStockSync>().Where(x => x.WarehouseId == id && x.ProductId==productId).FirstOrDefaultAsync();
        }
    }
}

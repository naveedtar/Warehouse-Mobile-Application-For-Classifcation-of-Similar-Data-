using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.InventoryStocks;

namespace WarehouseHandheld.Database.InventoryStocks
{
    public interface IInventoryStocksTable
    {
        Task AddUpdateInventoryStocks(IList<InventoryStockSync> inventoryStockSync);
        Task<List<InventoryStockSync>> GetAllInventoryStocks();
        Task<InventoryStockSync> GetInventoryStocksByWarehouseIdAndProductId(int id, int productId);
    }
}

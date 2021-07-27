using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.InventoryStocks;

namespace WarehouseHandheld.Modules.InventoryStocks
{
    public interface IInventoryStocksModule
    {
        Task SyncInventoryStocks();
        Task<List<InventoryStockSync>> GetAllInventoryStock();
    }
}

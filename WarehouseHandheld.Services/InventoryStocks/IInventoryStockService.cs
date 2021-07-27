using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.InventoryStocks;
namespace WarehouseHandheld.Services.InventoryStocks
{
    public interface IInventoryStockService
    {
        Task<InventoryStockSyncCollection> GetInventoryStockAsync(DateTime dateUpdated, string serialNo);
    }
}

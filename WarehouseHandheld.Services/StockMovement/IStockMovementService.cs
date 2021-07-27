using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.StockMovement;

namespace WarehouseHandheld.Services.StockMovement
{
    public interface IStockMovementService
    {
        Task<LocationSyncCollection> GetStockLocationsAsync(DateTime dateTime, string serialNo);
        Task<bool> PostStockMovementAsync(StockMovemeneCollectionViewModel stockMovemeneCollection);
        bool HandleConflictStatusStockMovement();
    }
}

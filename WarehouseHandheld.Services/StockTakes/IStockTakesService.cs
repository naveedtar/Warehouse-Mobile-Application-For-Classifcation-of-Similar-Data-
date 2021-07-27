using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.StockTakes;
namespace WarehouseHandheld.Services.StockTakes
{
    public interface IStockTakesService
    {
        Task<StockTakeSyncCollection> GetStockTakesAsync(DateTime dateUpdated, string serialNo);
        Task<StockTakeProductCodeScanResponse> RecordStockScan(StockTakeProductCodeScanRequest request);
        Task<ResponseObject> UpdateStockQuantity(StockDetailQuantityUpdateRequest request);
        bool HandleStatusConflictScan();
        bool HandleStatusConflictUpdate();
    }
}

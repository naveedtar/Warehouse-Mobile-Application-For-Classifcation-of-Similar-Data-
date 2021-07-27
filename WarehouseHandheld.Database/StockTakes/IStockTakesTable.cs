using System;
using System.Collections.Generic;
using WarehouseHandheld.Models.StockTakes;
using System.Threading.Tasks;

namespace WarehouseHandheld.Database.StockTakes
{
    public interface IStockTakesTable
    {
        Task AddUpdateStockTakes(IList<StockTakeSync> stockTakesSync);
        Task<int> AddUpdateStockTakeProductScanrequest(StockTakeProductCodeScanRequest ProductScanRequest);
        Task DeleteStockTakeProductRequest(StockTakeProductCodeScanRequest ProductScanRequest);
        Task<int> AddUpdateStockQuantityUpdateRequest(StockDetailQuantityUpdateRequest ProductScanRequest);
        Task<StockTakeProductCodeScanRequest> GetStockTakeProductScanrequest(int Id);
        Task<List<StockTakeProductCodeScanRequest>> GetAllStockTakeProductScanrequest();
        Task<List<StockTakeSync>> GetStockTakes();
        Task<StockDetailQuantityUpdateRequest> GetStockQuantityUpdateRequest(int StockTakeRequestId);
        Task<List<StockDetailQuantityUpdateRequest>> GetAllStockQuantityUpdateRequests();
    }
}

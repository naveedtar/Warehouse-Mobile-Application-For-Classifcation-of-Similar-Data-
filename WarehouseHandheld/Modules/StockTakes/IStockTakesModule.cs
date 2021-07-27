using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Models.StockTakes;

namespace WarehouseHandheld.Modules.StockTakes
{
    public interface IStockTakesModule
    {
        Task SyncStockTakes();
        Task<List<StockTakeSync>> GetStockTakes();
        Task<StockTakeProductCodeScanResponse> PostScanStockTakeProduct(string LocationCode, ProductMasterSync product, int StockTakeId, int WareHouseId, string batchNumber, DateTime? expiryDate, string serial, string PalletSerial, decimal quantity, bool exisitingPallet = false);
        Task<StockTakeProductCodeScanResponse> PostAllScanStockProduct(StockTakeProductCodeScanRequest stockTakeRequest);
        void PostUpdateQuantityRequests();
        Task UpdateQuantity(decimal Quantity, int StockTakeRequestId, string batchnumber, DateTime? expiryDate);
    }
}

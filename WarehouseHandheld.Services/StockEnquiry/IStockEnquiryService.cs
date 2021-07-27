using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.InventoryStocks;

namespace WarehouseHandheld.Services.StockEnquiry
{
    public interface IStockEnquiryService
    {
        Task<InventoryStockSync> GetStockTakesAsync(int productId, string serialNo, int WarehouseId);
    }
}

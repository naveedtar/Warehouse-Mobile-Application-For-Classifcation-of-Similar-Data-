using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Products;

namespace WarehouseHandheld.Services.Products
{
    public interface IProductsService
    {
        Task<ProductMasterSyncCollection> GetProductsAsync(DateTime dateUpdated, string serialNo);
        Task<ProductSerialSyncCollection> GetProductSerialsAsync(DateTime dateUpdated, string serialNo);
    }
}

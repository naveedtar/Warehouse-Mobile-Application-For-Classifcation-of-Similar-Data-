using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Products;

namespace WarehouseHandheld.Modules.Products
{
    public interface IProductsModule
    {
        Task SyncProducts();
        Task SyncProductSerials();
        Task<ProductMasterSync> GetProductById(int id);
        Task<List<ProductMasterSync>> GetAllProducts();
        Task<List<ProductSerialSync>> GetProductSerialByProductId(int id);
        Task<List<ProductSerialSync>> GetAllProductSerials();
        Task<ProductSerialSync> GetProductSerialBySerialNo(string serialNo);

    }
}

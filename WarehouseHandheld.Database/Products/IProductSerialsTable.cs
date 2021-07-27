using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Products;
namespace WarehouseHandheld.Database.Products
{
    public interface IProductSerialsTable
    {
        Task AddUpdateProductSerials(IList<ProductSerialSync> productSerials);
        Task<List<ProductSerialSync>> GetAllProductSerials();
        Task<List<ProductSerialSync>> GetProductSerialByProductId(int id);
        Task<ProductSerialSync> GetProductSerialBySerialNo(string serialNo);
    }
}

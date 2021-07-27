using System;
using System.Collections.Generic;
using WarehouseHandheld.Models.Products;
using System.Threading.Tasks;
namespace WarehouseHandheld.Database.Products
{
    public interface IProductsTable
    {
        Task AddUpdateProducts(IList<ProductMasterSync> userSync);
        Task<ProductMasterSync> GetProductById(int id);
        Task<List<ProductMasterSync>> GetAllProducts();
        Task<List<ProductMasterSync>> GetProductByCode(string code);
    }
}

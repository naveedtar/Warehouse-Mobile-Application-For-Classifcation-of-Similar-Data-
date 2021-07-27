using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseHandheld.Database.DatabaseHandler;
using WarehouseHandheld.Models.Products;

namespace WarehouseHandheld.Database.Products
{
    public class ProductsTable : IProductsTable
    {
        public LocalDatabase Handler { get; private set; }
        public ProductsTable(LocalDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("Database");
            this.Handler = database;
        }

        public async Task DeleteProduct(ProductMasterSync product)
        {
            await Handler.Database.DeleteAsync(product);
        }

        public async Task AddUpdateProducts(IList<ProductMasterSync> productSync)
        {
            foreach (var product in productSync)
            {
                var productItem = await GetProductById(product.ProductId);
                if (productItem == null)
                {
                    if(product.IsDeleted==null || !(bool)product.IsDeleted)
                    {
                        await Handler.Database.InsertAsync(product);
                        if (product.ProductType == ProductKitTypeEnum.Kit)
                        {
                            if (product.ProductKitMapViewModelList != null && product.ProductKitMapViewModelList.Any())
                            {
                                await AddProductKitsViewModel(product.ProductKitMapViewModelList);
                            }
                        }
                    }
                }
                else
                {
                    if(product.IsDeleted==null || !(bool)product.IsDeleted)
                    {
                        await Handler.Database.UpdateAsync(product);
                        if (product.ProductType == ProductKitTypeEnum.Kit)
                        {
                            if (product.ProductKitMapViewModelList != null && product.ProductKitMapViewModelList.Any())
                            {
                                await AddProductKitsViewModel(product.ProductKitMapViewModelList);
                            }
                        }
                    }
                    else
                        await DeleteProduct(productItem);
                }
            }
        }

        public async Task<ProductMasterSync> GetProductById(int id)
        {
            return await Handler.Database.Table<ProductMasterSync>().Where(x => x.ProductId.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<ProductKitMapViewModel> GetProductKitViewModelById(int id)
        {
            return await Handler.Database.Table<ProductKitMapViewModel>().Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<List<ProductMasterSync>> GetAllProducts()
        {
            return await Handler.Database.Table<ProductMasterSync>().ToListAsync();
        }

        private async Task AddProductKitsViewModel(List<ProductKitMapViewModel> productKitMapViewModels)
        {
            if (productKitMapViewModels != null && productKitMapViewModels.Any())
            {
                foreach (var productKitVm in productKitMapViewModels)
                {
                    var productKitVmItemInDb = await GetProductKitViewModelById(productKitVm.Id);
                    if (productKitVmItemInDb == null)
                    {
                        if (productKitVm.IsDeleted == null || (bool)!productKitVm.IsDeleted)
                        {
                            await Handler.Database.InsertAsync(productKitVm);
                        }
                        else
                            await Handler.Database.DeleteAsync(productKitVm);
                    }
                    else
                    {
                        if (productKitVm.IsDeleted == null || (bool)!productKitVm.IsDeleted)
                        {
                            await Handler.Database.UpdateAsync(productKitVm);
                        }
                        else
                            await Handler.Database.DeleteAsync(productKitVm);
                    }
                }
            }
        }

        public async Task<List<ProductMasterSync>> GetProductByCode(string code)
        {
            return await Handler.Database.Table<ProductMasterSync>().Where(x => (x.SKUCode != null && x.SKUCode.ToLower().Contains(code.ToLower()))
                                              || (x.Name.ToLower().Contains(code.ToLower()))
                                              || (x.BarCode != null && x.BarCode.ToLower().Contains(code.ToLower()))
                                              || (x.BarCode2 != null && x.BarCode2.ToLower().Contains(code.ToLower()))
                                              || (x.SecondCode != null && x.SecondCode.ToLower().Contains(code.ToLower()))).ToListAsync();
        }
    }
}

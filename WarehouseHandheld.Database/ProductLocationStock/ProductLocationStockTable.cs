using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseHandheld.Database.DatabaseHandler;
using WarehouseHandheld.Models.ProductStockLocation;
using WarehouseHandheld.Models.StockMovement;

namespace WarehouseHandheld.Database.ProductLocationStock
{
    public class ProductLocationStockTable : IProductLocationStockTable
    {
        public LocalDatabase Handler { get; private set; }
        public ProductLocationStockTable(LocalDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("Database");
            this.Handler = database;
        }


        public async Task AddUpdateProductStockLocation(List<ProductLocationStocksSync> productLocationStocksSyncs)
        {
            try
            {
                if (productLocationStocksSyncs != null && productLocationStocksSyncs.Any())
                {
                    foreach (var productLocation in productLocationStocksSyncs)
                    {
                        var productLocationStocksInDb = await GetProductStockLocationByProductId(productLocation.Id);
                        if (productLocationStocksInDb == null)
                        {
                            if (productLocation.IsDeleted == null || !(bool)productLocation.IsDeleted)
                                await Handler.Database.InsertAsync(productLocation);
                        }
                        else
                        if (productLocation.IsDeleted != null && productLocation.IsDeleted == true)
                        {
                            await DeleteProductLocationStock(productLocation.Id);
                        }
                        else
                        {
                            await Handler.Database.UpdateAsync(productLocation);
                        }
                    }   
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           
        }

        public async Task<ProductLocationStocksSync> GetProductStockLocationByProductId(int productId)
        {
            return await Handler.Database.Table<ProductLocationStocksSync>().FirstOrDefaultAsync(x => x.ProductId.Equals(productId));
        }
        
        public async Task<ProductLocationStocksSync> GetProductStockLocationSortedByProductId(int productId, decimal quantityRequired)
        {
            var sortedProductLocations = new List<ProductLocationStocksSync>();
            var productLocations = await Handler.Database.Table<ProductLocationStocksSync>().Where(x => x.ProductId.Equals(productId)).OrderByDescending(x => x.Quantity).ToListAsync();
            if (productLocations != null && productLocations.Any())
            {
                foreach (var productLocation in productLocations)
                {
                    var locationsInDb = await Handler.Database.Table<LocationSync>().Where(x => x.LocationId.Equals(productLocation.LocationId)).OrderBy(x => x.SortOrder).FirstOrDefaultAsync();
                    if (locationsInDb != null)
                    {
                        productLocation.Location = locationsInDb;
                        sortedProductLocations.Add(productLocation);
                    }
                }
                // sortedProductLocations = sortedProductLocations.OrderBy(x=> x.Location.SortOrder).ThenByDescending(x => x.Quantity).ToList();
                sortedProductLocations = sortedProductLocations.OrderBy(x => x.Location.SortOrder).ToList();
                var productLocationInDb = sortedProductLocations.FirstOrDefault(x => x.Quantity >= quantityRequired);
                if (productLocationInDb == null)
                {
                    productLocationInDb = sortedProductLocations.FirstOrDefault();
                }

                return productLocationInDb;
            }
            return null;
        }

        public async Task<List<ProductLocationStocksSync>> GetProductStockLocationsSortedByProductId(int productId)
        {
            var productLocations = await Handler.Database.Table<ProductLocationStocksSync>().Where(x => x.ProductId.Equals(productId)).OrderByDescending(x => x.Quantity).ToListAsync();
            if (productLocations != null && productLocations.Any())
            {
                foreach (var productLocation in productLocations)
                {
                    var locationsInDb = await Handler.Database.Table<LocationSync>().Where(x => x.LocationId.Equals(productLocation.LocationId)).FirstOrDefaultAsync();
                    if (locationsInDb != null)
                    {
                        productLocation.Location = locationsInDb;
                    }
                }

                return productLocations;
            }
            return null;
        }

        
        public async Task<List<ProductLocationStocksSync>> GetProductStocksLocationByProductId(int productId)
        {
            return await Handler.Database.Table<ProductLocationStocksSync>().Where(x => x.ProductId.Equals(productId)).ToListAsync();
        }

        public async Task DeleteProductLocationStock(int id)
        {
            var productLocationStockInDb = await GetProductStockLocationByProductId(id);
            if (productLocationStockInDb != null)
            {
                await Handler.Database.DeleteAsync(productLocationStockInDb);
            }
        }

        public async Task<ProductLocationStocksSync> GetProductStockLocationByProductIdAndLocationId(int productId, int locationId)
        {
           return await Handler.Database.Table<ProductLocationStocksSync>().FirstOrDefaultAsync(x=> x.LocationId.Equals(locationId) && x.ProductId.Equals(productId));
        }
    }

    public interface IProductLocationStockTable
    {
        Task AddUpdateProductStockLocation(List<ProductLocationStocksSync> productLocationStocksSyncs);
        Task<ProductLocationStocksSync> GetProductStockLocationByProductId(int productId);
        Task<ProductLocationStocksSync> GetProductStockLocationByProductIdAndLocationId(int productId, int locationId);
        Task<ProductLocationStocksSync> GetProductStockLocationSortedByProductId(int productId, decimal quantityRequired);
        Task<List<ProductLocationStocksSync>> GetProductStocksLocationByProductId(int productId);
        Task<List<ProductLocationStocksSync>> GetProductStockLocationsSortedByProductId(int productId);
        Task DeleteProductLocationStock(int id);
    }
}
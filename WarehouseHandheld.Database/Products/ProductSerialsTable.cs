using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Database.DatabaseHandler;
using WarehouseHandheld.Models.Products;

namespace WarehouseHandheld.Database.Products
{

    public class ProductSerialsTable : IProductSerialsTable
    {
        public LocalDatabase Handler { get; private set; }
        public ProductSerialsTable(LocalDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("Database");
            this.Handler = database;
        }

        public async Task AddUpdateProductSerials(IList<ProductSerialSync> productSerials)
        {
            foreach (var serial in productSerials)
            {
                var serialItem = await GetProductSerialById(serial.SerialID);
                if (serialItem == null)
                    await Handler.Database.InsertAsync(serial);
                else
                    await Handler.Database.UpdateAsync(serial);
            }
        }

        public async Task<List<ProductSerialSync>> GetAllProductSerials()
        {
            return await Handler.Database.Table<ProductSerialSync>().ToListAsync();
        }

        private async Task<ProductSerialSync> GetProductSerialById(int id)
        {
            return await Handler.Database.Table<ProductSerialSync>().Where(x => x.SerialID.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<ProductSerialSync> GetProductSerialBySerialNo(string serialNo)
        {
            return await Handler.Database.Table<ProductSerialSync>().Where(x => x.SerialNo.Equals(serialNo)).FirstOrDefaultAsync();
        }

        public async Task<List<ProductSerialSync>> GetProductSerialByProductId(int id)
        {
            return await Handler.Database.Table<ProductSerialSync>().Where(x => x.ProductId.Equals(id)).ToListAsync();
        }

    }
}

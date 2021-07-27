using System;
using System.Collections.Generic;
using System.Linq;
using WarehouseHandheld.Database.DatabaseHandler;
using WarehouseHandheld.Models.StockTakes;
using System.Threading.Tasks;

namespace WarehouseHandheld.Database.StockTakes
{
    public class StockTakesTable : IStockTakesTable
    {
        public LocalDatabase Handler { get; private set; }
        public StockTakesTable(LocalDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("Database");
            this.Handler = database;
        }

        public async Task AddUpdateStockTakes(IList<StockTakeSync> stockTakesSync)
        {
            foreach (var stockTake in stockTakesSync)
            {
                var stockTakeItem = await GetStockTakeById(stockTake.StockTakeId);
                if (stockTakeItem == null)
                {
                    if (stockTake.IsDeleted==null || !(bool)stockTake.IsDeleted)
                        await Handler.Database.InsertAsync(stockTake);
                }
                else
                {
                    if (stockTake.IsDeleted == null || !(bool)stockTake.IsDeleted)
                        await Handler.Database.UpdateAsync(stockTake);
                    else
                        await Handler.Database.DeleteAsync(stockTakeItem);
                }
            }
        }

        public async Task DeleteStockTakeProductRequest(StockTakeProductCodeScanRequest ProductScanRequest)
        {
            await Handler.Database.DeleteAsync(ProductScanRequest);
        }

        public async Task<int> AddUpdateStockTakeProductScanrequest(StockTakeProductCodeScanRequest ProductScanRequest)
        {
            if(ProductScanRequest.Id==0)
            {
                return await Handler.Database.InsertAsync(ProductScanRequest);
            }
            else{
                return await Handler.Database.UpdateAsync(ProductScanRequest);
            }
        }

        public async Task<int> AddUpdateStockQuantityUpdateRequest(StockDetailQuantityUpdateRequest ProductQuantityUpdateRequest)
        {
            return await Handler.Database.InsertOrReplaceAsync(ProductQuantityUpdateRequest);

        }

        private async Task<StockTakeSync> GetStockTakeById(int id)
        {
            return await Handler.Database.Table<StockTakeSync>().Where(x => x.StockTakeId.Equals(id)).FirstOrDefaultAsync();
        }
        public async Task<List<StockTakeSync>> GetStockTakes()
        {
            return await Handler.Database.Table<StockTakeSync>().ToListAsync();
        }

        public async Task<StockTakeProductCodeScanRequest> GetStockTakeProductScanrequest(int Id)
        {
            return await Handler.Database.Table<StockTakeProductCodeScanRequest>().Where(x => x.Id == Id).FirstOrDefaultAsync();
        }

        public async Task<List<StockTakeProductCodeScanRequest>> GetAllStockTakeProductScanrequest()
        {
            return await Handler.Database.Table<StockTakeProductCodeScanRequest>().ToListAsync();
        }

        public async Task<List<StockDetailQuantityUpdateRequest>> GetAllStockQuantityUpdateRequests()
        {
            return await Handler.Database.Table<StockDetailQuantityUpdateRequest>().ToListAsync();
        }

        public async Task<StockDetailQuantityUpdateRequest> GetStockQuantityUpdateRequest(int StockTakeRequestId)
        {
            return await Handler.Database.Table<StockDetailQuantityUpdateRequest>().Where(x => x.StockTakeRequestId == StockTakeRequestId).FirstOrDefaultAsync();
        }
    }
}

using System;
using System.Threading.Tasks;
using WarehouseHandheld.Database.DatabaseHandler;
using WarehouseHandheld.Models.Sync;

namespace WarehouseHandheld.Database.Sync
{
    public class RequestLogTable : IRequestLogTable
    {
        public LocalDatabase Handler { get; private set; }
        public RequestLogTable(LocalDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("Database");
            this.Handler = database;
        }

        public async Task AddRequestLogItem(RequestLog log)
        {
            await Handler.Database.InsertAsync(log);
        }

        public async Task<bool> UpdateRequestLogItem(RequestLog log)
        {
            var logItem = await GetRequestLogById(log.Id);
            if (logItem != null)
            {
                await Handler.Database.UpdateAsync(log);
                return true;
            }
            return false;
        }

        private async Task<RequestLog> GetRequestLogById(string id)
        {
            return await Handler.Database.Table<RequestLog>().Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }
    
    }
}

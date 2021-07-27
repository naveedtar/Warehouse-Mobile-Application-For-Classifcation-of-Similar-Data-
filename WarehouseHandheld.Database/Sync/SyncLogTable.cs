using System;
using System.Reflection;
using WarehouseHandheld.Database.DatabaseHandler;
using WarehouseHandheld.Models.Sync;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WarehouseHandheld.Database.Sync
{
    public class SyncLogTable : ISyncLogTable
    {
        public LocalDatabase Handler { get; private set; }
        public SyncLogTable(LocalDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("Database");
            this.Handler = database;
            InitializeTableData();
        }

        private async void InitializeTableData()
        {
            var tables = Enum.GetValues(typeof(DatabaseConfig.Tables));
            foreach (var item in tables)
            {
                if (await GetSyncLogByTableName(item.ToString()) == null)
                {
                    try
                    {
                        await Handler.Database.InsertAsync(new SyncLog { TableName = item.ToString()});
                    }
                    catch(Exception e)
                    {
                        Debug.WriteLine(e.StackTrace);
                    }
                }
            }
        }

        public async Task AddSyncLogItem(SyncLog log)
        {
            await Handler.Database.InsertAsync(log);
        }

        public async Task<bool> UpdateSyncLogItem(SyncLog log)
        {
            var logItem = await GetSyncLogById(log.Id);
            if (logItem != null)
            {
                await Handler.Database.UpdateAsync(log);
                return true;
            }
            return false;
        }

        private async Task<SyncLog> GetSyncLogById(int id)
        {
            return await Handler.Database.Table<SyncLog>().Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<SyncLog> GetSyncLogByTableName(string name)
        {
            try
            {
                return await Handler.Database.Table<SyncLog>().Where(x => x.TableName.Equals(name)).FirstOrDefaultAsync();
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return null;
            }
        }

        public async Task<List<SyncLog>> GetAllSyncLogsByTableName(string name, bool isPost, bool synced)
        {
            return await Handler.Database.Table<SyncLog>().Where(x => x.TableName.Equals(name) && x.IsPost.Equals(isPost) && x.Synced.Equals(synced)).ToListAsync();
        }

        public async Task<List<SyncLog>> GetAll()
        {
            return await Handler.Database.Table<SyncLog>().ToListAsync();
        }

        public async Task<int> GetPrimaryKey()
        {
            if (await Handler.Database.Table<SyncLog>().CountAsync() > 0)
            {
                var list = await Handler.Database.Table<SyncLog>().ToListAsync();
                return list.Max(x=>x.Id);
            }
            else
                return 1;
        }
    }
}

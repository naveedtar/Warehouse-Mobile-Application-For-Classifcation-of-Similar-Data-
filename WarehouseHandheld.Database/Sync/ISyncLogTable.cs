using System;
using WarehouseHandheld.Models.Sync;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WarehouseHandheld.Database.Sync
{
    public interface ISyncLogTable
    {
        Task AddSyncLogItem(SyncLog log);
        Task<SyncLog> GetSyncLogByTableName(string name);
        Task<List<SyncLog>> GetAllSyncLogsByTableName(string name, bool isPost, bool synced);
        Task<bool> UpdateSyncLogItem(SyncLog log);
        Task<List<SyncLog>> GetAll();
    }
}

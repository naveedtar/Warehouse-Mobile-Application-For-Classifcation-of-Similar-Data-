using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Sync;

namespace WarehouseHandheld.Modules.Base
{
    public interface IBaseModule
    {
        Task<SyncLog> AddSyncLogBeforeRequest(string TableName, string Url, bool IsPost, string request);
        Task UpdateSyncLogAfterRequest(SyncLog synclog, string TerminalLogId, int ErrorCode, bool Synced, int ResultCount);

    }
}

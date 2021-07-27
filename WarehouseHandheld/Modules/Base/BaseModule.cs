using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Sync;
using WarehouseHandheld.Services;

namespace WarehouseHandheld.Modules.Base
{
    public class BaseModule : IBaseModule
    {
        public async Task<SyncLog> AddSyncLogBeforeRequest(string TableName, string Url, bool IsPost, string request)
        {
            SyncLog synclog = await InitializeSyncLog(Url,TableName,IsPost);
            var date = ModulesConfig.SyncDate;
            string serialNo = ModulesConfig.SerialNo;
            //Add Sync Log before sending request
            synclog.SerialNo = serialNo;
            synclog.request = request;
            synclog.IsPost = IsPost;
            await App.Database.SyncLog.UpdateSyncLogItem(synclog);
            return synclog;
        }

        public async Task UpdateSyncLogAfterRequest(SyncLog synclog,string TerminalLogId, int ErrorCode, bool Synced, int ResultCount)
        {
            synclog.TerminalLogId = TerminalLogId;
            synclog.LastSynced = DateTime.UtcNow;
            synclog.ErrorCode = ErrorCode;
            synclog.IsPending = false;
            synclog.Synced = Synced;
            synclog.ResponseTime = DateTime.UtcNow;

            if (ErrorCode == 200)
            {
                synclog.LastSynced = DateTime.UtcNow;
                synclog.ResultCount = ResultCount;
                synclog.ErrorCode = 0;
            }

            await App.Database.SyncLog.UpdateSyncLogItem(synclog);
        }

        private async Task<SyncLog> InitializeSyncLog(string Url, string TableName,bool IsPost)
        {
            
            SyncLog synclog = await App.Database.SyncLog.GetSyncLogByTableName(TableName);;
            if (IsPost || synclog==null)
            {
                
                synclog = new SyncLog();
                synclog.TableName = TableName;
                synclog.RequestUrl = Url;
                synclog.RequestedTime = DateTime.UtcNow;
                await App.Database.SyncLog.AddSyncLogItem(synclog);
            }

            return synclog;
        }

    }
}

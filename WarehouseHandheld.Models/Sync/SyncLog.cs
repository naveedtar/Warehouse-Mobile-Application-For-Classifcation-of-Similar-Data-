using System;
using SQLite;
namespace WarehouseHandheld.Models.Sync
{
    public class SyncLog
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string TableName { get; set; }
        public string SerialNo { get; set; }
        public DateTime ReqDate { get; set; }
        public string TerminalLogId { get; set; }
        public DateTime RequestedTime { get; set; }
        public string RequestUrl { get; set; }
        public int ErrorCode { get; set; }
        public DateTime LastSynced { get; set; }
        public bool Synced { get; set; }
        public int ResultCount { get; set; }
        public DateTime ResponseTime { get; set; }
        public bool IsPost { get; set; }
        public bool IsPending { get; set; }
        public string request { get; set; }
    }
}

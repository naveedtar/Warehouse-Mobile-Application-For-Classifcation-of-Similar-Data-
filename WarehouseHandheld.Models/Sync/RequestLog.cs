using System;
using SQLite;

namespace WarehouseHandheld.Models.Sync
{
    public class RequestLog
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string TableName { get; set; }
        public string SerialNo { get; set; }
        public DateTime ReqDate { get; set; }
        public string TerminalLogId { get; set; }
        public DateTime RequestedTime { get; set; }
        public string RequestUrl { get; set; }
        public int ErrorCode { get; set; }
        public bool Success { get; set; }
        public int ResultCount { get; set; }
        public DateTime ResponseTime { get; set; }
    }
}

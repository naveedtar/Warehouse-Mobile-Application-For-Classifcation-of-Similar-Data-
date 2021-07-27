using System;
using SQLite;
using System.Collections.Generic;

namespace WarehouseHandheld.Models.Accounts
{
    public class AccountSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }

        [IgnoreAttribute]
        public List<AccountSync> Accounts { get; set; }
    }

}

using System;
using System.Collections.Generic;
using SQLite;

namespace WarehouseHandheld.Models.Users
{
    public class UsersSyncCollection
    {
        public int count { get; set; }
        public string TerminalLogId { get; set; }

        [IgnoreAttribute]
        public IList<UserSync> Users { get; set; }
    }
}

using System;
using SQLite;

namespace WarehouseHandheld.Models.Pallets
{
    public class PalletDispatchMethodSync
    {
        [PrimaryKey]
        public int SentMethodID { get; set; }
        public string SentMethod { get; set; }
    }
}

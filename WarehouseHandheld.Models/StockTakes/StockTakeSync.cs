using System;
using SQLite;

namespace WarehouseHandheld.Models.StockTakes
{
    public class StockTakeSync
    {
        [PrimaryKey]
        public int StockTakeId { get; set; }
        public string StockTakeReference { get; set; }
        public string StockTakeDescription { get; set; }
        public int StockTakeStatusId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string WarehouseName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool? IsDeleted { get; set; }
        public int WarehouseId { get; set; }
    }
}

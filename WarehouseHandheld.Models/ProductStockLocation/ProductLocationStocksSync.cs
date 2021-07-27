using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SQLite;
using WarehouseHandheld.Models.StockMovement;

namespace WarehouseHandheld.Models.ProductStockLocation
{
    public class ProductLocationStocksSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public List<ProductLocationStocksSync> ProductLocationStocksSync { get; set; }
    }

    public class ProductLocationStocksSync
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int LocationId { get; set; }
        public decimal Quantity { get; set; }
        public int WarehouseId { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        
        [IgnoreAttribute]
        public LocationSync Location { get; set; }
    }
}
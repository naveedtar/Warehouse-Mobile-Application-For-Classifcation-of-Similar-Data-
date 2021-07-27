using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using SQLite;
using Xamarin.Forms;

namespace WarehouseHandheld.Models.StockMovement
{
    public class StockMovementViewModel
    {
        [PrimaryKey, AutoIncrement, JsonIgnore]
        public int Id { get; set; }
        public int? FromLocation { get; set; }
        public int? ToLocation { get; set; }
        public decimal Qty { get; set; }
        public int ProductId { get; set; }
        public string ProductSkuCode { get; set; }
        public string ProductName { get; set; }
        public int UserId { get; set; }
        public int WarehouseId { get; set; }
        public int TenentId { get; set; }
        public DateTime DateCreated { get; set; }
        
        [IgnoreAttribute]
        public List<int> SerialIds { get; set; }
        [IgnoreAttribute]
        public List<StockMovementPalletSerialsViewModel> PalletSerials { get; set; }


        [JsonIgnore]
        public bool IsFromLocationComplete { get; set; }

        [JsonIgnore]
        public string FromLocationCode { get; set; }
        [JsonIgnore]
        public string ToLocationCode { get; set; }
      
        [JsonIgnore]
        public decimal QtyProcessed { get; set; }
        
        [JsonIgnore]
        public decimal QuantityAvailableOnLocation { get; set; }
        
        [JsonIgnore, Ignore]
        public Color RowColor
        {
            get
            {
                    return this.Qty == this.QtyProcessed ? Color.LightGreen : Color.Transparent;
            }
        }

        
    }

    public class StockMovemeneCollectionViewModel
    {
        public int Count { get; set; }
        public string SerialNo { get; set; }
        public List<StockMovementViewModel> StockMovements { get; set; }
    }

   
    
    public class StockMovementPalletSerialsViewModel
    {
        [PrimaryKey, AutoIncrement, JsonIgnore]
        public int Id { get; set; }
        public int PalletSerialId { get; set; }
        public decimal Cases { get; set; }
  
        [JsonIgnore]
        public int ProductId { get; set; }
        [JsonIgnore]
        public bool IsFromComplete { get; set; }
        [JsonIgnore]
        public bool IsSerialised { get; set; }
    }
}

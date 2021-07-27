using System;
using Newtonsoft.Json;

namespace WarehouseHandheld.Models.Pallets
{
    public class PalletSerial
    {
        public int PalletTrackingId { get; set; }
        public string Serial { get; set; }
        public decimal Cases { get; set; }
        public decimal Quantity { get; set; }

        [JsonIgnore]
        public decimal RemainingCases { get; set; }

        [JsonIgnore]
        public bool IsSaleOrder { get; set; }

        [JsonIgnore]
        public decimal OrderQuantity { get; set; }

        [JsonIgnore]
        public decimal OrderQuantityProcessed { get; set; }

        [JsonIgnore]
        public decimal prdPerCase { get; set; }

        [JsonIgnore]
        public string CasesFormatted
        {
            get
            {
                return Cases.ToString("F");
            }
        }
    }
}

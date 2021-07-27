using System;
using Newtonsoft.Json;

namespace WarehouseHandheld.Models.Pallets
{
    public class PalleTrackingProcess
    {
        public int PalletTrackingId { get; set; }
        public decimal ProcessedQuantity { get; set; }
        public string palletSerial { get; set; }
        public bool isExistingPallet { get; set; }

        [JsonIgnore]
        public decimal RemainingCasesAfterProcessing{ get; set; }

        [JsonIgnore]
        public bool IsIncluded { get; set; }
    }
}

using System;
using SQLite;

namespace WarehouseHandheld.Models.Pallets
{
    public class PalletProductsSync
    {
        public double PalletQuantity { get; set; }
        public int CurrentPalletID { get; set; }
        public int OrderProcessDetailID { get; set; }
        public int AccountID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }

        [PrimaryKey]
        public int PalletProductID { get; set; }
        public bool PostedSuccess { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int? UpdatedBy { get; set; }
    }
}

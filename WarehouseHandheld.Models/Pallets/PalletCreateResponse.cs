using System;
using System.Collections.Generic;

namespace WarehouseHandheld.Models.Pallets
{
    public class AllCurrentPallet
    {
        public bool Disabled { get; set; }
        public object Group { get; set; }
        public bool Selected { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
    }

    public class PalletCreateResponse
    {
        public bool PalletsEnabled { get; set; }
        public string NextPalletNumber { get; set; }
        public bool IsNewPallet { get; set; }
        public bool IsCompleted { get; set; }
        public object PalletDateCompleted { get; set; }
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int ProcessedQuantity { get; set; }
        public object ProductName { get; set; }
        public int SelectedOrderProcessId { get; set; }
        public int SelectedPalletID { get; set; }
        public int RecipientAccountID { get; set; }
        public List<object> AllCustomers { get; set; }
        public List<AllCurrentPallet> AllCurrentPallets { get; set; }
        public List<object> AllCurrentOrders { get; set; }
    }
}

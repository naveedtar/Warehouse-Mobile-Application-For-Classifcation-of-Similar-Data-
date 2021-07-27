using System;
using WarehouseHandheld.Models.Accounts;
using SQLite;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using Xamarin.Forms;

namespace WarehouseHandheld.Models.Pallets
{
    public class PalletSync : INotifyPropertyChanged
    {
        public PalletSync()
        {
            PalletDispatchInfo = new PalletDispatchSync();
        }

        [PrimaryKey]
        public int PalletID { get; set; }
        public string SerialNumber { get; set; }
        public string ProofOfLoadingImage { get; set; }
        public int RecipientAccountID { get; set; }
        public DateTime? DateCompleted { get; set; }
        public int? CompletedBy { get; set; }
        public int CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public int? PalletsDispatchID { get; set; }
        public bool IsDispatched { get; set; }
        public string TerminalSerialNumber { get; set; }
        public int TenantId { get; set; }
        public int? OrderProcessID { get; set; }
        public Guid TransactionLogId { get; set; }
        [IgnoreAttribute]
        public List<int> SelectedPallets { get; set; }

        [IgnoreAttribute]
        public byte[][] ProofOfLoadingImageBytes { get; set; }

        [IgnoreAttribute]
        public AccountSync RecipientAccount { get; set; }

        [IgnoreAttribute]
        public PalletDispatchSync PalletDispatchInfo { get; set; }
        [Ignore]
        public Color SelectedColor { get; set; }

        private string palletNumber;
        public string PalletNumber
        {
            get { return palletNumber; }
            set
            {
                palletNumber = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(PalletNumber)));
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedColor)));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;



    }
}

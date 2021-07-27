using System;
using System.Collections.ObjectModel;
using WarehouseHandheld.Models.Accounts;
using WarehouseHandheld.Models.Pallets;
namespace WarehouseHandheld.ViewModels.Pallets
{
    public class AddPalletViewModel : PalletsViewModel
    {
        private PalletCreateResponse palletCreate;
        public PalletCreateResponse PalletCreate
        {
            get { return palletCreate; }
            set
            {
                palletCreate = value;
                OnPropertyChanged();
            }
        }

        public AccountSync Account { get; set; }

        public async void Initialize()
        {
            IsBusy = true;
            try
            {
                IsAddPalletPopup = true;
                var AllPallets = (await App.Pallets.GetPallets()).FindAll((obj) => !obj.IsDispatched && obj.RecipientAccountID==Account.AccountID);
                Pallets = new ObservableCollection<PalletSync>(AllPallets);
            }
           
            finally
            {
                IsBusy = false;
            }
        }
    }
}

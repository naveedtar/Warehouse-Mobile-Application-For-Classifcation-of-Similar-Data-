using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using WarehouseHandheld.Models.Accounts;
using Xamarin.Forms;

namespace WarehouseHandheld.ViewModels.Accounts
{
    public class AccountsViewModel : BaseViewModel
    {
        List<AccountSync> AllAccounts;
        private ObservableCollection<AccountSync> accounts;
        public ObservableCollection<AccountSync> Accounts
        {
            get { return accounts; }
            set
            {
                accounts = value;
                OnPropertyChanged();
            }
        }

        public AccountsViewModel()
        {
            Initialize();
        }

        public async void Initialize()
        {
            AllAccounts = await App.Accounts.GetAllAccounts();
            Accounts = new ObservableCollection<AccountSync>();
        }

        public void FindAccount(string text)
        {
            if(string.IsNullOrEmpty(text))
            {
                Accounts = new ObservableCollection<AccountSync>(); 
            }
            else{

                List<AccountSync> FoundAccounts = AllAccounts.FindAll((obj) => obj.CompanyName.ToLower().Contains(text.ToLower()));
                Accounts = new ObservableCollection<AccountSync>(FoundAccounts); 
            }
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using System.Diagnostics;
using Xamarin.Essentials;
using WarehouseHandheld.Resources;

namespace WarehouseHandheld.ViewModels.Sync
{
    public class SyncViewModel : BaseViewModel
    {
        private ObservableCollection<SyncModel> syncList;
        public ObservableCollection<SyncModel> SyncList
        {
            get { return syncList; }
            set
            {
                syncList = value;
                OnPropertyChanged();
            }
        }

        private bool autoSync;
        public bool AutoSync
        {
            get { return autoSync; }
            set
            {
                autoSync = value;
                OnPropertyChanged();
            }
        }

        public ICommand ItemTappedCommand { get; private set; }

        public SyncViewModel()
        {
            ItemTappedCommand = new Command(ItemTapped);
            SyncList = new ObservableCollection<SyncModel>();
            var tables = Enum.GetNames(typeof(Database.DatabaseConfig.Tables));
            foreach (var item in tables)
            {
                SyncList.Add(new SyncModel() { Name = item });
            }
            AutoSync = App.AutoSync;
        }

        public async void AutoSyncToggled()
        {
            await App.ToggleAutoSync(AutoSync);
        }

        private async void ItemTapped(object obj)
        {
            var item = (SyncModel)((ItemTappedEventArgs)obj).Item;
            if(item.Name == Database.DatabaseConfig.Tables.Users.ToString())
            {
                item.IsSyncing = true;
                await App.Users.SyncUsers();
                item.IsSyncing = false;
            }
            else if (item.Name == Database.DatabaseConfig.Tables.Accounts.ToString())
            {
                item.IsSyncing = true;
                await App.Accounts.SyncAccounts();
                item.IsSyncing = false;
            }
            else if(item.Name == Database.DatabaseConfig.Tables.Products.ToString())
            {
                item.IsSyncing = true;
                await App.Products.SyncProducts();
                item.IsSyncing = false;
            }
            else if (item.Name == Database.DatabaseConfig.Tables.ProductSerials.ToString())
            {
                item.IsSyncing = true;
                await App.Products.SyncProductSerials();
                item.IsSyncing = false;
            }
            else if (item.Name == Database.DatabaseConfig.Tables.InventoryStocks.ToString())
            {
                item.IsSyncing = true;
                await App.InventoryStocks.SyncInventoryStocks();
                item.IsSyncing = false;
            }
            else if (item.Name == Database.DatabaseConfig.Tables.StockTakes.ToString())
            {
                item.IsSyncing = true;
                await App.StockTakes.SyncStockTakes();
                item.IsSyncing = false;
            }
            else if (item.Name == Database.DatabaseConfig.Tables.Orders.ToString())
            {
                item.IsSyncing = true;
                await App.Orders.SyncOrders();
                item.IsSyncing = false;
            }
            else if (item.Name == Database.DatabaseConfig.Tables.OrderProcesses.ToString())
            {
                item.IsSyncing = true;
                await App.OrderProcesses.SyncOrderProcesses();
                item.IsSyncing = false;
            }
            else if (item.Name == Database.DatabaseConfig.Tables.Pallets.ToString())
            {
                item.IsSyncing = true;
                await App.Pallets.SyncPallets();
                item.IsSyncing = false;
            }
            else if (item.Name == Database.DatabaseConfig.Tables.PalletMethods.ToString())
            {
                item.IsSyncing = true;
                await App.Pallets.SyncPalletMethods();
                item.IsSyncing = false;
            }
            else if (item.Name == Database.DatabaseConfig.Tables.PalletProducts.ToString())
            {
                item.IsSyncing = true;
                await App.Pallets.SyncPalletProducts();
                item.IsSyncing = false;
            }
            else if (item.Name == Database.DatabaseConfig.Tables.Vehicles.ToString())
            {
                item.IsSyncing = true;
                await App.Vehicles.SyncVehicles();
                item.IsSyncing = false;
            }

            else if (item.Name == Database.DatabaseConfig.Tables.PalletTracking.ToString())
            {
                item.IsSyncing = true;
                await App.Pallets.SyncPalletTracking();
                item.IsSyncing = false;
            }

            else if (item.Name == Database.DatabaseConfig.Tables.PalletDispatch.ToString())
            {
                item.IsSyncing = true;
                await App.Pallets.SyncPalletDispatches();
                item.IsSyncing = false;
            }
            else if (item.Name == Database.DatabaseConfig.Tables.StockLocationMovement.ToString())
            {
                item.IsSyncing = true;
                await App.StockMovements.SyncLocations();
                item.IsSyncing = false;
            }
            else if (item.Name == Database.DatabaseConfig.Tables.ProductLocationStock.ToString())
            {
                item.IsSyncing = true;
                await App.StockMovements.SyncProductLocationStock();
                item.IsSyncing = false;
            }
            else if (item.Name == Database.DatabaseConfig.Tables.StockMovementViewModel.ToString())
            {
                item.IsSyncing = true;
                await App.StockMovements.SyncStockMovement();
                item.IsSyncing = false;
            }
            //Preferences.Set(Keys.IsSyncingModules, false);


        }
    }
}

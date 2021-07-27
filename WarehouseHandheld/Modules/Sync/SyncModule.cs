using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Connectivity;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Models.Sync;
using WarehouseHandheld.Resources;
using Xamarin.Essentials;

namespace WarehouseHandheld.Modules.Sync
{
    public class SyncModule : ISyncModule
    {
        public SyncModule()
        {
        }

        public async Task SyncAllModules()
        {
            bool isSyncing = Preferences.Get(Keys.IsSyncingModules, false);
            if (CrossConnectivity.Current.IsConnected && await Util.Util.IsConnected())
            {
                if (!isSyncing)
                {
                    Preferences.Set(Keys.IsSyncingModules, true);
                    "Sync Started".ToToast();
                    await App.Vehicles.SyncTerminalMetaData();
                    await App.Users.SyncUsers();
                    await App.Accounts.SyncAccounts();
                    await App.InventoryStocks.SyncInventoryStocks();
                    await App.OrderProcesses.SyncOrderProcesses();
                    await App.Orders.SyncOrders();
                    await App.Products.SyncProducts();
                    await App.Products.SyncProductSerials();
                    await App.StockTakes.SyncStockTakes();
                    await App.Pallets.SyncPallets();
                    App.Users.SyncUserLocation();
                    await App.Pallets.SyncPalletTracking();
                    await App.Pallets.SyncPalletDispatches();
                    await App.StockMovements.SyncLocations();
                    await App.StockMovements.SyncStockMovement();
                    await App.StockMovements.SyncProductLocationStock();
                    "Sync Completed".ToToast();
                    Preferences.Set(Keys.NotFirstTime, true);
                    Preferences.Set(Keys.IsSyncingModules, false);
                    Preferences.Set(Keys.LastSyncDateTime, DateTime.UtcNow);
                }
                else
                {
                    "Sync in progress".ToToast();
                }
            }
            else if (!CrossConnectivity.Current.IsConnected)
            {
                "No Internet Connection.".ToToast();
            }
        }
    }
}

using WarehouseHandheld.Services.WebService;
using WarehouseHandheld.Views.Login;
using Xamarin.Forms;
using System.Diagnostics;
using Xamarin.Forms.Xaml;
using WarehouseHandheld.Modules.Users;
using WarehouseHandheld.Database.DatabaseHandler;
using WarehouseHandheld.Modules.Accounts;
using WarehouseHandheld.Modules.Products;
using WarehouseHandheld.Views;
using WarehouseHandheld.Modules.InventoryStocks;
using WarehouseHandheld.Modules.Orders;
using WarehouseHandheld.Modules.StockTakes;
using WarehouseHandheld.Modules.OrderProcesses;
using WarehouseHandheld.Modules.Pallets;
using WarehouseHandheld.Views.OrderItems;
using WarehouseHandheld.Elements.ControlTemplates;
using WarehouseHandheld.Modules.Sync;
using WarehouseHandheld.Modules;
using System.Linq;
using System.Threading.Tasks;
using WarehouseHandheld.Views.DomainSetup;
using System;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using WarehouseHandheld.Modules.Base;
using Plugin.Connectivity;
using WarehouseHandheld.Resources;
using Xamarin.Essentials;
using WarehouseHandheld.Models.Vehicles;
using WarehouseHandheld.Views.Sync;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Modules.StockMovement;
using Device = Xamarin.Forms.Device;

namespace WarehouseHandheld
{
    public partial class App : Application
    {
        private const string AutoSyncKey = "AutoSync";
        private const string Domain = "Domain";
        private bool autoSync = true;
        public static bool AutoSync
        {
            get; set;
        } = true;

        public static int ScreenWidth;
        public static int ScreenHeight;

        public static IWarehouseHandheldService WarehouseService { get; private set; }
        public static ILocalDatabase Database { get; private set; }
        public static IUsersModule Users { get; private set; }
        public static IAccountsModule Accounts { get; private set; }
        public static IProductsModule Products { get; private set; }
        public static IInventoryStocksModule InventoryStocks { get; private set; }
        public static IStockTakesModule StockTakes { get; private set; }
        public static IOrdersModule Orders { get; private set; }
        public static IOrderProcessesModule OrderProcesses { get; private set; }
        public static IPalletsModule Pallets { get; private set; }
        public static ISyncModule Sync { get; private set; }
        public static IBaseModule Base { get; private set; }
        public static IVehiclesModule Vehicles { get; private set; }
        public static IStockMovementModule StockMovements { get; private set; }

        //For Background table sync
        private static Stopwatch stopWatch = new Stopwatch();
        public static int backgroundTaskTime = ModulesConfig.SyncTime;
        private bool firstRun = true;

        private bool isSyncing = false;
        private static Stopwatch longSyncStopWatch = new Stopwatch();
        private static int lastSyncTaskTimeInterval = ModulesConfig.SyncInterval;


        public App()
        {
            
            InitializeComponent();
            //Initializing modules and services
            Initialize();

            Xamarin.Forms.DataGrid.DataGridComponent.Init();

            TemplateLoader.Init();

            //setting up main page of app
            //if (Application.Current.Properties.ContainsKey(Domain))
            if (Preferences.ContainsKey(Domain))
            {
                MainPage = new NavigationPage(new LoginPage());
                //App.WarehouseService.BaseUri = new Uri((string)Current.Properties[Domain]);
                App.WarehouseService.BaseUri = new Uri((string)Preferences.Get(Domain,App.WarehouseService.BaseUri.ToString()));

            }
            else
                MainPage = new NavigationPage(new DomainSetupPage());
        }

        private static void Initialize()
        {
            WarehouseService = new WarehouseHandheldService();
            Users = new UsersModule();
            Accounts = new AccountsModule();
            Products = new ProductsModule();
            InventoryStocks = new InventoryStocksModule();
            StockTakes = new StockTakesModule();
            Orders = new OrdersModule();
            OrderProcesses = new OrderProcessesModule();
            Pallets = new PalletsModule();
            Sync = new SyncModule();
            Base = new BaseModule();
            Vehicles = new VehiclesModule();
            StockMovements = new StockMovementModule();

            Database = new LocalDatabase();
            Database.OpenConnection();
            SetTenantId();
            
            AppCenter.Start("android=accd0dcf-e882-4179-907a-0367e3085fbe;" +
                typeof(Analytics), typeof(Crashes));
        }

        async static void SetTenantId()
        {
            var terminalData = await App.Database.Vehicle.GetTerminalMetaData();
            if (terminalData != null)
            {
                ModulesConfig.TenantID = terminalData.TenantId;
                ModulesConfig.WareHouseID = terminalData.ParentWarehouseId;
                if (terminalData.PalletTrackingScheme == 0)
                {
                   terminalData.PalletTrackingScheme = PalletTrackingSchemeEnum.DontEnforce;
                }
                ModulesConfig.TrackingScheme = terminalData.PalletTrackingScheme;
            }
        }

        protected override void OnStart()
        {
            isSyncing = true;

            //ModulesConfig.SyncTime = App.Current.Properties.ContainsKey(Keys.SyncTime) ? Convert.ToInt32(App.Current.Properties[Keys.SyncTime]) : ModulesConfig.SyncTime;
            ModulesConfig.SyncTime = Preferences.ContainsKey(Keys.SyncTime) ? Convert.ToInt32(Preferences.Get(Keys.SyncTime,ModulesConfig.SyncTime)) : ModulesConfig.SyncTime;
            backgroundTaskTime = ModulesConfig.SyncTime;
            lastSyncTaskTimeInterval = ModulesConfig.SyncInterval;

            StartAutoSync();
            isSyncing = false;

        }



        private void StartAutoSync()
        {
            //if (Current.Properties.ContainsKey(AutoSyncKey))
             //AutoSync = (bool)Current.Properties[AutoSyncKey];

            if (Preferences.ContainsKey(Keys.AutoSyncKey))
                AutoSync = (bool)Preferences.Get(Keys.AutoSyncKey,true);

            if (!longSyncStopWatch.IsRunning)
                longSyncStopWatch.Start();

            if (!stopWatch.IsRunning && AutoSync)
                stopWatch.Start();

            Device.StartTimer(new System.TimeSpan(0, 0, 1), () =>
            {
                if (stopWatch.IsRunning)
                {
                    if (stopWatch.Elapsed.Minutes >= backgroundTaskTime)
                    {
                        //restart stopwatch
                        stopWatch.Restart();
                        //Need to run our Tables Sync from server here
                        Sync.SyncAllModules();
                    }
                }

                if (longSyncStopWatch.IsRunning)
                {
                    var IsSyncing = Preferences.Get(Keys.IsSyncingModules, true);

                    // lastSync DateTime Difference
                    var lastSyncDateTime = (DateTime)Preferences.Get(Keys.LastSyncDateTime, DateTime.UtcNow);
                    var timeDifference = (int)(DateTime.UtcNow - lastSyncDateTime).TotalMinutes;

                    // timeConstant
                    var timeConstant = lastSyncTaskTimeInterval * backgroundTaskTime;

                    if (timeDifference >= timeConstant && ((int)(timeDifference % timeConstant) == 0) && !isSyncing && !IsSyncing && Preferences.ContainsKey(Keys.NotFirstTime))
                    {
                        isSyncing = true;
                        var syncAlertPopup = new SyncAlertPopup();
                        syncAlertPopup.isSyncing += (obj) =>
                        {
                            isSyncing = obj;
                        };

                        PopupNavigation.PushAsync(syncAlertPopup);
                    }
                }


                //returning true to keep timer running
                firstRun = false;
                return true;
            });
        }

        public static async Task ToggleAutoSync(bool autoSync)
        {
            if (!autoSync)
                stopWatch.Reset();
            else
            {
                if (!stopWatch.IsRunning)
                    stopWatch.Start();
            }

            //Current.Properties[AutoSyncKey] = autoSync;
            //await Current.SavePropertiesAsync();

            Preferences.Set(Keys.AutoSyncKey,autoSync);

            AutoSync = autoSync;
        }

        protected override void OnSleep()
        {
            //long sync stopwatch on device sleep
            longSyncStopWatch.Reset();

            //restart stopwatch on device sleep
            if (AutoSync)
                stopWatch.Reset();
        }

        protected override void OnResume()
        {
            //long sync stopwatch on device resume
            longSyncStopWatch.Start();

            //start stopwatch on app resume
            if (AutoSync)
                stopWatch.Start();
        }
    }
}

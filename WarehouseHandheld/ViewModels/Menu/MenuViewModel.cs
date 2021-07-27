using System;
using System.Windows.Input;
using Xamarin.Forms;
using WarehouseHandheld.Views.Login;
using WarehouseHandheld.Views.Sync;
using System.Collections.Generic;
using System.Diagnostics;
using WarehouseHandheld.Models.OrderProcesses;
using WarehouseHandheld.Resources;
using WarehouseHandheld.Views.Orders;
using static WarehouseHandheld.Models.Orders.OrdersSync;
using WarehouseHandheld.Views.Pallets;
using WarehouseHandheld.Views.StockTake;
using WarehouseHandheld.Views.Returns;
using WarehouseHandheld.Views.Orders.TransferOrders;
using WarehouseHandheld.Views.Orders.PickList;
using WarehouseHandheld.Views.StockEnquiry;
using WarehouseHandheld.Views.DeviceSettings;
using WarehouseHandheld.Views.GenerateLabels;
using WarehouseHandheld.Views.About;
using WarehouseHandheld.Views.PrintBarCode;
using WarehouseHandheld.Helpers;
using WarehouseHandheld.Views.StockMovement;

namespace WarehouseHandheld.ViewModels.Menu
{
    public class MenuViewModel : BaseViewModel
    {
        public ICommand SettingsCommand { get; private set; }
        protected CommandLockerHelper SettingsCommandLocker => new CommandLockerHelper((e) => { OpenSettings(e); });
        public ICommand AboutCommand { get; private set; }
        protected CommandLockerHelper AboutCommandLocker => new CommandLockerHelper((e) => { OpenAboutPage(e); });
        public ICommand LogoutCommand { get; private set; }
        protected CommandLockerHelper LogoutCommandLocker => new CommandLockerHelper((e) => { Logout(e); });
        public ICommand SyncCommand { get; private set; }
        protected CommandLockerHelper SyncCommandLocker => new CommandLockerHelper((e) => { OpenSyncPage(e); });
        public ICommand OrdersCommand { get; private set; }
        protected CommandLockerHelper OrderCommandLocker => new CommandLockerHelper((e) => { OpenOrdersPage(e); });
        public ICommand ReturnsCommand { get; private set; }
        protected CommandLockerHelper ReturnsCommandLocker => new CommandLockerHelper((e) => { OpenReturnsPage(e); });
        public ICommand PalletingCommand { get; private set; }
        protected CommandLockerHelper PalletingCommandLocker => new CommandLockerHelper((e) => { OpenPalletsPage(e); });
        public ICommand StockTakeCommand { get; private set; }
        protected CommandLockerHelper StockTakeCommandLocker => new CommandLockerHelper((e) => { OpenStockTakePage(e); });
        public ICommand WastagesCommand { get; private set; }
        protected CommandLockerHelper WastagesCommandLocker => new CommandLockerHelper((e) => { OpenWastagesPage(e); });
        public ICommand StockEnquiryCommand { get; private set; }
        protected CommandLockerHelper StockEnquiryCommandLocker => new CommandLockerHelper((e) => { OpenStockEnquiry(e); });
        public ICommand GenerateLabelCommand { get; private set; }
        protected CommandLockerHelper GenerateLabelCommandLocker => new CommandLockerHelper((e) => { OpenGenerateLabelsPage(e); });
        public ICommand GoodsReceiveCommand { get; private set; }
        protected CommandLockerHelper GoodsReceiveCommandLocker => new CommandLockerHelper((e) => { OpenGoodsReceive(e); });
        public ICommand PrintBarCodeCommand { get; private set; }
        protected CommandLockerHelper PrintBarCodeCommandLocker => new CommandLockerHelper((e) => { OpenPrintBarCodePage(e); });
        public ICommand LoadingCommand { get; private set; }
        protected CommandLockerHelper LoadingCommandLocker => new CommandLockerHelper((e) => { OpenLoadingPage(e); });
        public ICommand StockMovementCommand { get; private set; }
        protected CommandLockerHelper StockMovementCommandLocker => new CommandLockerHelper((e) => { OpenStockMovementPage(e); });

        public MenuViewModel()
        {
            SettingsCommand = new Command(SettingsCommandLocker.Execute);
            LogoutCommand = new Command(LogoutCommandLocker.Execute);
            SyncCommand = new Command(SyncCommandLocker.Execute);
            OrdersCommand = new Command(OrderCommandLocker.Execute);
            ReturnsCommand = new Command(ReturnsCommandLocker.Execute);
            PalletingCommand = new Command(PalletingCommandLocker.Execute);
            StockTakeCommand = new Command(StockTakeCommandLocker.Execute);
            WastagesCommand = new Command(WastagesCommandLocker.Execute);
            StockEnquiryCommand = new Command(StockEnquiryCommandLocker.Execute);
            GenerateLabelCommand = new Command(GenerateLabelCommandLocker.Execute);
            AboutCommand = new Command(AboutCommandLocker.Execute);
            GoodsReceiveCommand = new Command(GoodsReceiveCommandLocker.Execute);
            PrintBarCodeCommand = new Command(PrintBarCodeCommandLocker.Execute);
            LoadingCommand = new Command(LoadingCommandLocker.Execute);
            StockMovementCommand = new Command(StockMovementCommandLocker.Execute);

        }



        void OpenGoodsReceive(object obj)
        {
            App.Current.MainPage.Navigation.PushAsync(new OrdersPage(InventoryTransactionTypeEnum.PurchaseOrder, true));
        }


        void OpenAboutPage(object obj)
        {
            App.Current.MainPage.Navigation.PushAsync(new AboutPage());

        }


        void OpenGenerateLabelsPage(object obj)
        {
            App.Current.MainPage.Navigation.PushAsync(new ScanProductGenerateLabel());

        }

        void OpenStockEnquiry(object obj)
        {
            App.Current.MainPage.Navigation.PushAsync(new StockEnquiryPage());
        }

        private void OpenReturnsPage(object obj)
        {
            App.Current.MainPage.Navigation.PushAsync(new ReturnsPage(false));
        }

        private void OpenWastagesPage(object obj)
        {
            App.Current.MainPage.Navigation.PushAsync(new ReturnsPage(true));
        }

        private void OpenStockTakePage(object obj)
        {
            App.Current.MainPage.Navigation.PushAsync(new StockTakePage());
        }

        private void OpenPalletsPage(object obj)
        {
            App.Current.MainPage.Navigation.PushAsync(new PalletsPage());
        }

        private void OpenOrdersPage(object obj)
        {
            string ordersType = (string)obj;
            if (ordersType.Equals(AppStrings.PurchaseOrders))
            {
                App.Current.MainPage.Navigation.PushAsync(new OrdersPage(InventoryTransactionTypeEnum.PurchaseOrder));
            }
            else if (ordersType.Equals(AppStrings.SalesOrders))
            {
                App.Current.MainPage.Navigation.PushAsync(new OrdersPage(InventoryTransactionTypeEnum.SaleOrder));
            }
            else if (ordersType.Equals(AppStrings.TransferOrders))
            {
                App.Current.MainPage.Navigation.PushAsync(new TransferOrdersPage());
            }
            else if (ordersType.Equals(AppStrings.WorkOrders))
            {
                App.Current.MainPage.Navigation.PushAsync(new OrdersPage(InventoryTransactionTypeEnum.WorkOrder));
            }
            else if (ordersType.Equals(AppStrings.PickList))
            {
                App.Current.MainPage.Navigation.PushAsync(new PickListPage(InventoryTransactionTypeEnum.SaleOrder));
            }
        }

        private async void TestClick(object obj)
        {
            OrderProcessesSyncCollection orderProcesses = new OrderProcessesSyncCollection();
            orderProcesses.SerialNo = "920013c000814";
            orderProcesses.TerminalLogId = Guid.NewGuid();
            orderProcesses.Count = 1;
            orderProcesses.OrderProcesses = new List<OrderProcessSync>();
            OrderProcessSync orderprocess = new OrderProcessSync();
            orderprocess.OrderToken = Guid.NewGuid();
            orderprocess.OrderNotes = "some notes";
            orderprocess.AccountID = 12313;
            orderprocess.CreatedBy = 3;
            orderprocess.DeliveryNO = "123";
            orderprocess.OrderID = 23;
            orderprocess.OrderProcessID = 41;
            orderprocess.TenentId = 44;
            orderprocess.OrderProcessDetails = new List<OrderProcessDetailSync>();
            var result = await App.WarehouseService.OrderProcesses.PostOrderProcessesAsync(orderProcesses);
            Debug.WriteLine("some test");
        }

        private void OpenSyncPage(object obj)
        {
            App.Current.MainPage.Navigation.PushAsync(new SyncPage());
        }

        private void Logout(object obj)
        {
            App.Current.MainPage = new NavigationPage(new LoginPage());
        }
        private void OpenSettings(object obj)
        {
            App.Current.MainPage.Navigation.PushAsync(new DeviceSettingsPage());

        }

        private void OpenPrintBarCodePage(object obj)
        {
            App.Current.MainPage.Navigation.PushAsync(new PrintBarCodePage());
        }

        private void OpenLoadingPage(object obj)
        {
            App.Current.MainPage.Navigation.PushAsync(new PalletDispatchPage());
        }

        private void OpenStockMovementPage(object e)
        {
            App.Current.MainPage.Navigation.PushAsync(new StockMovementPage());
        }

    }
}

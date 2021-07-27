using System;
using System.Windows.Input;
using Xamarin.Forms;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Views.Menu;
using WarehouseHandheld.Models.OrderProcesses;
using WarehouseHandheld.Modules;
using System.Collections.Generic;
using System.Diagnostics;
using WarehouseHandheld.Views.About;

namespace WarehouseHandheld.ViewModels.Login
{
    public class LoginViewModel : BaseViewModel
    {

        private string _username = string.Empty;
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoginEnabled
        {
            get
            {
                // OnPropertyChanged();
                return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
            }

        }

        public ICommand LoginCommand { get; private set; }
        public ICommand AboutCommand { get; private set; }
        public ICommand TestCommand { get; private set; }

        public LoginViewModel()
        {
            LoginCommand = new Command(Login);
            // TestCommand = new Command(TestOrderProcess);
            AboutCommand = new Command(OpenAboutPage);
        }

        public void Initialize()
        {
        }

        void OpenAboutPage(object obj)
        {
            App.Current.MainPage.Navigation.PushAsync(new AboutPage());

        }


        private async void Login(object obj)
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                return;

            if (await App.Users.Login(Username, Password))
            {
                "Logged In".ToToast();
                App.Current.MainPage = new NavigationPage(new MenuPage());
            }
            else
                "Invalid Credentials".ToToast();
        }

        private async void TestOrderProcess(object obj)
        {
            OrderProcessesSyncCollection orderProcesses = new OrderProcessesSyncCollection();
            orderProcesses.SerialNo = ModulesConfig.SerialNo;
            orderProcesses.Count = 1;

            OrderProcessSync orderprocess = new OrderProcessSync();
            orderprocess.OrderToken = Guid.NewGuid();

            orderProcesses.OrderProcesses = new List<OrderProcessSync>();

            orderprocess.OrderProcessDetails = new List<OrderProcessDetailSync>();
            OrderProcessDetailSync orderProcessDetail = new OrderProcessDetailSync() {ProductId=1};
            orderprocess.OrderProcessDetails.Add(orderProcessDetail);

            orderProcesses.OrderProcesses.Add(orderprocess);

            var response = await App.WarehouseService.OrderProcesses.PostOrderProcessesAsync(orderProcesses);
        }


    }
}

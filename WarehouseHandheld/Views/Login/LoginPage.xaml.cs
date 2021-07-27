using System;
using System.Collections.Generic;

using Xamarin.Forms;
using WarehouseHandheld.ViewModels.Login;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Views.Pallets;
using System.IO;
using WarehouseHandheld.Views.Base.BaseContentPage;

namespace WarehouseHandheld.Views.Login
{
    public partial class LoginPage : BasePage
    {
        //public LoginViewModel ViewModel => BindingContext as LoginViewModel;
        //Define Properties Here...
        protected LoginViewModel ViewModel;
        public LoginPage()
        {
            try
            {
                InitializeComponent();
                NavigationPage.SetHasNavigationBar(this, false);
                ViewModel = new LoginViewModel();
                BindingContext = ViewModel;
                logo.WidthRequest = (App.ScreenWidth * (2 / 3.0));
                logo.HeightRequest = (App.ScreenHeight * (2 / 16.0));
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var terminal = await App.Database.Vehicle.GetTerminalMetaData();
            if (terminal != null && !string.IsNullOrEmpty(terminal.LogoPath))
            {
                logo.Source = ImageSource.FromFile(terminal.LogoPath);
            }

        }
    }
}

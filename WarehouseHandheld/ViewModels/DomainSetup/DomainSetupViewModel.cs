using System;
using WarehouseHandheld.Services;
using System.Windows.Input;
using Xamarin.Forms;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Views.Login;
using WarehouseHandheld.Views.About;
using WarehouseHandheld.Resources;
using Xamarin.Essentials;
namespace WarehouseHandheld.ViewModels.DomainSetup
{
    public class DomainSetupViewModel : BaseViewModel
    {
        private string domain = WebServiceConfig.BaseUrl;
        public string Domain
        {
            get { return domain.Replace("http://", "").Replace("https://",""); }
            set
            {
                domain = value;
                OnPropertyChanged();
            }
        }

        private string protocol = "http://";
        public string Protocol
        {
            get { return protocol; }
            set
            {
                protocol = value;
                OnPropertyChanged();
            }
        }


        public ICommand SaveCommand => new Command(OnSave);
        public ICommand ChangeProtocolCommand => new Command(ChangeProtocol);
        public ICommand AboutCommand => new Command(OpenAboutPage);

        public DomainSetupViewModel()
        {

        }

        private void ChangeProtocol()
        {
            if (Protocol.Equals("http://"))
            {
                Protocol = "https://";
            }
            else
            {
                Protocol = "http://";
            }
        }

        private async void OpenAboutPage(object obj)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new AboutPage());
        }

        private async void OnSave(object obj)
        {
            if (string.IsNullOrEmpty(Domain))
                "Please enter a domain.".ToToast(showOnTop:false);
            else
            {
//                var domainFormatted = "http://" + Domain;
                var domainFormatted = Protocol + Domain;
                if (Uri.IsWellFormedUriString(domainFormatted, UriKind.Absolute))
                {
                    //App.Current.Properties.Add(Keys.IsDomainSet, true);
                    //Application.Current.Properties["Domain"] = domainFormatted;
                    //await Application.Current.SavePropertiesAsync();

                    Preferences.Set(Keys.IsDomainSet, true);
                    Preferences.Set("Domain", domainFormatted);
                    Preferences.Set(Keys.AutoSyncKey, true);

                    App.WarehouseService.BaseUri = new Uri(domainFormatted);
                    Application.Current.MainPage = new NavigationPage(new LoginPage());
                    App.Sync.SyncAllModules();
                }
                else
                    "Please enter a valid Url.".ToToast();
            }
        }
    }
}

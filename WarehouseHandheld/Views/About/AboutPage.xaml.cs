using System;
using System.Collections.Generic;
using WarehouseHandheld.Modules;
using WarehouseVanSales.Helpers;
using Xamarin.Forms;
using Xamarin.Essentials;
using WarehouseHandheld.Resources;
using WarehouseHandheld.Views.Base.BaseContentPage;
using WarehouseHandheld.Helpers;
using WarehouseHandheld.Extensions;
using System.Threading.Tasks;

namespace WarehouseHandheld.Views.About
{
    public partial class AboutPage : BasePage
    {
        public AboutPage()
        {
            InitializeComponent();
            identifierLabel.Text = ModulesConfig.deviceIMEI;
            versionNumberLabel.Text = DependencyService.Get<IDeviceIMEI>().ApplicationsPrivateVersion;
            versionNameLabel.Text = DependencyService.Get<IDeviceIMEI>().ApplicationsPublicVersion;
            //if(Application.Current.Properties.ContainsKey("Domain"))
            //{
            //    domainLabel.Text =  Application.Current.Properties["Domain"].ToString();
            //}
            if (Preferences.ContainsKey("Domain"))
            {
                domainLabel.Text = Preferences.Get("Domain",App.WarehouseService.BaseUri.ToString());
            }

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var terminal = await App.Database.Vehicle.GetTerminalMetaData();
            //terminal.AllowExportDatabase = true;
            if (terminal != null && terminal.AllowExportDatabase)
            {
                ExportDbBtn.IsVisible = true;
            }
        }
        void Handle_Clicked(object sender, System.EventArgs e)
        {
            App.Sync.SyncAllModules();
            //Preferences.Set(Keys.IsSyncingModules, false);
        }

        async void ExportDb(object sender, System.EventArgs e)
        {
            var path = DependencyService.Get<IDatabaseExportHelper>().ExportDb();
            "Database exported successfully.".ToToast();
            await Task.Delay(1000);
            path.ToToast();
        }
    }
}

using System;
using System.Collections.Generic;

using Xamarin.Forms;
using WarehouseHandheld.ViewModels.Sync;
using WarehouseHandheld.Resources;
using System.Linq;
using WarehouseHandheld.Modules;
using Xamarin.Essentials;
using WarehouseHandheld.Views.Base.BaseContentPage;

namespace WarehouseHandheld.Views.Sync
{
    public partial class SyncPage : BasePage
    {
        public new SyncViewModel ViewModel => BindingContext as SyncViewModel;

        void Handle_SwitchToggled(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            ViewModel.AutoSyncToggled();
        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        void Handle_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            App.backgroundTaskTime = Constants.MinutesList[minutesPicker.SelectedIndex];
            ModulesConfig.SyncTime = App.backgroundTaskTime;
            //if (App.Current.Properties.ContainsKey(Keys.SyncTime))
            //{
            //    App.Current.Properties[Keys.SyncTime] = App.backgroundTaskTime;
            //}
            //else
            //{
            //    App.Current.Properties.Add(Keys.SyncTime, App.backgroundTaskTime);
            //}

            if (Preferences.ContainsKey(Keys.SyncTime))
            {
                Preferences.Set(Keys.SyncTime, App.backgroundTaskTime);
            }
            else
            {
                Preferences.Set(Keys.SyncTime, App.backgroundTaskTime);
            }
        }

        public SyncPage()
        {
            InitializeComponent();
            minutesPicker.SelectedIndex = Constants.MinutesList.ToList().FindIndex((x) => x == App.backgroundTaskTime);

        }
    }
}

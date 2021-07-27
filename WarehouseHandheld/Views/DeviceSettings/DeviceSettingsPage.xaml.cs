using System;
using System.Collections.Generic;
using System.Linq;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Models.DeviceSettings;
using WarehouseHandheld.ViewModels.DeviceSettings;
using WarehouseHandheld.Views.DeviceSettings.AddDevice;
using Xamarin.Forms;
using WarehouseHandheld.Views.Base.BaseContentPage;

namespace WarehouseHandheld.Views.DeviceSettings
{
    public partial class DeviceSettingsPage : BasePage
    {
        DeviceSettingsViewModel ViewModel => BindingContext as DeviceSettingsViewModel;
        public DeviceSettingsPage()
        {
            InitializeComponent();
        }

        void AddDevice_Clicked(object sender, System.EventArgs e)
        {
            var addDevicePopup = new AddDevicePopup();
            addDevicePopup.ViewModel.AddNewDevice += (obj) => {
                ViewModel.DeviceList.Add(obj);
            };
            PopupNavigation.PushAsync(addDevicePopup);
        }

        async void Device_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }

            var result = await DisplayActionSheet("", "Cancel", "", "Connect", "Modify");
            if (result != null && result.Equals("Connect"))
            {

                var name = ((DeviceModel)e.SelectedItem).DeviceName;
                bool IsConfirm = await DisplayAlert("", "Are you sure you want to connect with " + name + "?", "Yes", "No");
                if (IsConfirm)
                {
                    await ViewModel.ConnectDevice(name);
                }
            }
            else if (result != null)
            {
                var deviceSetting = (DeviceModel)e.SelectedItem;
                var addDevicePopup = new AddDevicePopup(deviceSetting);
                addDevicePopup.ViewModel.AddNewDevice += (obj) => {
                    var index = ViewModel.DeviceList.ToList().FindIndex((x) => x.Id == deviceSetting.Id);
                    ViewModel.DeviceList.RemoveAt(index);
                    ViewModel.DeviceList.Insert(index, obj);
                };
                await PopupNavigation.PushAsync(addDevicePopup);
            }
            DevicesListView.SelectedItem = null;
        }
    }
}

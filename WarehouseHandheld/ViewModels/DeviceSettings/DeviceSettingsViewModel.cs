using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Helpers;
using WarehouseHandheld.Models.DeviceSettings;
using Xamarin.Forms;

namespace WarehouseHandheld.ViewModels.DeviceSettings
{
    public class DeviceSettingsViewModel : BaseViewModel
    {
        private ObservableCollection<DeviceModel> deviceList;
        public ObservableCollection<DeviceModel> DeviceList
        {
            get { return deviceList; }
            set
            {
                deviceList = value;
                OnPropertyChanged();
            }
        }

        public DeviceSettingsViewModel()
        {
            Initialize();
        }

        public async Task Initialize()
        {
            DeviceList = new ObservableCollection<DeviceModel>(await App.Database.DeviceSettings.GetAllDevices());
        }

        public async Task ConnectDevice(string deviceName)
        {
            bool Found = false;
            var printer = DependencyService.Get<IPrinterHelper>();
            var devices = await printer.GetPairedDevices();
            foreach(var device in devices)
            {
                if(!string.IsNullOrEmpty(device.Name) && device.Name.ToLower().Contains(deviceName.ToLower()))
                {

                    if(printer.Connect(device.MacAddress,2))
                    {
                        "Connected".ToToast();
                    }
                    else{
                        "Error in connection".ToToast();
                    }
                    Found = true;
                }
            }
            if(!Found)
            {
                "Device Not Found. Please pair this device first.".ToToast();
            }
        }
    }
}

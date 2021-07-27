using System;
using System.Collections.Generic;
using WarehouseHandheld.Models.DeviceSettings;
using WarehouseHandheld.ViewModels.DeviceSettings.AddDevice;
using WarehouseHandheld.Views.Base.Popup;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.DeviceSettings.AddDevice
{
    public partial class AddDevicePopup : PopupBase
    {
        public AddDeviceViewModel ViewModel => BindingContext as AddDeviceViewModel;
        public AddDevicePopup()
        {
            InitializeComponent();
        }

        public AddDevicePopup(DeviceModel device)
        {
            InitializeComponent();
            ViewModel.Device = device;
            ViewModel.DeviceName = device.DeviceName;
            ViewModel.DeviceType = device.DeviceType;
            ViewModel.Notes = device.Notes;
            ViewModel.Font = device.Font.ToString();
            ViewModel.TextHeight = device.TextHeight;
            ViewModel.TextWidth = device.TextWidth;
            ViewModel.HeaderWidth = device.HeaderWidth;
            ViewModel.HeaderHeight = device.HeaderHeight;
            ViewModel.PaperWidth = device.PaperWidth;
            ViewModel.PaperHeight = device.PaperHeight;
            ViewModel.PaperGap = device.PaperGap;
            ViewModel.MediaType = device.MediaType.ToString();
        }

        void Picker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if (((Picker)sender).SelectedIndex == 0)
            {
                ViewModel.IsVisiblePaperConfigurations = false;
                ViewModel.PaperWidth = "80";
            }
            else if (((Picker)sender).SelectedIndex == 1)
            {
                ViewModel.IsVisiblePaperConfigurations = true;
                ViewModel.PaperWidth = "76";
            }
        }
    }
}

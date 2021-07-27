using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Models.DeviceSettings;
using WarehouseHandheld.Models.Enums;
using Xamarin.Forms;
using System.Linq;

namespace WarehouseHandheld.ViewModels.DeviceSettings.AddDevice
{
    public class AddDeviceViewModel : BaseViewModel
    {
        public ICommand SaveCommand => new Command(OnSave);
        public Action<DeviceModel> AddNewDevice;
        public DeviceModel Device;
        public AddDeviceViewModel()
        {

            SetFontList();
            SetSizeList();
            SetMediaTypeList();
            SetDeviceTypeList();
        }

        [Obsolete]
        async void OnSave(object obj)
        {
            if (string.IsNullOrEmpty(DeviceName) || string.IsNullOrEmpty(DeviceType) || string.IsNullOrEmpty(TextWidth) || string.IsNullOrEmpty(TextHeight) || string.IsNullOrEmpty(MediaType) || string.IsNullOrEmpty(HeaderWidth) || string.IsNullOrEmpty(HeaderHeight) || string.IsNullOrEmpty(paperWidth))
            {
                "Please fill all the details".ToToast();
            }
            else {

                if (MediaType == "Gap")
                {
                    //IsVisiblePaperConfigurations = true;

                    if (string.IsNullOrEmpty(DeviceName) || string.IsNullOrEmpty(DeviceType) || string.IsNullOrEmpty(TextWidth) || string.IsNullOrEmpty(TextHeight) || string.IsNullOrEmpty(MediaType) || string.IsNullOrEmpty(HeaderWidth) || string.IsNullOrEmpty(HeaderHeight) || string.IsNullOrEmpty(PaperWidth) || string.IsNullOrEmpty(PaperHeight) || string.IsNullOrEmpty(PaperGap))
                    {
                        "Please fill all the details".ToToast();
                    }
                    else
                    {
                        DeviceModel device = new DeviceModel();
                        if (Device != null)
                        {
                            device.Id = Device.Id;
                        }
                        device.DeviceName = DeviceName;
                        device.DeviceType = DeviceType;
                        device.Notes = Notes;
                        device.TextHeight = TextHeight;
                        device.TextWidth = TextWidth;
                        device.HeaderHeight = HeaderHeight;
                        device.HeaderWidth = HeaderWidth;
                        device.PaperWidth = PaperWidth;
                        device.PaperHeight = PaperHeight;
                        device.PaperGap = PaperGap;
                        device.MediaType = (PrintMediaTypeEnum)Enum.Parse(typeof(PrintMediaTypeEnum), MediaType);
                        device.Font = (PrintFontTypeEnum)Enum.Parse(typeof(PrintFontTypeEnum), Font);
                        await App.Database.DeviceSettings.AddDeviceSettings(device);
                        AddNewDevice?.Invoke(device);
                        "Device Added Successfully".ToToast();
                        await PopupNavigation.PopAsync();
                    }
                }

                else if (MediaType == "Continues")
                {
                    if (string.IsNullOrEmpty(DeviceName) || string.IsNullOrEmpty(DeviceType) || string.IsNullOrEmpty(TextWidth) || string.IsNullOrEmpty(TextHeight) || string.IsNullOrEmpty(MediaType) || string.IsNullOrEmpty(HeaderWidth) || string.IsNullOrEmpty(HeaderHeight) || string.IsNullOrEmpty(PaperWidth))
                    {
                        "Please fill all the details".ToToast();
                    }
                    else
                    {
                        DeviceModel device = new DeviceModel();
                        if (Device != null)
                        {
                            device.Id = Device.Id;
                        }
                        device.DeviceName = DeviceName;
                        device.DeviceType = DeviceType;
                        device.Notes = Notes;
                        device.TextHeight = TextHeight;
                        device.TextWidth = TextWidth;
                        device.HeaderHeight = HeaderHeight;
                        device.HeaderWidth = HeaderWidth;
                        device.PaperWidth = PaperWidth;
                        device.PaperHeight = PaperHeight;
                        device.PaperGap = PaperGap;
                        device.MediaType = (PrintMediaTypeEnum)Enum.Parse(typeof(PrintMediaTypeEnum), MediaType);
                        device.Font = (PrintFontTypeEnum)Enum.Parse(typeof(PrintFontTypeEnum), Font);
                        await App.Database.DeviceSettings.AddDeviceSettings(device);
                        AddNewDevice?.Invoke(device);
                        "Device Added Successfully".ToToast();
                        await PopupNavigation.PopAsync();
                    }
                }
            }

        }


           

        private string deviceName = string.Empty;
        public string DeviceName
        {
            get { return deviceName; }
            set
            {
                deviceName = value;
                OnPropertyChanged();
            }
        }

        private string paperWidth = "80";
        public string PaperWidth
        {
            get { return paperWidth; }
            set
            {
                paperWidth = value;
                OnPropertyChanged();
            }
        }

        private string paperHeight = "50";
        public string PaperHeight
        {
            get { return paperHeight; }
            set
            {
                paperHeight = value;
                OnPropertyChanged();
            }
        }

        private string paperGap = "3";
        public string PaperGap
        {
            get { return paperGap; }
            set
            {
                paperGap = value;
                OnPropertyChanged();
            }
        }





        private string deviceType = string.Empty;
        public string DeviceType
        {
            get { return deviceType; }
            set
            {
                deviceType = value;
                OnPropertyChanged();
            }
        }

        private string font = string.Empty;
        public string Font
        {
            get { return font; }
            set
            {
                font = value;
                OnPropertyChanged();
            }
        }

        private string textHeight = string.Empty;
        public string TextHeight
        {
            get { return textHeight; }
            set
            {
                textHeight = value;
                OnPropertyChanged();
            }
        }

        private string textWidth = string.Empty;
        public string TextWidth
        {
            get { return textWidth; }
            set
            {
                textWidth = value;
                OnPropertyChanged();
            }
        }

        private string headerHeight = string.Empty;
        public string HeaderHeight
        {
            get { return headerHeight; }
            set
            {
                headerHeight = value;
                OnPropertyChanged();
            }
        }

        private string headerWidth = string.Empty;
        public string HeaderWidth
        {
            get { return headerWidth; }
            set
            {
                headerWidth = value;
                OnPropertyChanged();
            }
        }

        private string notes = string.Empty;
        public string Notes
        {
            get { return notes; }
            set
            {
                notes = value;
                OnPropertyChanged();
            }
            }
    
        private string mediaType = string.Empty;
        public string MediaType
        {
            get { return mediaType; }
            set
            {
                mediaType = value;
                OnPropertyChanged();
            }
        }

        private bool isVisiblePaperConfigurations = false;
        public bool IsVisiblePaperConfigurations
        {
            get
            {
                return isVisiblePaperConfigurations;
            }
            set
            {
                isVisiblePaperConfigurations = value;
                OnPropertyChanged(nameof(IsVisiblePaperConfigurations));
            }
        }

        private ObservableCollection<string> fontList;
        public ObservableCollection<string> FontList
        {
            get { return fontList; }
            set
            {
                fontList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> mediaTypeList;
        public ObservableCollection<string> MediaTypeList
        {
            get { return mediaTypeList; }
            set
            {
                mediaTypeList = value;
               
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> deviceTypeList;
        public ObservableCollection<string> DeviceTypeList
        {
            get { return deviceTypeList; }
            set
            {
                deviceTypeList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> sizeList;
        public ObservableCollection<string> SizeList
        {
            get { return sizeList; }
            set
            {
                sizeList = value;
                OnPropertyChanged();
            }
        }

        void SetFontList()
        {
            FontList = new ObservableCollection<string>(Enum.GetValues(typeof(PrintFontTypeEnum)).Cast<PrintFontTypeEnum>().Select(v => v.ToString()).ToList());

        }

        void SetDeviceTypeList()
        {
            DeviceTypeList = new ObservableCollection<string>(Enum.GetValues(typeof(DeviceTypeEnum)).Cast<DeviceTypeEnum>().Select(v => v.ToString()).ToList());

        }

        void SetMediaTypeList()
        {
            MediaTypeList = new ObservableCollection<string>(Enum.GetValues(typeof(PrintMediaTypeEnum)).Cast<PrintMediaTypeEnum>().Select(v => v.ToString()).ToList());

        }

        void SetSizeList()
        {
            SizeList = new ObservableCollection<string>();
            SizeList.Add("6");
            SizeList.Add("8");
            SizeList.Add("10");
            SizeList.Add("12");
            SizeList.Add("14");
            SizeList.Add("16");
            SizeList.Add("18");
            SizeList.Add("20");
            SizeList.Add("22");
            SizeList.Add("24");
            SizeList.Add("26");
            SizeList.Add("28");
            SizeList.Add("32");
            SizeList.Add("36");
            SizeList.Add("40");
            SizeList.Add("44");
            SizeList.Add("48");
            SizeList.Add("52");
            SizeList.Add("56");
            SizeList.Add("60");
            SizeList.Add("64");
            SizeList.Add("68");
            SizeList.Add("72");

        }

    }
}

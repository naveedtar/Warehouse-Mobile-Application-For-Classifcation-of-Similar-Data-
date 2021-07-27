using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Media;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Models.Users;
using WarehouseHandheld.Models.Vehicles;
using WarehouseHandheld.Resources;
using Xamarin.Forms;

namespace WarehouseHandheld.ViewModels.Pallets
{
    public class PalletDispatchViewModel : BaseViewModel
    {
        List<UserSync> AllDrivers;
        List<MarketVehiclesSync> AllVehicles;
        List<byte[]> ProofImages = new List<byte[]>();
        List<PalletDispatchMethodSync> AllDispatchMethods;
        public ICommand TakePictureCommand => new Command(TakePicture);
        public Action<List<byte[]>, PalletDispatchSync, string> OnDispatch;
        string FileName;
        public PalletDispatchViewModel()
        {
            SetData();
        }

        async void TakePicture()
        {
            try
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    "Unable to open camera".ToToast();
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg",
                    CompressionQuality = 0
                });

                if (file != null)
                {
                    ProofImagesCount++;
                    FileName = file.Path;
                    byte[] imageAsBytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        file.GetStream().CopyTo(memoryStream);
                        file.Dispose();
                        imageAsBytes = memoryStream.ToArray();
                    }
                    ProofImages.Add(imageAsBytes);
                    "Proof Image Saved.".ToToast();
                }
            }
            catch
            {
                "Unable to open camera".ToToast();
            }
        }

        private ObservableCollection<string> drivers = new ObservableCollection<string>();
        public ObservableCollection<string> Drivers
        {
            get { return drivers; }
            set
            {
                drivers = value;
                OnPropertyChanged();
            }
        }

        private bool isCustomVehicle;
        public bool IsCustomVehicle
        {
            get { return isCustomVehicle; }
            set
            {
                isCustomVehicle = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> dispatchMethods = new ObservableCollection<string>();
        public ObservableCollection<string> DispatchMethods
        {
            get { return dispatchMethods; }
            set
            {
                dispatchMethods = value;
                OnPropertyChanged();
            }
        }

        public string TakePictureText => ProofImagesCount == 0 ? AppStrings.TakePicture : AppStrings.TakeAnotherPicture;

        private ObservableCollection<string> vehicles = new ObservableCollection<string>();
        public ObservableCollection<string> Vehicles
        {
            get { return vehicles; }
            set
            {
                vehicles = value;
                OnPropertyChanged();
            }
        }

        private string selectedVehicle;
        public string SelectedVehicle
        {
            get { return selectedVehicle; }
            set
            {
                selectedVehicle = value;
                SetCustomVehicle(value);
                OnPropertyChanged();
            }
        }

        void SetCustomVehicle(string vehicleType)
        {
            if (!string.IsNullOrEmpty(vehicleType) && vehicleType.Equals("Other"))
            {
                IsCustomVehicle = true;
            }
            else
            {
                IsCustomVehicle = false;
            }
        }


        private string selectedDispatchMethod;
        public string SelectedDispatchMethod
        {
            get { return selectedDispatchMethod; }
            set
            {
                selectedDispatchMethod = value;
                OnPropertyChanged();
            }
        }

        private string selectedDriver;
        public string SelectedDriver
        {
            get { return selectedDriver; }
            set
            {
                selectedDriver = value;
                OnPropertyChanged();
            }
        }

        private string trackReference;
        public string TrackReference
        {
            get { return trackReference; }
            set
            {
                trackReference = value;
                OnPropertyChanged();
            }
        }

        private string dispatchNotes;
        public string DispatchNotes
        {
            get { return dispatchNotes; }
            set
            {
                dispatchNotes = value;
                OnPropertyChanged();
            }
        }

        private string customVehicleIdentifier;
        public string CustomVehicleIdentifier
        {
            get { return customVehicleIdentifier; }
            set
            {
                customVehicleIdentifier = value;
                OnPropertyChanged();
            }
        }

        private string customVehicleModel;
        public string CustomVehicleModel
        {
            get { return customVehicleModel; }
            set
            {
                customVehicleModel = value;
                OnPropertyChanged();
            }
        }

        private string customVehicleDescription;
        public string CustomVehicleDescription
        {
            get { return customVehicleDescription; }
            set
            {
                customVehicleDescription = value;
                OnPropertyChanged();
            }
        }


        private string dispatchReference;
        public string DispatchReference
        {
            get { return dispatchReference; }
            set
            {
                dispatchReference = value;
                OnPropertyChanged();
            }
        }

        private int proofImagesCount = 0;
        public int ProofImagesCount
        {
            get { return proofImagesCount; }
            set
            {
                proofImagesCount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TakePictureText));
            }
        }

        async void SetData()
        {
            AllDispatchMethods = await App.Pallets.GetPalletDispatchMethods();
            if (AllDispatchMethods != null)
            {
                foreach (var method in AllDispatchMethods)
                {
                    DispatchMethods.Add(method.SentMethod);
                }
            }

            AllVehicles = await App.Vehicles.GetAllVehicles();
            if (AllVehicles != null)
            {
                foreach (var vehicle in AllVehicles)
                {
                    Vehicles.Add(vehicle.Name);
                }
                Vehicles.Add("Other");
            }

            AllDrivers = await App.Users.GetDrivers();
            if (AllDrivers != null)
            {
                foreach (var driver in AllDrivers)
                {
                    Drivers.Add(driver.Name);
                }
            }
        }

        public bool OnSave()
        {
            if (string.IsNullOrEmpty(SelectedVehicle) || (SelectedVehicle.Equals("Other") && string.IsNullOrEmpty(CustomVehicleIdentifier)) || string.IsNullOrEmpty(SelectedDispatchMethod))
            {
                return false;
            }
            var Driver = AllDrivers.Find((obj) => obj.Name == SelectedDriver);
            var DispatchMethod = AllDispatchMethods.Find((obj) => obj.SentMethod == SelectedDispatchMethod);
            var dispatchInfo = new PalletDispatchSync();
            if (!SelectedVehicle.Equals("Other"))
            {
                var Vehicle = AllVehicles.Find((obj) => obj.Name == SelectedVehicle);
                dispatchInfo.VehicleIdentifier = Vehicle.VehicleIdentifier;
                dispatchInfo.MarketVehicleID = Vehicle.MarketId;
            }
            else
            {
                dispatchInfo.CustomVehicleNumber = CustomVehicleIdentifier;
                dispatchInfo.CustomVehicleModel = CustomVehicleModel;
                dispatchInfo.VehicleDescription = CustomVehicleDescription;
            }

            dispatchInfo.DispatchNotes = DispatchNotes;
            dispatchInfo.TrackingReference = TrackReference;
            if (DispatchMethod != null)
            {
                dispatchInfo.SentMethodID = DispatchMethod.SentMethodID;
            }

            if (Driver != null)
            {
                dispatchInfo.MarketVehicleDriverID = Driver.ResourceId;
            }

            dispatchInfo.DispatchReference = DispatchReference;
            OnDispatch?.Invoke(ProofImages, dispatchInfo, FileName);
            return true;
        }

    }
}

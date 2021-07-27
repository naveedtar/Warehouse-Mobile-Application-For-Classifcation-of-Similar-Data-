using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Helpers;
using WarehouseHandheld.Models.Enums;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Views.Pallets;
using Xamarin.Forms;

namespace WarehouseHandheld.ViewModels.Pallets
{
    public class PalletDispatchPodViewModel : BaseViewModel
    {

        public bool IsAcceptedOpen;
        private ObservableCollection<PalletDispatchSync> beloaded;
        public ObservableCollection<PalletDispatchSync> BeLoaded
        {
            get { return beloaded; }
            set
            {
                beloaded = value;
                OnPropertyChanged();
            }
        }

    




        public async Task<bool> Initialize()
        {
            IsBusy = true;
            var palletDispatches = await App.Pallets.GetAllPalletDispatches();
            if (palletDispatches == null)
            {
                IsBusy = false;
                return false;
            }
            IsBusy = false;
            BeLoaded = new ObservableCollection<PalletDispatchSync>(palletDispatches.FindAll((obj) => obj.DispatchStatus == (int)PalletDispatchStatusEnum.Scheduled));
            BeLoaded = new ObservableCollection<PalletDispatchSync>(BeLoaded.Reverse());
         
            return true;

        }


        protected CommandLockerHelper SelectedCommandLocker => new CommandLockerHelper(async (e) => { await OnItemSelected(e); });
        public ICommand ItemSelectedCommand => new Command(SelectedCommandLocker.Execute);

        private async Task OnItemSelected(object e)
        {
            PalletDispatchSync palletDispatchObj;

            if (e is PalletDispatchSync)
            {
                palletDispatchObj = (PalletDispatchSync)e;
            }
            else
            {
                palletDispatchObj = (PalletDispatchSync)((SelectedItemChangedEventArgs)e).SelectedItem;
            }

            var terminalMetaData = await App.Database.Vehicle.GetTerminalMetaData();
            //terminalMetaData.ScanVehicleLicensePlate = true;
            if (terminalMetaData.ScanVehicleLicensePlate)
            {
                var palletDispatchVehicleScanPopup = new PalletDispatchVehicleScanPopup(palletDispatchObj);
                palletDispatchVehicleScanPopup.IsVehicleRegistrationMatched += (async (obj) =>
                {
                    var isProceed = (bool)obj;
                    if (isProceed)
                    {
                       await GoToPalletDispatchDetailProofOfDeliveryPage(palletDispatchObj);
                    }
                });
                
                await PopupNavigation.Instance.PushAsync(palletDispatchVehicleScanPopup);
            }
            else
            {
                await GoToPalletDispatchDetailProofOfDeliveryPage(palletDispatchObj);
            }
        }

        private async Task GoToPalletDispatchDetailProofOfDeliveryPage(PalletDispatchSync palletDispatchSync)
        {
            if (CrossConnectivity.Current.IsConnected && await Util.Util.IsConnected())
            {
                await Application.Current.MainPage.Navigation.PushAsync(new PalletDispatchDetailPodPage(palletDispatchSync));
            }
            else
            {
                if (CrossConnectivity.Current.IsConnected)
                    "No Internet Connection.".ToToast();
            }
        }
    }
}

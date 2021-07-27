using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Models.Vehicles;
using WarehouseHandheld.Views.Base.Popup;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.Pallets
{
    public partial class PalletDispatchVehicleScanPopup : PopupBase
    {
        private bool IsAppearing;
        private bool isProceed;
        public Action<bool> IsVehicleRegistrationMatched;
        private readonly PalletDispatchSync _palletDispatchSync;
        public PalletDispatchVehicleScanPopup(PalletDispatchSync palletDispatchSync)
        {
            InitializeComponent();
            _palletDispatchSync = palletDispatchSync;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Device.BeginInvokeOnMainThread(() =>
            {
                scanEntry.Focus();
            });
        }

        private async void OnSave()
        {
            SaveButtonEnabled = false;

            if (!string.IsNullOrEmpty(scanEntry.Text))
            {
                if (_palletDispatchSync != null && _palletDispatchSync.MarketVehicle != null)
                {
                    if (scanEntry.Text.ToLower().Equals(_palletDispatchSync.MarketVehicle.Name.ToLower()) || scanEntry.Text.ToLower().Equals(_palletDispatchSync.MarketVehicle.VehicleIdentifier.ToLower()))
                    {
                        isProceed = true;
                        IsVehicleRegistrationMatched?.Invoke(isProceed);
                        await PopupNavigation.Instance.PopAsync();
                    }
                    else
                    {
                        isProceed = await App.Current.MainPage.DisplayAlert("Warning", "Scanned vehicle registration number doesn't match with registration number assigned to this order. Do you wish to proceed?", "Yes", "No");
                        if (isProceed)
                        {
                            IsVehicleRegistrationMatched?.Invoke(isProceed);
                            await PopupNavigation.Instance.PopAsync();
                        }
                        else
                        {
                            SaveButtonEnabled = true;
                            return;
                        }
                    }
                }
            }
            else
            {
                await Util.Util.ShowErrorPopupWithBeep("Enter Vehicle Registration");
                SaveButtonEnabled = true;
                return;
            }
        }

        async void Handle_Completed(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(scanEntry.Text))
            {
                if (_palletDispatchSync != null && _palletDispatchSync.MarketVehicle != null)
                {
                    if (scanEntry.Text.ToLower().Equals(_palletDispatchSync.MarketVehicle.Name.ToLower()) || scanEntry.Text.ToLower().Equals(_palletDispatchSync.MarketVehicle.VehicleIdentifier.ToLower()))
                    {
                        isProceed = true;
                        IsVehicleRegistrationMatched?.Invoke(isProceed);
                        await PopupNavigation.Instance.PopAsync();
                    }
                    else
                    {
                        isProceed = await App.Current.MainPage.DisplayAlert("Warning", "Scanned vehicle registration number doesn't match with registration number assigned to this order. Do you wish to proceed?", "Yes", "No");
                        if (isProceed)
                        {
                            IsVehicleRegistrationMatched?.Invoke(isProceed);
                            await PopupNavigation.Instance.PopAsync();
                        }
                        else
                        {
                            SaveButtonEnabled = true;
                            scanEntry.Unfocus();
                            scanEntry.ShowKeyboard = false;
                            scanEntry.Text = string.Empty;
                            await Task.Delay(200);
                            scanEntry.Focus();
                            return;
                        }
                    }
                }
            }
            else
            {
                await Util.Util.ShowErrorPopupWithBeep("Enter Vehicle Registration");
                SaveButtonEnabled = true;
                scanEntry.Unfocus();
                scanEntry.ShowKeyboard = false;
                scanEntry.Text = string.Empty;
                await Task.Delay(200);
                scanEntry.Focus();
                return;
            }

            scanEntry.Unfocus();
            scanEntry.ShowKeyboard = false;
            scanEntry.Text = string.Empty;
            await Task.Delay(200);
            scanEntry.Focus();
        }

        async void Keyboard_Tapped(object sender, System.EventArgs e)
        {
            scanEntry.ShowKeyboard = !scanEntry.ShowKeyboard;
            await System.Threading.Tasks.Task.Delay(200);
            scanEntry.Focus();
        }

    }
}

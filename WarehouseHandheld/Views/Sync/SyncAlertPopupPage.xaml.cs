using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Resources;
using WarehouseHandheld.Views.Base.Popup;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.Sync
{
    public partial class SyncAlertPopupPage : PopupBase
    {
        private readonly string _syncAlertText;
        private readonly string _connectionErrorMsg;
        private readonly string _syncModule;
        private readonly string _syncCompletedMsg;
        private bool _syncStarted;
        private readonly Stopwatch _stopWatch = new Stopwatch();
        public Action SetFocus;

        public SyncAlertPopupPage(string syncAlertText, string connectionErrorMsg, string syncModule, string syncCompletedMsg)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(syncAlertText) && !string.IsNullOrEmpty(connectionErrorMsg) && !string.IsNullOrEmpty(syncModule) && !string.IsNullOrEmpty(syncModule) && !string.IsNullOrEmpty(syncCompletedMsg))
            {
                _syncAlertText = syncAlertText;
                _connectionErrorMsg = connectionErrorMsg;
                _syncModule = syncModule;
                _syncCompletedMsg = syncCompletedMsg;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            SyncAlertText.Text = _syncAlertText;

            if (!_stopWatch.IsRunning)
                _stopWatch.Start();

            if (CrossConnectivity.Current.IsConnected)
            {
                SyncInProgress.IsVisible = true;
                if (SyncInProgress.IsVisible)
                {
                    Device.StartTimer(new System.TimeSpan(0, 0, 1), () =>
                    {
                        if (_stopWatch.IsRunning)
                        {
                            if (_stopWatch.Elapsed.Seconds >= 15)
                            {
                                if (SyncInProgress.IsVisible)
                                {
                                    if (!_syncStarted)
                                    {
                                        SyncInProgress.IsVisible = false;
                                        SyncAlertText.Text = _connectionErrorMsg;
                                        CtBtn.IsVisible = true;
                                        _stopWatch.Reset();
                                        return false;
                                    }
                                }
                            }
                        }

                        return true;
                    });
                }

                for (var i = 0; i < 5; i++)
                {
                    if (await Util.Util.IsConnected())
                    {
                        SyncInProgress.IsVisible = true;
                        _syncStarted = true;
                        SyncAlertText.HorizontalTextAlignment = TextAlignment.Center;
                        SyncAlertText.HorizontalOptions = LayoutOptions.CenterAndExpand;
                        await SyncModules();
                        _syncCompletedMsg.ToToast();
                        _stopWatch.Reset();
                        var popUpStack = PopupNavigation.Instance.PopupStack;
                        if (popUpStack.Any(x => x is SyncAlertPopupPage))
                        {
                            await PopupNavigation.Instance.PopAsync();
                        }

                        break;
                    }
                }
            }
            else
            {
                _stopWatch.Reset();
                SyncAlertText.Text = _connectionErrorMsg;
                CtBtn.IsVisible = true;
            }
        }

        private async void Done_Clicked(object sender, System.EventArgs e)
        {
            var popUpStack = PopupNavigation.Instance.PopupStack;
            if (popUpStack.Any(x => x is SyncAlertPopupPage))
            {
                await PopupNavigation.Instance.PopAsync();
            }
        }

        private async void Close_Clicked(object sender, System.EventArgs e)
        {
            var popUpStack = PopupNavigation.Instance.PopupStack;
            if (popUpStack.Any(x => x is SyncAlertPopupPage))
            {
                SetFocus?.Invoke();
                await PopupNavigation.Instance.PopAsync();
            }
        }

        private async Task SyncModules()
        {
            if (Constants.PalletTrackingSync.Equals(_syncModule))
            {
                await App.Pallets.SyncPalletTracking(false);
            }
        }
    }
}
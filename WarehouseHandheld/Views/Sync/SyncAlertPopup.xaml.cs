using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Resources;
using WarehouseHandheld.Views.Base.Popup;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.Sync
{
    public partial class SyncAlertPopup : PopupBase     {         public Action<bool> isSyncing;
        private static Stopwatch stopWatch = new Stopwatch();         private bool SyncStarted = false;
        public SyncAlertPopup()         {             InitializeComponent();

        }          protected override async void OnAppearing()         {             base.OnAppearing();
            var timeSyncLoop = new Stopwatch(); 
            SyncAlertText.Text = "It's been a while since last sync with server, Please wait for sync to complete.";
            if (!stopWatch.IsRunning)                 stopWatch.Start();

            if (CrossConnectivity.Current.IsConnected)             {
                SyncInProgress.IsVisible = true;                 if (SyncInProgress.IsVisible)
                {
                    Device.StartTimer(new System.TimeSpan(0, 0, 1), () =>
                    {
                        if (stopWatch.IsRunning)
                        {
                            if (stopWatch.Elapsed.Seconds >= 15)
                            {
                                if (SyncInProgress.IsVisible)
                                {                                     if (!SyncStarted)
                                    {
                                        Debug.WriteLine("Sync Overlay : Time Exceeded : couldn't connect: Time taken by Sync_Timer " + timeSyncLoop.Elapsed);
                                        Debug.WriteLine("Sync Overlay : Time Exceeded : couldn't connect: Time by Main_StopWatch " + stopWatch.Elapsed);


                                        SyncInProgress.IsVisible = false;
                                        SyncAlertText.Text = "Device is not synced with server, Please check your connection or contact support. Do you wish to proceed?";
                                        CtBtn.IsVisible = true;
                                        timeSyncLoop.Reset();
                                        stopWatch.Reset();                                         return false;
                                    }
                                 }
                            }
                        }
                        return true;
                    });                 }
                for (int i = 0; i < 5; i++)
                {                     timeSyncLoop.Start();                     Debug.WriteLine("Sync Overlay : I am in Loop No " + i);                     if (await Util.Util.IsConnected())
                    {                         var timeSyncIsConnect = new Stopwatch();                         timeSyncIsConnect.Start();                         Debug.WriteLine("Sync Overlay : Sync Started : I am in IsConnected If");
                        SyncInProgress.IsVisible = true;                         SyncStarted = true;
                        SyncAlertText.HorizontalTextAlignment = TextAlignment.Center;
                        SyncAlertText.HorizontalOptions = LayoutOptions.CenterAndExpand;
                        await App.Sync.SyncAllModules();
                        Debug.WriteLine("Sync Overlay : Sync Completed : I am in IsConnected If : Time taken by Sync_IsConnected_Timer " + timeSyncIsConnect.Elapsed);
                        timeSyncIsConnect.Reset();
                        timeSyncLoop.Reset();
                        stopWatch.Reset();

                        isSyncing?.Invoke(false);
                        Preferences.Set(Keys.LastSyncDateTime, DateTime.UtcNow);                         var popUpStack = PopupNavigation.Instance.PopupStack;
                        if (popUpStack.Any(x => x is SyncAlertPopup))
                        {
                            await PopupNavigation.Instance.PopAsync();
                        }                         break;
                    }
                }

            }             else             {
                Debug.WriteLine("Sync Overlay : Loop Completed : couldn't connect: Time taken by Sync_Timer " + timeSyncLoop.Elapsed);
                timeSyncLoop.Reset();
                stopWatch.Reset();

                SyncAlertText.Text = "Device is not synced with server, Please check your connection or contact support. Do you wish to proceed?";                 CtBtn.IsVisible = true;              }
        }


        async void Done_Clicked(object sender, System.EventArgs e)
        {
            isSyncing?.Invoke(false);
            Preferences.Set(Keys.LastSyncDateTime, DateTime.UtcNow);
            await PopupNavigation.Instance.PopAsync();
        }

        async void Close_Clicked(object sender, System.EventArgs e)
        {
            isSyncing?.Invoke(false);
            Preferences.Set(Keys.LastSyncDateTime, DateTime.UtcNow);
            var popUpStack = PopupNavigation.Instance.PopupStack;
            if (popUpStack.Any(x => x is SyncAlertPopup))
            {
                await PopupNavigation.Instance.PopAsync();
            }
        }
    }
}

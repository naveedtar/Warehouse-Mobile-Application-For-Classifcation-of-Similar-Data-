using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.IO;
using Plugin.Permissions;
using WarehouseHandheld.Resources;
using WarehouseHandheld.Views.OrderItems;
using WarehouseHandheld.Views.Returns;
using Xamarin.Essentials;
using SegmentedControl.FormsPlugin.Android;
using Android.Telephony;
using Com.Microsoft.Appcenter.Utils;
using Microsoft.AppCenter.Crashes;

namespace WarehouseHandheld.Droid
{
    [Activity(Label = "WarehouseHandheld.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            #region INITIALIZE FIREBASE CRASHLYTICS
            //TelephonyManager telephonyManager = (TelephonyManager)Forms.Context.GetSystemService(Android.Content.Context.TelephonyService);
            //var deviceId = telephonyManager.DeviceId;
            #endregion

            SegmentedControlRenderer.Init();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            global::Xamarin.Forms.Forms.Init(this, bundle);

            SetupScreenSize();
            //Crashlytics.Crashlytics.Instance.Crash();

            LoadApplication(new App());

            MessagingCenter.Subscribe<OrderItemsPage>(this, "allowLandScapePortrait", sender =>
            {
                RequestedOrientation = ScreenOrientation.Unspecified;
            });

            //during page close setting back to portrait
            MessagingCenter.Subscribe<OrderItemsPage>(this, "preventLandScape", sender =>
            {
                RequestedOrientation = ScreenOrientation.Portrait;
            });

            MessagingCenter.Subscribe<ReturnsPage>(this, "allowLandScapePortrait", sender =>
            {
                RequestedOrientation = ScreenOrientation.Unspecified;
            });

            //during page close setting back to portrait
            MessagingCenter.Subscribe<ReturnsPage>(this, "preventLandScape", sender =>
            {
                RequestedOrientation = ScreenOrientation.Portrait;
            });


            //Crashlytics.Instance.Crash();

            Rg.Plugins.Popup.Popup.Init(this, bundle);
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;

            Android.Support.V7.Widget.Toolbar toolbar
                   = this.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            Preferences.Set(Keys.IsSyncingModules, false);
        }

        void SetupScreenSize()
        {
            App.ScreenWidth = (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density);
            App.ScreenHeight = (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == 16908332) // xam forms nav bar back button id
            {
                Xamarin.Forms.Application.Current.
                       MainPage.SendBackButtonPressed();
            }
            return base.OnOptionsItemSelected(item);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return true;
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (Preferences.ContainsKey(Keys.IsDomainSet))
            {
                var autoSyncOn = (bool)Preferences.Get(Keys.AutoSyncKey, false);
                if (autoSyncOn)
                {
                    App.Sync.SyncAllModules();
                }
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (Preferences.ContainsKey(Keys.IsDomainSet))
            {
                var autoSyncOn = (bool)Preferences.Get(Keys.AutoSyncKey, false);
                if (autoSyncOn)
                {
                    App.Sync.SyncAllModules();
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            LogUnhandledException(newExc);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
            LogUnhandledException(newExc);
        }

        internal static void LogUnhandledException(Exception exception)
        {
            try
            {
                const string errorFileName = "Fatal.log";
                var libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // for iOS, I can use Environment.SpecialFolder.Resources
                var errorFilePath = Path.Combine(libraryPath, errorFileName);
                var errorMessage = String.Format("Time: {0}\r\nError: Unhandled Exception\r\n{1}",
                DateTime.Now, exception.ToString());
                File.WriteAllText(errorFilePath, errorMessage);
                // Log to Android Device Logging.
                Android.Util.Log.Error("Crash Report", errorMessage);
                Crashes.TrackError(exception, new Dictionary<string, string> {
                    { "User", App.Users.LoggedInUserId.ToString() },
                    { "Date", DateTime.Now.Date.ToString("MM/dd/yyyy HH:mm tt")},
                    { "AppID", Modules.ModulesConfig.deviceIMEI },
                });
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }
    }
}

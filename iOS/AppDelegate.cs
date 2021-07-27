using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Rg.Plugins.Popup;

namespace WarehouseHandheld.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            //Rg.Plugins.Popup.IOS.Popup.Init();
            SetupScreenSize();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        void SetupScreenSize()
        {
            App.ScreenHeight = (int)UIScreen.MainScreen.Bounds.Height;
            App.ScreenWidth = (int)UIScreen.MainScreen.Bounds.Width;
        }

    }
}

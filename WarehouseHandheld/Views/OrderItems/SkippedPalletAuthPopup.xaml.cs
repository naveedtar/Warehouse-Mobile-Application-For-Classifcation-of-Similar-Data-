using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Views.Base.Popup;

using Xamarin.Forms;

namespace WarehouseHandheld.Views.OrderItems
{
    public partial class SkippedPalletAuthPopup : PopupBase
    {
        public Action<string> VerifyUser;
        public SkippedPalletAuthPopup()
        {
            InitializeComponent();
            OnSaveClicked += () => {
                VerifyUser?.Invoke((passEntry.Text));
                PopupNavigation.PopAsync();
            };
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            passEntry.Unfocus();
            await System.Threading.Tasks.Task.Delay(200);
            passEntry.Focus();
        }
    }
}

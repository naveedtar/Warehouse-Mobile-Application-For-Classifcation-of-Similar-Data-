using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Views.Base.Popup;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.Pallets.PalletOrder
{
    public partial class PalletProductQuantityPopup : PopupBase
    {
        public Action<double> SaveQty;
        public PalletProductQuantityPopup(double maxQty)
        {
            InitializeComponent();
            OnSaveClicked += OnSave;
            OnCancelClicked += OnCancel;
            QtyStepper.Value = maxQty;
            QtyStepper.Maximum = maxQty;
            AvailableQuatity.Text = maxQty.ToString();
        }

        private async void OnCancel()
        {
            await PopupNavigation.PopAsync();
        }

        private async void OnSave()
        {
            SaveQty?.Invoke(QtyStepper.Value);
            await PopupNavigation.PopAsync();
        }
    }
}

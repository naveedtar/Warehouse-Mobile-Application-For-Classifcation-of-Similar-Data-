using System;
using UIKit;
using WarehouseHandheld.Elements.CustomEntry;
using WarehouseHandheld.iOS.CustomRenderers.CustomEntryRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(KeyboardDisabledEntry), typeof(DisabledKeyboardEntryRenderer))]
namespace WarehouseHandheld.iOS.CustomRenderers.CustomEntryRenderers
{
    public class DisabledKeyboardEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            // Disabling the keyboard
            this.Control.InputView = new UIView();
        }
    }
}

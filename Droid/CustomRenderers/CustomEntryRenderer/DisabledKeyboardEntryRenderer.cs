using System;
using Android.App;
using Android.Content;
using Android.Views.InputMethods;
using WarehouseHandheld.Droid.CustomRenderers.CustomEntryRenderer;
using WarehouseHandheld.Elements.CustomEntry;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(KeyboardDisabledEntry), typeof(DisabledKeyboardEntryRenderer))]
namespace WarehouseHandheld.Droid.CustomRenderers.CustomEntryRenderer
{
    public class DisabledKeyboardEntryRenderer : EntryRenderer
    {
        public DisabledKeyboardEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                ((KeyboardDisabledEntry)e.NewElement).PropertyChanging += OnPropertyChanging;
            }

            if (e.OldElement != null)
            {
                ((KeyboardDisabledEntry)e.OldElement).PropertyChanging -= OnPropertyChanging;
            }

            // Disable the Keyboard on Focus
            this.Control.ShowSoftInputOnFocus = false;
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

        }

        private void OnPropertyChanging(object sender, PropertyChangingEventArgs propertyChangingEventArgs)
        {
            if (Element != null && propertyChangingEventArgs != null)
            {
                if ((Element as KeyboardDisabledEntry).ShowKeyboard && ((propertyChangingEventArgs.PropertyName == VisualElement.IsFocusedProperty.PropertyName && !(Element as KeyboardDisabledEntry).IsFocused) || propertyChangingEventArgs.PropertyName == KeyboardDisabledEntry.ShowKeyboardProperty.PropertyName))
                {
                    InputMethodManager imm = (InputMethodManager)this.Context.GetSystemService(Android.Content.Context.InputMethodService);
                    imm.ShowSoftInput(this.Control, ShowFlags.Forced);
                    imm.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
                }
                // Check if the view is about to get Focus
                else if (propertyChangingEventArgs.PropertyName == KeyboardDisabledEntry.ShowKeyboardProperty.PropertyName || propertyChangingEventArgs.PropertyName == VisualElement.IsFocusedProperty.PropertyName)
                {
                    InputMethodManager imm = (InputMethodManager)this.Context.GetSystemService(Android.Content.Context.InputMethodService);
                    var activity = this.Context as Activity;
                    var token = activity.CurrentFocus?.WindowToken;
                    imm.HideSoftInputFromWindow(token, HideSoftInputFlags.None);
                }
            }
        }
    }
}

using System;
using Xamarin.Forms;

namespace WarehouseHandheld.Elements.CustomEntry
{
    public class KeyboardDisabledEntry : Entry
    {
        public static readonly BindableProperty ShowKeyboardProperty =
            BindableProperty.Create("ShowKeyboard", typeof(bool), typeof(KeyboardDisabledEntry), false);

        bool _showKeyboard;
        public bool ShowKeyboard
        {
            get { return _showKeyboard; }
            set {
                _showKeyboard = value;
                SetValue(ShowKeyboardProperty, value); }
        }
        
    }
}

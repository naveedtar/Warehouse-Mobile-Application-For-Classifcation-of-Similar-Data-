using System;
using System.Collections.Generic;
using Xamarin.Forms;
using WarehouseHandheld.Elements.MenuList;
using System.Collections;
using System.Diagnostics;

namespace WarehouseHandheld.Elements.Fab
{
    public partial class FabMenu : ContentView
    {
        public FabMenu()
        {
            InitializeComponent();
        }

        public Action<FabMenuModel> MenuItemSelected;

        void OnViewClicked(object sender, EventArgs e)
        {
            var item = (FabMenuModel)(sender as WarehouseHandheld.Elements.ButtonRound.ButtonRound).Source;
            MenuItemSelected?.Invoke(item);
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(FabMenu),
        null,
        BindingMode.TwoWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            ((FabMenu)bindable).ItemsSourceChanged(bindable, (IList)oldValue, (IList)newValue);
        }
    );

        private void ItemsSourceChanged(BindableObject bindable, IList oldValue, IList newValue)
        {
            if (ItemsSource == null || newValue == null)
                return;
            menuList.ItemsSource = newValue;
        }

        public IList ItemsSource
        {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
    }
}

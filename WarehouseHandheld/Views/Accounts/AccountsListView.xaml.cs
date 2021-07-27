using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using System.Diagnostics;
using WarehouseHandheld.ViewModels.Accounts;
using System.Collections;

namespace WarehouseHandheld.Views.Accounts
{
    public partial class AccountsListView : ContentView
    {
        public AccountsListView()
        {
            InitializeComponent();
        }

        public static BindableProperty ItemSelectedCommandProperty = BindableProperty.Create(
            propertyName: nameof(ItemSelectedCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(AccountsListView),
            defaultValue: null
            );

        public ICommand ItemSelectedCommand
        {
            get { return (ICommand)GetValue(ItemSelectedCommandProperty); }
            set { SetValue(ItemSelectedCommandProperty, value); }
        }

        public static BindableProperty ItemsSourceProperty = BindableProperty.Create(
            propertyName: nameof(ItemsSource),
            returnType: typeof(IList),
            declaringType: typeof(AccountsListView),
            defaultValue: null
        );

        public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                if(ItemSelectedCommand!=null && ItemSelectedCommand.CanExecute(null))
                    ItemSelectedCommand.Execute(e.SelectedItem);
                ItemSelected?.Invoke(sender, e);
            }
            ((ListView)sender).SelectedItem = null;
        }
    }
}

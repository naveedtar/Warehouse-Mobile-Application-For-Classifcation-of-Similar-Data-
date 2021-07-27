using System;
using System.Collections.Generic;
using WarehouseHandheld.Helpers;
using Xamarin.Forms;
using WarehouseHandheld.ViewModels;
using XLabs.Forms.Mvvm;

namespace WarehouseHandheld.Views.Base.BaseContentPage
{
    public partial class BasePage : ContentPage
    {
        public BasePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DependencyService.Get<ICrashLogHelper>().CrashLogs("Page Name : " + this.GetType().Name);
        }

    }
}

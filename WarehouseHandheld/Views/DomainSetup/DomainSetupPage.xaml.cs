using System;
using System.Collections.Generic;
using WarehouseHandheld.Extensions;
using Xamarin.Forms;
using WarehouseHandheld.Views.Base.BaseContentPage;

namespace WarehouseHandheld.Views.DomainSetup
{
    public partial class DomainSetupPage : BasePage
    {
        public DomainSetupPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}

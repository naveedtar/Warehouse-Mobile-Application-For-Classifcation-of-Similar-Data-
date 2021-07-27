using WarehouseHandheld.Models.OrderProcesses;
using WarehouseHandheld.Resources;
using WarehouseHandheld.ViewModels.Pallets;
using WarehouseHandheld.Views.Base.BaseContentPage;

namespace WarehouseHandheld.Views.Pallets
{
    public partial class OrderPalletsPage : BasePage
    {
        public PalletsViewModel ViewModel => BindingContext as PalletsViewModel;
        public OrderPalletsPage(OrderProcessSync orderAccount)
        {
            InitializeComponent();
            if (orderAccount != null)
            {
                ViewModel.OrderProcessId = orderAccount.OrderProcessID;
                ViewModel.OrderProcess = orderAccount;
            }

        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            Constants.SetGridProperties(grid);
            ViewModel.Initialize();
        }


        //async void Handle_Add_Clicked(object sender, System.EventArgs e)
        //{
        //    AccountsPopup account = new AccountsPopup();
        //    account.OnAccountSelected += OnAccountSelected;
        //    await PopupNavigation.PushAsync(account);
        //}

        //private async void OnAccountSelected(AccountSync account)
        //{
        //    var AddPalletPopup = new AddPalletPopup(account);
        //    AddPalletPopup.AddNewPallet += (obj) => {
        //        ViewModel.Pallets.Insert(0, obj);
        //    };
        //    await PopupNavigation.PushAsync(AddPalletPopup);
        //}



        //void SearchPallets(object sender, Xamarin.Forms.TextChangedEventArgs e)
        //{
        //    var searchText = Search.Text;
        //    if (!string.IsNullOrEmpty(searchText))
        //    {
        //        var pallets = new List<PalletSync>(ViewModel.Pallets);
        //        ViewModel.Pallets.Clear();
        //        var palletsByAccountName = pallets.Where(c => c.RecipientAccount.CompanyName.ToLower().Contains(searchText.ToLower()));
        //        if (palletsByAccountName != null)
        //        {
        //            foreach (var pallet in palletsByAccountName)
        //            {
        //                ViewModel.Pallets.Add(pallet);
        //            }
        //        }
        //        if (palletsByAccountName == null)
        //            ViewModel.Pallets.Clear();

        //    }
        //    else
        //    {
        //        ViewModel.Initialize(ViewModel.OrderId);
        //    }
        //}

    }
}

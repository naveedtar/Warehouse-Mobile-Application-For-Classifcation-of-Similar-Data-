using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WarehouseHandheld.Helpers;
using WarehouseHandheld.Models.OrderProcesses;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Views.Pallets;
using Xamarin.Forms;
using static WarehouseHandheld.Models.Orders.OrdersSync;

namespace WarehouseHandheld.ViewModels.Pallets
{
    public class PalletsOrderViewModel : ContentPage
    {
        private ObservableCollection<OrderProcessSync> saleOrders;
        public ObservableCollection<OrderProcessSync> SaleOrders
        {
            get { return saleOrders; }
            set
            {
                saleOrders = value;
                OnPropertyChanged();
            }
        }

        protected CommandLockerHelper SelectedCommandLocker => new CommandLockerHelper(async (e) => { await OnItemSelected(e); });
        public ICommand ItemSelectedCommand => new Command(SelectedCommandLocker.Execute);

        private async Task OnItemSelected(object e)
        {
            if (((SelectedItemChangedEventArgs)e).SelectedItem != null)
            {
                var orderAccount = ((OrderProcessSync)((SelectedItemChangedEventArgs)e).SelectedItem);
                await App.Current.MainPage.Navigation.PushAsync(new OrderPalletsPage(orderAccount));
               
            }
        }


        public PalletsOrderViewModel()
        {
           
        }


        public async Task Initialize()
        {
            IsBusy = true;
            try
            {
                var saleorders = await App.OrderProcesses.GetCompletedSaleOrdersProcess((int)InventoryTransactionTypeEnum.SaleOrder);
                if (saleorders != null)
                {
                    SaleOrders = new ObservableCollection<OrderProcessSync>(saleorders);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}


using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Helpers;
using WarehouseHandheld.Models.StockTakes;
using WarehouseHandheld.Views.StockTake;
using Xamarin.Forms;

namespace WarehouseHandheld.ViewModels.StockTake
{
    public class StockTakeViewModel : BaseViewModel
    {
        private ObservableCollection<StockTakeSync> stockTakes;
        public ObservableCollection<StockTakeSync> StockTakes
        {
            get { return stockTakes; }
            set
            {
                stockTakes = value;
                OnPropertyChanged();
            }
        }

        public StockTakeViewModel()
        {
        }

        protected CommandLockerHelper SelectedCommandLocker => new CommandLockerHelper(async (e) => { await OnItemSelected(e); });
        public ICommand ItemSelectedCommand => new Command(SelectedCommandLocker.Execute);

        private async Task OnItemSelected(object e)
        {
            if (((SelectedItemChangedEventArgs)e).SelectedItem != null)

            {
                var stockTake = ((SelectedItemChangedEventArgs)e).SelectedItem as StockTakeSync;
                if (stockTake.EndDate == DateTime.MinValue)
                {
                    await App.Current.MainPage.Navigation.PushAsync(new ScanStockTakeProducts(stockTake));
                }else{
                    "This is not a running StockTake.".ToToast();
                }
            }
        }

        public async void Initialize()
        {
            StockTakes = new ObservableCollection<StockTakeSync>((await App.StockTakes.GetStockTakes()).FindAll((x)=>x.EndDate==DateTime.MinValue));
        }


    }
}

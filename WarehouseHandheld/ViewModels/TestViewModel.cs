using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WarehouseHandheld.Elements.Fab;
using Xamarin.Forms;

namespace WarehouseHandheld.ViewModels
{
    public class TestViewModel : BaseViewModel
    {
        private ObservableCollection<FabMenuModel> fabList;
        public ObservableCollection<FabMenuModel> FabList
        {
            get { return fabList; }
            set
            {
                fabList = value;
                OnPropertyChanged();
            }
        }

        public ICommand ToggleViewMenuCommand => new Command(ToggleViewMenu);


        private void ToggleViewMenu()
        {
            if (IsBusy)
                return;

            IsViewMenuShown = !IsViewMenuShown;
        }


        private bool _isViewMenuShown = false;
        public bool IsViewMenuShown
        {
            get => _isViewMenuShown;
            set
            {
                _isViewMenuShown = value;
                OnPropertyChanged();
            }
        }
        public TestViewModel()
        {
            FabList = new ObservableCollection<FabMenuModel>();
            FabList.Add(new FabMenuModel() { Name = "First Item" });
            FabList.Add(new FabMenuModel() { Name = "Second Item" });
            FabList.Add(new FabMenuModel() { Name = "Third Item" });
        }
    }
}

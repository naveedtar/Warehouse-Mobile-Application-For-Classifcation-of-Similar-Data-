using System;
namespace WarehouseHandheld.ViewModels.Sync
{
    public class SyncModel : BaseViewModel
    {
        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        private bool isSyncing;
        public bool IsSyncing
        {
            get { return isSyncing; }
            set
            {
                isSyncing = value;
                OnPropertyChanged();
            }
        }
    }
}

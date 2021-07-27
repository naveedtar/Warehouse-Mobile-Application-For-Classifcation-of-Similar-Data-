using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WarehouseHandheld.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public BaseViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var ev = PropertyChanged;
            if (ev != null)
            {
                ev(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event EventHandler IsBusyChanged;

        protected virtual void OnIsBusyChanged()
        {
            var ev = IsBusyChanged;
            if (ev != null)
            {
                ev(this, EventArgs.Empty);
            }
        }

        readonly object _lockerIsLoading = new object();
        bool _isLoading = false;
        public bool IsLoading
        {
            get
            {
                lock (_lockerIsLoading)
                    return _isLoading;
            }
            set
            {
                lock (_lockerIsLoading)
                    if (_isLoading != value)
                    {
                        _isLoading = value;
                        OnPropertyChanged();
                    }
            }
        }

        readonly object _lockerIsBusy = new object();
        bool _isBusy = false;
        public bool IsBusy
        {
            get
            {
                lock (_lockerIsBusy)
                    return _isBusy;
            }
            set
            {
                lock (_lockerIsBusy)
                    if (_isBusy != value)
                    {
                        _isBusy = value;
                        OnPropertyChanged();
                        OnIsBusyChanged();
                    }
            }

        }

        public bool IsNotBusy
        {
            get
            {
                return !_isBusy;
            }
        }

        public virtual void Initialize(object navigationData = null)
        {
        }

        public virtual Task InitializeAsync(object navigationData = null)
        {
            return Task.FromResult(false);
        }

        public Action CloseCallback;
    }
}

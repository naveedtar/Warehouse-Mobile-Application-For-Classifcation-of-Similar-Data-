using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace WarehouseHandheld.Helpers
{
    public class CommandLockerHelper : Command
    {
        static private readonly EventWaitHandle _lockerEventHandle = new EventWaitHandle(true, EventResetMode.ManualReset);
        public int LockTimeout { get; set; } = 2000;

        public CommandLockerHelper(Action<object> execute) : base(execute)
        {
        }

        public CommandLockerHelper(Action execute) : base(execute)
        {
        }

        public CommandLockerHelper(Action<object> execute, Func<object, bool> canExecute) : base(execute, canExecute)
        {
        }

        public CommandLockerHelper(Action execute, Func<bool> canExecute) : base(execute, canExecute)
        {
        }

        public new async void Execute(object parameter)
        {
            if (!_lockerEventHandle.WaitOne(1))
            {
                return;
            }

            try
            {
                _lockerEventHandle.Reset();

                base.Execute(parameter);
                await Task.Delay(LockTimeout);
            }
            finally
            {
                _lockerEventHandle.Set();
            }
        }
    }
}

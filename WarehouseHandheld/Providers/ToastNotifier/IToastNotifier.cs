using System;
using System.Threading.Tasks;

namespace WarehouseHandheld.Providers.ToastNotifier
{
    public interface IToastNotifier
    {
        Task<bool> Notify(string title, string description, TimeSpan duration, object context = null, bool showOnTop = true);

        void HideAll();
    }

}

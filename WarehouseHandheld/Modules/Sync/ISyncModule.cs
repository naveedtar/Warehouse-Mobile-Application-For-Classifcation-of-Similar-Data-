using System;
using System.Threading.Tasks;

namespace WarehouseHandheld.Modules.Sync
{
    public interface ISyncModule
    {
        Task SyncAllModules();
    }
}

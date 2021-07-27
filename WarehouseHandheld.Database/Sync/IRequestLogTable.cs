using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Sync;

namespace WarehouseHandheld.Database.Sync
{
    public interface IRequestLogTable
    {
        Task AddRequestLogItem(RequestLog log);
        Task<bool> UpdateRequestLogItem(RequestLog log);
    }
}

using System;
using System.Threading.Tasks;

namespace WarehouseHandheld.Services.ServerPing
{
    public interface IServerPingService
    {
        Task<ServerStatusEnum> CheckPortConnection(string serialNo);
    }
}

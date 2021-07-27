using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Wastages;

namespace WarehouseHandheld.Services.Wastages
{
    public interface IWastagesService
    {
        Task<WastedGoodsReturnResponse> PostWastedGoodsReturn(WastedGoodsReturnRequestSync request);
        bool HandleStatusConflict();
    }
}

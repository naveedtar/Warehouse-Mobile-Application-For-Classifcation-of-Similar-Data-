using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Returns;

namespace WarehouseHandheld.Services.Returns
{
    public interface IReturnsService
    {
        Task<GoodsReturnResponse> PostGoodsReturn(GoodsReturnRequestSync request);
        bool HandleStatusConflict();
    }
}

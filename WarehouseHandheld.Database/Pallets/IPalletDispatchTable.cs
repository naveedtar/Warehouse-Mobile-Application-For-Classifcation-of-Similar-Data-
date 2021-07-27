using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Pallets;
namespace WarehouseHandheld.Database.Pallets
{
    public interface IPalletDispatchTable
    {
        Task AddUpdatePalletDispatch(PalletDispatchSync palletDispatch);
        Task AddUpdatePalletDispatch(IList<PalletDispatchSync> palletDispatch);
        Task<List<PalletDispatchSync>> GetAllPalletDispatches();
        Task<PalletDispatchSync> GetPalletDispatchById(int id);
        Task<bool> AddUpdatePalletDispatchProgress(PalletDispatchProgress palletDispatchProgress);
        Task<PalletDispatchProgress> GetPalletDispatchProgressById(int id);
        Task<List<PalletDispatchProgress>> GetAllPalletDispatchProgress();

    }
}

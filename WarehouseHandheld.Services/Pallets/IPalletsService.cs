using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Pallets;
namespace WarehouseHandheld.Services.Pallets
{
    public interface IPalletsService
    {
        Task<PalletsSyncCollection> GetPalletsAsync(DateTime dateUpdated, string serialNo);
        Task<PalletDispatchMethodSyncCollection> GetPalletDispatchMethodsAsync(DateTime dateUpdated, string serialNo);
        Task<PalletProductsSyncCollection> GetPalletProductsAsync(DateTime dateUpdated, string serialNo);
        Task<PalletTrackingSyncCollection> GetPalletTrackingAsync(DateTime dateUpdated, string serialNo);
        Task<PalletTrackingSyncCollection> PostPalletTrackingAsync(PalletTrackingSyncCollection palletTracking);
        Task<PalletSync> UpdatePalletStatus(PalletSync palletSync);
        Task<PalletSync> UpdatePalletImage(PalletSync palletSync);
        Task<PalletSync> DispatchPallet(PalletSync palletSync, string serialNo, int palletId);
        Task<PalletProductsSyncCollection> AddPalletItem(PalletProductsSyncCollection palletSync, string serialNo);
        Task<PalletCreateResponse> AddPallet(int accountID);

        Task<PalletsDispatchSyncCollection> GetPalletsDispatchesAsync(DateTime dateUpdated, string serialNo);
        Task<PalletDispatchProgress> PostPalletsDispatchesAsync(PalletDispatchProgress palletDispatch);


        bool HandleConflictStatusPalletTracking();
        bool HandleConflictStatusDispatchPallet();
        bool HandleConflictStatusPostDispatchPallet();


    }
}

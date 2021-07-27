using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Models.Vehicles;

namespace WarehouseHandheld.Database.Pallets
{
    public interface IPalletsTable
    {
        Task AddUpdatePallets(IList<PalletSync> pallets);
        Task AddUpdatePalletTracking(IList<PalletTrackingSync> pallets);
        Task AddUpdatePalletTracking(PalletTrackingSync pallet);
        Task AddUpdatePalletProducts(IList<PalletProductsSync> palletProducts);
        Task AddUpdatePalletDispatchMethods(IList<PalletDispatchMethodSync> palletMethods);
        Task<List<PalletSync>> GetAllPallets();
        Task<List<PalletSync>> GetAllPalletsByOrderId(int id);
        Task<List<PalletDispatchMethodSync>> GetAllPalletDispatchMethods();
        Task<List<PalletProductsSync>> GetAllPalletProducts();
        Task<List<PalletTrackingSync>> GetAllPalletTrackings();
        Task<PalletTrackingSync> GetPalletTrackingBySerial(string serial, int productId);
        Task<PalletTrackingSync> GetAllPalletTrackingsWithTrackingId(int trackingId);
        Task<PalletTrackingSync> GetAllPalletTrackingsWithSerial(string serial);
        Task<PalleTrackingProcess> GetPalletTrackingProcessBySerial(string serial);

        Task<PalletTrackingSync> GetPalletTrackingForGoodsOut(int productId, List<PalleTrackingProcess> PalletTrackingProcesses, List<PalleTrackingProcess> PalletTrackingProcessesForAll, List<PalletTrackingSync> SkippedPallets, PalletTrackingSchemeEnum scheme);
        Task<PalletTrackingSync> GetPalletTrackingForGoodsOutWithSerial(int productId, List<PalleTrackingProcess> PalletTrackingProcesses, List<PalleTrackingProcess> PalletTrackingProcessesForAll, List<PalletTrackingSync> SkippedPallets, PalletTrackingSchemeEnum trackingScheme, string serial);


        Task<List<PalletSync>> GetDispatchedPalletsSyncByDispatchedId(int id);
        Task<List<PalletProductsSync>> GetAllPalletProductsByPalletId(int palletId);
        Task DeletePalletTrackingsWithArchivedStatus();

    }
}

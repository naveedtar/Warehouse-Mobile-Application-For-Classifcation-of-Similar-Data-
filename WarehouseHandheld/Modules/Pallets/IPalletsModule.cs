using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.OrderProcesses;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Models.Vehicles;

namespace WarehouseHandheld.Modules.Pallets
{
    public interface IPalletsModule
    {
        Task SyncPallets();
        Task SyncPalletMethods();
        Task SyncPalletProducts();
        Task SyncPalletTracking(bool isSyncModule = true);
        Task SyncPalletDispatches();

        Task<List<PalletSync>> GetPallets();
        Task<List<PalletSync>> GetPalletsByOrderId(int id);
        Task<List<PalletProductsSync>> GetPalletProducts();
        Task<List<PalletProducts>> GetPalletProductsWithPalletId(int PalletId, DateTime PalletCreatedDate);
        Task<List<PalletDispatchMethodSync>> GetPalletDispatchMethods();
        Task<PalletCreateResponse> AddPallet(int accountID);
        Task AddPalletInDB(PalletSync pallet);
        Task<PalletProductsSyncCollection> AddPalletItem(PalletSync pallet, OrderDetailsProduct orderProduct, double qty,OrderProcessSync orderProcessDetail);
        Task<List<PalletSync>> DispatchPallet(List<PalletSync> pallets);
        Task<List<PalletTrackingSync>> GetPalletTracking();
        Task<PalletTrackingSync> GetPalletTrackingWithTrackingId(int trackingId);
        Task<PalletTrackingSync> GetPalletTrackingWithSerial(string serial);
        Task<PalletTrackingSync> GetPalletTrackingBySerial(string palletSerial, int productId);
        Task AddPalletTrackingToDatabase(PalletTrackingSyncCollection pallets);

        Task<PalletTrackingSync> GetPalletTrackingForGoodsOut(int productId, List<PalleTrackingProcess> PalletTrackingProcesses,List<PalleTrackingProcess> palleTrackingProcessesForSameProduct, List<PalletTrackingSync> SkippedPallets, PalletTrackingSchemeEnum scheme);
        Task<PalletTrackingSync> GetPalletTrackingForGoodsOutWithSerial(int productId, List<PalleTrackingProcess> PalletTrackingProcesses, List<PalleTrackingProcess> PalletTrackingProcessesForAll, List<PalletTrackingSync> SkippedPallets, PalletTrackingSchemeEnum trackingScheme, string serial);


        Task<List<PalletDispatchSync>> GetAllPalletDispatches();
        Task<PalletDispatchProgress> UpdatePalletDispatchProgress(PalletDispatchProgress palletDispatchProgress);

        Task<List<PalletProductsSync>> GetPalletProductsByPalletId(int palletID);
    }
}

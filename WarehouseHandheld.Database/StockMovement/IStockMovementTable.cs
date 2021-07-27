using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.StockMovement;

namespace WarehouseHandheld.Database.StockMovement
{
    public interface IStockMovementTable
    {
        Task<StockMovementViewModel> GetStockMovementViewModelById(int id);

        Task<List<StockMovementViewModel>> GetStockMovements();

        Task AddUpdateStockMovement(StockMovementViewModel stockMovementViewModel);

        Task<List<StockMovementPalletSerialsViewModel>> GetSerialPalletsByProductId(int productId);

        Task AddPalletSerial(List<StockMovementPalletSerialsViewModel> serialsPallet);

        Task AddUpdateStockLocations(List<LocationSync> stockLocations);

        Task<LocationSync> GetStockLocationByLocationId(int locationId);

        Task<LocationSync> GetStockLocationByLocationCode(string locationCode);

        Task DeleteStockMovement();
        Task AddUpdateStockMovementList(List<StockMovementViewModel> stockMovementViewModels);
    }
}

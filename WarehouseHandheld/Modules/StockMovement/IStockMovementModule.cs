using System.Threading.Tasks;

namespace WarehouseHandheld.Modules.StockMovement
{
    public interface IStockMovementModule
    {
        Task SyncLocations();
        Task SyncStockMovement();
        Task SyncProductLocationStock();
    }
}

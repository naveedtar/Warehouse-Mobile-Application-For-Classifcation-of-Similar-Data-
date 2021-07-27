using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Models.Products;

namespace WarehouseHandheld.ViewModels.StockTake
{
    public class StockTakeScanPalletPopupViewModel
    {
        public ProductMasterSync Product;
        public Action<PalleTrackingProcess> palletNotFound;
        public Action<PalleTrackingProcess> palletScanned;
        public StockTakeScanPalletPopupViewModel(ProductMasterSync product)
        {
            this.Product = product;
        }
        public async Task Scan(string serial)
        {
            var pallet = await App.Pallets.GetPalletTrackingBySerial(serial, Product.ProductId);
            if (pallet == null)
            {
                var palletProcess = new PalleTrackingProcess() { palletSerial = serial, isExistingPallet = false };
                palletNotFound?.Invoke(palletProcess);
            }
            else
            {
                var palletProcess = new PalleTrackingProcess(){PalletTrackingId = pallet.PalletTrackingId, palletSerial = pallet.PalletSerial, ProcessedQuantity = pallet.RemainingCases, isExistingPallet = true};
                palletScanned?.Invoke(palletProcess);

            }
        }

    }
}

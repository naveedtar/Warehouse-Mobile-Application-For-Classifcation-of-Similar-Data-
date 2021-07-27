using System;
using WarehouseHandheld.Models.Products;

namespace WarehouseHandheld.Models.Pallets
{
    public class PalletProductsDetail
    {
        public PalletProductsSync PalletProduct { get; set; }
        public ProductMasterSync Product { get; set; }
    }
}

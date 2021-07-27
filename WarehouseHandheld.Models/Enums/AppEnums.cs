using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseHandheld.Models.Enums
{
    public enum PrintMediaTypeEnum
    {
        Continues = 1,
        Gap = 2
    }

    public enum PrintFontTypeEnum //If new font added here, it should also be added in GetDownloadFontId method in PrinterHelper class.
    {
        Internal,
        Arial,
        Verdana,
        CourierNew
    }

    public enum DeviceTypeEnum
    {
        GodexMX30
    }
    public enum PalletDispatchStatusEnum
    {
        Created = 1,
        Loaded = 2,
        Delivered = 3,
        Scheduled = 4
    }

    public static class DbConstants
    {
        public static int TakeLimitValue = 200;
        public static DateTime StartDate = DateTime.UtcNow.AddDays(-7);

    }

    public enum ProductKitTypeEnum
    {
        Kit = 1,
        Grouped = 2,
        Recipe = 3,
        Simple = 4,
        ProductByAttribute = 5,
        RelatedProduct = 6
    }
}

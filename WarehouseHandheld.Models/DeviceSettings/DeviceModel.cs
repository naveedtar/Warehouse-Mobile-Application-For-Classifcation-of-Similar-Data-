using System;
using SQLite;
using WarehouseHandheld.Models.Enums;

namespace WarehouseHandheld.Models.DeviceSettings
{
    public class DeviceModel
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public PrintFontTypeEnum Font { get; set; }
        public PrintMediaTypeEnum MediaType { get; set; }
        public string TextHeight { get; set; }
        public string TextWidth { get; set; }
        public string HeaderHeight { get; set; }
        public string HeaderWidth { get; set; }
        public string Notes { get; set; }
        public string PaperWidth { get; set; }
        public string PaperHeight { get; set; }
        public string PaperGap { get; set; }


    }
}

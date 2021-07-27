using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Godex;
using WarehouseHandheld.Models.Enums;

namespace WarehouseHandheld.Models.DeviceSettings
{
    public class PrintLine
    {
        public string Text { get; set; }
        public string TextRight { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int PosXRight { get; set; }
        public bool ShouldAlignRight { get; set; }
        public byte[] Image { get; set; }
        public PrintTextType TextType { get; set; }
        public Godex.BarCodeType BarcodeType { get; set; }
        public PrintFontTypeEnum Font { get; set; }
        public Godex.Readable BarcodeReadable { get; set; }
    }

    public enum PrintTextType
    {
        TextInternal = 1,
        TextTrueType = 2,
        OneDBarcode = 3,
        TwoDBarcode = 4,
        Line = 5,
        Image = 6
    }
}

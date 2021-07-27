using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Com.Godex;
using WarehouseHandheld.Models.DeviceSettings;
using WarehouseHandheld.Models.Enums;
using WarehouseHandheld.Resources;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace WarehouseHandheld.Helpers
{
    public class PrintLineHelper
    {
        public static async Task<List<PrintLine>> GetPrintListHeader(Ref<int> PosY)
        {
            var printerHelper = DependencyService.Get<IPrinterHelper>();
            await GetLatestSettings();

            var terminal = await App.Database.Vehicle.GetTerminalMetaData();
            List<PrintLine> printLineList = new List<PrintLine>();
            if (ConnectedDevice == null)
            {
                ConnectedDevice = await printerHelper.ConnectPairedDevice();
            }
            if (ConnectedDevice != null)
            {
                PosY.Value = 10;

                int TxtHeight = printerHelper.GetTextHeight(ConnectedDevice.TextHeight);
                int TxtWidth = printerHelper.GetTextWidth(ConnectedDevice.TextWidth);
                int HeaderTxtHeight = printerHelper.GetTextHeight(ConnectedDevice.HeaderHeight);
                int HeaderTxtWidth = printerHelper.GetTextWidth(ConnectedDevice.HeaderWidth);
                if (terminal.PrintLogoForReceipts && !string.IsNullOrEmpty(terminal.LogoPath))
                {
                    try
                    {
                        var bytes = DependencyService.Get<IImageHelper>().GetImageBytes(terminal.LogoPath);
                        printLineList.Add(await GetLine("", PosY.Value, 0, PrintTextType.Image, false, bytes));
                        PosY.Value += 100;
                    }
                    catch { }
                }
                if (!string.IsNullOrEmpty(terminal.TenantReceiptPrintHeaderLine1))
                {
                    printLineList.Add(await GetLine(terminal.TenantReceiptPrintHeaderLine1, PosY.Value));
                    PosY.Value += printerHelper.HeightForPosY(TxtHeight);
                }
                if (!string.IsNullOrEmpty(terminal.TenantReceiptPrintHeaderLine2))
                {
                    printLineList.Add(await GetLine(terminal.TenantReceiptPrintHeaderLine2, PosY.Value));
                    PosY.Value += printerHelper.HeightForPosY(TxtHeight);
                }
                if (!string.IsNullOrEmpty(terminal.TenantReceiptPrintHeaderLine3))
                {
                    printLineList.Add(await GetLine(terminal.TenantReceiptPrintHeaderLine3, PosY.Value));
                    PosY.Value += printerHelper.HeightForPosY(TxtHeight);
                }
                if (!string.IsNullOrEmpty(terminal.TenantReceiptPrintHeaderLine4))
                {
                    printLineList.Add(await GetLine(terminal.TenantReceiptPrintHeaderLine4, PosY.Value));
                    PosY.Value += printerHelper.HeightForPosY(TxtHeight);
                }

            }
            return printLineList;
        }

        static DeviceModel ConnectedDevice;

        public static async Task<int> GetConnectedDeviceTxtHeight(Ref<int> TxtHeaderHeight)
        {
            var printerHelper = DependencyService.Get<IPrinterHelper>();
            await GetLatestSettings();

            List<PrintLine> printLineList = new List<PrintLine>();
            if (ConnectedDevice == null)
            {
                ConnectedDevice = await printerHelper.ConnectPairedDevice();
            }
            if (ConnectedDevice != null)
            {
                TxtHeaderHeight.Value = printerHelper.GetTextHeight(ConnectedDevice.HeaderHeight);
                return printerHelper.GetTextHeight(ConnectedDevice.TextHeight);

            }
            return 0;
        }

        public static async Task<PrintLine> GetLine(string text, int PosY, int posXRight = 320, PrintTextType printType = PrintTextType.TextInternal, bool IsHeader = false, byte[] Image = null, bool barcodeReadable = false)
        {
            var printerHelper = DependencyService.Get<IPrinterHelper>();
            await GetLatestSettings();

            if (ConnectedDevice == null)
            {
                ConnectedDevice = await printerHelper.ConnectPairedDevice();
            }
            if (ConnectedDevice != null)
            {
                int TxtHeight = printerHelper.GetTextHeight(ConnectedDevice.TextHeight);
                int TxtWidth = printerHelper.GetTextWidth(ConnectedDevice.TextWidth);
                int HeaderTxtHeight = printerHelper.GetTextHeight(ConnectedDevice.HeaderHeight);
                int HeaderTxtWidth = printerHelper.GetTextWidth(ConnectedDevice.HeaderWidth);
                if (printType == PrintTextType.TextTrueType || printType == PrintTextType.TextInternal)
                {
                    if (ConnectedDevice.Font == PrintFontTypeEnum.Internal)
                    {
                        printType = PrintTextType.TextInternal;
                    }
                    else
                    {
                        printType = PrintTextType.TextTrueType;
                    }
                }

                var printFont = ConnectedDevice.Font;
                var width = TxtWidth;
                var height = TxtHeight;
                if (IsHeader)
                {
                    width = HeaderTxtWidth;
                    height = HeaderTxtHeight;
                }
                if (printType == PrintTextType.OneDBarcode)
                {
                    return new PrintLine() { Font = printFont, Text = text, TextType = PrintTextType.OneDBarcode, height = Constants.BarcodeHeight, width = Constants.BarcodeWidth, PosX = Constants.PosX, PosY = PosY, BarcodeType = Com.Godex.Godex.BarCodeType.Code128Auto, BarcodeReadable = barcodeReadable ? Godex.Readable.BottomLeft : Godex.Readable.None };

                }
                else if (printType == PrintTextType.Image)
                {
                    return new PrintLine() { Image = Image, Font = printFont, TextType = PrintTextType.Image, height = height, width = 6, PosX = 180, PosY = PosY };

                }
                else
                {
                    var lines = text?.Split(Constants.PrintLineSeparator, StringSplitOptions.None);
                    if (lines != null && lines.Length == 2)
                    {
                        return new PrintLine() { Font = printFont, Text = lines[0], TextType = printType, height = height, width = width, PosX = Constants.PosX, PosY = PosY, PosXRight = posXRight, TextRight = lines[1], ShouldAlignRight = true };
                    }
                    else
                    {
                        return new PrintLine() { Font = printFont, Text = text, TextType = printType, height = height, width = width, PosX = Constants.PosX, PosY = PosY };
                    }
                }
            }

            return null;

        }

        public static async Task<PrintLine> GetSeparatorLine(int posY, PrintTextType printType = PrintTextType.TextInternal)
        {
            var printerHelper = DependencyService.Get<IPrinterHelper>();
            await GetLatestSettings();
            if (ConnectedDevice == null)
            {
                ConnectedDevice = await printerHelper.ConnectPairedDevice();
            }
            if (ConnectedDevice != null)
            {
                int TxtHeight = printerHelper.GetTextHeight(ConnectedDevice.TextHeight);
                int TxtWidth = printerHelper.GetTextWidth(ConnectedDevice.TextWidth);

                if (printType == PrintTextType.TextTrueType || printType == PrintTextType.TextInternal)
                {
                    if (ConnectedDevice.Font == PrintFontTypeEnum.Internal)
                    {
                        printType = PrintTextType.TextInternal;
                    }
                    else
                    {
                        printType = PrintTextType.TextTrueType;
                    }
                }

                var printFont = ConnectedDevice.Font;
                var width = TxtWidth;
                var height = TxtHeight;

                var seperatorWidht = Convert.ToInt32(ConnectedDevice.TextWidth);
                string separatorString = GetSeparatorString('-', seperatorWidht);

                return new PrintLine() { Font = printFont, Text = separatorString, TextType = printType, height = height, width = width, PosX = Constants.PosX, PosY = posY };

            }

            return null;

        }


        

        private static string GetSeparatorString(char character, int width)
        {
            string res = "";

            int charWidth = Convert.ToInt32(width * 1.3);

            int numberOfCharacter = 600 / charWidth;

            for (int x = 1; x <= numberOfCharacter; x++)
            {
                res += character.ToString();
            }

            return res;
        }

        public static string ArrayJoiner(string[] stringArray)
        {
            string res = string.Join(",", stringArray);
            return res;
        }

        public static async Task GetLatestSettings()
        {
            if (ConnectedDevice != null)
            {
                var devicesInDb = await App.Database.DeviceSettings.GetAllDevices();
                var selectedDevice = devicesInDb.Find(x => x.DeviceName.Equals(ConnectedDevice.DeviceName));
                ConnectedDevice = selectedDevice;
            }
        }

    }

    public class Ref<T>
    {
        public Ref() { }
        public Ref(T value) { Value = value; }
        public T Value { get; set; }
        public override string ToString()
        {
            T value = Value;
            return value == null ? "" : value.ToString();
        }
        public static implicit operator T(Ref<T> r) { return r.Value; }
        public static implicit operator Ref<T>(T value) { return new Ref<T>(value); }
    }
}


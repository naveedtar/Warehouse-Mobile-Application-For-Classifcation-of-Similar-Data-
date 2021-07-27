using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Graphics;
using Com.Godex;
using Xamarin.Forms;
using WarehouseHandheld.Models.DeviceSettings;
using WarehouseHandheld.Models.Enums;
using WarehouseHandheld.Helpers.Droid;

[assembly: Dependency(typeof(PrinterHelper))]
namespace WarehouseHandheld.Helpers.Droid
{
    public class PrinterHelper : IPrinterHelper
    {
        public static DeviceModel ConnectedDevice;

        public bool Connect(string address, int port)
        {
            try
            {
                string res = Godex.CheckStatus();
                if (res != null)
                {
                    Disconnect();
                }

                bool status = Godex.Openport(address, port);
                return status;

            }
            catch (Exception e)
            {
                string exception = e.ToString();
                return false;
            }
        }

        public async Task<List<BluetoothDeviceModel>> GetPairedDevices()
        {
            BluetoothAdapter a = BluetoothAdapter.DefaultAdapter;
            a.Enable();
            await Task.Delay(200);
            var devices = a.BondedDevices.ToList();
            List<BluetoothDeviceModel> bluetoothDevices = new List<BluetoothDeviceModel>();
            foreach (var device in devices)
            {
                BluetoothDeviceModel bluetoothDevice = new BluetoothDeviceModel();
                bluetoothDevice.Name = device.Name;
                bluetoothDevice.MacAddress = device.Address;
                bluetoothDevices.Add(bluetoothDevice);
            }
            return bluetoothDevices;

        }

        public bool Disconnect()
        {
            try
            {
                bool status = Godex.Close();
                return status;
            }
            catch
            {
                return false;
            }
        }

        public async Task<DeviceModel> ConnectPairedDevice()
        {
            string res = Godex.CheckStatus();
            var AddedDevices = await App.Database.DeviceSettings.GetAllDevices();
            if (res == null || ConnectedDevice == null)
            {

                var pairedDevices = await GetPairedDevices();
                foreach (var device in AddedDevices)
                {
                    foreach (var pairedDevice in pairedDevices)
                    {
                        if (!string.IsNullOrEmpty(pairedDevice.Name) && !string.IsNullOrEmpty(device.DeviceName) && pairedDevice.Name.ToLower().Contains(device.DeviceName.ToLower()))
                        {
                            ConnectedDevice = device;
                            Connect(pairedDevice.MacAddress, 2);
                            break;
                        }
                    }
                }

            }
            else if (ConnectedDevice != null)
            {
                var deviceFound = AddedDevices.Find((x) => x.DeviceName == ConnectedDevice.DeviceName);
                if (deviceFound != null)
                {
                    ConnectedDevice = deviceFound;
                }
            }


            return ConnectedDevice;
        }

        public async Task<bool> ConnectionScheck()
        {
            string res = Godex.CheckStatus();

            if (res == null)
            {
                ConnectedDevice = await ConnectPairedDevice();
                res = Godex.CheckStatus();
            }

            if (res == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<string> Print(List<PrintLine> printLineList)
        {
            try
            {

                string res = Godex.CheckStatus();

                if (res == null)
                {
                    ConnectedDevice = await ConnectPairedDevice();
                    res = Godex.CheckStatus();
                }
                await GetLatestSettings();
                if (res == null)
                {
                    return null;
                }
                //var bitmap = DrawText(text);

                // checking printer status
                if (res == "00\r\n")
                {
                    //Godex.SendCommand("AD,30,130,1,1,0,0,Width:" + App.ScreenWidth);
                    //Godex.SendCommand("AD,30,200,1,1,0,0,Height:" + App.ScreenHeight);
                    //Godex.PutImage(10, 10, bitmap);



                    // ***** set stop position *****
                    //Godex.SendCommand("^E8"); 

                    // ***** line feed ****
                    //Godex.SendCommand("~S,FEED");


                    if (ConnectedDevice.MediaType == PrintMediaTypeEnum.Continues)
                    {
                        
                        // set length of print as per content
                        Godex.SendCommand("^XSET,REALLENGTHPRINT,1");
                        // ***** setting lebel length as 80 and height as zero for continuous printing   
                        Godex.SendCommand("^Q10,0,0");
                        string paperWidth = "^W" + ConnectedDevice.PaperWidth;
                        Godex.SendCommand(paperWidth);
                    }

                    else if (ConnectedDevice.MediaType == PrintMediaTypeEnum.Gap)
                    {
                        string paperHeightAndGap = "^Q" + ConnectedDevice.PaperHeight + ","  +ConnectedDevice.PaperGap;
                        string paperWidth = "^W" + ConnectedDevice.PaperWidth;

                        Godex.SendCommand(paperHeightAndGap);
                        Godex.SendCommand(paperWidth);
                    }

                    // settings number of prints as one
                    Godex.SendCommand("^P1");

                    Godex.SendCommand("^L");

                    //print logo from internal memory
                    //Godex.SendCommand("Y10,10,print-logo");

                    int lastPosY = 0;
                    int lastWidth = 0;
                    int lastHeight = 0;

                    foreach (var line in printLineList)
                    {
                        switch (line.TextType)
                        {
                            case PrintTextType.OneDBarcode:
                                Godex.Bar_1D(line.BarcodeType, line.PosX, line.PosY, 3, line.width, line.height, 0, line.BarcodeReadable, line.Text);
                                break;
                            case PrintTextType.TextInternal:
                                Godex.InternalFont_TextOut(GetInternalFontId(line.height), line.PosX, line.PosY, line.width, line.height, 2, "0", line.Text);
                                if (line.ShouldAlignRight)
                                    Godex.InternalFont_TextOut(GetInternalFontId(line.height), line.PosXRight, line.PosY, line.width, line.height, 2, "0", line.TextRight);
                                break;
                            case PrintTextType.TextTrueType:
                                Godex.TrueTypeFont_TextOut(GetDownloadFontId(line.Font), line.PosX, line.PosY, line.width, line.height, 2, "0", line.Text);
                                if (line.ShouldAlignRight)
                                    Godex.TrueTypeFont_TextOut(GetDownloadFontId(line.Font), line.PosXRight, line.PosY, line.width, line.height, 2, "0", line.TextRight);
                                break;
                            case PrintTextType.Image:
                                Bitmap bmp = BitmapFactory.DecodeByteArray(line.Image, 0, line.Image.Length);
                                Godex.PutImage(line.PosX, line.PosY, bmp);
                                break;
                        }

                        lastPosY = line.PosY;
                        lastWidth = line.width;
                        lastHeight = line.height;

                    }

                    if (ConnectedDevice.MediaType == PrintMediaTypeEnum.Continues)
                    {
                        Godex.InternalFont_TextOut(Godex.InternalFontID.A, 0, lastPosY, lastWidth, lastHeight, 2, "0", " ");
                    }

                    Godex.SendCommand("E");

                }

                return GetStatusName(res);

            }
            catch (Exception e)
            {
                string exception = e.ToString();
                return exception;
            }
        }
        public string GetPrinterStatus()
        {
            return Godex.CheckStatus();
        }

        Godex.InternalFontID GetInternalFontId(int TextSize)
        {
            switch (TextSize)
            {
                case 6:
                    return Godex.InternalFontID.A;
                case 8:
                    return Godex.InternalFontID.B;
                case 10:
                    return Godex.InternalFontID.C;
                case 12:
                    return Godex.InternalFontID.D;
                case 14:
                    return Godex.InternalFontID.E;
                case 18:
                    return Godex.InternalFontID.F;
                case 24:
                    return Godex.InternalFontID.G;
                case 30:
                    return Godex.InternalFontID.H;
                default:
                    return Godex.InternalFontID.A;
            }
        }

        Godex.DownlaodFontID GetDownloadFontId(PrintFontTypeEnum Font)
        {
            switch (Font)
            {
                case PrintFontTypeEnum.Arial:
                    return Godex.DownlaodFontID.A;

                case PrintFontTypeEnum.Verdana:
                    return Godex.DownlaodFontID.B;

                case PrintFontTypeEnum.CourierNew:
                    return Godex.DownlaodFontID.C;

                default:
                    return Godex.DownlaodFontID.A;
            }
        }

        private Bitmap DrawText(String text)
        {
            var paint = new Paint { Color = Android.Graphics.Color.Blue, StrokeWidth = 10f, AntiAlias = true };
            var canvasBitmap = Bitmap.CreateBitmap(App.ScreenWidth, App.ScreenHeight, Bitmap.Config.Argb8888);
            var drawCanvas = new Canvas(canvasBitmap);
            drawCanvas.DrawText(text, 0, 0, paint);

            return canvasBitmap;

        }

        public string GetStatusName(string status)
        {
            switch (status)
            {
                case "00\r\n":
                    return "00 Ready";
                case "01\r\n":
                    return "01 Media Empty or Media Jam";
                case "02\r\n":
                    return "02 Media Empty or Media Jam";
                case "03\r\n":
                    return "03 Ribbon Empty";
                case "04\r\n":
                    return "04 Printhead Up (Open)";
                case "05\r\n":
                    return "05 Rewinder Full";
                case "06\r\n":
                    return "06 File System Full";
                case "07\r\n":
                    return "07 File Name Not Found";
                case "08\r\n":
                    return "08 Duplicate Name";
                case "09\r\n":
                    return "09 Syntax Error";
                case "10\r\n":
                    return "10 Cutter JAM";
                case "11\r\n":
                    return "11 Extended Memory Not Found";
                case "20\r\n":
                    return "20 Pause";
                case "21\r\n":
                    return "21 In Setting Mode";
                case "22\r\n":
                    return "22 In Keyboard Mode";
                case "55\r\n":
                    return "55 Printer is Printing";
                case "60\r\n":
                    return "60 Data in Process";


            }

            return null;
        }

        public int HeightForPosY(int textHeight)
        {
            int res = Convert.ToInt32(Math.Floor(textHeight * 1.3));
            return res;
        }

        public int GetTextHeight(string textHeight)
        {
            int height = Convert.ToInt32(textHeight);
            int res = Convert.ToInt32(Math.Floor(height * 2.8));
            return res;
        }

        public int GetTextWidth(string textWidth)
        {
            int width = Convert.ToInt32(textWidth);
            int res = Convert.ToInt32(Math.Floor(width * 2.8));
            return res;
        }

        public async Task GetLatestSettings()
        {
            if (ConnectedDevice != null)
            {
                var devicesInDb = await App.Database.DeviceSettings.GetAllDevices();
                var selectedDevice = devicesInDb.Find(x => x.DeviceName.Equals(ConnectedDevice.DeviceName));
                ConnectedDevice = selectedDevice;
            }
        }
    }




}

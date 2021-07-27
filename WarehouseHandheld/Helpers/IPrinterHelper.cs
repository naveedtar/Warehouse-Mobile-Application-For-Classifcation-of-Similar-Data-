using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.DeviceSettings;

namespace WarehouseHandheld.Helpers
{
    public interface IPrinterHelper
    {
        bool Connect(string address, int port);
        bool Disconnect();
        Task<string> Print(List<PrintLine> textList);
        Task<List<BluetoothDeviceModel>> GetPairedDevices();
        Task<DeviceModel> ConnectPairedDevice();
        int HeightForPosY(int textHeight);
        int GetTextHeight(string textHeight);
        int GetTextWidth(string textWidth);
        Task<bool> ConnectionScheck();
        string GetPrinterStatus();
    }
}

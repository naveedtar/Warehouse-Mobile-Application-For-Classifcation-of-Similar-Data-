using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.DeviceSettings;

namespace WarehouseHandheld.Database.DeviceSettings
{
    public interface IDeviceSettingsTable
    {
        Task AddDeviceSettings(DeviceModel deviceSetting);
        Task<List<DeviceModel>> GetAllDevices();
    }
}

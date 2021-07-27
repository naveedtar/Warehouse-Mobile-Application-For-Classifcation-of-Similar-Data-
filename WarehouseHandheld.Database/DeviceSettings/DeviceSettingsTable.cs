using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Database.DatabaseHandler;
using WarehouseHandheld.Database.DeviceSettings;
using WarehouseHandheld.Models.DeviceSettings;

namespace WarehouseVanSales.Database.DeviceSettings
{
    public class DeviceSettingsTable : IDeviceSettingsTable
    {
        public LocalDatabase Handler { get; private set; }
        public DeviceSettingsTable(LocalDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("Database");
            this.Handler = database;
        }

        public async Task AddDeviceSettings(DeviceModel deviceSetting)
        {
            if (deviceSetting.Id == 0)
            {
                await Handler.Database.InsertAsync(deviceSetting);
            }
            else{
                await Handler.Database.UpdateAsync(deviceSetting);
 
            }
        }
        public async Task<List<DeviceModel>> GetAllDevices()
        {
            return await Handler.Database.Table<DeviceModel>().ToListAsync();
        }
    }
}

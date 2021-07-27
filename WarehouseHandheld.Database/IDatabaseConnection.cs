using System;
namespace WarehouseHandheld.Database
{
    public interface IDatabaseConnection
    {
        string GetDatabasePath(string dbName);
    }
}

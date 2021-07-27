using System;
using System.IO;
using Android.Widget;
using SQLite;
using WarehouseHandheld.Database;
using WarehouseHandheld.Droid.Database;
using WarehouseHandheld.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(DatabaseConnection))]
namespace WarehouseHandheld.Droid.Database
{
    public class DatabaseConnection : IDatabaseConnection, IDatabaseExportHelper
    {
        public string GetDatabasePath(string dbName)
        {
            var path = Path.Combine(System.Environment.
              GetFolderPath(System.Environment.
                            SpecialFolder.Personal), dbName);

//#if DEBUG
//            var szDatabaseBackupPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/databasename_Backup.db3";
//            if (File.Exists(path))
//            {
//                File.Copy(path, szDatabaseBackupPath, true);
//            }
//#endif
            return path;
        }

        public string ExportDb()
        {
            string path = "/data/user/0/com.ganedata.Warehouse_Handheld/files/WarehouseHandheld.db3";

            var databaseBackupPath = string.Format("{0}/{1}HandheldDb.db3", Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, DateTime.UtcNow.ToFileTimeUtc().ToString());

            if (File.Exists(path))
            {
                File.Copy(path, databaseBackupPath, true);
            }
            return databaseBackupPath;
        }
    }
}

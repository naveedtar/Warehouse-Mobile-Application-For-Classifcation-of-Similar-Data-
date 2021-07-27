using System;
using System.IO;
using Foundation;
using WarehouseHandheld.Database;
using WarehouseHandheld.iOS.Database;

[assembly: Xamarin.Forms.Dependency(typeof(DatabaseConnection))]
namespace WarehouseHandheld.iOS.Database
{
    public class DatabaseConnection : IDatabaseConnection
    {
        public string GetDatabasePath(string dbName)
        {
            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libFolder = Path.Combine(docFolder, "..", "Library", "Databases");

            if (!Directory.Exists(libFolder))
            {
                Directory.CreateDirectory(libFolder);
            }

            //if (System.IO.File.Exists(Path.Combine(libFolder, dbName)))
            //{
            //    // Do not backup to iCloud
            //    NSFileManager.SetSkipBackupAttribute(dbName, true);
            //}

            return Path.Combine(libFolder, dbName);

        }
    }
}

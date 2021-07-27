using System;
using WarehouseHandheld.Database.DatabaseHandler;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WarehouseHandheld.Database.Base
{
    public class BaseTable<T> 
    {
        public LocalDatabase Handler { get; private set; }
        public BaseTable(LocalDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("Database");
            this.Handler = database;
        }
    }
}

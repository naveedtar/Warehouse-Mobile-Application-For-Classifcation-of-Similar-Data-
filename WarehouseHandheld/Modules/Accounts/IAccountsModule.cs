using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Accounts;

namespace WarehouseHandheld.Modules.Accounts
{
    public interface IAccountsModule
    {
        Task SyncAccounts();
        Task<List<AccountSync>> GetAllAccounts();
    }
}

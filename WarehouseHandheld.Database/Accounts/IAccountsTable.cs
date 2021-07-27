using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Accounts;

namespace WarehouseHandheld.Database.Accounts
{
    public interface IAccountsTable
    {
        Task AddUpdateAccounts(IList<AccountSync> userSync);
        Task<List<AccountSync>> GetAllAccounts();
        Task<AccountSync> GetAccountById(int id);
    }
}

using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Accounts;

namespace WarehouseHandheld.Services.Accounts
{
    public interface IAccountsService
    {
        Task<AccountSyncCollection> GetAccountsAsync(DateTime dateUpdated, string serialNo);
    }
}

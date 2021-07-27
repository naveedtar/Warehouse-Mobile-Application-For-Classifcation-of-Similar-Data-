using System;
using System.Collections.Generic;
using WarehouseHandheld.Database.DatabaseHandler;
using WarehouseHandheld.Models.Accounts;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseHandheld.Database.Accounts
{
    public class AccountsTable : IAccountsTable
    {
        public LocalDatabase Handler { get; private set; }
        public AccountsTable(LocalDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("Database");
            this.Handler = database;
        }

        public async Task AddUpdateAccounts(IList<AccountSync> accountSync)
        {
            foreach (var account in accountSync)
            {
                var accountItem = await GetAccountById(account.AccountID);
                if (accountItem == null)
                {
                    if (account.IsDeleted == null || !(bool)account.IsDeleted)
                        await Handler.Database.InsertAsync(account);
                }
                else
                {
                    if (account.IsDeleted == null || !(bool)account.IsDeleted)
                    {
                        await Handler.Database.UpdateAsync(account);
                    }
                    else{
                        await Handler.Database.DeleteAsync(accountItem);
                    }
                }
            }
        }

        public async Task<AccountSync> GetAccountById(int id)
        {
            return await Handler.Database.Table<AccountSync>().Where(x => x.AccountID.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<List<AccountSync>> GetAllAccounts()
        {
            return await Handler.Database.Table<AccountSync>().ToListAsync();
        }
    }
}

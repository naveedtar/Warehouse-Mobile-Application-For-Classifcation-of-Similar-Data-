using System;
using System.Collections.Generic;
using WarehouseHandheld.Database.DatabaseHandler;
using WarehouseHandheld.Models.Users;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using WarehouseHandheld.Database.Accounts;

namespace WarehouseHandheld.Database.Users
{
    public class UsersTable : IUsersTable
    {

        public LocalDatabase Handler { get; private set; }
        public UsersTable(LocalDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("Database");
            this.Handler = database;
        }

        public async Task AddUpdateUsersSyncCollection(UsersSyncCollection userCollection)
        {
            await Handler.Database.InsertAsync(userCollection);
        }

        public async Task AddUpdateUsers(UserSync userSync)
        {
            await Handler.Database.InsertAsync(userSync);
        }

        public async Task AddUpdateUsers(IList<UserSync> userSync)
        {
            foreach (var user in userSync)
            {
                var userItem = await GetUserById(user.UserId);
                if (userItem == null)
                {
                    if (user.IsDeleted==null || !(bool)user.IsDeleted)
                        await Handler.Database.InsertAsync(user);
                }
                else
                {
                    if (user.IsDeleted == null || !(bool)user.IsDeleted)
                        await Handler.Database.UpdateAsync(user);
                    else
                        await Handler.Database.DeleteAsync(userItem);
                }
            }
        }

        public async Task<bool> CheckUser(string username, string password)
        {
            return await Handler.Database.Table<UserSync>().Where(x => x.Username.ToLower().Equals(username.ToLower()) && x.Password.Equals(password)).FirstOrDefaultAsync() != null;
        }

        private async Task<UserSync> GetUserById(int id)
        {
            return await Handler.Database.Table<UserSync>().Where(x => x.UserId.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<List<UserSync>> GetAllUsers()
        {
            return await Handler.Database.Table<UserSync>().ToListAsync();
        }

        public async Task<int> LoginUser(string username, string password)
        {
            
            var user = await Handler.Database.Table<UserSync>().Where(x => x.Username.ToLower().Equals(username.ToLower()) && x.Password.Equals(password)).FirstOrDefaultAsync();

            if (user == null)
                return 0;
            
            return user.UserId;
        }

        public async Task<bool> VerifyUserPass (string password)
        {
            var user = await Handler.Database.Table<UserSync>().Where(x => x.Password.Equals(password) && x.HandheldOverridePerm == true).FirstOrDefaultAsync();

            if (user == null)
                return false;

            return true;
        }
    }
}

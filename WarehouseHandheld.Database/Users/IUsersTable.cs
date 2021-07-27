using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Users;

namespace WarehouseHandheld.Database.Users
{
    public interface IUsersTable
    {
        Task AddUpdateUsersSyncCollection(UsersSyncCollection userCollection);
        Task AddUpdateUsers(UserSync userSync);
        Task AddUpdateUsers(IList<UserSync> userSync);
        Task<List<UserSync>> GetAllUsers();
        Task<bool> CheckUser(string username, string password);
        Task<int> LoginUser(string username, string password);
        Task<bool> VerifyUserPass(string password);
    }
}

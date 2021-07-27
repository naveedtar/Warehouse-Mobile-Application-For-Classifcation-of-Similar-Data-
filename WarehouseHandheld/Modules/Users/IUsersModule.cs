using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Users;

namespace WarehouseHandheld.Modules.Users
{
    public interface IUsersModule
    {
        int LoggedInUserId { get; set; }
        Task SyncUsers();
        Task<List<UserSync>> GetDrivers();
        Task<bool> Login(string username, string password);
        Task SyncUserLocation();
        Task<bool> VerifyUserPass(string password);
    }
}

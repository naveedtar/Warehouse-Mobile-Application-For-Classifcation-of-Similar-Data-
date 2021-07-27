using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Users;
namespace WarehouseHandheld.Services.Users
{
    public interface IUsersService
    {
        Task<UsersSyncCollection> GetUsersAsync(DateTime dateUpdated, string serialNo);
    }
}

using System;
using WarehouseHandheld.Models.Accounts;
using WarehouseHandheld.Models.OrderProcesses;

namespace WarehouseHandheld.Models.OrderProcesses
{
    public class OrderProcessAccount
    {
        public OrderProcessSync Order { get; set; }
        public AccountSync Account
        {
            get; set;
        }
    }
}

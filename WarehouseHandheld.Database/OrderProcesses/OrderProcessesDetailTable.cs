using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseHandheld.Database.DatabaseHandler;
using WarehouseHandheld.Models.OrderProcesses;

namespace WarehouseHandheld.Database.OrderProcesses
{
    public class OrderProcessesDetailTable : IOrderProcessesDetailTable
    {
        public LocalDatabase Handler { get; private set; }
        public OrderProcessesDetailTable(LocalDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("Database");
            this.Handler = database;
        }

        public async Task AddUpdateOrderProcessesDetail(IList<OrderProcessDetailSync> ordersProcessesDetailSync)
        {
            foreach (var orderProcessDetail in ordersProcessesDetailSync)
            {
                var orderProcessDetailItem = await GetOrderProcessDetailById(orderProcessDetail.OrderProcessDetailID);
                if (orderProcessDetailItem == null)
                {
                    //if (orderProcessDetail.OrderProcessDetailID == 0)
                    //{
                    //    var orderProcessList = await Handler.Database.Table<OrderProcessDetailSync>().ToListAsync();
                    //    if(orderProcessList != null && orderProcessList.Count > 0)
                    //    {
                    //        orderProcessDetail.OrderProcessDetailID = orderProcessList[orderProcessList.Count - 1].OrderProcessDetailID +1;
                    //    }
                    //}
                    if (orderProcessDetail.IsDeleted == null || !(bool)orderProcessDetail.IsDeleted)
                    {
                        var IsSave = await Handler.Database.InsertAsync(orderProcessDetail);
                    }
                }
                else
                {
                    if (orderProcessDetail.IsDeleted != null && (bool)orderProcessDetail.IsDeleted)
                    {
                        await Handler.Database.DeleteAsync(orderProcessDetail);
                    }
                    else
                    {
                        if (orderProcessDetail.OrderProcessDetailID == 0)
                        {
                            orderProcessDetail.QtyProcessed += orderProcessDetailItem.QtyProcessed;
                        }
                       var IsSave = await Handler.Database.UpdateAsync(orderProcessDetail);
                    }
                }
           
            }
        }

        public async Task<List<OrderProcessDetailSync>> GetOrderProcessDetailByOrderProcessId(int id)
        {
            if (id != 0)
                return await Handler.Database.Table<OrderProcessDetailSync>().Where(x => x.OrderProcessId == id).ToListAsync();
            else
                return null;
        }

        private async Task <OrderProcessDetailSync> GetOrderProcessDetailById(int id)
        {
            if (id != 0)
                return await Handler.Database.Table<OrderProcessDetailSync>().Where(x => x.OrderProcessDetailID.Equals(id)).FirstOrDefaultAsync();
            else
                return null;
        }

        public async Task<List<OrderProcessDetailSync>> GetOrderProcessDetailByOrderDetailId(int id)
        {
            if (id != 0)
            {
                var orderProcessDetails = await Handler.Database.Table<OrderProcessDetailSync>().Where(x => x.OrderDetailID.Equals(id)).ToListAsync();
                return orderProcessDetails;
            }
            else
                return null;
        }


        public async Task DeleteOrderProcessDetail(OrderProcessDetailSync orderProcessDetailSync)
        {
            await Handler.Database.DeleteAsync(orderProcessDetailSync);
        }
    }
}

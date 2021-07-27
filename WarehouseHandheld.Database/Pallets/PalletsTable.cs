using System;
using System.Collections.Generic;
using WarehouseHandheld.Database.DatabaseHandler;
using WarehouseHandheld.Models.Pallets;
using System.Linq;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Vehicles;

namespace WarehouseHandheld.Database.Pallets
{
    public class PalletsTable : IPalletsTable
    {
        public LocalDatabase Handler { get; private set; }
        public PalletsTable(LocalDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("Database");
            this.Handler = database;
        }


        public async Task AddUpdatePallets(IList<PalletSync> palletsSync)
        {
            foreach (var pallet in palletsSync)
            {
                if (pallet.PalletDispatchInfo != null)
                {
                    pallet.PalletsDispatchID = pallet.PalletDispatchInfo.PalletsDispatchID;
                }
                var palletItem = await GetPalletById(pallet.PalletID);
                if (palletItem == null)
                    await Handler.Database.InsertAsync(pallet);
                else
                    await Handler.Database.UpdateAsync(pallet);

                //Adding Pallet Dispatch info in its table
                if (pallet.PalletDispatchInfo != null)
                {
                    await Handler.PalletDispatch.AddUpdatePalletDispatch(pallet.PalletDispatchInfo);
                }
            }
        }

        public async Task AddUpdatePalletTracking(IList<PalletTrackingSync> pallets)
        {
            var newPallets = pallets.Where(x => x.Status != PalletTrackingStatusEnum.Archived).ToList();
            foreach (var pallet in newPallets)
            {
                var palletItem = await GetPalletTrackingById(pallet.PalletTrackingId);
                if (palletItem == null)
                    await Handler.Database.InsertAsync(pallet);
                else
                    await Handler.Database.UpdateAsync(pallet);
            }
        }

        public async Task AddUpdatePalletTracking(PalletTrackingSync pallet)
        {
            var palletItem = await GetPalletTrackingById(pallet.PalletTrackingId);
            if (palletItem == null)
                await Handler.Database.InsertAsync(pallet);
            else
                await Handler.Database.UpdateAsync(pallet);
        }

        public async Task AddUpdatePalletDispatchMethods(IList<PalletDispatchMethodSync> palletDispatchMethods)
        {
            foreach (var palletmethod in palletDispatchMethods)
            {
                var palletItem = await GetPalletDispatchMethodById(palletmethod.SentMethodID);
                if (palletItem == null)
                    await Handler.Database.InsertAsync(palletmethod);
                else
                    await Handler.Database.UpdateAsync(palletmethod);
            }
        }


        public async Task AddUpdatePalletProducts(IList<PalletProductsSync> palletProducts)
        {
            foreach (var pallet in palletProducts)
            {
                var palletItem = await GetPalletProductById(pallet.PalletProductID);
                if (palletItem == null)
                    await Handler.Database.InsertAsync(pallet);
                else
                    await Handler.Database.UpdateAsync(pallet);

            }
        }

        private async Task<PalletSync> GetPalletById(int id)
        {
            return await Handler.Database.Table<PalletSync>().Where(x => x.PalletID.Equals(id)).FirstOrDefaultAsync();
        }

        private async Task<PalletTrackingSync> GetPalletTrackingById(int id)
        {
            return await Handler.Database.Table<PalletTrackingSync>().Where(x => x.PalletTrackingId.Equals(id)).FirstOrDefaultAsync();
        }

        private async Task<PalletDispatchMethodSync> GetPalletDispatchMethodById(int id)
        {
            return await Handler.Database.Table<PalletDispatchMethodSync>().Where(x => x.SentMethodID.Equals(id)).FirstOrDefaultAsync();
        }

        private async Task<PalletProductsSync> GetPalletProductById(int id)
        {
            return await Handler.Database.Table<PalletProductsSync>().Where(x => x.PalletProductID.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<List<PalletSync>> GetAllPallets()
        {
            var pallets = await Handler.Database.Table<PalletSync>().ToListAsync();
            foreach (var item in pallets)
            {
                item.RecipientAccount = await Handler.Accounts.GetAccountById(item.RecipientAccountID);
            }
            return pallets;
        }

        public async Task<List<PalletSync>> GetAllPalletsByOrderId(int orderId)
        {
            //var palletsAll = await GetAllPallets();
            var pallets = await Handler.Database.Table<PalletSync>().Where(x => x.OrderProcessID.Equals(orderId)).ToListAsync();
            foreach (var item in pallets)
            {
                item.RecipientAccount = await Handler.Accounts.GetAccountById(item.RecipientAccountID);
            }
            return pallets;
        }


        public async Task<List<PalletProductsSync>> GetAllPalletProducts()
        {
            var pallets = await Handler.Database.Table<PalletProductsSync>().ToListAsync();
            return pallets;
        }

        public async Task<List<PalletTrackingSync>> GetAllPalletTrackings()
        {
            var pallets = await Handler.Database.Table<PalletTrackingSync>().ToListAsync();
            return pallets;
        }


        public async Task<List<PalletDispatchMethodSync>> GetAllPalletDispatchMethods()
        {
            return await Handler.Database.Table<PalletDispatchMethodSync>().ToListAsync();

        }

        public async Task<PalletTrackingSync> GetPalletTrackingBySerial(string serial, int productId)
        {
            return await Handler.Database.Table<PalletTrackingSync>().Where(x => (x.PalletSerial.Equals(serial)) && (x.ProductId == productId)).FirstOrDefaultAsync();
        }

        public async Task<PalletTrackingSync> GetAllPalletTrackingsWithTrackingId(int trackingId)
        {
            return await Handler.Database.Table<PalletTrackingSync>().Where(x => (x.PalletTrackingId == trackingId)).FirstOrDefaultAsync();
        }

        public async Task<PalletTrackingSync> GetAllPalletTrackingsWithSerial(string serial)
        {
            return await Handler.Database.Table<PalletTrackingSync>().Where(x => (x.PalletSerial.Equals(serial))).FirstOrDefaultAsync();
        }

        public async Task<PalletTrackingSync> GetPalletTrackingForGoodsOut(int productId, List<PalleTrackingProcess> PalletTrackingProcesses, List<PalleTrackingProcess> PalletTrackingProcessesForAll, List<PalletTrackingSync> SkippedPallets, PalletTrackingSchemeEnum trackingScheme)
        {
            List<PalletTrackingSync> list = new List<PalletTrackingSync>();
            switch (trackingScheme)
            {
                case PalletTrackingSchemeEnum.FirstInFirstOut:
                    list = await Handler.Database.Table<PalletTrackingSync>().Where(x => ((x.ProductId == productId) && (x.Status == PalletTrackingStatusEnum.Active) && (x.RemainingCases > 0))).OrderBy((x) => x.DateCreated).ToListAsync();
                    break;
                case PalletTrackingSchemeEnum.FirstInLastOut:
                    list = await Handler.Database.Table<PalletTrackingSync>().Where(x => ((x.ProductId == productId) && (x.Status == PalletTrackingStatusEnum.Active) && (x.RemainingCases > 0))).OrderByDescending((x) => x.DateCreated).ToListAsync();
                    break;
                case PalletTrackingSchemeEnum.ByExpiryDate:
                    list = await Handler.Database.Table<PalletTrackingSync>().Where(x => ((x.ProductId == productId) && (x.Status == PalletTrackingStatusEnum.Active) && (x.RemainingCases > 0))).OrderBy((x) => x.ExpiryDate).ToListAsync();
                    break;
                case PalletTrackingSchemeEnum.ByExpiryMonth:
                    list = await Handler.Database.Table<PalletTrackingSync>().Where(x => (x.ProductId == productId) && (x.Status == PalletTrackingStatusEnum.Active) && (x.RemainingCases > 0)).OrderBy((x) => x.ExpiryDate).ToListAsync();
                    break;
                case PalletTrackingSchemeEnum.DontEnforce:
                    list = await Handler.Database.Table<PalletTrackingSync>().Where((x) => ((x.ProductId == productId) && (x.Status == PalletTrackingStatusEnum.Active) && (x.RemainingCases > 0))).ToListAsync();
                    break;
                default:
                    list = await Handler.Database.Table<PalletTrackingSync>().Where(x => ((x.ProductId == productId) && (x.Status == PalletTrackingStatusEnum.Active) && (x.RemainingCases > 0))).OrderBy((x) => x.DateCreated).ToListAsync();
                    break;
            }
            list.RemoveAll((x) => PalletTrackingProcesses.Any(y => y.PalletTrackingId == x.PalletTrackingId));
            list.RemoveAll((x) => SkippedPallets.Any(y => y.PalletTrackingId == x.PalletTrackingId));
            list.RemoveAll((x) => PalletTrackingProcessesForAll.Any(y => y.PalletTrackingId == x.PalletTrackingId));
            if (list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        public async Task<PalletTrackingSync> GetPalletTrackingForGoodsOutWithSerial(int productId, List<PalleTrackingProcess> PalletTrackingProcesses, List<PalleTrackingProcess> PalletTrackingProcessesForAll, List<PalletTrackingSync> SkippedPallets, PalletTrackingSchemeEnum trackingScheme, string serial)
        {
            List<PalletTrackingSync> list = new List<PalletTrackingSync>();
            switch (trackingScheme)
            {
                case PalletTrackingSchemeEnum.FirstInFirstOut:
                    list = await Handler.Database.Table<PalletTrackingSync>().Where(x => (x.PalletSerial.Equals(serial) && (x.ProductId == productId) && (x.Status == PalletTrackingStatusEnum.Active) && (x.RemainingCases > 0))).OrderBy((x) => x.DateCreated).ToListAsync();
                    break;
                case PalletTrackingSchemeEnum.FirstInLastOut:
                    list = await Handler.Database.Table<PalletTrackingSync>().Where(x => (x.PalletSerial.Equals(serial) && (x.ProductId == productId) && (x.Status == PalletTrackingStatusEnum.Active) && (x.RemainingCases > 0))).OrderByDescending((x) => x.DateCreated).ToListAsync();
                    break;
                case PalletTrackingSchemeEnum.ByExpiryDate:
                    list = await Handler.Database.Table<PalletTrackingSync>().Where(x => (x.PalletSerial.Equals(serial) && (x.ProductId == productId) && (x.Status == PalletTrackingStatusEnum.Active) && (x.RemainingCases > 0))).OrderBy((x) => x.ExpiryDate).ToListAsync();
                    break;
                case PalletTrackingSchemeEnum.ByExpiryMonth:
                    list = await Handler.Database.Table<PalletTrackingSync>().Where(x => x.PalletSerial.Equals(serial) && (x.ProductId == productId) && (x.Status == PalletTrackingStatusEnum.Active) && (x.RemainingCases > 0)).OrderBy((x) => x.ExpiryDate).ToListAsync();
                    break;
                case PalletTrackingSchemeEnum.DontEnforce:
                    list = await Handler.Database.Table<PalletTrackingSync>().Where((x) => (x.PalletSerial.Equals(serial) && (x.ProductId == productId) && (x.Status == PalletTrackingStatusEnum.Active) && (x.RemainingCases > 0))).ToListAsync();
                    break;
            }
            list.RemoveAll((x) => PalletTrackingProcesses.Any(y => (y.PalletTrackingId == x.PalletTrackingId && y.RemainingCasesAfterProcessing == 0)));
            list.RemoveAll((x) => SkippedPallets.Any(y => y.PalletTrackingId == x.PalletTrackingId));
            list.RemoveAll((x) => PalletTrackingProcessesForAll.Any(y => (y.PalletTrackingId == x.PalletTrackingId && y.RemainingCasesAfterProcessing == 0)));
            if (list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        public async Task<PalleTrackingProcess> GetPalletTrackingProcessBySerial(string serial)
        {
            return await Handler.Database.Table<PalleTrackingProcess>().Where(x => x.palletSerial.Equals(serial)).FirstOrDefaultAsync();
        }

        public async Task<List<PalletSync>> GetDispatchedPalletsSyncByDispatchedId(int palletDispatchId)
        {
            var palletsSyncInDb = await Handler.Database.Table<PalletSync>().Where(x => x.IsDispatched == true && x.PalletsDispatchID.Equals(palletDispatchId)).ToListAsync();
            return palletsSyncInDb;
        }

        public async Task<List<PalletProductsSync>> GetAllPalletProductsByPalletId(int palletId)
        {
            var palletProducts = await Handler.Database.Table<PalletProductsSync>().Where(x => x.CurrentPalletID.Equals(palletId)).ToListAsync();
            return palletProducts;
        }

        public async Task DeletePalletTrackingsWithArchivedStatus()
        {
          var isDeleted =  await Handler.Database.Table<PalletTrackingSync>().Where(x => x.Status.Equals(PalletTrackingStatusEnum.Archived)).DeleteAsync();
        }
    }
}

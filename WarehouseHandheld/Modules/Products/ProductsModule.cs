using System;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Sync;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Services;
using WarehouseHandheld.Extensions;
using System.Net.Http;
using WarehouseHandheld.Resources;
using System.Collections.Generic;
using Plugin.Connectivity;

namespace WarehouseHandheld.Modules.Products
{
    public class ProductsModule : IProductsModule
    {
        public ProductsModule()
        {
        }
        bool isSyncingProducts = false;
        bool isSyncingProductsSerials = false;

        public async Task SyncProducts()
        {
            if (!CrossConnectivity.Current.IsConnected || !await Util.Util.IsConnected())
            {
                if (!CrossConnectivity.Current.IsConnected)
                    AppStrings.NoInternet.ToToast();
                return;
            }
            if (isSyncingProducts)
            {
                "Already syncing products".ToToast();
            }
            isSyncingProducts = true;

            DateTime date;
            SyncLog synclog = await App.Database.SyncLog.GetSyncLogByTableName(Database.DatabaseConfig.Tables.Products.ToString());

            if (synclog != null && synclog.RequestedTime != DateTime.MinValue)
                date = synclog.RequestedTime;
            else
                date = ModulesConfig.SyncDate;

            string serialNo = ModulesConfig.SerialNo;

            if (synclog == null)
                synclog = await InitializeSyncLog(Database.DatabaseConfig.Tables.Products.ToString(), WebServiceConfig.SyncProducts);

            //Update Sync Log before sending request
            synclog.SerialNo = serialNo;
            synclog.RequestUrl = WebServiceConfig.SyncProducts;
            synclog.RequestedTime = DateTime.UtcNow;

            //Service Call
            ProductMasterSyncCollection products = await App.WarehouseService.Products.GetProductsAsync(date, serialNo);
            if(products==null)
            {
                Constants.ApiErrorMsg.ToToast();
                isSyncingProducts = false;
                return;
            }

            //Adding fetched data to database
            await AddProductsToDatabase(products);

            //acknowledgement service call
            HttpResponseMessage ackResponse = await App.WarehouseService.Acknowledgement.VerifyAckAsync(products.TerminalLogId.ToString(), products.Products.Count, serialNo);

            synclog.ResponseTime = DateTime.UtcNow;
            synclog.Synced = false;
            synclog.ErrorCode = (int)ackResponse.StatusCode;

            if ((int)ackResponse.StatusCode == 200)
            {
                synclog.TerminalLogId = products.TerminalLogId.ToString();
                synclog.LastSynced = DateTime.UtcNow;
                synclog.Synced = true;
                synclog.ResultCount = products.Count;
                synclog.ErrorCode = 0;
                await App.Database.SyncLog.UpdateSyncLogItem(synclog);
            }
            isSyncingProducts = false;
        }

        public async Task SyncProductSerials()
        {
            if (!CrossConnectivity.Current.IsConnected || !await Util.Util.IsConnected())
            {
                if (!CrossConnectivity.Current.IsConnected)
                    AppStrings.NoInternet.ToToast();
                return;
            }
            if (isSyncingProductsSerials)
            {
                "Already syncing product serials".ToToast();
                return;
            }
            isSyncingProductsSerials = true;

            DateTime date;
            SyncLog synclog = await App.Database.SyncLog.GetSyncLogByTableName(Database.DatabaseConfig.Tables.ProductSerials.ToString());

            if (synclog != null && synclog.RequestedTime != DateTime.MinValue)
                date = synclog.RequestedTime;
            else
                date = ModulesConfig.SyncDate;

            string serialNo = ModulesConfig.SerialNo;

            if (synclog == null)
                synclog = await InitializeSyncLog(Database.DatabaseConfig.Tables.ProductSerials.ToString(), WebServiceConfig.SyncProductSerials);

            //Update Sync Log before sending request
            synclog.SerialNo = serialNo;
            synclog.RequestUrl = WebServiceConfig.SyncProductSerials;
            synclog.RequestedTime = DateTime.UtcNow;

            //Service Call
            ProductSerialSyncCollection productSerials = await App.WarehouseService.Products.GetProductSerialsAsync(date, serialNo);
            if (productSerials == null)
            {
                Constants.ApiErrorMsg.ToToast();
                isSyncingProductsSerials = false;
                return;
            }
            
            //Adding fetched data to database
            await AddProductSerialsToDatabase(productSerials);

            //acknowledgement service call
            HttpResponseMessage ackResponse = await App.WarehouseService.Acknowledgement.VerifyAckAsync(productSerials.TerminalLogId.ToString(), productSerials.ProductSerials.Count, serialNo);

            synclog.ResponseTime = DateTime.UtcNow;
            synclog.Synced = false;
            synclog.ErrorCode = (int)ackResponse.StatusCode;

            if ((int)ackResponse.StatusCode == 200)
            {
                synclog.TerminalLogId = productSerials.TerminalLogId.ToString();
                synclog.LastSynced = DateTime.UtcNow;
                synclog.Synced = true;
                synclog.ResultCount = productSerials.Count;
                synclog.ErrorCode = 0;
                await App.Database.SyncLog.UpdateSyncLogItem(synclog);
            }
            isSyncingProductsSerials = false;
        }


        private async Task AddProductsToDatabase(ProductMasterSyncCollection products)
        {
            if (products.Products != null)
                await App.Database.Products.AddUpdateProducts(products.Products);
        }

        private async Task AddProductSerialsToDatabase(ProductSerialSyncCollection serials)
        {
            if (serials != null && serials.ProductSerials != null)
                await App.Database.ProductSerials.AddUpdateProductSerials(serials.ProductSerials);
        }

        private async Task<SyncLog> InitializeSyncLog(string tableName, string requestUrl)
        {
            SyncLog synclog = new SyncLog();
            synclog.TableName = tableName;
            synclog.RequestUrl = requestUrl;
            await App.Database.SyncLog.AddSyncLogItem(synclog);
            return synclog;
        }

        public async Task<ProductMasterSync> GetProductById(int id)
        {
            return await App.Database.Products.GetProductById(id);
        }

        public async Task<List<ProductMasterSync>> GetAllProducts()
        {
            return await App.Database.Products.GetAllProducts();
        }

        public async Task<List<ProductSerialSync>> GetProductSerialByProductId(int id)
        {
            return await App.Database.ProductSerials.GetProductSerialByProductId(id);
        }

        public async Task<List<ProductSerialSync>> GetAllProductSerials()
        {
            return await App.Database.ProductSerials.GetAllProductSerials();
        }

        public async Task<ProductSerialSync> GetProductSerialBySerialNo(string serialNo)
        {
            return await App.Database.ProductSerials.GetProductSerialBySerialNo(serialNo);
        }
    }
}

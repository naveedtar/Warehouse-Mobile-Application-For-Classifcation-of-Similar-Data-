using System;
using System.Collections.Generic;
using System.Linq;
using WarehouseHandheld.Database.DatabaseHandler;
using WarehouseHandheld.Models.Orders;
using System.Threading.Tasks;
using WarehouseHandheld.Models.Products;

namespace WarehouseHandheld.Database.Orders
{
    public class OrderDetailsTable : IOrderDetailsTable
    {
        public LocalDatabase Handler { get; private set; }
        public OrderDetailsTable(LocalDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("Database");
            this.Handler = database;
        }


        public async Task AddUpdateOrderDetails(IList<OrderDetailSync> orderDetailsSync, int orderId)
        {
            foreach (var orderDetail in orderDetailsSync)
            {
                var orderDetailItem = await GetOrderDetailById(orderDetail.OrderDetailID);
                if (orderDetailItem == null)
                {
                    if (orderDetail.IsDeleted == null || !(bool)orderDetail.IsDeleted) { 
                    await Handler.Database.InsertAsync(orderDetail); }
                }
                else
                {
                    if (orderDetail.IsDeleted != null && orderDetail.IsDeleted == true)
                    {
                        await Handler.Database.DeleteAsync(orderDetailItem);
                    }
                    else
                    {
                        await Handler.Database.UpdateAsync(orderDetail);
                    }
                }
            }


            //todo improve this logic
            //var details = await GetOrderDetailsByOrderId(orderId);
            //foreach (var item in details)
            //{
            //    if (orderDetailsSync.FirstOrDefault(x => x.OrderDetailID == item.OrderDetailID) == null)
            //        await Handler.Database.DeleteAsync(item);
            //}
        }

        public async Task<OrderDetailSync> GetOrderDetailById(int id)
        {
            return await Handler.Database.Table<OrderDetailSync>().Where(x => x.OrderDetailID.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<List<OrderDetailSync>> GetOrderDetailsByOrderId(int id)
        {
            return await Handler.Database.Table<OrderDetailSync>().Where(x => x.OrderID == id).OrderBy((x) => x.SortOrder).ToListAsync();
        }

        public async Task<List<OrderDetailSync>> GetAllOrderDetails()
        {
            return (await Handler.Database.Table<OrderDetailSync>().ToListAsync()).OrderBy((x)=>x.SortOrder).ToList();
        }

        public async Task<List<OrderDetailsProduct>> GetOrderDetailsWithProduct(int orderId)
        {
            var orderDetails = await GetOrderDetailsByOrderId(orderId);
            //orderDetails = orderDetails.Where(x => x.OrderID == orderId).ToList();
            var products = await Handler.Products.GetAllProducts();

            var joined =
                from order in orderDetails
                join product in products on order.ProductId equals product.ProductId into ps
                from product in ps.DefaultIfEmpty()
                select new OrderDetailsProduct() { OrderDetails = order, Product = product };

            return joined.ToList();
        }
        public async Task<List<OrderDetailsProduct>> GetOrderDetailsWithProductAndKit(int orderId)
        {
            var orderDetailsProducts = new List<OrderDetailsProduct>();
            var orderDetails = await GetOrderDetailsByOrderId(orderId);
            foreach (var orderDetail in orderDetails)
            {
                //ProductLocation Based on Priority
                // var productLocations = await Handler.ProductLocationStock.GetProductStocksLocationByProductId(orderDetail.ProductId);
                var productStockLocation = await Handler.ProductLocationStock.GetProductStockLocationSortedByProductId(orderDetail.ProductId, orderDetail.Qty);
                var product = await Handler.Products.GetProductById(orderDetail.ProductId);
                if (product != null)
                {
                    var orderDetailsProduct = new OrderDetailsProduct
                    {
                        OrderDetails = orderDetail,
                        Product = product,
                        IsProductKit = (product.ProductType == ProductKitTypeEnum.Kit),
                        ProductLocation = productStockLocation
                    };
                    orderDetailsProducts.Add(orderDetailsProduct);
                    if (product.ProductType == ProductKitTypeEnum.Kit)
                    {
                        var productKits = await GetProductKitsForSpecificKitProduct(product.ProductId); 
                        if (productKits != null)
                        {
                            foreach (var productKit in productKits)
                            {
                                if (productKit.ProductKitType.Equals(ProductKitTypeEnum.Kit))
                                {
                                    var kitProduct = await Handler.Products.GetProductById(productKit.KitProductId);
                                    var orderDetailKitSubProduct = new OrderDetailSync
                                    {
                                        OrderDetailID = orderDetail.OrderDetailID,
                                        OrderID = orderDetail.OrderID,
                                        Qty = orderDetail.Qty * productKit.Quantity,
                                        Price = orderDetail.Price,
                                        ProductId = productKit.KitProductId,
                                    };

                                    var kitOrderDetailProduct = new OrderDetailsProduct
                                    {
                                        OrderDetails = orderDetailKitSubProduct,
                                        Product = kitProduct,
                                        IsProductInKit = true,
                                        KitOrderDetail = orderDetail,
                                        KitQuantity = productKit.Quantity,
                                    };
                                    orderDetailsProducts.Add(kitOrderDetailProduct);
                                }
                            }
                        }
                    }
                }
            }
            return orderDetailsProducts;
        }

        public async Task<List<ProductKitMapViewModel>> GetProductKitsForSpecificKitProduct(int kitProductId)
        {
            if (kitProductId > 0)
            {
                var kitItems = await Handler.Database.Table<ProductKitMapViewModel>().Where(x => x.ProductId.Equals(kitProductId)).ToListAsync();
                return kitItems;
            }
            return null;
        }


        public async Task<List<ProductMasterSync>> GetProductsForSpecificKitProduct(int kitProductId)
        {
            List<ProductMasterSync> kitProducts = new List<ProductMasterSync>();

            if (kitProductId > 0)
            {
                var kitItems = await Handler.Database.Table<ProductKitMapViewModel>().Where(x => x.KitProductId.Equals(kitProductId)).ToListAsync();
                foreach (var kitItem in kitItems)
                {
                    var kitProduct = await Handler.Products.GetProductById(kitItem.ProductId);

                    kitProducts.Add(kitProduct);
                }
            }
            return kitProducts;
        }

        public async Task<List<OrderDetailsProduct>> GetOrderDetailsProductForSpecificKitProduct(List<OrderDetailsProduct> orderDetailsProducts)
        {
            List<OrderDetailsProduct> _orderDetailsProduct = new List<OrderDetailsProduct>();
            foreach (var item in orderDetailsProducts)
            {
                if (item.Product != null)
                {
                    if (item.Product.ProductType == ProductKitTypeEnum.Kit)
                    {
                        var kitItems = await Handler.Database.Table<ProductKitMapViewModel>().Where(x => x.KitProductId.Equals(item.Product.ProductId)).ToListAsync();
                        foreach (var kitItem in kitItems)
                        {
                            var kitProduct = await Handler.Products.GetProductById(kitItem.ProductId);
                            var orderDetailsProduct = new OrderDetailsProduct
                            {
                                Product = kitProduct,
                                OrderDetails = item.OrderDetails,
                                IsProductKit = true,
                                Quantity = item.Quantity * kitItem.Quantity,
                            };
                            _orderDetailsProduct.Add(orderDetailsProduct);
                        }
                    }
                }
            }
            return _orderDetailsProduct;
        }
    }
}

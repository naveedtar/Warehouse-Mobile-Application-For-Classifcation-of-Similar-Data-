using System;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Models.Orders;
using System.Collections.ObjectModel;
using System.Linq;
using WarehouseHandheld.Extensions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using WarehouseHandheld.Models.Returns;
using WarehouseHandheld.Modules;
using WarehouseHandheld.Views.ScanItems;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Models.Wastages;
using Ganedata.Core.Barcoding;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Views.Returns;
using static WarehouseHandheld.Models.Orders.OrdersSync;
using WarehouseHandheld.Views.OrderItems;

namespace WarehouseHandheld.ViewModels.Returns
{
    public class ReturnsViewModel : BaseViewModel
    {
        public Action<bool> OnFocus;
        public ICommand ReturnCommand { get; private set; }

        bool checkOrderDetailNull = false;
        private ProductMasterSync productSync;
        int orderId = 0;

        bool isWastages;
        public bool IsWastages
        {
            get { return isWastages; }
            set
            {
                isWastages = value;
                OnPropertyChanged();
            }
        }

        bool issellableGoods = true;
        public bool IsSellableGoods
        {
            get { return issellableGoods; }
            set
            {
                issellableGoods = value;
                OnPropertyChanged();
            }
        }

        bool ismissingTrackingNo = false;
        public bool IsMissingTrackingNo
        {
            get { return ismissingTrackingNo; }
            set
            {
                ismissingTrackingNo = value;
                OnPropertyChanged();
            }
        }

        bool isReturnEnable = true;
        public bool IsReturnEnable
        {
            get { return isReturnEnable; }
            set
            {
                isReturnEnable = value;
                OnPropertyChanged();
            }
        }


        private List<OrderDetailsProduct> orderDetailsProduct = new List<OrderDetailsProduct>();
        public List<OrderDetailsProduct> OrderDetailsProduct
        {
            get { return orderDetailsProduct; }
            set
            {
                orderDetailsProduct = value;
                OnPropertyChanged();
            }
        }

        private List<ProductMasterSync> allProducts = new List<ProductMasterSync>();
        public List<ProductMasterSync> AllProducts
        {
            get { return allProducts; }
            set
            {
                allProducts = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> products = new ObservableCollection<string>();
        public ObservableCollection<string> Products
        {
            get { return products; }
            set
            {
                products = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<GoodsReturnRequestSync> productsReturn = new ObservableCollection<GoodsReturnRequestSync>();
        public ObservableCollection<GoodsReturnRequestSync> ProductsReturn
        {
            get { return productsReturn; }
            set
            {
                productsReturn = value;
                OnPropertyChanged();
            }
        }

        int productIndex;
        public int ProductIndex
        {
            get { return productIndex; }
            set
            {
                productIndex = value;
                OnPropertyChanged();
            }
        }

        private PalletTrackingSync palletTrackings;
        public PalletTrackingSync PalletTrackings
        {
            get { return palletTrackings; }
            set
            {
                palletTrackings = value;
                OnPropertyChanged();
            }
        }

        private ProductMasterSync product;
        public ProductMasterSync Product
        {
            get { return product; }
            set
            {
                product = value;
                OnPropertyChanged();
            }
        }

        private decimal casesinPallet;
        public decimal CasesinPallet
        {
            get { return casesinPallet; }
            set
            {
                casesinPallet = value;
                OnPropertyChanged();
            }
        }

        private decimal remainingProductsinPallet;
        public decimal RemainingProductsinPallet
        {
            get { return remainingProductsinPallet; }
            set
            {
                remainingProductsinPallet = value;
                OnPropertyChanged();
            }
        }

        public ReturnsViewModel()
        {
            ReturnCommand = new Command(PostReturn);
        }

        public async void InitializeProducts()
        {
            AllProducts = await App.Products.GetAllProducts();
            foreach (var product in AllProducts)
            {
                Products.Add(product.Name);
            }
            if(Products.Count()>0)
            ProductIndex = 0;
        }

        async void PostReturn(object obj)
        {
            IsReturnEnable = false;
            IsBusy = true;
            try
            {
                if (ProductIndex >= 0)
                {
                    OrderDetailsProduct orderDetail = null;
                    GoodsReturnRequestSync request = new GoodsReturnRequestSync();

                    if (!IsWastages)
                    {
                        if (OrderDetailsProduct != null && OrderDetailsProduct.Count()>0)
                        {
                            orderDetail = OrderDetailsProduct[productIndex];
                            var quantity = orderDetail.QuantityProcessed;
                            orderDetail.Quantity = orderDetail.OrderDetails.Qty;
                            orderDetail.OrderDetails.Qty = quantity;
                            orderDetail.QuantityProcessed = 0;
                            request.OrderId = orderDetail.OrderDetails.OrderID;
                            request.ProductId = orderDetail.Product.ProductId;

                        }
                        else
                        {
                            request.ProductId = AllProducts[ProductIndex].ProductId;
                            request.OrderId = orderId;
                        }

                        request.sellableFormat = IsSellableGoods;
                        if (!IsSellableGoods)
                        {
                            request.InventoryTransactionType = (int)InventoryTransactionTypeEnum.WastedReturn;
                        }
                        else
                        {
                            request.InventoryTransactionType = (int)InventoryTransactionTypeEnum.Returns;
                        }
                        request.MissingTrackingNo = IsMissingTrackingNo;
                        request.warehouseId = ModulesConfig.WareHouseID;
                        request.userId = App.Users.LoggedInUserId;
                        request.tenantId = ModulesConfig.TenantID;
                        request.SerialNo = ModulesConfig.SerialNo;
                        request.TransactionLogId = Guid.NewGuid();
                        request.ReturnReason = "Return";
                    }
                   
                        ProductMasterSync productFound = null;
                        if(Products.Count > ProductIndex)
                        productFound = AllProducts.Find((x) => x.Name == Products[ProductIndex]);
                        if ((IsWastages && productFound != null && productFound.ProcessByPallet && !IsMissingTrackingNo) || (!IsWastages && productFound != null && productFound.ProcessByPallet && !IsMissingTrackingNo))
                        {
                             await OpenPalletSerialPage(orderDetail, request);
                        }
                        else if ((IsWastages && productFound!=null && productFound.Serialisable && !IsMissingTrackingNo) || (!IsWastages && productFound != null && productFound.Serialisable || (orderDetail != null &&  orderDetail.Product.Serialisable && !IsMissingTrackingNo)))
                        {
                            await OpenSerialisePage(orderDetail, request);
                        }
                        else
                        {
                            if(orderDetail == null || orderDetail.OrderDetails.Qty > 0)
                            {
                                if (orderDetail == null)
                                {
                                    checkOrderDetailNull = true;
                                    orderDetail = new OrderDetailsProduct();
                                    orderDetail.Product = productFound;
                                }
                                await OpenNonSearlisePage(orderDetail, request);
                            }
                            else
                            {
                                await Util.Util.ShowErrorPopupWithBeep("No item processed in this order");
                            }
                        }
                    }
                    //else
                    //{
                    //    await Util.Util.ShowErrorPopupWithBeep("Already reached maximum limit");
                    //}

                //}
            }
            catch { }
            finally
            {
                IsReturnEnable = true;
                IsBusy = false;
            }
        }

       

        async Task PostReturns(OrderDetailsProduct orderDetail, GoodsReturnRequestSync request, string[] serials, decimal quantityArg)
        {
            if ((!IsWastages && orderDetail != null) || OrderDetailsProduct.Count()>0) 
            {
                var quantityProcessed = orderDetail.OrderDetails.Qty;
                orderDetail.OrderDetails.Qty = orderDetail.Quantity - quantityArg;
                orderDetail.QuantityProcessed = quantityProcessed - quantityArg;
                if(orderDetail.Product.ProductsPerCase != null)
                    orderDetail.BoxesRemaining = Math.Round(((orderDetail.OrderDetails.Qty - orderDetail.QuantityProcessed) / (decimal)orderDetail.Product.ProductsPerCase),2);
            

            }
            else
            {
                var productFound = AllProducts.Find((x) => x.Name == Products[ProductIndex]);
                request.ProductId = productFound.ProductId;
               
            }
            request.Quantity = Convert.ToInt32(quantityArg);
            if (serials != null && serials.Count()>0)
            {
                request.ProductSerials = new List<string>();
                foreach (var serial in serials)
                {
                    request.ProductSerials.Add(serial);
                }
            }
           
            if (!IsWastages)
            {
                var response = await App.WarehouseService.Returns.PostGoodsReturn(request);
                if (response != null && (response.IsSuccess || response.CanProceed))
                {
                    ProductsReturn.Add(request);
                    if (!response.IsSuccess && !string.IsNullOrEmpty(response.FailureMessage))
                    {
                        response.FailureMessage.ToToast();
                    }
                    else
                    {
                        if (response.orderId != null && response.orderId != 0)
                        {
                            orderId = response.orderId;
                        }
                        "Return Posted Successfully".ToToast();
                    }
                }
                else
                {
                    if (response!=null && !string.IsNullOrEmpty(response.FailureMessage) && !App.WarehouseService.Returns.HandleStatusConflict())
                    {
                        response.FailureMessage.ToToast();
                    }

                }
            }
            else
            {
                var wastageRequest = new GoodsReturnRequestSync();
                var productFound = AllProducts.Find((x) => x.Name == Products[ProductIndex]);
                if (productFound != null && productFound.ProcessByPallet)
                {
                    wastageRequest.ProductId = productFound.ProductId;
                    wastageRequest.PalleTrackingProcess = request.PalleTrackingProcess;
                    wastageRequest.PalletTrackingId = request.PalletTrackingId;
                }
                else
                {
                    wastageRequest.ProductId = productFound.ProductId;
                }
                wastageRequest.sellableFormat = null;
                wastageRequest.InventoryTransactionType = (int)InventoryTransactionTypeEnum.Wastage;
                wastageRequest.MissingTrackingNo = IsMissingTrackingNo;
                wastageRequest.Quantity = Convert.ToInt32(quantityArg);
                wastageRequest.SerialNo = ModulesConfig.SerialNo;
                wastageRequest.warehouseId = ModulesConfig.WareHouseID;
                wastageRequest.tenantId = ModulesConfig.TenantID;
                wastageRequest.userId = App.Users.LoggedInUserId;
                wastageRequest.ReturnReason = "Wastage";
                wastageRequest.TransactionLogId = Guid.NewGuid();

                wastageRequest.ProductSerials = request.ProductSerials;
              
                var response = await App.WarehouseService.Returns.PostGoodsReturn(wastageRequest);
                if (response != null && (response.IsSuccess || response.CanProceed))
                {
                    ProductsReturn.Add(request);
                    if (!response.IsSuccess && !string.IsNullOrEmpty(response.FailureMessage))
                    {
                        response.FailureMessage.ToToast();
                    }
                    else
                    {
                        "Return Posted Successfully".ToToast();
                    }
                }
                else
                {
                    if (response != null && !string.IsNullOrEmpty(response.FailureMessage) && !App.WarehouseService.Returns.HandleStatusConflict())
                    {
                        response.FailureMessage.ToToast();
                    }
                }
            }
            //await App.Database.OrderDetails.AddUpdateOrderDetails(OrderDetailsList,orderDetail.OrderDetails.OrderID);

            OnFocus?.Invoke(true);
        }

        async Task OpenSerialisePage(OrderDetailsProduct orderDetail, GoodsReturnRequestSync request)
        {
            var scanPage = new ScanSerialItemsPopup(orderDetail,IsWastages);
            scanPage.OnCancelClicked += () =>
            {
                OnFocus?.Invoke(true);
            };
            scanPage.ViewModel.OnSaveSerial += async (arg1, serials, quantityArg) =>
           {
               await PostReturns(orderDetail, request, serials, quantityArg);

           };
            await PopupNavigation.PushAsync(scanPage);
        }



        async Task OpenNonSearlisePage(OrderDetailsProduct orderDetail, GoodsReturnRequestSync request)
        {
            var scanPage = new ScanNonSerialItemsPopup(orderDetail, orderDetail.Product);
            scanPage.OnCancelClicked += () =>
            {
                OnFocus?.Invoke(true);
            };
            if (checkOrderDetailNull)
            {
                orderDetail = null;
            }
            scanPage.ViewModel.OnSave += async (arg1, quantityArg) =>
           {
               await PostReturns(orderDetail, request, null, quantityArg);
           };
            await PopupNavigation.PushAsync(scanPage);
        }

        async Task OpenPalletSerialPage(OrderDetailsProduct orderDetail, GoodsReturnRequestSync request)
        {
            decimal quantity = 1;
            var returnOrWastage = true;
            var productFound = AllProducts.Find((x) => x.Name == Products[ProductIndex]);
            var scanPage = new PalletTrackingScanPage(productFound, orderDetail, returnOrWastage,IsWastages);
            scanPage.CancelClicked += () =>
            {
                OnFocus?.Invoke(true);
            };

            scanPage.PalletListAdded += async (obj, quantityProcessed) =>
            {
                quantityProcessed = Math.Round(quantityProcessed, 2);
                if (quantityProcessed > quantity)
                {
                    quantity = quantityProcessed;
                }
                obj.RemoveAll(x => x.PalletTrackingId == 0);
                request.PalleTrackingProcess = new List<PalleTrackingProcess>(obj);
                await PostReturns(orderDetail, request, null, quantity);
            };
            await App.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(scanPage));
        }


        public async Task<bool> ScanWastagesTextChanged(string code)
        {
            var barcode = new GS128Decoder();
            code = barcode.GS128DecodeGTINOrDefault(code);
            var productIndexInProducts = AllProducts.FindIndex((obj) => (!string.IsNullOrEmpty(obj.SKUCode) && obj.SKUCode.ToLower() == code.ToLower()) || (!string.IsNullOrEmpty(obj.BarCode) && obj.BarCode.ToLower() == code.ToLower()) || (!string.IsNullOrEmpty(obj.BarCode2) && obj.BarCode2.ToLower() == code.ToLower()));
            if (productIndexInProducts >= 0 && OrderDetailsProduct != null && OrderDetailsProduct.Count > 0)
            {
                var productFound = AllProducts.Find((x) => x.Name == Products[productIndexInProducts]);
                var orderfound = OrderDetailsProduct.Find((x) => x.Product.ProductId == productFound.ProductId);
                if (orderfound == null)
                {
                   productIndexInProducts = -1;
                }

            }
            ProductIndex = productIndexInProducts;
            PalletTrackings = null;
            if (productIndexInProducts >= 0)
            {
                "Product found".ToToast();

                return true;
            }

            "Product not found".ToToast();
            return false;
        }

        public async Task<bool> ScanCodeTextChanged(string code)
        {

            var barcode = new GS128Decoder();
            code = barcode.GS128DecodeGTINOrDefault(code);
            var orders = await App.Orders.GetAllOrders();
            var order = orders.Find(x => x.OrderNumber.ToLower().Equals(code.ToLower()));

            if (order != null)
            {
                Products.Clear();
                OrderDetailsProduct.Clear();
                var orderdetails = await App.Orders.GetOrderDetailsWithProduct(order.OrderID);
                if (orderdetails != null)
                {
                    foreach (var orderdetail in orderdetails)
                    {
                        Products.Add(orderdetail.Product.Name);
                        OrderDetailsProduct.Add(orderdetail);
                    }
                    ProductIndex = 0;
                }
                "Order found, please select product which you want to return.".ToToast();
                return true;
            }
            else{
                OrderDetailsProduct.Clear();
            }
            return false;
        }

        public void SetAllProducts()
        {
            Products.Clear();
            foreach (var product in AllProducts)
            {
                Products.Add(product.Name);
            }
            ProductIndex = 0;
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Modules;
using Xamarin.Forms;

namespace WarehouseHandheld.ViewModels.GoodsReceive
{
    public class GoodsReceiveViewModel : BaseViewModel
    {
        public ICommand SaveCommand{ get; private set; }
        Guid ReceiveCountId;
        public Action GoBack;
        public GoodsReceiveViewModel()
        {
            SaveCommand = new Command(OnSave);
            ReceiveCountId = Guid.NewGuid();
        }

        async void OnSave(object obj)
        {
            IsBusy = true;
            try
            {
                if (string.IsNullOrEmpty(Reference))
                {
                    "Enter required values".ToToast();
                    return;
                }
                var OrderReceive = new OrderReceiveCountSync();
                OrderReceive.CreatedBy = App.Users.LoggedInUserId;
                OrderReceive.DateCreated = DateTime.UtcNow;
                OrderReceive.OrderID = order.Order.OrderID;
                OrderReceive.TenantId = ModulesConfig.TenantID;
                OrderReceive.ReferenceNo = Reference;
                OrderReceive.Notes = Notes;
                OrderReceive.WarehouseId = (int)order.Order.WarehouseId;
                OrderReceive.OrderReceiveCountDetail = GoodsReceiveList;
                OrderReceive.ReceiveCountId = ReceiveCountId;
                var OrderReceiveCollection = new OrderReceiveCountSyncCollection();
                OrderReceiveCollection.SerialNo = ModulesConfig.SerialNo;
                OrderReceiveCollection.Count = 1;
                OrderReceiveCollection.TerminalLogId = Guid.NewGuid();
                OrderReceiveCollection.TransactionLogId = Guid.NewGuid();
                OrderReceiveCollection.OrderReceiveCountSync = new System.Collections.Generic.List<OrderReceiveCountSync>();
                OrderReceiveCollection.OrderReceiveCountSync.Add(OrderReceive);
                var res = await App.WarehouseService.Orders.PostReceiveCount(OrderReceiveCollection);
                if(res!=null){
                    "Goods Receive Posted Successfully".ToToast();
                    GoBack?.Invoke();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }


        public async void SetgoodsReceive()
        {
           var OrderItems = new ObservableCollection<OrderDetailsProduct>(await App.Orders.GetOrderDetailsWithProduct(order.Order.OrderID));
            foreach(var orderItem in OrderItems)
            {
                var goodsReceived = new OrderReceiveCountDetailSync();
                goodsReceived.OrderDetailID = orderItem.OrderDetails.OrderDetailID;
                goodsReceived.ProductId =  orderItem.OrderDetails.ProductId;
                goodsReceived.ProductName = orderItem.Product.Name;
                goodsReceived.DateCreated = DateTime.UtcNow;
                goodsReceived.DateUpdated = DateTime.UtcNow;
                goodsReceived.CreatedBy = App.Users.LoggedInUserId;
                goodsReceived.TenentId = ModulesConfig.TenantID;
                goodsReceived.ReceiveCountId = ReceiveCountId;
                goodsReceived.ReceiveCountDetailId = Guid.NewGuid();
                GoodsReceiveList.Add(goodsReceived);
            }

        }
        public OrderAccount order;

        ObservableCollection<OrderReceiveCountDetailSync> goodsReceiveList = new ObservableCollection<OrderReceiveCountDetailSync>();
        public ObservableCollection<OrderReceiveCountDetailSync> GoodsReceiveList{
            get { return goodsReceiveList; }
            set
            {
                goodsReceiveList = value;
                OnPropertyChanged();
            }
        }

        string reference ;
        public string Reference
        {
            get { return reference; }
            set
            {
                reference = value;
                OnPropertyChanged();
            }
        }

        string notes="";
        public string Notes
        {
            get { return notes; }
            set
            {
                notes = value;
                OnPropertyChanged();
            }
        }

    }
}

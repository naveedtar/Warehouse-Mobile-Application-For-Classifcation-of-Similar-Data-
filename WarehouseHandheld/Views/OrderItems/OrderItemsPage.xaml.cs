using System.Linq;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Resources;
using WarehouseHandheld.ViewModels.OrderItems;
using WarehouseHandheld.Views.Base.BaseContentPage;
using Xamarin.Forms;
using static WarehouseHandheld.Models.Orders.OrdersSync;

namespace WarehouseHandheld.Views.OrderItems
{
    public partial class OrderItemsPage : BasePage
    {
        public OrderItemsViewModel ViewModel => BindingContext as OrderItemsViewModel;
        OrderAccount order;
        bool IsAppearing;
        bool IsPageAppeared = false;
        bool IsPickCheck;
        bool IsDoneOrBackClicked;
        public OrderItemsPage(OrderAccount order, InventoryTransactionTypeEnum orderType)
        {
            NavigationPage.SetHasBackButton(this, false);
            InitializeComponent();
            //if(orderType== InventoryTransactionTypeEnum.TransferOut)
            //grid.Columns.Add(new Xamarin.Forms.DataGrid.DataGridColumn() {  Title="Stock",PropertyName = "InventoryStock.InStock" });
            Constants.SetGridProperties(grid);
            Title = orderType.ToString();
            ViewModel.OrderAccount = order;
            this.order = order;
            scanEntry.Keyboard = null;
            ViewModel.SetFocus += (obj) =>
            {
                scanEntry.Unfocus();
                scanEntry.Focus();
                IsAppearing = true;
            };
            ViewModel.ShowPopup += (obj) =>
            {
                IsAppearing = false;
            };

            ViewModel.GoBack += () =>
            {
                Navigation.PopAsync();
            };

            ViewModel.OrderProcessStarted += () =>
            {
                SaveToolbarItem.IsEnabled = false;
                CompleteToolbarItem.IsEnabled = false;
            };

            ViewModel.OrderProcessFinished += () =>
            {
                SaveToolbarItem.IsEnabled = true;
                CompleteToolbarItem.IsEnabled = true;
            };
        }


        async void Back_Clicked(object sender, System.EventArgs e)
        {
            IsDoneOrBackClicked = true;
            await ViewModel.CheckOrder();

        }


        protected override bool OnBackButtonPressed()
        {
            IsDoneOrBackClicked = true;
            ViewModel.CheckOrder();
            return true;
        }

        protected async override void OnAppearing()
        {
            IsAppearing = true;
            base.OnAppearing();
            MessagingCenter.Send(this, "preventLandScape");
            if (IsPageAppeared == false)
            {
                await ViewModel.Initialize();
                if (!ViewModel.IsShowLocationColumn)
                {
                    grid.Columns[2].Width = 0;
                    grid.Columns[2].Title = string.Empty;
                    LocationColumn.Width = 0;
                }
                IsPageAppeared = true;
            }
            if (this.order.Order.InventoryTransactionTypeId.Equals((int)InventoryTransactionTypeEnum.SaleOrder))
            {
                var terminalMetadataSync = await App.Database.Vehicle.GetTerminalMetaData();
                if (terminalMetadataSync != null && terminalMetadataSync.PickByContainer)
                {
                    ScanPickContainerCode.IsVisible = true;
                    ScanPickContainerLabel.IsVisible = true;
                    scanEntry.IsEnabled = false;
                    FocusPickCode();
                }
                else
                {
                    FocusProductCode();
                }
            }
            else if (this.order.Order.InventoryTransactionTypeId.Equals((int)InventoryTransactionTypeEnum.PurchaseOrder))
            {
                FocusProductCode();
            }
            if (!IsPickCheck)
            {
                IsPickCheck = true;
                await ViewModel.CheckOrdeStatus();
            }
            scanEntry.Focus();
            if (ViewModel.serialPopup != null)
            {
                ViewModel.serialPopup.PopupAppeared();
            }
        }


        protected async override void OnDisappearing()
        {
            IsAppearing = false;
            base.OnDisappearing();
            MessagingCenter.Send(this, "allowLandScapePortrait");
        }

        async void Handle_ScanTextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
                return;
            //if(await ViewModel.ScanCodeTextChanged(e.NewTextValue))
            //{
            //    scanEntry.Unfocus();
            //    scanEntry.Text = string.Empty;
            //    await System.Threading.Tasks.Task.Delay(300);
            //    if (IsAppearing)
            //    {
            //        scanEntry.Focus();
            //    }
            //}

        }

        async void Handle_Completed(object sender, System.EventArgs e)
        {
            scanEntry.Unfocus();
            scanEntry.ShowKeyboard = false;
             
            if (!string.IsNullOrEmpty(ViewModel.ScanCode) && !await ViewModel.ScanCodeTextChanged(ViewModel.ScanCode))
            {
                await Util.Util.ShowErrorPopupWithBeep("Item " + ViewModel.ScanCode + " not found.");

                scanEntry.Text = string.Empty;
                await System.Threading.Tasks.Task.Delay(300);

                if (IsAppearing)
                    scanEntry.Focus();
            }
            else
            {
                var orders = ViewModel.OrderItems.ToList();
                var orderitem = orders.Where(x => x.Product.SKUCode.ToLower() == scanEntry.Text.ToLower()).FirstOrDefault();

                var isExist = orders.Any(x => x.Product.SKUCode.ToLower() == scanEntry.Text.ToLower());
                if (isExist)
                {
                    if (orderitem.QuantityProcessed == orderitem.OrderDetails.Qty)
                    {
                        ViewModel.AutoSave();
                        ViewModel.AutoComplete();
                    }
                }
            }
        }

        void Handle_Add_Clicked(object sender, System.EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new CreateOrderDetailsPopup(order));
        }

        async void Keyboard_Tapped(object sender, System.EventArgs e)
        {
            if (scanPickEntry.Text != null && scanPickEntry.Text != "")
            {
                scanEntry.ShowKeyboard = !scanEntry.ShowKeyboard;
                await System.Threading.Tasks.Task.Delay(200);
                scanEntry.Focus();
            }
        }

        async void Keyboard_TappedPick(object sender, System.EventArgs e)
        {
            scanPickEntry.ShowKeyboard = !scanPickEntry.ShowKeyboard;
            await System.Threading.Tasks.Task.Delay(200);
            scanPickEntry.Focus();
        }

        async void ScanPickEntry_Completed(System.Object sender, System.EventArgs e)
        {
            ViewModel.ScanPickContainerCode = scanPickEntry.Text;
            if (scanPickEntry.Text != null && scanPickEntry.Text != "")
            {
                scanEntry.IsEnabled = true;
                scanEntry.ShowKeyboard = !scanEntry.ShowKeyboard;
                await System.Threading.Tasks.Task.Delay(200);
                scanEntry.Focus();
            }

        }

        async void FocusPickCode()
        {
            if (scanPickEntry != null && scanPickEntry.Text == "")
            {
                scanPickEntry.ShowKeyboard = !scanPickEntry.ShowKeyboard;
                await System.Threading.Tasks.Task.Delay(200);
                scanPickEntry.Focus();
            }
            else
            {
                scanEntry.IsEnabled = true;
                scanEntry.ShowKeyboard = !scanEntry.ShowKeyboard;
                await System.Threading.Tasks.Task.Delay(200);
                scanEntry.Focus();
            }
        }

        async void FocusProductCode()
        {
            scanEntry.IsEnabled = true;
            scanEntry.ShowKeyboard = !scanEntry.ShowKeyboard;
            await System.Threading.Tasks.Task.Delay(200);
            scanEntry.Focus();
        }
    }
}

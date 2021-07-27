using WarehouseHandheld.ViewModels.Menu;
using WarehouseHandheld.Views.Base.BaseContentPage;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.Menu
{
    public partial class MenuPage : BasePage
    {
        public MenuViewModel ViewModel => BindingContext as MenuViewModel;

        public MenuPage()
        {
            InitializeComponent();
            salesOrdersButton.IsVisible = false;
            SetMenu();
        }

        async void SetMenu()
        {
            bool IsFirstColumn = true;
            int RowCount = 0;
            var User = (await App.Database.Users.GetAllUsers()).Find((x) => x.UserId == App.Users.LoggedInUserId);

            if (User.PurchaseOrderPerm)
            {
                purchaseOrdersButton.IsVisible = true;
                Grid.SetRow(purchaseOrdersButton, RowCount);
                if (IsFirstColumn)
                {
                    Grid.SetColumn(purchaseOrdersButton, 0);

                }
                IsFirstColumn = !IsFirstColumn;

            }
            if (User.WorksOrderPerm || User.SalesOrderPerm)
            {
                salesOrdersButton.IsVisible = true;
                Grid.SetRow(salesOrdersButton, RowCount);
                if (IsFirstColumn)
                {
                    Grid.SetColumn(salesOrdersButton, 0);

                }
                else
                {
                    Grid.SetColumn(salesOrdersButton, 1);
                    RowCount++;
                }
                IsFirstColumn = !IsFirstColumn;

            }

            if (User.TransferOrderPerm)
            {
                transferOrdersButton.IsVisible = true;
                Grid.SetRow(transferOrdersButton, RowCount);
                if (IsFirstColumn)
                {
                    Grid.SetColumn(transferOrdersButton, 0);

                }
                else
                {
                    Grid.SetColumn(transferOrdersButton, 1);
                    RowCount++;
                }
                IsFirstColumn = !IsFirstColumn;
                watageButton.IsVisible = true;

            }
            if (User.WastagesPerm)
            {
                watageButton.IsVisible = true;
                Grid.SetRow(watageButton, RowCount);
                if (IsFirstColumn)
                {
                    Grid.SetColumn(watageButton, 0);

                }
                else
                {
                    Grid.SetColumn(watageButton, 1);
                    RowCount++;
                }
                IsFirstColumn = !IsFirstColumn;
            }

            if (User.GoodsReturnPerm)
            {
                returnButton.IsVisible = true;
                Grid.SetRow(returnButton, RowCount);
                if (IsFirstColumn)
                {
                    Grid.SetColumn(returnButton, 0);

                }
                else
                {
                    Grid.SetColumn(returnButton, 1);
                    RowCount++;
                }
                IsFirstColumn = !IsFirstColumn;

            }
            if (User.StockTakePerm)
            {
                stockTakeButton.IsVisible = true;
                Grid.SetRow(stockTakeButton, RowCount);
                if (IsFirstColumn)
                {
                    Grid.SetColumn(stockTakeButton, 0);

                }
                else
                {
                    Grid.SetColumn(stockTakeButton, 1);
                    RowCount++;
                }
                IsFirstColumn = !IsFirstColumn;

            }
            //User.PalletingPerm = true;
            if (User.PalletingPerm)
            {
                palletingButton.IsVisible = true;
                Grid.SetRow(palletingButton, RowCount);
                if (IsFirstColumn)
                {
                    Grid.SetColumn(palletingButton, 0);

                }
                else
                {
                    Grid.SetColumn(palletingButton, 1);
                    RowCount++;
                }
                IsFirstColumn = !IsFirstColumn;

            }
            if (User.StockEnquiryPerm)
            {
                stockEnquiryButton.IsVisible = true;
                Grid.SetRow(stockEnquiryButton, RowCount);
                if (IsFirstColumn)
                {
                    Grid.SetColumn(stockEnquiryButton, 0);

                }
                else
                {
                    Grid.SetColumn(stockEnquiryButton, 1);
                    RowCount++;
                }
                IsFirstColumn = !IsFirstColumn;
            }
            if (User.GeneratePalletLabelsPerm)
            {
                generateLabelsButton.IsVisible = true;
                Grid.SetRow(generateLabelsButton, RowCount);
                if (IsFirstColumn)
                {
                    Grid.SetColumn(generateLabelsButton, 0);

                }
                else
                {
                    Grid.SetColumn(generateLabelsButton, 1);
                    RowCount++;
                }
                IsFirstColumn = !IsFirstColumn;
            }
            if (User.GoodsReturnPerm)
            {

                goodsReceive.IsVisible = true;
                Grid.SetRow(goodsReceive, RowCount);
                if (IsFirstColumn)
                {
                    Grid.SetColumn(goodsReceive, 0);

                }
                else
                {
                    Grid.SetColumn(goodsReceive, 1);
                    RowCount++;
                }
                IsFirstColumn = !IsFirstColumn;
            }

            if (User.PrintBarcodePerm)
            {
                printBarCode.IsVisible = true;
                Grid.SetRow(printBarCode, RowCount);
                if (IsFirstColumn)
                {
                    Grid.SetColumn(printBarCode, 0);

                }
                else
                {
                    Grid.SetColumn(printBarCode, 1);
                    RowCount++;
                }
                IsFirstColumn = !IsFirstColumn;
            }
            //User.PODPerm = true;
            if (User.PODPerm)
            {
                loading.IsVisible = true;
                Grid.SetRow(loading, RowCount);
                if (IsFirstColumn)
                {
                    Grid.SetColumn(loading, 0);

                }
                else
                {
                    Grid.SetColumn(loading, 1);
                    RowCount++;
                }
                IsFirstColumn = !IsFirstColumn;
            }

            //if (User.PODPerm)
            {
                stockMovement.IsVisible = true;
                Grid.SetRow(stockMovement, RowCount);
                if (IsFirstColumn)
                {
                    Grid.SetColumn(stockMovement, 0);

                }
                else
                {
                    Grid.SetColumn(stockMovement, 1);
                    RowCount++;
                }
                IsFirstColumn = !IsFirstColumn;
            }

        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }
    }
}

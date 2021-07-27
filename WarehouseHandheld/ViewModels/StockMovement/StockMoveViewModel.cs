using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Helpers;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Models.ProductStockLocation;
using WarehouseHandheld.Models.StockMovement;
using WarehouseHandheld.Models.Sync;
using WarehouseHandheld.Services;
using WarehouseHandheld.Views.OrderItems;
using WarehouseHandheld.Views.Products;
using WarehouseHandheld.Views.ScanItems;
using WarehouseHandheld.Views.StockMovement;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace WarehouseHandheld.ViewModels.StockMovement
{
    public class StockMoveViewModel : BaseViewModel
    {
        public Action ClearScanEntry;
        public Action<bool> ShowPopup;
        public Action<bool> SetFocus;
        bool shouldAllowOverrider;

        public ICommand CompleteCommand { get; private set; }
        protected CommandLockerHelper CompleteCommandLocker => new CommandLockerHelper((e) => { OnComplete(e); });
        public ICommand ItemSelectedCommand => new Command(SelectedCommandLocker.Execute);
        protected CommandLockerHelper SelectedCommandLocker => new CommandLockerHelper(async (e) => { await OnItemSelected(e); });

        private async Task OnItemSelected(object e)
        {
            try
            {
                if (SelectedStockLocation != null)
                {
                    var item = e as SelectedItemChangedEventArgs;
                    if (e != null)
                    {
                        var selectedStockItem = (StockMovementViewModel)((SelectedItemChangedEventArgs)e).SelectedItem;
                        if (item == null)
                            return;
                        await ScanProductCodeTextChanged(selectedStockItem.ProductSkuCode);
                    }
                }
                else
                {
                    await Util.Util.ShowErrorPopupWithBeep("Select location first");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public StockMoveViewModel()
        {
            CompleteCommand = new Command(CompleteCommandLocker.Execute);
            ScanText = "Scan From Location";
            TitleText = "Scan Movement From";
        }

        private ProductMasterSync _product;
        public ProductMasterSync SelectedProduct
        {
            get { return _product; }
            set
            {
                _product = value;
                OnPropertyChanged();
            }
        }

        private StockMovementViewModel _stockMovementVm;
        public StockMovementViewModel StockMovementVm
        {
            get { return _stockMovementVm; }
            set
            {
                _stockMovementVm = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<StockMovementViewModel> _stockMovements;
        public ObservableCollection<StockMovementViewModel> StockMovements
        {
            get { return _stockMovements; }
            set
            {
                _stockMovements = value;
                OnPropertyChanged();
            }
        }


        private bool _isFromLocationComplete;
        public bool IsFromLocationComplete
        {
            get
            {
                return _isFromLocationComplete;
            }
            set
            {
                _isFromLocationComplete = value;
                OnPropertyChanged(nameof(IsFromLocationComplete));
            }
        }
        private LocationSync _selectedStockLocation;
        public LocationSync SelectedStockLocation
        {
            get
            {
                return _selectedStockLocation;
            }
            set
            {
                _selectedStockLocation = value;
                OnPropertyChanged(nameof(SelectedStockLocation));
            }
        }

        private string _scanProductCode;
        public string ScanProductCode
        {
            get
            {
                return _scanProductCode;
            }
            set
            {
                _scanProductCode = value;
                OnPropertyChanged(nameof(ScanProductCode));
            }
        }

        private string _scanText;
        public string ScanText
        {
            get
            {
                return _scanText;
            }
            set
            {
                _scanText = value;
                OnPropertyChanged(nameof(ScanText));
            }
        }

        private string _titleText;
        public string TitleText
        {
            get
            {
                return _titleText;
            }
            set
            {
                _titleText = value;
                OnPropertyChanged(nameof(TitleText));
            }
        }

        private string _scanLocationCode;
        public string ScanLocationCode
        {
            get
            {
                return _scanLocationCode;
            }
            set
            {
                _scanLocationCode = value;
                OnPropertyChanged(nameof(ScanLocationCode));
            }
        }

        public async Task Initialize()
        {
            IsBusy = true;
            var stockMovements = await App.Database.StockMovements.GetStockMovements();
            if (stockMovements != null && stockMovements.Any() && !stockMovements.Any(x => x.IsFromLocationComplete))
            {
                StockMovements = new ObservableCollection<StockMovementViewModel>(stockMovements);
                ScanText = "Scan From Location";
                TitleText = "Scan Movement From";
            }
            else if (stockMovements != null && stockMovements.Any() && stockMovements.All(x => x.IsFromLocationComplete))
            {
                SelectedStockLocation = null;
                IsFromLocationComplete = true;
                StockMovements = new ObservableCollection<StockMovementViewModel>(stockMovements);
                ScanText = "Scan To Location";
                TitleText = "Scan Movement To";
            }
            else
            {
                if (StockMovements == null)
                {
                    StockMovements = new ObservableCollection<StockMovementViewModel>();
                    await App.Database.StockMovements.DeleteStockMovement();
                }
            }
            IsBusy = false;
        }

        public async Task<bool> ScanLocationCodeTextChanged(string scanLocationCode)
        {
            if (string.IsNullOrEmpty(scanLocationCode))
            {
                return false;
            }

           
            var locationFound = await App.Database.StockMovements.GetStockLocationByLocationCode(scanLocationCode);
            if (locationFound != null)
            {
                if (IsFromLocationComplete)
                {
                    var itemFound = StockMovements.All(x => x.FromLocationCode.Equals(locationFound.LocationCode));
                    if (itemFound)
                    {
                        await Util.Util.ShowErrorPopupWithBeep("'From' and 'To' locations cannot be same");
                        return false;
                    }
                }
                SelectedStockLocation = locationFound;
                "Location Selected".ToToast();
                return true;
            }
            await Util.Util.ShowErrorPopupWithBeep("Location " + ScanLocationCode + " not found.");
            return false;
        }

        public async Task<bool> ScanProductCodeTextChanged(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return false;
            }
            if (SelectedStockLocation == null)
            {
                await Util.Util.ShowErrorPopupWithBeep("Select location first");
                return true;
            }
            var productsFound = await App.Database.Products.GetProductByCode(code);
            if (productsFound.Count() == 0)
            {
                return false;
            }
            else
            {
                SelectedProduct = productsFound[0];
                bool IsBarcode2 = false;
                if (SelectedProduct.BarCode2 != null)
                    IsBarcode2 = SelectedProduct.BarCode2.ToLower().Contains(code.ToLower());

                var productStockLocation =
                    await App.Database.ProductLocationStock.GetProductStockLocationByProductIdAndLocationId(
                        SelectedProduct.ProductId, SelectedStockLocation.LocationId);

              
                if (SelectedProduct.ProcessByPallet)
                {
                    await HandleProcessByPallet(productStockLocation);
                }
                else if (SelectedProduct.Serialisable)
                {
                    if (StockMovements != null && StockMovements.Any())
                    {
                        StockMovementVm = StockMovements.ToList().Find(x => x.ProductId.Equals(SelectedProduct.ProductId));
                        if (StockMovementVm == null)
                        {
                            if (!IsFromLocationComplete)
                            {
                                StockMovementVm = new StockMovementViewModel
                                {
                                    ProductId = SelectedProduct.ProductId,
                                    ProductName = SelectedProduct.Name,
                                    ProductSkuCode = SelectedProduct.SKUCode,
                                    FromLocation = SelectedStockLocation.LocationId,
                                    FromLocationCode = SelectedStockLocation.LocationCode,
                                    SerialIds = new List<int>(),
                                    PalletSerials = new List<StockMovementPalletSerialsViewModel>()
                                };
                                if (productStockLocation != null)
                                    StockMovementVm.QuantityAvailableOnLocation = productStockLocation.Quantity;
                            }
                            else
                            {
                                await Util.Util.ShowErrorPopupWithBeep("Product not found is stock movement.");
                                return false;
                            }
                        }
                        else
                        {
                            if (IsFromLocationComplete)
                            {
                                if (SelectedStockLocation.LocationCode.Equals(StockMovementVm.FromLocationCode))
                                {
                                    await Util.Util.ShowErrorPopupWithBeep("'From' and 'To' locations cannot be same");
                                    return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        StockMovementVm = new StockMovementViewModel
                        {
                            ProductId = SelectedProduct.ProductId,
                            ProductName = SelectedProduct.Name,
                            ProductSkuCode = SelectedProduct.SKUCode,
                            FromLocation = SelectedStockLocation.LocationId,
                            FromLocationCode = SelectedStockLocation.LocationCode,
                            SerialIds = new List<int>(),
                            PalletSerials = new List<StockMovementPalletSerialsViewModel>()
                        };
                        if (productStockLocation != null)
                            StockMovementVm.QuantityAvailableOnLocation = productStockLocation.Quantity;
                    }

                    var serialPopup = new ScanProductStockMovementPopup(StockMovementVm);
                    serialPopup.ViewModel.OnSaveSerial += OnSaveSerial;
                    serialPopup.ViewModel.Back += (obj) =>
                    {
                        //SetFocus?.Invoke(true);
                        ClearScanEntry?.Invoke();
                    };
                    await PopupNavigation.Instance.PushAsync(serialPopup);
                    ShowPopup?.Invoke(true);
                }
                else
                {
                    // Non-Serial Items
                    if (StockMovements != null && StockMovements.Any())
                    {
                        StockMovementVm = StockMovements.ToList().Find(x => x.ProductId.Equals(SelectedProduct.ProductId));
                        if (StockMovementVm == null)
                        {
                            if (!IsFromLocationComplete)
                            {
                                StockMovementVm = new StockMovementViewModel
                                {
                                    ProductId = SelectedProduct.ProductId,
                                    ProductName = SelectedProduct.Name,
                                    ProductSkuCode = SelectedProduct.SKUCode,
                                    FromLocation = SelectedStockLocation.LocationId,
                                    FromLocationCode = SelectedStockLocation.LocationCode,
                                };
                                if (productStockLocation != null)
                                    StockMovementVm.QuantityAvailableOnLocation = productStockLocation.Quantity;
                            }
                            else
                            {
                                await Util.Util.ShowErrorPopupWithBeep("Product not found is stock movement.");
                                return false;
                            }
                          
                        }
                        else
                        {
                            if (IsFromLocationComplete)
                            {
                                if (SelectedStockLocation.LocationCode.Equals(StockMovementVm.FromLocationCode))
                                {
                                    await Util.Util.ShowErrorPopupWithBeep("'From' and 'To' locations cannot be same");
                                    return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        StockMovementVm = new StockMovementViewModel
                        {
                            ProductId = SelectedProduct.ProductId,
                            ProductName = SelectedProduct.Name,
                            ProductSkuCode = SelectedProduct.SKUCode,
                            FromLocation = SelectedStockLocation.LocationId,
                            FromLocationCode = SelectedStockLocation.LocationCode,
                        };
                        
                        if (productStockLocation != null)
                            StockMovementVm.QuantityAvailableOnLocation = productStockLocation.Quantity;
                    }

                    var serialPopup = new ScanProductStockMovementPopup(StockMovementVm);
                    serialPopup.ViewModel.OnSave += async (updatedstock) =>
                    {

                        var stockIndex = StockMovements.ToList().FindIndex(x => x.ProductId.Equals(updatedstock.ProductId));
                        if (stockIndex != -1)
                        {
                            StockMovements.RemoveAt(stockIndex);
                            await AddUpdateStockMovement(updatedstock);
                        }
                        else
                        {
                            await AddUpdateStockMovement(updatedstock);
                        }
                        ClearScanEntry?.Invoke();
                    };
                    serialPopup.ViewModel.Back += (obj) =>
                    {
                        //SetFocus?.Invoke(true);
                        ClearScanEntry?.Invoke();
                    };
                    await PopupNavigation.Instance.PushAsync(serialPopup);
                    ShowPopup?.Invoke(true);
                }
                "Item Found".ToToast();


            }
            return true;
        }

        private async void OnSaveSerial(StockMovementViewModel updatedstock)
        {
            var stockIndex = StockMovements.ToList().FindIndex(x => x.ProductId.Equals(updatedstock.ProductId));
            if (stockIndex != -1)
            {
                StockMovements.RemoveAt(stockIndex);
                await AddUpdateStockMovement(updatedstock);
            }
            else
            {
                await AddUpdateStockMovement(updatedstock);
            }
        }

        private async Task<bool> HandleProcessByPallet(ProductLocationStocksSync productLocation)
        {
            if (StockMovements != null && StockMovements.Any())
            {
                StockMovementVm = StockMovements.ToList().Find(x => x.ProductId.Equals(SelectedProduct.ProductId));
                if (StockMovementVm == null)
                {
                    if (!IsFromLocationComplete)
                    {
                        StockMovementVm = new StockMovementViewModel
                        {
                            ProductId = SelectedProduct.ProductId,
                            ProductName = SelectedProduct.Name,
                            ProductSkuCode = SelectedProduct.SKUCode,
                            FromLocation = SelectedStockLocation.LocationId,
                            FromLocationCode = SelectedStockLocation.LocationCode,
                            PalletSerials = new List<StockMovementPalletSerialsViewModel>()
                        };
                        if (productLocation != null)
                            StockMovementVm.QuantityAvailableOnLocation = productLocation.Quantity;
                    }
                    else
                    {
                        await Util.Util.ShowErrorPopupWithBeep("Product not found is stock movement.");
                        return false;
                    }
                }
                else
                {
                    if (IsFromLocationComplete)
                    {
                        if (SelectedStockLocation.LocationCode.Equals(StockMovementVm.FromLocationCode))
                        {
                            await Util.Util.ShowErrorPopupWithBeep("'From' and 'To' locations cannot be same");
                            return false;
                        }
                    }
                }
            }
            else
            {
                StockMovementVm = new StockMovementViewModel
                {
                    ProductId = SelectedProduct.ProductId,
                    ProductName = SelectedProduct.Name,
                    ProductSkuCode = SelectedProduct.SKUCode,
                    FromLocation = SelectedStockLocation.LocationId,
                    FromLocationCode = SelectedStockLocation.LocationCode,
                    PalletSerials = new List<StockMovementPalletSerialsViewModel>()
                };
                if (productLocation != null)
                    StockMovementVm.QuantityAvailableOnLocation = productLocation.Quantity;
            }

          
            var serialPopup = new PalletTrackingScanPage(SelectedProduct, StockMovementVm);
            serialPopup.StockAdded += async (StockMovement) =>
            {
                var stockIndex = StockMovements.ToList().FindIndex(x => x.ProductId.Equals(SelectedProduct.ProductId));
                if (stockIndex != -1)
                {
                    var stock = StockMovements[stockIndex];
                    StockMovements.RemoveAt(stockIndex);
                    await AddUpdateStockMovement(StockMovement);
                }
                else
                {
                    
                    await AddUpdateStockMovement(StockMovement);
                }

                SetFocus?.Invoke(true);
                serialPopup = null;
            };
            serialPopup.CancelClicked += () =>
            {
                SetFocus?.Invoke(true);
                serialPopup = null;
            };
            serialPopup.SetShouldAllowOverridPerm += (bool perm) =>
            {
                shouldAllowOverrider = perm;
            };
            await App.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(serialPopup));
            ShowPopup?.Invoke(true);
            return true;
        }



        public async Task AddUpdateStockMovement(StockMovementViewModel stockMovement)
        {
            if (IsFromLocationComplete)
            {
                var newStockMovement = new StockMovementViewModel
                {
                    Id = stockMovement.Id,
                    Qty = stockMovement.Qty,
                    QtyProcessed = stockMovement.QtyProcessed,
                    SerialIds = stockMovement.SerialIds,
                    ProductId = stockMovement.ProductId,
                    ProductName = stockMovement.ProductName,
                    ProductSkuCode = stockMovement.ProductSkuCode,
                    ToLocation = SelectedStockLocation.LocationId,
                    FromLocation = stockMovement.FromLocation,
                    FromLocationCode = stockMovement.FromLocationCode,
                    DateCreated = DateTime.Now,
                    IsFromLocationComplete = stockMovement.IsFromLocationComplete,
                    PalletSerials = stockMovement.PalletSerials,
                    TenentId = Modules.ModulesConfig.TenantID, // May need to change
                    ToLocationCode = SelectedStockLocation.LocationCode,
                    UserId = App.Users.LoggedInUserId,
                    WarehouseId = SelectedStockLocation.WarehouseId,
                    QuantityAvailableOnLocation = stockMovement.QuantityAvailableOnLocation
                };
                await App.Database.StockMovements.AddUpdateStockMovement(newStockMovement);
                StockMovements.Insert(0, newStockMovement);
            }
            else
            {
                var newStockMovement = new StockMovementViewModel
                {
                    Id = stockMovement.Id,
                    Qty = stockMovement.Qty,
                    QtyProcessed = stockMovement.QtyProcessed,
                    SerialIds = stockMovement.SerialIds,
                    ProductId = stockMovement.ProductId,
                    ProductName = stockMovement.ProductName,
                    ProductSkuCode = stockMovement.ProductSkuCode,
                    ToLocation = stockMovement.ToLocation,
                    FromLocation = stockMovement.FromLocation,
                    FromLocationCode = stockMovement.FromLocationCode,
                    DateCreated = DateTime.Now,
                    IsFromLocationComplete = stockMovement.IsFromLocationComplete,
                    PalletSerials = stockMovement.PalletSerials,
                    TenentId = Modules.ModulesConfig.TenantID, // May need to change
                    ToLocationCode = stockMovement.ToLocationCode,
                    UserId = App.Users.LoggedInUserId,
                    WarehouseId = SelectedStockLocation.WarehouseId,
                    QuantityAvailableOnLocation = stockMovement.QuantityAvailableOnLocation
                };
                await App.Database.StockMovements.AddUpdateStockMovement(newStockMovement);
                StockMovements.Insert(0, newStockMovement);
            }


        }

        private async void OnComplete(object e)
        {
            if (StockMovements.All(x => x.IsFromLocationComplete) && StockMovements.All(x => x.Qty.Equals(x.QtyProcessed)))
            {
                var isSave = await Util.Util.ShowErrorPopupPromptWithBeep("Do you want to complete stock movement?", "Yes", "No");
                if (isSave)
                {
                    IsBusy = true;
                    await AddStockMovementToSyncLog(StockMovements.ToList());
                    await App.Database.StockMovements.DeleteStockMovement();
                    IsBusy = false;
                    await App.Current.MainPage.Navigation.PopAsync();
                    return;
                }
                //var updatedStockMovements = StockMovements.ToList();
                //await App.Database.StockMovements.AddUpdateStockMovementList(updatedStockMovements);
            }
            else if (StockMovements.All(x => x.IsFromLocationComplete))
            {
                await Util.Util.ShowErrorPopupWithBeep("Scan all products to complete");
            }
            if (!StockMovements.All(x => x.IsFromLocationComplete))
            {
                var isSave = await Util.Util.ShowErrorPopupPromptWithBeep("Do you want to save 'stock movement from' progress and go to 'stock movement to' to ?", "Yes", "No");
                IsBusy = true;
                await Task.Delay(700);
                if (!isSave)
                {
                    await App.Database.StockMovements.DeleteStockMovement();
                }
                else
                {
                    //await App.Current.MainPage.Navigation.PopAsync();
                    //await App.Current.MainPage.Navigation.PushAsync(new StockMovementPage());
                    ScanText = "Scan To Location";
                    TitleText = "Scan Movement To";
                    ScanLocationCode = null;
                    SelectedStockLocation = null;
                    StockMovements.ToList().ForEach(x => x.IsFromLocationComplete = true);
                    var updatedStockMovements = StockMovements.ToList();
                    IsFromLocationComplete = true;
                    await App.Database.StockMovements.AddUpdateStockMovementList(updatedStockMovements);
                }
                IsBusy = false;
            }

        }

        private async Task AddStockMovementToSyncLog(List<StockMovementViewModel> stockMovements)
        {
            try
            {
                if (stockMovements != null && stockMovements.Any())
                {
                    var syncLog = new SyncLog();
                    syncLog.IsPost = true;
                    foreach (var stockMovement in stockMovements)
                    {
                        stockMovement.SerialIds = new List<int>();
                        stockMovement.PalletSerials = new List<StockMovementPalletSerialsViewModel>();
                        if (stockMovement.PalletSerials != null && stockMovement.PalletSerials.Any())
                        {
                            stockMovement.SerialIds = stockMovement.PalletSerials.Where(x => x.IsSerialised).Select(x => x.PalletSerialId).ToList();
                            stockMovement.PalletSerials = stockMovement.PalletSerials.Where(x => x.IsSerialised == false).ToList();
                        }
                    }
                    var stockMovementVmCollection = new StockMovemeneCollectionViewModel
                    {
                        StockMovements = stockMovements,
                        Count = stockMovements.Count(),
                        SerialNo = Modules.ModulesConfig.SerialNo,
                    };
                    syncLog.request = JsonConvert.SerializeObject(stockMovementVmCollection);
                    syncLog.RequestUrl = WebServiceConfig.PostStockMovement;
                    syncLog.TableName = Database.DatabaseConfig.Tables.StockMovementViewModel.ToString();
                    await App.Database.SyncLog.AddSyncLogItem(syncLog);
                    await App.StockMovements.SyncStockMovement();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
         
        }
    }
}

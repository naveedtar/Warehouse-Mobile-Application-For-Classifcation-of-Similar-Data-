using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Modules;
using WarehouseHandheld.Views.Pallets;
using Xamarin.Forms;

namespace WarehouseHandheld.ViewModels.Pallets
{
    public class PalletDispatchDetailPodViewModel : BaseViewModel
    {
        bool IsLoaded = false;

        public ICommand PalletInfoCommand { get; private set; }

        public PalletDispatchDetailPodViewModel()
        {
            PalletInfoCommand = new Command<PalletSync>(GetPalletInfo);
        }


        private ObservableCollection<PalletSync> pallets;
        public ObservableCollection<PalletSync> Pallets
        {
            get { return pallets; }
            set
            {
                pallets = value;
                OnPropertyChanged(nameof(Pallets));
            }
        }


        private Color palletDispatchColor;
        public Color PalletDispatchColor
        {
            get { return palletDispatchColor; }
            set
            {
                palletDispatchColor = value;
                OnPropertyChanged();
            }
        }

        private PalletDispatchSync palletDispatchObj;
        public PalletDispatchSync PalletDispatchObj
        {
            get { return palletDispatchObj; }
            set
            {
                palletDispatchObj = value;
                OnPropertyChanged();
            }
        }

        private string comments;
        public string Comments
        {
            get { return comments; }
            set
            {
                comments = value;
                OnPropertyChanged();
            }
        }

        private bool commentbox = false;
        public bool CommentBox
        {
            get { return commentbox; }
            set
            {
                commentbox = value;
                OnPropertyChanged();
            }
        }

        private PalletSync palletObjColor;
        public PalletSync PalletColor
        {
            get { return palletObjColor; }
            set
            {
                palletObjColor = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<PalletSync> scannedPallets = new ObservableCollection<PalletSync>();
        public ObservableCollection<PalletSync> ScannedPallets
        {
            get { return scannedPallets; }
            set
            {
                scannedPallets = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<PalletProductsDetail> palletProductDetailObj = new ObservableCollection<PalletProductsDetail>();
        public ObservableCollection<PalletProductsDetail> PalletProductDetailObj
        {
            get { return palletProductDetailObj; }
            set
            {
                palletProductDetailObj = value;
                OnPropertyChanged();
            }
        }



        public async Task ScanToBeDelivered(string code)
        {
            IsLoaded = true;
            if (!string.IsNullOrEmpty(code))
            {
                var scannedPallet = ScannedPallets.FirstOrDefault(x => x.PalletNumber.Equals(code));
                if (scannedPallet != null)
                {
                    await Util.Util.ShowErrorPopupWithBeep("Pallet already scanned.");
                    return;
                }
                var palletFound = Pallets.FirstOrDefault((x) => x.PalletNumber.Equals(code));
                if (palletFound != null)
                {
                    foreach (var pallet in Pallets)
                    {
                        if (pallet.PalletID.Equals(palletFound.PalletID))
                        {
                            PalletDispatchColor = Color.LightGreen;
                            pallet.SelectedColor = PalletDispatchColor;
                            PalletColor = pallet;
                            pallet.PalletNumber = PalletColor.PalletNumber;
                            ScannedPallets.Add(PalletColor);
                        }
                    }

                    return;
                }
                else
                {
                    await Util.Util.ShowErrorPopupWithBeep("No Pallet Found.");
                    return;
                }
            }
        }

        public async Task ScanTobeLoaded(string code)
        {
            IsLoaded = false;
            if (!string.IsNullOrEmpty(code))
            {
                var scannedPallet = ScannedPallets.FirstOrDefault(x => x.PalletNumber.Equals(code));
                if (scannedPallet != null)
                {
                    await Util.Util.ShowErrorPopupWithBeep("Pallet already scanned.");
                    return;
                }
                var palletFound = Pallets.FirstOrDefault((x) => x.PalletNumber.Equals(code));
                if (palletFound != null)
                {
                    foreach (var pallet in Pallets)
                    {
                        if (pallet.PalletID.Equals(palletFound.PalletID))
                        {
                            PalletDispatchColor = Color.LightGreen;
                            pallet.SelectedColor = PalletDispatchColor;
                            PalletColor = pallet;
                            pallet.PalletNumber = PalletColor.PalletNumber;
                            ScannedPallets.Add(PalletColor);
                        }
                    }

                    return;
                }
                else
                {
                    await Util.Util.ShowErrorPopupWithBeep("No Pallet Found.");
                    return;
                }
            }
        }


        private async void GetPalletInfo(PalletSync obj)
        {
            await PopupNavigation.PushAsync(new PalletDetailsPopup(obj));

        }



        public async Task SubmitPalletDispatch()
        {
            if (ScannedPallets.Count != Pallets.Count)
            {
                if (string.IsNullOrEmpty(Comments))
                {
                    if (ScannedPallets.Count != 0)
                    {
                        var response = await Application.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to complete without scanning all pallets?", "No", "Yes");
                        if (!response)
                        {
                            Util.Util.ShowErrorPopupWithBeep("Enter Comments");
                            CommentBox = true;
                        }
                        return;
                    }
                    else
                    {
                        Util.Util.ShowErrorPopupWithBeep("No Pallets Scanned");
                        return;
                    }
                }
                else
                {
                    Util.Util.ShowErrorPopupWithBeep("No Pallets Scanned");
                    return;
                }
            }
            if (ScannedPallets != null && ScannedPallets.Count != 0)
            {
                var palletDispatchProgress = new PalletDispatchProgress();
                palletDispatchProgress.DispatchId = PalletDispatchObj.PalletsDispatchID;
                palletDispatchProgress.CreatedBy = App.Users.LoggedInUserId;
                palletDispatchProgress.DateCreated = DateTime.UtcNow.Date;
                palletDispatchProgress.SerialNo = ModulesConfig.SerialNo;
                palletDispatchProgress.ScannedPalletSerials = new List<int>();
                palletDispatchProgress.TransactionLogId = Guid.NewGuid();
                palletDispatchProgress.Comments = comments;
                foreach (var pallet in ScannedPallets)
                {
                    if (pallet != null)
                    {
                        palletDispatchProgress.ScannedPalletSerials.Add(pallet.PalletID);
                    }
                }

              
                if (!IsLoaded)
                {
                    palletDispatchProgress.DispatchStatus = (int)Models.Enums.PalletDispatchStatusEnum.Loaded;

                    var response = await App.WarehouseService.Pallets.PostPalletsDispatchesAsync(palletDispatchProgress);
                    if (App.WarehouseService.Pallets.HandleConflictStatusPostDispatchPallet())
                    {
                        return;
                    }
                    if (response == null)
                    {
                        await Util.Util.ShowErrorPopupWithBeep("Some issue with pallet dispatch progress");
                        return;
                    }
                    else
                    {
                        PalletDispatchObj.DispatchStatus = (int)Models.Enums.PalletDispatchStatusEnum.Loaded;
                        await App.Database.PalletDispatch.AddUpdatePalletDispatch(PalletDispatchObj);
                        await App.Current.MainPage.Navigation.PopAsync();
                    }
                }
            }
        }




        public async Task Initialize()
        {

            var palletsInDb = await App.Database.Pallets.GetDispatchedPalletsSyncByDispatchedId(PalletDispatchObj.PalletsDispatchID);
            if (palletsInDb != null && palletsInDb.Count() != 0)
                Pallets = new ObservableCollection<PalletSync>(palletsInDb);
            else
            {
                await Util.Util.ShowErrorPopupWithBeep("No pallets exist for current Pallet dispatch");
                await App.Current.MainPage.Navigation.PopAsync();
            }
        }


        public async Task InitPalletDetail(PalletSync pallet)
        {
            if (pallet != null)
            {
                var palletProducts = await App.Pallets.GetPalletProductsByPalletId(pallet.PalletID);
                if (palletProducts != null && palletProducts.Count != 0)
                {
                    foreach (var palletProduct in palletProducts)
                    {
                        var product = await App.Products.GetProductById(palletProduct.ProductID);
                        if (product != null)
                        {
                            var palletProductDetails = new PalletProductsDetail();
                            palletProductDetails.Product = product;
                            palletProductDetails.PalletProduct = palletProduct;

                            PalletProductDetailObj.Add(palletProductDetails);
                        }

                    }
                }
                else
                {
                    "No products exist.".ToToast();
                }
            }
        }
    }
}

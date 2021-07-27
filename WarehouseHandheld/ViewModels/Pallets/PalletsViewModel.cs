using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Helpers;
using WarehouseHandheld.Models.Accounts;
using WarehouseHandheld.Models.OrderProcesses;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Modules;
using WarehouseHandheld.Resources;
using WarehouseHandheld.Views.Pallets;
using WarehouseHandheld.Views.Pallets.PalletOrder;
using Xamarin.Forms;

namespace WarehouseHandheld.ViewModels.Pallets
{
    public class PalletsViewModel : BaseViewModel
    {

        public bool IsAddPalletPopup { get; set; }


        private int orderProcessId;
        public int OrderProcessId
        {
            get { return orderProcessId; }
            set
            {
                orderProcessId = value;
                OnPropertyChanged();
            }
        }

        public bool isdispatchEnable = false;
        public bool IsDispatchEnable 
        {
            get { return isdispatchEnable; }
            set
            {
                isdispatchEnable = value;
                OnPropertyChanged();
            }
        }

        private OrderProcessSync orderProcess;
        public OrderProcessSync OrderProcess
        {
            get { return orderProcess; }
            set
            {
                orderProcess = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<PalletSync> pallets;
        public ObservableCollection<PalletSync> Pallets
        {
            get { return pallets; }
            set
            {
                pallets = value;
                OnPropertyChanged();
            }
        }



        private List<AccountSync> accounts;
        public List<AccountSync> Accounts
        {
            get { return accounts; }
            set
            {
                accounts = value;
                OnPropertyChanged();
            }
        }

        public ICommand DispatchCommand { get; private set; }
        protected CommandLockerHelper DispatchCommandLocker => new CommandLockerHelper(() => { DispatchPallet(); });

        public ICommand AddPalletCommand { get; private set; }
        protected CommandLockerHelper AddPalletCommandLocker => new CommandLockerHelper(() => { AddPallet(); });

        public ICommand ItemSelectedCommand => new Command(SelectedCommandLocker.Execute);
        protected CommandLockerHelper SelectedCommandLocker => new CommandLockerHelper(async (e) => { await OnItemSelected(e); });


        private async Task OnItemSelected(object e)
        {
            if (((SelectedItemChangedEventArgs)e).SelectedItem != null)
            {
                var pallet = ((PalletSync)((SelectedItemChangedEventArgs)e).SelectedItem);
                await App.Current.MainPage.Navigation.PushAsync(new PalletOrdersPage(pallet,OrderProcess));
                if (IsAddPalletPopup)
                {
                    await PopupNavigation.PopAsync(false);
                }
            }
        }

        public PalletsViewModel()
        {
            DispatchCommand = new Command(DispatchCommandLocker.Execute);
            AddPalletCommand = new Command(AddPalletCommandLocker.Execute);
        }

        private async void AddPallet()
        {
            var process = await OnPalletAdd(OrderProcess);
        }

        async void DispatchPallet()
        {
            var selectedPallets = new List<PalletSync>();
            foreach (var pallet in Pallets)
            {
                if (!pallet.IsDispatched)
                {
                    selectedPallets.Add(pallet);
                }
            }

            if (selectedPallets.Count == 0)
                await Util.Util.ShowErrorPopupWithBeep("No Pallets Founds.");
            else
            {
                var confirm = await Util.Util.ShowErrorPopupPromptWithBeep("Are you sure you want to dispatch selected pallet?", "Ok", "Cancel");
                if (confirm)
                {
                    if (CrossConnectivity.Current.IsConnected && await Util.Util.IsConnected())
                    {
                        var PalletDispatchPopup = new PalletDispatchPopup();
                        PalletDispatchPopup.ViewModel.OnDispatch += async (List<byte[]> ImagesList, PalletDispatchSync DispatchInfo, string FileName) =>
                        {
                            IsBusy = true;
                            byte[][] ImagesArray = new byte[ImagesList.Count][];

                            for (int i = 0; i < ImagesList.Count; i++)
                            {
                                ImagesArray[i] = ImagesList[i];
                            }
                            Guid guid = Guid.NewGuid();
                            foreach (var pallet in selectedPallets)
                            {
                                pallet.ProofOfLoadingImageBytes = ImagesArray;
                                pallet.PalletDispatchInfo = DispatchInfo;
                                pallet.TransactionLogId = guid;
                                pallet.SerialNumber = ModulesConfig.SerialNo;
                            }
                            foreach (var pallet in selectedPallets)
                            {
                                pallet.SelectedPallets = new List<int>();
                                foreach (var selectedPallet in selectedPallets)
                                {
                                    pallet.SelectedPallets.Add(selectedPallet.PalletID);
                                }
                            }
                            try
                            {
                                var dispatchedPallets = await App.Pallets.DispatchPallet(selectedPallets);
                                foreach (var pallet in dispatchedPallets)
                                {
                                    if (pallet != null && pallet.IsDispatched)
                                    {
                                        "Pallet Dispatched Successfully.".ToToast();
                                        await App.Pallets.SyncPallets();
                                        await App.OrderProcesses.SyncOrderProcesses();
                                        //var AllPallets = (await App.Pallets.GetPallets()).FindAll((obj) => !obj.IsDispatched);
                                        //Pallets = new ObservableCollection<PalletSync>(AllPallets);
                                        await App.Current.MainPage.Navigation.PopAsync();
                                    }
                                    else
                                    {
                                        "There is some error in dispatching some pallets.".ToToast();
                                    }

                                }

                            }
                            finally
                            {
                                IsBusy = false;
                            }

                        };
                        await PopupNavigation.PushAsync(PalletDispatchPopup);

                    }
                }


                else if (!CrossConnectivity.Current.IsConnected)
                {
                    AppStrings.NoInternet.ToToast();
                }


            }
        }



        public async void Initialize()
        {
            IsBusy = true;
            try
            {
                Pallets = new ObservableCollection<PalletSync>((await App.Pallets.GetPalletsByOrderId(OrderProcess.OrderProcessID)).FindAll((obj) => !obj.IsDispatched));
                //Accounts = await App.Accounts.GetAllAccounts();

                var orderWithDetailList = await App.Orders.GetOrderDetailsForPallets((int)OrderProcess.OrderID);
                if (orderWithDetailList != null && orderWithDetailList.Count != 0)
                {
                    IsDispatchEnable = false;
                    foreach (var orderWithDetail in orderWithDetailList)
                    {
                        if (Pallets.Count != 0 && orderWithDetail.Quantity > 0 && orderWithDetail.Quantity <= orderWithDetail.QuantityProcessed)
                        {
                            IsDispatchEnable = true;
                        }
                    }
                }
              
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<PalletSync> OnPalletAdd(OrderProcessSync orderProcess)
        {
            if (orderProcess != null)
            {
                try
                {
                    IsBusy = true;
                    var response = await App.Pallets.AddPallet(orderProcess.OrderProcessID);
                    if (response != null)
                    {
                        PalletSync pallet = new PalletSync();
                        pallet.PalletID = response.SelectedPalletID;
                        pallet.OrderProcessID = orderProcess.OrderProcessID;
                        pallet.RecipientAccountID = orderProcess.AccountID ?? 0;
                        pallet.PalletNumber = response.NextPalletNumber;
                        pallet.DateCreated = DateTime.UtcNow;
                        await App.Pallets.AddPalletInDB(pallet);
                        return pallet;
                    }
                  
                }
                finally
                {
                    Device.BeginInvokeOnMainThread(() => {
                        Initialize();
                    });
                    IsBusy = false;
                }
            }
            return null;
        }
    }
}

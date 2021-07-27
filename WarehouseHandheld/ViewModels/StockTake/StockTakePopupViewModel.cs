using System;
using Xamarin.Forms;
using WarehouseHandheld.Models.Products;
using System.Threading.Tasks;

namespace WarehouseHandheld.ViewModels.StockTake
{
    public class StockTakePopupViewModel : BaseViewModel
    {
        public Action<bool> Back;
        public Action<ProductMasterSync,decimal,string,string,string, DateTime?,bool> OnSave;

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

        private decimal quantity;
        public decimal Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                OnPropertyChanged();
            }
        }

        private string serialNumber;
        public string SerialNumber
        {
            get { return serialNumber; }
            set
            {
                serialNumber = value;
                OnPropertyChanged();
            }
        }

        private string palletSerial;
        public string PalletSerial
        {
            get { return palletSerial; }
            set
            {
                palletSerial = value;
                OnPropertyChanged();
            }
        }

        private bool existingPallet;
        public bool ExistingPallet
        {
            get { return existingPallet; }
            set
            {
                existingPallet = value;
                OnPropertyChanged();
            }
        }
        private string batchNumber;
        public string BatchNumber
        {
            get { return batchNumber; }
            set
            {
                batchNumber = value;
                OnPropertyChanged();
            }
        }

        private DateTime? expiryDate = DateTime.Today.Date;
        public DateTime? ExpiryDate
        {
            get { return expiryDate; }
            set
            {
                expiryDate = value;
                OnPropertyChanged();
            }
        }

        public async Task<bool> OnSaveClicked()
        {
    
            if ((bool)Product.RequiresBatchNumberOnReceipt && string.IsNullOrEmpty(BatchNumber))
            {
                await Util.Util.ShowErrorPopupWithBeep("Please enter batch number.");
                return false;
            }

            if ((bool)Product.RequiresExpiryDateOnReceipt && ExpiryDate <= DateTime.Today.Date)
            {
                await Util.Util.ShowErrorPopupWithBeep("Expiry date must be greater than today.");
                return false;
            }
            OnSave?.Invoke(Product, Quantity, SerialNumber, PalletSerial, BatchNumber, expiryDate, ExistingPallet);
            Back?.Invoke(true);
            return true;

        }

        public async Task<bool> OnCancelClicked()
        {
            Back?.Invoke(true);
            return true;
        }

    }
}



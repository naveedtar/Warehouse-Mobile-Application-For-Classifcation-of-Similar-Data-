using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Models.Products;
using WarehouseHandheld.Models.StockTakes;
using WarehouseHandheld.Views.Base.Popup;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.StockTake
{
    public partial class StockTakeQuantityPopup : PopupBase
    {
        public Action<double, string, DateTime?> SaveQuantity;
        public Action<bool> Cancel;
        private StockTakeScanProduct _product;
        private DateTime? expirydate;
        private string batchnumber;

        public StockTakeQuantityPopup(double quantity, StockTakeScanProduct product)
        {
            InitializeComponent();
            _product = product;
            Initialize(quantity);
            OnSaveClicked += OnSave;
            OnCancelClicked += OnCancel;
            Title = product.Product.ProcessByCase ? "Update Cases" : "Update Quantity";
            if (!string.IsNullOrEmpty(product.BatchNumber))
            {
                BatchNumber.Text = product.BatchNumber;
            }
            if (product.ExpiryDate != null)
            {
                ExpiryDatePicker.Date = (DateTime)product.ExpiryDate;
            }
            else
            {
                ExpiryDatePicker.Date = DateTime.Today.Date;
            }

            if (!_product.Product.Serialisable && !_product.Product.ProcessByPallet)
            {
                if (_product.Product.RequiresBatchNumberOnReceipt ?? false)
                {
                    BatchNumberLabel.IsVisible = true;
                    BatchNumber.IsVisible = true;
                }

                if (_product.Product.RequiresExpiryDateOnReceipt ?? false)
                {
                    ExpiryDateLabel.IsVisible = true;
                    ExpiryDatePicker.IsVisible = true;
                }
            }

        }

        async void OnSave()
        {
            if (!_product.Product.Serialisable && !_product.Product.ProcessByPallet && _product.Product.RequiresBatchNumberOnReceipt == true)
            {
                if (string.IsNullOrEmpty(BatchNumber.Text))
                {
                    await Util.Util.ShowErrorPopupWithBeep("Enter Batch Number");
                    SaveButtonEnabled = true;
                    return;
                }
                else
                {
                    batchnumber = BatchNumber.Text;
                }
            }

            if (!_product.Product.Serialisable && !_product.Product.ProcessByPallet && _product.Product.RequiresExpiryDateOnReceipt == true)
            {
                if (ExpiryDatePicker.Date == null || ExpiryDatePicker.Date <= DateTime.Today.Date)
                {
                    await Util.Util.ShowErrorPopupWithBeep("Expiry date must be greater than today.");
                    SaveButtonEnabled = true;
                    return;
                }
                else
                {
                    expirydate = ExpiryDatePicker.Date;
                }
            }
            else
            {
                expirydate = null;
            }
            SaveQuantity?.Invoke((double)Cases, batchnumber, expirydate);
            PopupNavigation.PopAsync();
        }

        void OnCancel()
        {
            Cancel?.Invoke(true);
            PopupNavigation.PopAsync();
        }

        void Handle_Toggled(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            CasesLabel.IsVisible = !e.Value;
            CasesStepper.IsVisible = !e.Value;
            QuantityLabel.IsVisible = e.Value;
            QuantityStepper.IsVisible = e.Value;
        }

        private void Initialize(double quantity)
        {
            PerCaseText.Text = _product.Product.ProductsPerCase.ToString();
            if (_product.Product.ProcessByCase)
            {
                Cases = quantity;
                CasesStepper.Value = quantity;
                CasesLabel.IsVisible = true;
                CasesStepper.IsVisible = true;
                QuantityLabel.IsVisible = false;
                QuantityStepper.IsVisible = false;
            }
            else
            {
                Quantity = cases;
                QuantityStepper.Value = quantity;
                CasesLabel.IsVisible = false;
                CasesStepper.IsVisible = false;
                QuantityLabel.IsVisible = true;
                QuantityStepper.IsVisible = true;
            }
        }

        private double quantity;
        public double Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                if (_product.Product.ProductsPerCase != null)
                {
                    cases = Math.Round(value / (double)_product.Product.ProductsPerCase, 2);
                    OnPropertyChanged(nameof(Cases));
                }
                else
                {
                    cases = value;
                }
                Progress = "Cases " + Cases + " , Products " + value;

                OnPropertyChanged();
            }
        }

        private string progress;
        public string Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                OnPropertyChanged();
            }
        }
        private double cases;
        public double Cases
        {
            get { return cases; }
            set
            {
                Progress = "Cases " + value + " , Products " + Quantity;
                if (_product.Product.ProductsPerCase != null)
                    Quantity += Math.Round((value - cases) * (double)_product.Product.ProductsPerCase);
                else
                    Quantity++;
                cases = value;
                OnPropertyChanged();
            }
        }

    }
}

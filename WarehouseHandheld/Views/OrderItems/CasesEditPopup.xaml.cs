using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Services;
using WarehouseHandheld.Models.Pallets;
using WarehouseHandheld.Views.Base.Popup;
using Xamarin.Forms;

namespace WarehouseHandheld.Views.OrderItems
{
    public partial class CasesEditPopup : PopupBase
    {
        public Action<decimal> SaveCases;
        PalletSerial pallet;

        public CasesEditPopup(PalletSerial pallet)
        {
            InitializeComponent();
            this.pallet = pallet;
            if (this.pallet != null)
            {
                casesEntry.Text = pallet.Cases.ToString("F");
                CasesEntryLabel.Text = "Total Cases ";
                OnSaveClicked += async () =>
                {
                    decimal quantity = 0;
                    decimal newQuantity = 0;
                    if (!string.IsNullOrEmpty(casesEntry.Text) && Decimal.TryParse(casesEntry.Text, out quantity))
                    {
                        newQuantity = Convert.ToDecimal(quantity);
                    }
                    var totalQtyProcessed = pallet.OrderQuantityProcessed - (pallet.Cases*pallet.prdPerCase);
                    if (newQuantity <= 0)
                    {
                        await Util.Util.ShowErrorPopupWithBeep("Pallet cases field can't be empty or zero.");
                        SaveButtonEnabled = true;
                        return;
                    }

                    if (newQuantity <= pallet.Cases + pallet.RemainingCases)
                    {
                        if (((newQuantity * pallet.prdPerCase) + totalQtyProcessed) <= pallet.OrderQuantity)
                        {
                            SaveCases?.Invoke(newQuantity);
                            await PopupNavigation.PopAsync();
                        }
                        else
                        {
                            await Util.Util.ShowErrorPopupWithBeep("Maximum " + ((pallet.OrderQuantity - totalQtyProcessed)/pallet.prdPerCase) + " more cases are needed in this order.");
                            SaveButtonEnabled = true;
                            return;
                        }

                    }
                    else
                    {
                        await Util.Util.ShowErrorPopupWithBeep("Selected No. of cases must be less than total cases.");
                        SaveButtonEnabled = true;
                        return;
                    }
                };
            }
        }

        // Not in Use
        public CasesEditPopup(decimal cases)
        {
            InitializeComponent();
            casesEntry.Text = cases.ToString("F");
            CasesEntryLabel.Text = "Total Cases ";
            OnSaveClicked += () => {
                SaveCases?.Invoke(Convert.ToDecimal(casesEntry.Text));
                PopupNavigation.PopAsync();
            };
        }

        public CasesEditPopup(decimal cases, bool IsWastages)
        {
            InitializeComponent();
            CasesInput.Text =  cases.ToString("F");
            casesEntry.Placeholder = "0.00";
            CasesEntryLabel.Text = "Enter cases";
            returnStack.IsVisible = true;
            if (IsWastages)
            {
                Totalcases.Text = "Remaining Cases";
            }
            else
            {
                Totalcases.Text = "Cases Sold";
            }

            OnSaveClicked += () => {
                if (Convert.ToDecimal(casesEntry.Text) <= 0)
                {
                    Util.Util.ShowErrorPopupWithBeep("Pallet cases field can't be empty or zero.");
                    SaveButtonEnabled = true;
                    return;
                }
                if (Convert.ToDecimal(casesEntry.Text) <= Convert.ToDecimal(CasesInput.Text))
                {
                    SaveCases?.Invoke(Convert.ToDecimal(casesEntry.Text));
                    PopupNavigation.PopAsync();
                }
                else
                {
                    Util.Util.ShowErrorPopupWithBeep("Selected No. of cases must be less than " + Totalcases.Text);
                    SaveButtonEnabled = true;
                    return;
                }
            };
        }
        
        
        public CasesEditPopup(decimal cases, decimal stockFromCases, bool IsWastages)
        {
            InitializeComponent();
            CasesInput.Text =  cases.ToString("F");
            casesEntry.Placeholder = "0.00";
            CasesEntryLabel.Text = "Enter cases";
            returnStack.IsVisible = true;
            if (IsWastages)
            {
                Totalcases.Text = "Remaining Cases";
            }
            else
            {
                Totalcases.Text = "Cases Sold";
            }

            casesEntry.Text = stockFromCases.ToString();
            OnSaveClicked += () => {
                if (Convert.ToDecimal(casesEntry.Text) <= 0)
                {
                    Util.Util.ShowErrorPopupWithBeep("Pallet cases field can't be empty or zero.");
                    SaveButtonEnabled = true;
                    return;
                }
                if (Convert.ToDecimal(casesEntry.Text) <= Convert.ToDecimal(CasesInput.Text))
                {
                    SaveCases?.Invoke(Convert.ToDecimal(casesEntry.Text));
                    PopupNavigation.PopAsync();
                }
                else
                {
                    Util.Util.ShowErrorPopupWithBeep("Selected No. of cases must be less than " + Totalcases.Text);
                    SaveButtonEnabled = true;
                    return;
                }
            };
        }

    }
}

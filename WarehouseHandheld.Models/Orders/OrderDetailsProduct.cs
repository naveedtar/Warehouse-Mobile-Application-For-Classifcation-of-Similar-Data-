using System;
using WarehouseHandheld.Models.Products;
using System.ComponentModel;
using WarehouseHandheld.Models.InventoryStocks;
using WarehouseHandheld.Models.ProductStockLocation;
using WarehouseHandheld.Models.StockMovement;
using Xamarin.Forms;

namespace WarehouseHandheld.Models.Orders
{
    public class OrderDetailsProduct : INotifyPropertyChanged
    {
        public OrderDetailSync OrderDetails { get; set; }
        public ProductMasterSync Product { get; set; }
        public InventoryStockSync InventoryStock { get; set; }
        public ProductLocationStocksSync ProductLocation { get; set; }
        public string BatchNumber { get; set; }
        public string LocationCode { get; set; }
        public DateTime? ExpiryDate { get; set; }

        //Kit Properties Start
        public OrderDetailSync KitOrderDetail { get; set; }
        public bool IsProductKit { get; set; }
        public bool IsProductInKit { get; set; }
        public decimal KitQuantity { get; set; }
        //Kit Properties End

        private decimal quantityProcessed;
        public decimal QuantityProcessed { get { return quantityProcessed; }
            set
            {
                quantityProcessed = value;
             
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(QuantityProcessed)));
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(RowColor)));
                }
            }
        }

        private decimal boxesRemaining;
        public decimal BoxesRemaining
        {
            get { return boxesRemaining; }
            set
            {
                boxesRemaining = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(BoxesRemaining)));
                }
            }
        }

        private decimal quantity;
        public decimal Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(Quantity)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public Color RowColor
        {
            get
            {
                if (IsProductInKit)
                {
                    return this.OrderDetails.Qty == this.QuantityProcessed ? Color.PaleGreen : Color.FromHex("#ffffeb");
                }
                else
                {
                    return this.OrderDetails.Qty == this.QuantityProcessed ? Color.LightGreen : Color.Transparent;
                }
            }
        }

        public int KitMeanQuantity
        {
            get
            {
                if (IsProductInKit)
                {
                    return (int)(this.QuantityProcessed / this.KitQuantity);
                }
                return 0;
            }
        }
    }
}

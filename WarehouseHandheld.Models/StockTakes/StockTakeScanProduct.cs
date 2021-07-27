using System;
using System.ComponentModel;
using WarehouseHandheld.Models.Products;

namespace WarehouseHandheld.Models.StockTakes
{
    public class StockTakeScanProduct : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ProductMasterSync Product { get; set; }
        public int StockDetailId { get; set; }
        public int StockTakeRequestId { get; set; }
        public string Serial { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }



        private decimal cases;
        public decimal Cases
        {
            get { return cases; }
            set
            {
                cases = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(Cases)));
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
    }
}

using System;
using SQLite;

namespace WarehouseHandheld.Models.Accounts
{
    public class AccountSync
    {
        [PrimaryKey]
        public int AccountID { get; set; }
        public string AccountCode { get; set; }
        public string CompanyName { get; set; }
        public string CountryName { get; set; }
        public string CurrencyName { get; set; }
        public int AccountStatusID { get; set; }
        public string PriceGroupName { get; set; }
        public int PriceGroupID { get; set; }
        public string VATNo { get; set; }
        public string RegNo { get; set; }
        public string Comments { get; set; }
        public string AccountEmail { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
        public string website { get; set; }
        public double? CreditLimit { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public bool? IsDeleted { get; set; }
        public int TenantId { get; set; }
        public bool AccountTypeCustomer { get; set; }
        public bool AccountTypeSupplier { get; set; }
        public bool AccountTypeEndUser { get; set; }
        public int OwnerUserId { get; set; }
        public decimal FinalBalance { get; set; }
        public bool CashOnlyAccount { get; set; }
        public int? AcceptedShelfLife { get; set; }
    }
}

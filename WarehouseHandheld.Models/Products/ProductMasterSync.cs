﻿using System;
using System.Collections.Generic;
using SQLite;

namespace WarehouseHandheld.Models.Products
{
    public class ProductMasterSync
    {
        [PrimaryKey]
        public int ProductId { get; set; }
        public string SKUCode { get; set; }
        public string SecondCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BarCode { get; set; }
        public string BarCode2 { get; set; }
        public bool Serialisable { get; set; }
        public bool? AllowZeroSale { get; set; }
        public int? ShelfLifeDays { get; set; }
        public decimal? ReorderQty { get; set; }
        public string ShipConditionCode { get; set; }
        public string CommodityCode { get; set; }
        public string CommodityClass { get; set; }
        public decimal? BuyPrice { get; set; }
        public decimal? LandedCost { get; set; }
        public decimal? SellPrice { get; set; }
        public decimal? MinThresholdPrice { get; set; }
        public decimal PercentMargin { get; set; }
        public ProductKitTypeEnum ProductType { get; set; }
        public bool IsActive { get; set; }
        public DateTime ProdStartDate { get; set; }
        public bool Discontinued { get; set; }
        public DateTime? DiscontDate { get; set; }
        public string DepartmentName { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductCategoryName { get; set; }
        public bool? RequiresBatchNumberOnReceipt { get; set; }
        public bool? RequiresExpiryDateOnReceipt { get; set; }
        public string ProductNotes { get; set; }
        public int? ProductsPerCase { get; set; }
        public int? CasesPerPallet { get; set; }
        public bool IsStockItem { get; set; }
        public bool IsRawMaterial { get; set; }
        public bool? EnableWarranty { get; set; }
        public bool? EnableTax { get; set; }
        public bool ProcessByCase { get; set; }
        public bool ProcessByPallet { get; set; }
        public bool? IsDeleted { get; set; }
        public int TaxID { get; set; }
        public int TaxPercent { get; set; }
        public bool AllowModifyPrice { get; set; }
        [IgnoreAttribute]
        public List<ProductKitMapViewModel> ProductKitMapViewModelList { get; set; }
        public string MainImage { get; set; }
        [IgnoreAttribute]
        public List<string> ProductTags { get; set; }
    }
}

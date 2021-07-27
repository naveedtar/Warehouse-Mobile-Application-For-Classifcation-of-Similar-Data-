using System;
using System.Collections.Generic;
using SQLite;

namespace WarehouseHandheld.Models.Orders
{
    public class OrdersSync
    {
        public OrdersSync()
        {
            OrderDetails = new List<OrderDetailSync>();
        }

        [PrimaryKey]
        public int OrderID { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string Note { get; set; }
        public int InventoryTransactionTypeId { get; set; }
        public int? AccountID { get; set; }
        public int? JobTypeId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public DateTime? CancelDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? ConfirmBy { get; set; }
        public int? CancelBy { get; set; }
        public int TenentId { get; set; }
        public bool IsCancel { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int OrderStatusID { get; set; }
        public int? LoanID { get; set; }
        // Account contact person for this specific order
        public int? AccountContactId { get; set; }
        public decimal OrderTotal { get; set; }
        // flag for Posted into accounting software eg. Sage
        public bool Posted { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDetails { get; set; }
        public decimal? OrderCost { get; set; }
        public int? ReportTypeId { get; set; }
        public int? ReportTypeChargeId { get; set; }
        public int? TransferWarehouseId { get; set; }
        public string TransferWarehouseName { get; set; }
        public int? DepartmentId { get; set; }
        public int? SLAPriorityId { get; set; }
        public short? ExpectedHours { get; set; }
        public DateTime? AuthorisedDate { get; set; }
        public int? AuthorisedUserID { get; set; }
        public string AuthorisedNotes { get; set; }
        public int? WarehouseId { get; set; }
        public string ShipmentAddressLine1 { get; set; }
        public string ShipmentAddressLine2 { get; set; }
        public string ShipmentAddressLine3 { get; set; }
        public string ShipmentAddressLine4 { get; set; }
        public string ShipmentAddressPostcode { get; set; }
        public int? PPropertyId { get; set; }
        public int? ShipmentPropertyId { get; set; }
        public Guid? OrderGroupToken { get; set; }
        public int? ShipmentWarehouseId { get; set; }
        public bool IsShippedToTenantMainLocation { get; set; }
        public string CustomEmailRecipient { get; set; }
        public string CustomCCEmailRecipient { get; set; }
        public string CustomBCCEmailRecipient { get; set; }
        public int? AccountCurrencyID { get; set; }
        public int? JobSubTypeId { get; set; }
        public bool? DirectShip { get; set; }
        public string RequestStatus { get; set; }
        public bool RequestSuccess { get; set; }
        public int? PickerId { get; set; }
        [IgnoreAttribute]
        public SerialProcessStatus SerialProcessStatus { get; set; }
        [IgnoreAttribute]
        public List<OrderDetailSync> OrderDetails { get; set; }


        public enum OrderStatusEnum
        {
            Active = 1,
            Complete = 2,
            Hold = 3,
            Pending = 4,
            NotScheduled = 5,
            Scheduled = 6,
            ReAllocationRequired = 7,
            AwaitingAuthorisation = 8,
            Cancelled = 9,
            BeingPicked = 10,
            AwaitingArrival = 11,
            Approved = 12,
            PostedToAccounts = 13,
            Invoiced = 14
        }

        public enum OrderProcessStatusEnum
        {
            Active = 1,
            Complete = 2,
            Dispatched = 3,
            Loaded = 4,
            Delivered = 5,
            Invoiced = 6,
            PostedToAccounts = 7
        }

        public enum InventoryTransactionTypeEnum
    {
        PurchaseOrder = 1,
        SaleOrder = 2,
        TransferIn = 3,
        TransferOut = 4,
        Allocated = 5,
        AdjustmentIn = 6,
        AdjustmentOut = 7,
        WorkOrder = 8,
        Proforma = 9,
        Quotation = 10,
        Loan = 11,
        Returns = 12,
        Samples = 13,
        Wastage = 14,
        DirectSales = 15,
        Exchange = 16,
        WastedReturn = 17
    }

    }

    public class SerialProcessStatus
    {
        public SerialProcessStatus()
        {
            ProcessedSerials = new List<string>();
            RejectedSerials = new List<string>();
        }

        public List<string> ProcessedSerials { get; set; }
        public List<string> RejectedSerials { get; set; }
    }

}

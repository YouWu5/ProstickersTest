using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ProStickers.CloudDAL.Entity.Customer
{
    public class OrderEntity : TableEntity
    {
        public OrderEntity()
        {

        }

        public OrderEntity(string date, string orderNumber)
        {
            PartitionKey = date;
            RowKey = orderNumber;
        }
        public string AppointmentNumber { get; set; }
        public int OrderNumber { get; set; }
        public string DesignNumber { get; set; }
        public string ImageSize { get; set; }
        public string ColorJSON { get; set; }
        public int PurchaseTypeID { get; set; }
        public double ShippingPrice { get; set; }
        public double VectorFilePrice { get; set; }
        public double DesignImagePrice { get; set; }
        public double Amount { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCodeName { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }      
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }       
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public int StateID { get; set; }
        public string StateName { get; set; }
        public string PostalCode { get; set; }
        public int OrderStatusID { get; set; }
        public string OrderStatus { get; set; }
        public string TrackingNumber { get; set; }
        public bool IsfeedbackDone { get; set; }
        public int StatusID { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public DateTime OrderDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTS { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTS { get; set; }
        public int Quantity { get; set; }

        public string CardNo { get; set; }
        public string NameOnCard { get; set; }
        public string StripeCardID { get; set; }
        public string PaymentTransactionID { get; set; }
        public string StripeCustomerID { get; set; }

    }
}

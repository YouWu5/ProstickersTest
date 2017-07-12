using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ProStickers.CloudDAL.Entity.Designer
{
    public class DesignerOrderEntity : TableEntity
    {
        public DesignerOrderEntity()
        {

        }

        public DesignerOrderEntity(string userID, string odrNumber)
        {
            PartitionKey = userID;
            RowKey = odrNumber;
        }

        public int OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string DesignNumber { get; set; }
        public string AppointmentNumber { get; set; }
        public int OrderStatusID { get; set; }
        public string OrderStatus { get; set; }
        public double ShippingPrice { get; set; }
        public double VectorFilePrice { get; set; }
        public double DesignImagePrice { get; set; }
        public double Amount { get; set; }
        public string UserName { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCodeName { get; set; }
        public string TrackingNumber { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTS { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTS { get; set; }
        public int Quantity { get; set; }
    }
}

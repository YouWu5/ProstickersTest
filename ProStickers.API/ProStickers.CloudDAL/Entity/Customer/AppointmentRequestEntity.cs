using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ProStickers.CloudDAL.Entity.Customer
{
    public class AppointmentRequestEntity : TableEntity
    {
        public AppointmentRequestEntity()
        {

        }

        public AppointmentRequestEntity(string date)
        {
            PartitionKey = date;
            RowKey = (DateTime.UtcNow.Ticks).ToString("d19");
        }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCodeName { get; set; }
        public int RequestNumber { get; set; }
        public int RequestStatusID { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailID { get; set; }
        public string RequestedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTS { get; set; }
        public double StartTime { get; set; }
    }
}

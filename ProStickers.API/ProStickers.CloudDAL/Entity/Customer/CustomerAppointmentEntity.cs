using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ProStickers.CloudDAL.Entity.Customer
{
    public class CustomerAppointmentEntity : TableEntity
    {
        public CustomerAppointmentEntity()
        {

        }


        public CustomerAppointmentEntity(string customerID, string appNumber)
        {
            this.PartitionKey = customerID;
            this.RowKey = appNumber;
        }

        public string AppointmentNumber { get; set; }
        public string Date { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public double AspectRatio { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string RequestDate { get; set; }
        public int RequestTime { get; set; }
        public int AppointmentStatusID { get; set; }
        public string AppointmentStatus { get; set; }
        public string CancellationReason { get; set; }
        public string RequestDateTime { get; set; }
        public string DesignNumber { get; set; }
        public string AppointmentDateTime { get; set; }
        public int StatusID { get; set; }
        public int ImageStatusID { get; set; }
        public string MeetingLink { get; set; }
        public string DesignerNote { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTS { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTS { get; set; }
    }
}

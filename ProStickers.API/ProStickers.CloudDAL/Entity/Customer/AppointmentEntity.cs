using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ProStickers.CloudDAL.Entity.Customer
{
    public class AppointmentEntity : TableEntity
    {
        public AppointmentEntity()
        {

        }
        public AppointmentEntity(string date, string appointmentNumber)
        {
            PartitionKey = date;
            RowKey = appointmentNumber;

        }
        public int TimeSlotID { get; set; }
        public string TimeSlot { get; set; }
        public int AppointmentStatusID { get; set; }
        public string AppointmentStatus { get; set; }
        public string CancellationReason { get; set; }
        public string CustomerID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string DesignNumber { get; set; }
        public int StatusID { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public bool IsCalendarRequest { get; set; }
        public string RequestDate { get; set; }
        public int RequestTime { get; set; }      
        public string RequestDateTime { get; set; }
        public double AspectRatio { get; set; }
        public string AppointmentDateTime { get; set; }
        public int ImageStatusID { get; set; }
        public string MeetingLink { get; set; }
        public string DesignerNote { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTS { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTS { get; set; }    
    }
}

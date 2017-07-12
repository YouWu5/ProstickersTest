using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ProStickers.CloudDAL.Entity.Designer
{
    public class DesignerAppointmentEntity : TableEntity
    {
        public DesignerAppointmentEntity()
        {

        }

        public DesignerAppointmentEntity(string userID, string appNumber)
        {
            this.PartitionKey = userID;
            this.RowKey = appNumber; //(DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
        }

        public string AppointmentNumber { get; set; }
        public string Date { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public string CustomerID { get; set; }
        public string CustomerCodeName { get; set; }
        public string CustomerName { get; set; }
        public string UserName { get; set; }
        public string ContactNumber { get; set; }
        public string RequestDate { get; set; }
        public int RequestTime { get; set; }
        public int AppointmentStatusID { get; set; }
        public string AppointmentStatus { get; set; }
        public string RequestDateTime { get; set; }
        public string AppointmentDateTime { get; set; }
        public string DesignNumber { get; set; }
        public double AspectRatio { get; set; }
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

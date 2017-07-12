using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace ProStickers.CloudDAL.Entity.Designer
{
    public class DesignerTimeSlotEntity : TableEntity
    {
        public DesignerTimeSlotEntity()
        {

        }

        public DesignerTimeSlotEntity(string userID, string datetimeslotID)
        {
            PartitionKey = userID;
            RowKey = datetimeslotID;
        }
        public int TimeSlotID { get; set; }
        public string Name { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public int TimeSlotStatus { get; set; }
        public int AppointmentCount { get; set; }
        public int AppointmentStatus { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsUserAvailable { get; set; }   
        public DateTime CreatedTS { get; set; }
        public DateTime UpdatedTS { get; set; }
        public string Date { get; set; }  
        public int StartTime { get; set; }
        public int EndTime { get; set; }
    }
}

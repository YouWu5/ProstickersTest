using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ProStickers.CloudDAL.Entity.Master
{
    public class UserFilesEntity : TableEntity
    {
        public UserFilesEntity()
        {

        }

        public UserFilesEntity(string userID)
        {
            PartitionKey = userID;
            RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
        }

        public string CustomerID { get; set; }
        public string AppointmentNumber { get; set; }
        public string FileNumber { get; set; } // UserID_FileName
        public double UploadedFileSize { get; set; }
        public DateTime CreatedTS { get; set; }
        public DateTime UpdatedTS { get; set; }

    }
}

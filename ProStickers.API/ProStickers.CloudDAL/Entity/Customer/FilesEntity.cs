using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ProStickers.CloudDAL.Entity.Customer
{
    public class FilesEntity : TableEntity
    {
        public FilesEntity()
        {

        }

        public FilesEntity(string customerID)
        {
            PartitionKey = customerID;
            RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
        }

        public string FileNumber { get; set; } // CustomerID_FileName
        public double FileSize { get; set; }
        public DateTime CreatedTS { get; set; }
        public DateTime UpdatedTS { get; set; }
    }
}

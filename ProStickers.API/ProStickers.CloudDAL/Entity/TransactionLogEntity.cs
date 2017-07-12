using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ProStickers.CloudDAL.Entity
{
    public class TransactionLogEntity : TableEntity
    {
        public TransactionLogEntity() { }

        public TransactionLogEntity(string PartitionKey)
        {
            this.PartitionKey = PartitionKey;
            this.RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
        }
        public string UserName { get; set; }
        public string FormName { get; set; }
        public long? ID { get; set; }
        public string StatusName { get; set; }
        public int SourceID { get; set; }
        public DateTime TransactionDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTS { get; set; }
        public string ActivityDateTime { get; set; }
    }
}

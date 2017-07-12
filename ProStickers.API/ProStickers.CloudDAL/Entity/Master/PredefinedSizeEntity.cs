using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ProStickers.CloudDAL.Entity.Master
{
    public class PredefinedSizeEntity : TableEntity
    {
        public PredefinedSizeEntity()
        {

        }

        public PredefinedSizeEntity(string predefinedSizeID)
        {
            this.PartitionKey = predefinedSizeID;
            this.RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
        }
        public double Size { get; set; }
        public double OneColorPrice { get; set; }
        public double TwoColorPrice { get; set; }
        public double MoreColorPrice { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTS { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTS { get; set; }
    }

}

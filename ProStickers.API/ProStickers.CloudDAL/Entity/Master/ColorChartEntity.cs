using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ProStickers.CloudDAL.Entity.Master
{
    public class ColorChartEntity : TableEntity
    {
        public ColorChartEntity()
        {

        }

        public ColorChartEntity(string colorID)
        {
            PartitionKey = colorID;
            RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
        }

        public string Name { get; set; }
        public string ImageGUID { get; set; }      
        public bool IsAllowForSale { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTS { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTS { get; set; }
    }
}

using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ProStickers.CloudDAL.Entity.Master
{
    public class FeedbackEntity : TableEntity
    {
        public FeedbackEntity()
        {

        }

        public FeedbackEntity(string userID)
        {
            PartitionKey = userID;
            RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
        }

        public string CustomerID { get; set; }
        public int OrderNumber { get; set; }
        public string FeedbackDate { get; set; }
        public string FeedbackDateTime { get; set; }
        public string MasterReply { get; set; }
        public string CustomerFeedback { get; set; }
        public string DesignerName { get; set; }
        public string DesignNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCodeName { get; set; }
        public bool IsDisplayInProfile { get; set; }
        public DateTime CreatedTS { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTS { get; set; }
    }
}

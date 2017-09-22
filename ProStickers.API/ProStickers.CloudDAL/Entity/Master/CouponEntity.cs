using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ProStickers.CloudDAL.Entity.Master
{
    public class CouponEntity : TableEntity
    {
        public CouponEntity()
        {

        }

        public CouponEntity(string couponID)
        {
            PartitionKey = couponID;
            RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
        }
        public string CouponCode { get; set; }
        public int CouponTypeID { get; set; }
        public string CouponType { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedTS { get; set; }
        public DateTime UpdatedTS { get; set; }
        public string ID { get; set; }
    }
}

using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ProStickers.CloudDAL.Entity.Master
{
    public class UserEntity : TableEntity
    {
        public UserEntity()
        {

        }

        public UserEntity(string userID)
        {
            PartitionKey = userID;
            RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
        }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string ImageGUID { get; set; }
        public string GoogleID { get; set; }
        public string SkypeID { get; set; }
        public string Description { get; set; }
        public int UserTypeID { get; set; }
        public string UserType { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedTS { get; set; }
        public DateTime UpdatedTS { get; set; }
        public string ID { get; set; }
        public bool Active { get; set; }
    }
}

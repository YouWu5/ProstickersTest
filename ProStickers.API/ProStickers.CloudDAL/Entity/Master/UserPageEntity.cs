using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ProStickers.CloudDAL.Entity.Master
{
    public class UserPageEntity : TableEntity
    {
        public UserPageEntity()
        {
        }

        public UserPageEntity(string isMasterUserPage)
        {
            this.RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
            this.PartitionKey = isMasterUserPage;
        }
        public int PageID { get; set; }
        public string Name { get; set; }
        public bool isMasterUserPage { get; set; }
        public int OrderSequence { get; set; }
        public string Url { get; set; }
        public string ApiUrl { get; set; }
        public bool Active { get; set; }
    }

    public class CustomerPageEntity : TableEntity
    {
        public CustomerPageEntity()
        {
        }

        public CustomerPageEntity(string isMasterUserPage)
        {
            this.RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
            this.PartitionKey = isMasterUserPage;
        }
        public int PageID { get; set; }
        public string Name { get; set; }
        public int OrderSequence { get; set; }
        public string Url { get; set; }
        public string ApiUrl { get; set; }
        public bool Active { get; set; }
    }
}

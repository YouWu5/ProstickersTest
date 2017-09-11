using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ProStickers.CloudDAL.Entity.Customer
{
    public class CustomerEntity : TableEntity
    {
        public CustomerEntity()
        {

        }

        public CustomerEntity(string customerID)
        {
            PartitionKey = customerID;
            RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
        }

        public string CustomerCodeName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ContactNo { get; set; }
        public string EmailID { get; set; }
        public string SkypeID { get; set; }     
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }      
        public int StateID { get; set; }
        public string StateName { get; set; }
        public string PostalCode { get; set; }
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public bool IsFacebookUser { get; set; }
        public string ImageGUID { get; set; }
        public string FileNote { get; set; }
        public bool IsPolicyAccepted { get; set; }
        public bool HaveSkype { get; set; }
        public DateTime? PolicyAcceptedDate { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTS { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTS { get; set; }
        public string FullName { get; set; }
        public string ID { get; set; }
        public int UploadedFileCount { get; set; }
        public double UploadedFileSize { get; set; }
    }
}

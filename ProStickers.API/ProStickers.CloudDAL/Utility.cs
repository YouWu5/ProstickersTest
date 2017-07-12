using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;
using ProStickers.CloudDAL.Entity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace ProStickers.CloudDAL
{
    public class Utility
    {
        public static CloudTable countertable;

        static Utility()
        {
            countertable = GetStorageTable("Counter");
        }

        public readonly static string StorageConnectionString = WebConfigurationManager.AppSettings["StorageConnectionString"].ToString();

        TableRequestOptions interactiveRequestOption = new TableRequestOptions()
        {
            RetryPolicy = new LinearRetry(TimeSpan.FromMilliseconds(20000), 3),
            // For Read-access geo-redundant storage, use PrimaryThenSecondary.
            // Otherwise set this to PrimaryOnly.
            LocationMode = LocationMode.PrimaryOnly,
            // Maximum execution time based on the business use case. Maximum value up to 10 seconds.
            MaximumExecutionTime = TimeSpan.FromSeconds(200)
        };

        static TableRequestOptions backgroundRequestOption = new TableRequestOptions()
        {
            // Client has a default exponential retry policy with 4 sec delay and 3 retry attempts
            // Retry delays will be approximately 3 sec, 7 sec, and 15 sec
            MaximumExecutionTime = TimeSpan.FromSeconds(200),
            // PrimaryThenSecondary in case of Read-access geo-redundant storage, else set this to PrimaryOnly
            LocationMode = LocationMode.PrimaryOnly
        };

        public static CloudTable GetStorageTable(string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString.ToString());
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            {
                tableClient.DefaultRequestOptions = backgroundRequestOption;
            }
            CloudTable table = tableClient.GetTableReference(tableName);
            Task<bool> b = table.CreateIfNotExistsAsync();
            return table;
        }

        public static CloudBlobContainer GetBlobContainer(string str)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString.ToString());
            var blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(str);
            container.CreateIfNotExistsAsync();
            return container;
        }

        public static string GetNextId(string partitionkey)
        {
            long id = 1;

            //get the only one that should be in this table
            CounterEntity ids = (from item in countertable.CreateQuery<CounterEntity>()
                                 where item.PartitionKey == partitionkey
                                 select item).FirstOrDefault();

            //first time - nothing in the table - insert "1"
            if (ids == null)
            {
                ids = new CounterEntity();
                ids.PartitionKey = partitionkey;
                ids.RowKey = "rowKey";
                ids.StartNo = 1;
                ids.LastNo = 1;
                countertable.Execute(TableOperation.InsertOrReplace(ids));
            }
            //found it
            else
            {
                //increment id just giving a start value, up to caller to increment each entry
                id = ids.LastNo + 1;
                ids.LastNo = id;
                //update entity and replace it in the table

                countertable.Execute(TableOperation.Replace(ids));
            }
            return id.ToString().PadLeft(8, '0');
        }

        public static string GetNextUserId(string partitionkey)
        {
            try
            {
                long id = 1;

                //get the only one that should be in this table
                CounterEntity ids = (from item in countertable.CreateQuery<CounterEntity>()
                                     where item.PartitionKey == partitionkey
                                     select item).FirstOrDefault();

                //first time - nothing in the table - insert "1"
                if (ids == null)
                {
                    ids = new CounterEntity();
                    ids.PartitionKey = partitionkey;
                    ids.RowKey = "rowKey";
                    ids.StartNo = 1;
                    ids.LastNo = 10000000;
                    countertable.Execute(TableOperation.InsertOrReplace(ids));
                    id = ids.LastNo + 1;
                }
                //found it
                else
                {
                    //increment id just giving a start value, up to caller to increment each entry
                    id = ids.LastNo + 1;
                    ids.LastNo = id;
                    //update entity and replace it in the table

                    countertable.Execute(TableOperation.Replace(ids));
                }
                return id.ToString();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static int ConvertTime(DateTime? time)
        {
            if (time != null)
            {
                return Math.Abs(((time.Value.Hour) * 60) + (time.Value.Minute));
            }
            else
            {
                return 0;
            }
        }

        public static double ConvertTimeToSec(DateTime time)
        {
            if (time != null)
            {
                TimeSpan tSpan = new TimeSpan(time.Hour, time.Minute, time.Second);
                return tSpan.TotalSeconds;
            }
            else
            {
                return 0;
            }
        }

        public static DateTime GetCSTDateTime()
        {
            DateTime cstTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Central Standard Time");
            return cstTime;
        }
    }
}


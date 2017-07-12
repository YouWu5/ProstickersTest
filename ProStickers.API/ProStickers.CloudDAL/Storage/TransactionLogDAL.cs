using Microsoft.WindowsAzure.Storage.Table;
using ProStickers.CloudDAL.Entity;
using System;

namespace ProStickers.CloudDAL.Storage
{
    public class TransactionLogDAL
    {
        public static CloudTable transactionLogtable;

        static TransactionLogDAL()
        {
            transactionLogtable = Utility.GetStorageTable("TransactionLog");
        }

        public static void InsertTransactionLog(string partitionKey, string formName, DateTime transactionDate, DateTime createdTS, string createdBy, string statusName, string userName)
        {
            try
            {
                TransactionLogEntity tlogEntity = new TransactionLogEntity(partitionKey);
                tlogEntity.FormName = formName;
                tlogEntity.CreatedBy = createdBy;
                tlogEntity.CreatedTS = createdTS;
                tlogEntity.TransactionDate = transactionDate;
                tlogEntity.StatusName = statusName;
                tlogEntity.UserName = userName;
                transactionLogtable.Execute(TableOperation.InsertOrReplace(tlogEntity));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

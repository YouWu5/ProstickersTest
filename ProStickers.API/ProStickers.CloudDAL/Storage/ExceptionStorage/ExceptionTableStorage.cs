using System;
using Microsoft.WindowsAzure.Storage.Table;
using ProStickers.CloudDAL.Entity;

namespace ProStickers.CloudDAL.Storage.ExceptionStorage
{
    public class ExceptionTableStorage
    {
        public static CloudTable table;
        public static CloudTable exceptionTable;
        static ExceptionTableStorage()
        {
            table = Utility.GetStorageTable("Exceptions");
        }

        public static void InsertOrReplaceEntity(string moduleName, string subModuleName, string methodName, DateTime currentDateTime, Exception e, string requestingURI = null, string requestJson = null, string userID = null)
        {
            ExceptionEntity entity = new ExceptionEntity(userID);

            entity.ModuleName = moduleName;
            entity.SubModuleName = subModuleName;
            entity.MethodName = methodName;
            entity.Date = currentDateTime;
            entity.Message = e.Message;
            entity.StackTrace = e.StackTrace;
            entity.InnerExceptionMessage = e.InnerException != null ? e.InnerException.Message : null;
            entity.RequestJson = requestJson;
            entity.UserID = userID;
            try
            {
                table.Execute(TableOperation.InsertOrReplace(entity));
            }
            catch
            {
                //Do nothing
            }
        }
    }
}

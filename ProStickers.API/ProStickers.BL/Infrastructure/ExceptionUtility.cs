using ProStickers.CloudDAL.Storage.ExceptionStorage;
using System;

namespace ProStickers.BL.Infrastructure
{
    public class ExceptionUtility
    {
        public static void InsertAPIException(string moduleName, string subModuleName, string actionName, string requestingURL, Exception e, string json, string userID)
        {
            ExceptionTableStorage.InsertOrReplaceEntity(moduleName, subModuleName, actionName, DateTime.UtcNow, e, requestingURL, json, userID);
        }
    }
}

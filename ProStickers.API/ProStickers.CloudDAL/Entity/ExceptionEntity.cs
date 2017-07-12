using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ProStickers.CloudDAL.Entity
{
    public class ExceptionEntity:TableEntity
    {
        public ExceptionEntity()
        {

        }
        public ExceptionEntity(string userID)
        {
            this.PartitionKey = userID.ToString();
            this.RowKey = string.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks);
        }
        //This field contains Either BL module name or Web API Controller name
        public string ModuleName { get; set; }
        //This field contains Either BL sub module name or Web API Controller name.
        public string SubModuleName { get; set; }
        //This field contains Either BL method name or Web API action method name.
        public string MethodName { get; set; }
        //This field is only set whe Exception Entity is created by Web not by BL.
        //Or we can say this field is only set when exception logged by Web API.
        public string RequestingURI { get; set; }
        // This Field contains error message.
        public string Message { get; set; }
        //This field contains stackTrace of exception (if any)
        public string StackTrace { get; set; }
        //date and Time at which exception is thrown.
        public DateTime Date { get; set; }

        public string InnerExceptionMessage { get; set; }

        public string RequestJson { get; set; }

        public string UserID { get; set; }
    }
}

namespace ProStickers.ViewModel.Core
{
    public class OperationResult
    {
        public OperationResult()
        {

        }

        public OperationResult(Result result, string message, object returnedData, string key = null, string sourceName = null)
        {
            this.Result = result;
            this.Message = message;
            this.ReturnedData = returnedData;
            this.Key = key;
            this.SourceName = sourceName;
        }

        public Result Result { get; set; }

        public string Message { get; set; }

        public object ReturnedData { get; set; }

        public string Key { get; set; }

        public string SourceName { get; set; }
    }

    public enum Result
    {
        Success = 1,

        /// <summary>
        /// User Defined Error.
        /// </summary>
        UDError = 2,

        /// <summary>
        /// System Defined Error.
        /// </summary>
        SDError = 3,

        /// <summary>
        /// User Defined Notification.
        /// </summary>
        Notification = 4,

        /// <summary>
        /// concurrency Notification.
        /// </summary>
        Concurrency = 5,


        InvalidModel = 6
    }
}

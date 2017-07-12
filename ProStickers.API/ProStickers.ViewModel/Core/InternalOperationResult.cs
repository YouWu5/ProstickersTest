namespace ProStickers.ViewModel.Core
{
    public class InternalOperationResult
    {
        public InternalOperationResult()
        {

        }

        public InternalOperationResult(Result result, string message, object returnedData)
        {
            this.Result = result;
            this.Message = message;
            this.ReturnedData = returnedData;
        }

        public Result Result { get; set; }

        public string Message { get; set; }

        public object ReturnedData { get; set; }
    }
}

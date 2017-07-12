using ProStickers.BL.Core;
using ProStickers.CloudDAL.Storage.Master;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Master;
using ProStickers.ViewModel.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProStickers.BL.Master
{
    public class PredefinedSizeBL
    {
        public static async Task<OperationResult> GetByIDAsync()
        {
            InternalOperationResult result = await PredefinedSizeDAL.GetByIDAsync();
            return new OperationResult(result.Result, result.Message, result.ReturnedData);
        }

        public static async Task<OperationResult> UpdateAsync(PredefinedSizeViewModel sizeVM, UserInfo userInfo)
        {
            if (sizeVM != null && userInfo != null)
            {
                InternalOperationResult result = await PredefinedSizeDAL.UpdateAsync(sizeVM, userInfo.UserName, userInfo.UserID);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<PredefinedSizeViewModel> GetDefaultViewModelAsync()
        {
            PredefinedSizeViewModel vm = new PredefinedSizeViewModel();
            return await Task.FromResult(vm);
        }
    }
}

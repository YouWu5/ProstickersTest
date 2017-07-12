using System;
using System.Threading.Tasks;
using ProStickers.CloudDAL.Storage.Designer;
using ProStickers.ViewModel.Designer;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Security;

namespace ProStickers.BL.Designer
{
    public class DesignerTimeSlotBL
    {
        public static async Task<DesignerTimeSlotViewModel> GetListAsync(UserInfo currentUserInfo, DateTime date, bool IsAllTimeSlots)
        {
            DesignerTimeSlotViewModel vm = await DesignerTimeSlotDAL.GetListAsync(currentUserInfo.UserID, currentUserInfo.UserName, date, IsAllTimeSlots);
            return vm;
        }

        public static async Task<OperationResult> CreateAsync(UserInfo currentUserInfo, DesignerTimeSlotViewModel vm)
        {
            if (vm != null)
            {
                InternalOperationResult result = await DesignerTimeSlotDAL.CreateAsync(currentUserInfo.UserID, currentUserInfo.UserName, vm);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }
    }
}

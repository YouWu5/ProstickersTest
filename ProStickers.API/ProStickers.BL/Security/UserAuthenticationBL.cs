using System.Collections.Generic;
using System.Threading.Tasks;
using ProStickers.CloudDAL.Storage.Security;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using ProStickers.ViewModel.Security;

namespace ProStickers.BL.Security
{
    public class UserAuthenticationBL
    {
        public static async Task<OperationResult> GetUserSessionAsync(string emailID, string ID)
        {
            InternalOperationResult result = await UserAuthenticationDAL.GetUserSessionAsync(emailID, ID);
            return new OperationResult(result.Result, result.Message, result.ReturnedData);
        }

        public static async Task<OperationResult> CreateCustomerAsync(CustomerViewModel vm)
        {
            InternalOperationResult result = await UserAuthenticationDAL.CreateCustomerAsync(vm);
            return new OperationResult(result.Result, result.Message, result.ReturnedData);
        }

        public static async Task<UserSession> LoadUserSessionAsync(string emailID, int sourceID)
        {
            if (sourceID == 1)
            {
                return await UserAuthenticationDAL.LoadUserSessionAsync(emailID);
            }
            else
            {
                return await UserAuthenticationDAL.LoadCustomerSessionAsync(emailID);
            }
        }

        public static OperationResult AcceptSignInPolicy(string userID)
        {
            InternalOperationResult result = UserAuthenticationDAL.AcceptSignInPolicy(userID);
            return new OperationResult(result.Result, result.Message, null);
        }

        public static OperationResult HaveSkype(string userID)
        {
            InternalOperationResult result = UserAuthenticationDAL.HaveSkype(userID);
            return new OperationResult(result.Result, result.Message, null);
        }

        public static bool CheckUserExistsAsync(string userID)
        {
            return UserAuthenticationDAL.CheckUserExistsAsync(userID);
        }

        public static OperationResult CheckMasterOrDesignerExistsAsync(string userID)
        {
           InternalOperationResult result = UserAuthenticationDAL.CheckMasterOrDesignerExistsAsync(userID);
            return new OperationResult(result.Result, result.Message, result.ReturnedData);
        }

        public static List<ListItem> GetPageList()
        {
            return UserAuthenticationDAL.GetPageList();
        }
    }
}

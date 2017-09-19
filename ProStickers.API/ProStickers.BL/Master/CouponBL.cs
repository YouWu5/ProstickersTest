using ProStickers.BL.Core;
using ProStickers.CloudDAL.Storage;
using ProStickers.CloudDAL.Storage.Master;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Master;
using ProStickers.ViewModel.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProStickers.BL.Master
{
    public class UserBL
    {
        public static async Task<Pager<UserListViewModel>> GetListAsync<UserListViewModel>(string userID)
        {
            Pager<UserListViewModel> pager = PagerOperations.InitializePager<UserListViewModel>
                                                        ("UpdatedTS", SortDirection.DESC.ToString(),
                                                          true, true, true, 30, true);

            pager.ColumnList = new List<ListItem>()
            {
                new ListItem { Text = "Name", Value = "FullName" },
                new ListItem { Text = "Google id", Value = "GoogleID" },
                new ListItem { Text = "Role", Value = "UserType" }
            };
            await UserDAL.GetListAsync(userID, pager);
            return pager;
        }

        public static async Task<OperationResult> GetListAsync<UserListViewModel>(string userID, Pager<UserListViewModel> pager)
        {
            if (pager != null)
            {
                PagerOperations.UpdatePager(pager);
                await UserDAL.GetListAsync(userID, pager);
                return new OperationResult(Result.Success, "", pager);
            }
            else
            {
                return new OperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<OperationResult> GetByIDAsync(string loggedInUserID,string userID)
        {
            if (userID != null)
            {
                InternalOperationResult result = await UserDAL.GetByIDAsync(loggedInUserID,userID);
                if (result.Result == Result.Success)
                {
                    UserViewModel userVM = (result.ReturnedData as UserViewModel);
                    if (userVM.ImageGUID != null && userVM.ImageGUID != "")
                    {
                        userVM.ImageGUID = BlobStorage.DownloadBlobUri(StatusEnum.Blob.userimage.ToString(), userVM.ImageGUID);
                    }
                }
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> CreateAsync(UserViewModel userVM, UserInfo userInfo)
        {
            if (userVM != null && userInfo != null)
            {
                if (UserDAL.CheckGoogleID(userVM.GoogleID))
                {
                    InternalOperationResult result = await UserDAL.CreateAsync(userVM, userInfo.UserID, userInfo.UserName);
                    return new OperationResult(result.Result, result.Message, result.ReturnedData);
                }
                else
                {
                    return new OperationResult(Result.UDError, "Google ID already exist.", null);
                }
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> UpdateAsync(UserViewModel userVM, UserInfo userInfo)
        {
            if (userVM != null && userInfo != null)
            {
                InternalOperationResult result = await UserDAL.UpdateAsync(userVM, userInfo.UserName, userInfo.UserID);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<List<ListItemTypes>> GetUserTypeListAsync()
        {
            List<ListItemTypes> list = new List<ListItemTypes>();

            list.Add(new ListItemTypes { Text = "Master", Value = 1 });
            list.Add(new ListItemTypes { Text = "Designer", Value = 2 });
            return await Task.FromResult(list);
        }

        public static async Task<UserViewModel> GetDefaultViewModelAsync()
        {
            UserViewModel vm = new UserViewModel();
            return await Task.FromResult(vm);
        }

        public static async Task<OperationResult> InactiveAsync(UserInfo userInfo,UserStatusChangeViewModel userVM)
        {
            if (userInfo != null && userVM!=null)
            {
                InternalOperationResult result = await UserDAL.InactiveAsync(userInfo.UserID, userVM);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }
    }
}

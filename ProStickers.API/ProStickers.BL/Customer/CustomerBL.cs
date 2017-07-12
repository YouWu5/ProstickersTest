using ProStickers.BL.Core;
using ProStickers.CloudDAL;
using ProStickers.CloudDAL.Storage;
using ProStickers.CloudDAL.Storage.Customer;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using ProStickers.ViewModel.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProStickers.BL.Customer
{
    public class CustomerBL
    {
        #region Customer Methods

        public static async Task<CustomerViewModel> GetDefaultViewModelAsync()
        {
            CustomerViewModel vm = new CustomerViewModel();
            return await Task.FromResult(vm);
        }

        public static async Task<OperationResult> GetByIDAsync(string customerID, UserInfo currentUserInfo)
        {
            if (customerID != null && customerID != "" && currentUserInfo != null)
            {
                InternalOperationResult result = await CustomerDAL.GetByIDAsync(customerID, currentUserInfo.UserID);
                if (result.Result == Result.Success)
                {
                    CustomerViewModel customerVM = (result.ReturnedData as CustomerViewModel);
                    if (customerVM.ImageGUID != null && customerVM.ImageGUID != "")
                    {
                        customerVM.ImageURL = BlobStorage.DownloadBlobUri("customerimage", customerVM.ImageGUID);
                    }
                }
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> CreateAsync(CustomerViewModel vm, UserInfo currentUserInfo)
        {
            if (vm != null)
            {
                InternalOperationResult result = await CustomerDAL.CreateAsync(vm);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> UpdateAsync(CustomerViewModel vm, UserInfo currentUserInfo)
        {
            if (vm != null)
            {
                if (!CustomerDAL.CheckCustomerEmailIDExists(vm.EmailID, vm.IsFacebookUser, vm.CustomerID))
                {
                    InternalOperationResult result = await CustomerDAL.UpdateAsync(vm, currentUserInfo.UserID, currentUserInfo.UserName);
                    return new OperationResult(result.Result, result.Message, result.ReturnedData);
                }
                else
                {
                    return new OperationResult(Result.UDError, "Customer already exist.", null);
                }
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request,customer doesn't exist.", null);
            }
        }

        public static async Task<OperationResult> GetDetailListAsync(UserInfo currentUserInfo)
        {
            if (currentUserInfo != null)
            {
                InternalOperationResult result = await CustomerDAL.GetDetailListAsync(currentUserInfo.UserID, currentUserInfo.UserName);
                if (result.Result == Result.Success)
                {
                    CustomerDetailListViewModel customerDetailListVM = (result.ReturnedData as CustomerDetailListViewModel);
                }
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        #endregion

        #region User Methods

        public static async Task<Pager<CustomerListViewModel>> GetListAsync<customerListViewModel>()
        {
            Pager<CustomerListViewModel> pager = PagerOperations.InitializePager<CustomerListViewModel>
                                                        ("UpdatedTS", SortDirection.DESC.ToString(),
                                                          true, true, true, 30, true);

            pager.SearchList = new List<SearchItem>
            {
                new SearchItem { DisplayName = "Customer" ,Name = "CustomerCodeName",Value ="" },
            };

            pager.ColumnList = new List<ListItem>()
            {
                new ListItem { Text = "Customer", Value = "CustomerCodeName" },
                new ListItem { Text = "Contact number", Value = "ContactNo" },
                new ListItem { Text = "Email", Value = "EmailID" },
            };
            await CustomerDAL.GetListAsync(pager);
            return await Task.FromResult(pager);
        }

        public static async Task<OperationResult> GetListAsync<CustomerListViewModel>(Pager<CustomerListViewModel> pager)
        {
            if (pager != null)
            {
                PagerOperations.UpdatePager<CustomerListViewModel>(pager);
                await CustomerDAL.GetListAsync(pager);
                return new OperationResult(Result.Success, "", pager);
            }
            else
            {
                return new OperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<OperationResult> GetCustomerDetailAsync(string customerID, UserInfo userInfo)
        {
            if (customerID != null && customerID != "" && userInfo != null)
            {
                InternalOperationResult result = await CustomerDAL.GetCustomerDetailAsync(customerID, userInfo.UserID);
                if (result.Result == Result.Success)
                {
                    CustomerDetailViewModel cdVM = (result.ReturnedData as CustomerDetailViewModel);
                    cdVM.Delete = new DeleteViewModel();
                    if (cdVM.ImageGUID != null && cdVM.ImageGUID != "")
                    {
                        cdVM.ImageURL = BlobStorage.DownloadBlobUri("customerimage", cdVM.ImageGUID);
                    }
                }
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> UploadUserFileAsync(UserInfo userInfo, UserFilesViewModel userFileVM)
        {
            if (userFileVM != null && userInfo != null)
            {
                double fileSize = 0.0;
                if (userFileVM.FileBuffer != null)
                {
                    fileSize = userFileVM.FileBuffer.Length / 1024f;
                }
                if (CustomerDAL.ValidateUserFileSize(userInfo.UserID, userFileVM.CustomerID, fileSize))
                {
                    string fileGUID = userInfo.UserID + "-" + Utility.GetCSTDateTime().ToString("yyyyMMdd") + "-" + Utility.GetCSTDateTime().TimeOfDay.ToString("hhmmss") + "-" + userFileVM.FileName + "." + userFileVM.FileExtension;
                    userFileVM.FileNumber = fileGUID;                    
                    await BlobStorage.UploadPrivateImage(StatusEnum.Blob.userfiles.ToString(), userFileVM.FileBuffer, fileGUID, await CommonDAL.GetContentType(userFileVM.FileExtension));
                    InternalOperationResult result = await CustomerDAL.UploadUserFileAsync(userFileVM, userInfo.UserID, userInfo.UserName, fileSize);
                    return new OperationResult(result.Result, result.Message, result.ReturnedData);
                }
                else
                {
                    return new OperationResult(Result.UDError, "Maximum file size is 20MB for uploaded files.", null);
                }
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> UpdateCustomerAsync(CustomerDetailViewModel vm, UserInfo userInfo)
        {
            if (vm != null)
            {
                InternalOperationResult result = await CustomerDAL.UpdateCustomerAsync(vm, userInfo.UserID, userInfo.UserName);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request,customer doesn't exist.", null);
            }
        }

        public static async Task<OperationResult> DeleteAsync(string customerID, DateTime updatedTS, UserInfo userInfo)
        {
            if (customerID != null && updatedTS != null)
            {
                InternalOperationResult result = await CustomerDAL.DeleteAsync(customerID, updatedTS, userInfo.UserID, userInfo.UserName);
                return new OperationResult(result.Result, result.Message, null);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<List<ListItem>> GetCustomerListAsync(string codeName)
        {
            return await CustomerDAL.GetCustomerListAsync(codeName);
        }

        public static async Task<OperationResult> DownloadDesignImageAsync(string userID, string designNumber)
        {
            if (designNumber != null && designNumber != "")
            {
                DownloadImageViewModel vm = await CommonDAL.DownloadDesignImageAsync(userID, designNumber);
                if (vm != null)
                {
                    return new OperationResult(Result.Success, "", vm);
                }
                else
                {
                    return new OperationResult(Result.UDError, "Design file doesn't exists.", null);
                }
            }
            else
            {
                return new OperationResult(Result.UDError, "Design file doesn't exists.", null);
            }
        }

        #endregion
    }
}

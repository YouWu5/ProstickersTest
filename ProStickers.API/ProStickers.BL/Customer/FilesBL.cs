using ProStickers.CloudDAL;
using ProStickers.CloudDAL.Storage;
using ProStickers.CloudDAL.Storage.Customer;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using ProStickers.ViewModel.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProStickers.BL.Customer
{
    public class FilesBL
    {
        public static async Task<OperationResult> UploadFileAsync(UserInfo userInfo, FilesViewModel fileVM)
        {
            if (userInfo != null && fileVM != null)
            {
                float fileSize = 0;

                if (fileVM.FileBuffer != null)
                {
                    fileSize = fileVM.FileBuffer.Length / 1024f;
                }

                if (FilesDAL.ValidateFileCount(userInfo.UserID))
                {
                    if (FilesDAL.ValidateFileSize(userInfo.UserID, fileSize))
                    {
                        if (fileVM.FileBuffer != null)
                        {
                            fileVM.FileNumber = userInfo.UserID + "-" + fileVM.FileName + "." + fileVM.FileExtension;
                            await BlobStorage.UploadFile(StatusEnum.Blob.customerfiles.ToString(), fileVM.FileBuffer, fileVM.FileNumber, await CommonDAL.GetContentType(fileVM.FileExtension));
                        }
                        fileVM.FileSize = fileSize;
                        InternalOperationResult result = await FilesDAL.UploadFileAsync(userInfo.UserName, userInfo.UserID,
                             fileVM, fileSize);
                        return new OperationResult(result.Result, result.Message, result.ReturnedData);
                    }
                    else
                    {
                        return new OperationResult(Result.SDError, "Maximum file size is 20MB for uploaded files.", null);
                    }
                }
                else
                {
                    return new OperationResult(Result.SDError, "File uploading limit exceeds.", null);
                }
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> DeleteFileAsync(string customerID, string fileNumber)
        {
            if (customerID != null && customerID != "" && fileNumber != null && fileNumber != "")
            {
                InternalOperationResult result = await FilesDAL.DeleteFileAsync(customerID, fileNumber);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> CreateAsync(string customerID, FilesViewModel filesVM)
        {
            if (customerID != null && customerID != "" && filesVM != null)
            {
                InternalOperationResult result = await FilesDAL.CreateAsync(customerID, filesVM);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<FilesListViewModel> GetListAsync(string customerID)
        {
            return await FilesDAL.GetListAsync(customerID);
        }

        public static async Task<FilesViewModel> GetDefaultViewModelAsync(UserInfo userInfo)
        {
            FilesViewModel filesVM = new FilesViewModel();
            return await Task.FromResult(filesVM);
        }
    }
}

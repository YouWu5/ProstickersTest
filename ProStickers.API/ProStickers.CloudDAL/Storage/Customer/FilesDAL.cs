using AutoMapper;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using ProStickers.CloudDAL.Entity.Customer;
using ProStickers.CloudDAL.Storage.ExceptionStorage;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProStickers.CloudDAL.Storage.Customer
{
    public class FilesDAL
    {
        public static CloudTable fileTable;

        static FilesDAL()
        {
            fileTable = Utility.GetStorageTable("Files");
        }

        public static async Task<InternalOperationResult> UploadFileAsync(string userName, string userID, FilesViewModel fileVM, float fileSize)
        {
            try
            {
                Mapper.Initialize(a =>
                {
                    a.CreateMap<FilesEntity, FilesViewModel>().ReverseMap();
                });
                FilesEntity fileEntity = Mapper.Map<FilesEntity>(fileVM);
                       
                fileEntity.PartitionKey = userID;
                fileEntity.CreatedTS = DateTime.UtcNow;
                fileEntity.UpdatedTS = DateTime.UtcNow;

                TableOperation insert = TableOperation.Insert(fileEntity);
                await fileTable.ExecuteAsync(insert);

                CustomerEntity cEntity = CustomerDAL.customerTable.ExecuteQuery(new TableQuery<CustomerEntity>()).
                                         Where(x => x.PartitionKey == userID).FirstOrDefault();

                cEntity.UploadedFileCount = cEntity.UploadedFileCount + 1;
                cEntity.UploadedFileSize = cEntity.UploadedFileSize + fileSize;
                cEntity.UpdatedBy = userID;
                cEntity.UpdatedTS = DateTime.UtcNow;
            
                TableOperation update = TableOperation.InsertOrReplace(cEntity);
                await CustomerDAL.customerTable.ExecuteAsync(update);

                TransactionLogDAL.InsertTransactionLog(fileEntity.PartitionKey, "Files", DateTime.UtcNow.Date, DateTime.UtcNow, userID, "Added", userName);
                return new InternalOperationResult(Result.Success, "File uploaded successfully.", fileEntity.PartitionKey);
            }
            catch (Exception e)
            {
                string _requestJSON = JsonConvert.SerializeObject(fileVM);
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", "FilesDAL", "UploadFileAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> DeleteFileAsync(string customerID, string fileNumber)
        {
            try
            {
                DownloadImageViewModel vm = BlobStorage.DownloadBlobByteArray(StatusEnum.Blob.customerfiles.ToString(), fileNumber);
                await BlobStorage.DeleteBlob(StatusEnum.Blob.customerfiles.ToString(), fileNumber);

                FilesEntity filesEntity = fileTable.ExecuteQuery(new TableQuery<FilesEntity>()).Where(c => c.PartitionKey == customerID && c.FileNumber == fileNumber).FirstOrDefault();
                if (filesEntity != null)
                {
                    TableOperation delete = TableOperation.Delete(filesEntity);
                    await fileTable.ExecuteAsync(delete);

                    CustomerEntity cEntity = CustomerDAL.customerTable.ExecuteQuery(new TableQuery<CustomerEntity>()).
                                             Where(x => x.PartitionKey == customerID).FirstOrDefault();

                    cEntity.UploadedFileCount = cEntity.UploadedFileCount - 1;
                    cEntity.UploadedFileSize = cEntity.UploadedFileSize - (vm.ImageBuffer.Length / 1024f);
                    cEntity.UpdatedBy = customerID;
                    cEntity.UpdatedTS = DateTime.UtcNow;

                    TableOperation update = TableOperation.InsertOrReplace(cEntity);
                    await CustomerDAL.customerTable.ExecuteAsync(update);

                    return new InternalOperationResult(Result.Success, "File deleted successfully.", filesEntity.PartitionKey);
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "File doesn't not exists.", null);
                }
            }
            catch (Exception e)
            {
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", "FilesDAL", "DeleteFileAsync", DateTime.UtcNow, e, null, fileNumber, customerID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> CreateAsync(string customerID, FilesViewModel filesVM)
        {
            try
            {
                CustomerEntity customerEntity = CustomerDAL.customerTable.ExecuteQuery(new TableQuery<CustomerEntity>()).Where(x => x.PartitionKey == customerID).FirstOrDefault();
                if (customerEntity != null)
                {
                    customerEntity.FileNote = filesVM.FileNote;
                    customerEntity.UpdatedBy = customerID;
                    customerEntity.UpdatedTS = DateTime.UtcNow;

                    TableOperation insertOrReplace = TableOperation.InsertOrReplace(customerEntity);
                    await CustomerDAL.customerTable.ExecuteAsync(insertOrReplace);

                    return new InternalOperationResult(Result.Success, "Files saved successfully.", customerEntity.PartitionKey);
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Customer doesn't not exists.", null);
                }
            }
            catch (Exception e)
            {
                string requestJson = JsonConvert.SerializeObject(filesVM);
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", "FilesDAL", "CreateAsync", DateTime.UtcNow, e, null, requestJson, customerID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<FilesListViewModel> GetListAsync(string customerID)
        {
            List<FilesEntity> filesEntity = fileTable.ExecuteQuery(new TableQuery<FilesEntity>()).Where(c => c.PartitionKey == customerID).ToList();
            Mapper.Initialize(x =>
                            {
                                x.CreateMap<FilesEntity, FilesViewModel>().
                                ForMember(a => a.CustomerID, b => b.MapFrom(c => c.PartitionKey));
                            });

            List<FilesViewModel> fileVM = Mapper.Map<List<FilesEntity>, List<FilesViewModel>>(filesEntity);

            FilesListViewModel fileList = new FilesListViewModel();

            fileList.filesList = fileVM;
            if (fileList != null)
            {
                fileList.FileNote = CustomerDAL.customerTable.CreateQuery<CustomerEntity>().Where(c => c.PartitionKey == customerID).Select(c => c.FileNote).FirstOrDefault();
            }
            return await Task.FromResult(fileList);
        }

        #region Helper Methods

        public static bool ValidateFileCount(string customerID)
        {
            int fileCount = CustomerDAL.customerTable.ExecuteQuery(new TableQuery<CustomerEntity>()).Where(x => x.PartitionKey == customerID).Select(x => x.UploadedFileCount).FirstOrDefault();

            if (fileCount < 10)
                return true;
            else
                return false;
        }

        public static bool ValidateFileSize(string customerID, double fileSize)
        {
            fileSize = fileSize + CustomerDAL.customerTable.ExecuteQuery(new TableQuery<CustomerEntity>()).Where(x => x.PartitionKey == customerID).Select(x => x.UploadedFileSize).FirstOrDefault();
            if (fileSize / 1024f < 20)
                return true;
            else
                return false;
        }

        public static bool ValidateFileName(string customerID, string fileName)
        {
            List<string> fileNameList = fileTable.ExecuteQuery(new TableQuery<FilesEntity>()).Where(x => x.PartitionKey == customerID).Select(x => x.FileNumber).ToList();
            
            if (!fileNameList.Contains(fileName))
                return true;
            else
                return false;
        }

        #endregion
    }
}

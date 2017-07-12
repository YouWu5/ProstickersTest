using AutoMapper;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using ProStickers.CloudDAL.Entity.Customer;
using ProStickers.CloudDAL.Entity.Designer;
using ProStickers.CloudDAL.Entity.Master;
using ProStickers.CloudDAL.Storage.Designer;
using ProStickers.CloudDAL.Storage.ExceptionStorage;
using ProStickers.CloudDAL.Storage.Master;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ProStickers.CloudDAL.Storage.Customer
{
    public class CustomerDAL
    {
        public static CloudTable customerTable;
        public static CloudTable userFileTable;

        static CustomerDAL()
        {
            customerTable = Utility.GetStorageTable("Customer");
            userFileTable = Utility.GetStorageTable("UserFiles");
        }

        #region Customer Methods

        public static async Task<InternalOperationResult> GetByIDAsync(string customerID, string userID)
        {
            try
            {
                CustomerViewModel customerVM = new CustomerViewModel();

                Mapper.Initialize(a =>
                {
                    a.CreateMap<CustomerViewModel, CustomerEntity>().ReverseMap()
                    .ForMember(x => x.CustomerID, y => y.MapFrom(z => z.PartitionKey));
                });

                customerVM = Mapper.Map<CustomerViewModel>(customerTable.CreateQuery<CustomerEntity>().Where(e => e.PartitionKey == customerID).FirstOrDefault());

                if (customerVM != null)
                {
                    return await Task.FromResult(new InternalOperationResult(Result.Success, "", customerVM));
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Oop's something went wrong. Please try again.", null);
                }
            }
            catch (Exception e)
            {
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", "CustomerDAL", "GetByIDAsync", DateTime.UtcNow, e, null, customerID, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong. Please try again.", null);
            }
        }

        public static async Task<InternalOperationResult> CreateAsync(CustomerViewModel vm)
        {
            try
            {
                if (vm.ImageURL != null)
                {
                    var webClient = new WebClient();
                    byte[] imageBytes = webClient.DownloadData(vm.ImageURL);
                    if (imageBytes != null)
                    {
                        string imageGUID = Guid.NewGuid().ToString();
                        vm.ImageGUID = imageGUID;
                        await BlobStorage.UploadPublicImage("customerimage", imageBytes, imageGUID, "image/jpeg");
                    }
                }

                Mapper.Initialize(a =>
                {
                    a.CreateMap<CustomerViewModel, CustomerEntity>().ReverseMap();
                });

                CustomerEntity customerEntity = Mapper.Map<CustomerEntity>(vm);

                string customerID = Utility.GetNextId("CustomerID");

                if (customerID != null && customerID != "")
                {
                    customerEntity.PartitionKey = customerID;
                    customerEntity.RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
                    customerEntity.CustomerCodeName = customerEntity.FullName + " " + "-" + " " + Convert.ToInt32(customerEntity.PartitionKey);
                    customerEntity.Active = true;
                    customerEntity.CreatedBy = customerID;
                    customerEntity.CreatedTS = DateTime.UtcNow;
                    customerEntity.UpdatedBy = customerID;
                    customerEntity.UpdatedTS = DateTime.UtcNow;

                    TableOperation insert = TableOperation.InsertOrReplace(customerEntity);
                    await customerTable.ExecuteAsync(insert);

                    TransactionLogDAL.InsertTransactionLog(customerEntity.PartitionKey, "CustomerProfile", DateTime.UtcNow.Date, DateTime.UtcNow, customerEntity.CreatedBy, "Added", vm.FullName);

                    return new InternalOperationResult(Result.Success, "Customer added successfully.", customerEntity);
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = JsonConvert.SerializeObject(vm);
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", "CustomerDAL", "CreateAsync", DateTime.UtcNow, e, null, _requestJSON, vm.EmailID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> UpdateAsync(CustomerViewModel vm, string userID, string userName)
        {
            try
            {
                CustomerEntity cEntity = (from c in customerTable.CreateQuery<CustomerEntity>().Where(c => c.PartitionKey == vm.CustomerID) select c).FirstOrDefault();
                if (cEntity != null)
                {
                    if (cEntity.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff") == (vm.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff")))
                    {
                        Mapper.Initialize(a =>
                        {
                            a.CreateMap<CustomerViewModel, CustomerEntity>().ReverseMap();
                        });

                        CustomerEntity customerEntity = Mapper.Map<CustomerEntity>(vm);

                        customerEntity.PartitionKey = cEntity.PartitionKey;
                        customerEntity.RowKey = cEntity.RowKey;
                        customerEntity.CustomerCodeName = cEntity.CustomerCodeName;
                        customerEntity.CreatedBy = cEntity.CreatedBy;
                        customerEntity.CreatedTS = cEntity.CreatedTS;
                        customerEntity.UpdatedBy = userID;
                        customerEntity.UpdatedTS = DateTime.UtcNow;

                        TableOperation update = TableOperation.InsertOrReplace(customerEntity);
                        await customerTable.ExecuteAsync(update);

                        TransactionLogDAL.InsertTransactionLog(customerEntity.PartitionKey, "CustomerProfile", DateTime.UtcNow.Date, DateTime.UtcNow, customerEntity.CreatedBy, "Edited", userName);

                        return new InternalOperationResult(Result.Success, "Customer profile updated successfully.", customerEntity.PartitionKey);
                    }
                    else
                    {
                        return new InternalOperationResult(Result.Concurrency, "Data is already changed by someone else. Please try again.", null);
                    }
                }
                else
                {
                    return new InternalOperationResult(Result.UDError, "Customer doesn't exist or is deleted.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = Newtonsoft.Json.JsonConvert.SerializeObject(vm);
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", "CustomerDAL", "UpdateAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> GetDetailListAsync(string userID, string userName)
        {
            try
            {
                CustomerDetailListViewModel customerDetailListVM = new CustomerDetailListViewModel();
                customerDetailListVM.DesignList = new List<CustomerDetailDesignViewModel>();
                customerDetailListVM.FilesList = new List<string>();

                List<CustomerAppointmentEntity> AppointmentEntityList = CalendarAppointmentDAL.customerAppointmentTable.ExecuteQuery(new TableQuery<CustomerAppointmentEntity>()).Where(x => x.PartitionKey == userID).OrderByDescending(x => x.UpdatedTS).Take(3).ToList();
                Mapper.Initialize(a =>
                {
                    a.CreateMap<CustomerAppointmentListViewModel, CustomerAppointmentEntity>().ReverseMap();
                });

                if (AppointmentEntityList != null && AppointmentEntityList.Count() > 0)
                {
                    customerDetailListVM.AppointmentList = Mapper.Map<List<CustomerAppointmentEntity>, List<CustomerAppointmentListViewModel>>(AppointmentEntityList);
                }

                List<OrderEntity> OrderEntityList = OrderDAL.orderTable.ExecuteQuery(new TableQuery<OrderEntity>()).Where(x => x.CustomerID == userID).OrderByDescending(x => x.UpdatedTS).Take(3).ToList();
                Mapper.Initialize(a =>
                {
                    a.CreateMap<OrderListViewModel, OrderEntity>().ReverseMap().
                    ForMember(x => x.OrderDate, y => y.MapFrom(z => z.PartitionKey));
                });
                if (OrderEntityList != null && OrderEntityList.Count() > 0)
                {
                    customerDetailListVM.OrderList = Mapper.Map<List<OrderEntity>, List<OrderListViewModel>>(OrderEntityList);
                }

                List<CustomerAppointmentEntity> AppEntityList = CalendarAppointmentDAL.customerAppointmentTable.ExecuteQuery(new TableQuery<CustomerAppointmentEntity>()).Where(x => x.PartitionKey == userID && x.DesignNumber != null && x.DesignNumber != "" && x.AppointmentStatusID == Convert.ToInt32(StatusEnum.AppointmentStatus.Completed)).OrderByDescending(x => x.UpdatedTS).Take(3).ToList();
                if (AppEntityList != null && AppEntityList.Count() > 0)
                {
                    foreach (var item in AppEntityList)
                    {
                        CustomerDetailDesignViewModel cddVM = new CustomerDetailDesignViewModel();
                        cddVM.DesignNumber = item.DesignNumber;
                        cddVM.DesignImage = BlobStorage.DownloadBlobByteArray(StatusEnum.Blob.designimage.ToString(), item.DesignNumber);
                        customerDetailListVM.DesignList.Add(cddVM);
                    }
                }

                customerDetailListVM.FilesList.AddRange(FilesDAL.fileTable.ExecuteQuery(new TableQuery<FilesEntity>()).Where(x => x.PartitionKey == userID).OrderByDescending(x => x.UpdatedTS).Take(3).Select(c => c.FileNumber).ToList());

                if (customerDetailListVM != null)
                {
                    return await Task.FromResult(new InternalOperationResult(Result.Success, "", customerDetailListVM));
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Oop's something went wrong. Please try again.", null);
                }
            }
            catch (Exception e)
            {
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", "CustomerDAL", "GetDetailListAsync", DateTime.UtcNow, e, null, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong. Please try again.", null);
            }
        }

        #endregion

        #region User Methods

        public static async Task GetListAsync<CustomerListViewModel>(Pager<CustomerListViewModel> pagerVM)
        {
            string filter = TableQuery.GenerateFilterConditionForBool("Active", QueryComparisons.Equal, true);

            if (pagerVM.SearchList != null && pagerVM.SearchList.Count() > 0)
            {
                foreach (var item in pagerVM.SearchList.Where(s => s.Value != null && s.Value != ""))
                {
                    filter = TableQuery.CombineFilters(filter, TableOperators.And, TableQuery.GenerateFilterCondition(item.Name, QueryComparisons.Equal, item.Value));
                }
            }

            TableQuery<CustomerEntity> listQuery = new TableQuery<CustomerEntity>();

            listQuery = new TableQuery<CustomerEntity>().Where(filter);

            List<CustomerEntity> customerList = customerTable.ExecuteQuery(listQuery).ToList();

            if (customerList != null && customerList.Count() > 0)
            {
                pagerVM.RecordsCount = customerList.Count();

                if (pagerVM.Sort == "UpdatedTS")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        customerList = customerList.OrderByDescending(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerEntity>();
                    }
                    else
                    {
                        customerList = customerList.OrderBy(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerEntity>();
                    }
                }
                else if (pagerVM.Sort == "CustomerCodeName")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        customerList = customerList.OrderByDescending(s => s.CustomerCodeName).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerEntity>();
                    }
                    else
                    {
                        customerList = customerList.OrderBy(s => s.CustomerCodeName).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerEntity>();
                    }
                }
                else if (pagerVM.Sort == "ContactNo")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        customerList = customerList.OrderByDescending(s => s.ContactNo).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerEntity>();
                    }
                    else
                    {
                        customerList = customerList.OrderBy(s => s.ContactNo).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerEntity>();
                    }
                }
                else if (pagerVM.Sort == "EmailID")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        customerList = customerList.OrderByDescending(s => s.EmailID).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerEntity>();
                    }
                    else
                    {
                        customerList = customerList.OrderBy(s => s.EmailID).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerEntity>();
                    }
                }
            }

            Mapper.Initialize(a =>
            {
                a.CreateMap<ViewModel.Customer.CustomerListViewModel, CustomerEntity>().ReverseMap()
                 .ForMember(x => x.CustomerID, y => y.MapFrom(z => z.PartitionKey))
                 .ForMember(x => x.Name, y => y.MapFrom(z => z.CustomerCodeName));
            });

            await Task.FromResult(pagerVM.Data = Mapper.Map<List<CustomerListViewModel>>(customerList));
        }

        public static async Task<InternalOperationResult> GetCustomerDetailAsync(string customerID, string userID)
        {
            try
            {
                CustomerDetailViewModel customerVM = new CustomerDetailViewModel();

                Mapper.Initialize(a =>
                {
                    a.CreateMap<CustomerEntity, CustomerDetailViewModel>()
                     .ForMember(x => x.CustomerID, y => y.MapFrom(z => z.PartitionKey)); ;
                });

                customerVM = Mapper.Map<CustomerDetailViewModel>(customerTable.CreateQuery<CustomerEntity>().Where(e => e.PartitionKey == customerID).FirstOrDefault());

                customerVM.UserID = userID;

                if (customerVM != null)
                {
                    List<string> fEntity = FilesDAL.fileTable.CreateQuery<FilesEntity>().Where(x => x.PartitionKey == customerID).Select(x => x.FileNumber).ToList();

                    if (fEntity != null)
                    {
                        List<CustomerFileViewModel> cfVMList = new List<CustomerFileViewModel>();
                        foreach (var cfl in fEntity)
                        {
                            CustomerFileViewModel cfVM = new CustomerFileViewModel();

                            cfVM.FileNumber = cfl;
                            cfVM.Image = null;
                            cfVMList.Add(cfVM);
                        }
                        if (cfVMList != null && cfVMList.Count() > 0)
                        {
                            customerVM.CustomerFileList = new List<CustomerFileViewModel>();
                            customerVM.CustomerFileList = cfVMList;
                        }
                    }

                    List<CustomerAppointmentEntity> caEntity = CalendarAppointmentDAL.customerAppointmentTable.CreateQuery<CustomerAppointmentEntity>().Where(x => x.PartitionKey == customerID).Select(x => x).ToList();

                    if (caEntity != null && caEntity.Count() > 0)
                    {
                        List<CustomerDesignViewModel> cdVMList = new List<CustomerDesignViewModel>();
                        foreach (var item1 in caEntity.Where(x => x.DesignNumber != null))
                        {
                            CustomerDesignViewModel cdVM = new CustomerDesignViewModel();

                            List<OrderEntity> oEntityList = OrderDAL.orderTable.CreateQuery<OrderEntity>().Where(x => x.CustomerID == customerID && x.DesignNumber == item1.DesignNumber).Select(x => x).ToList();

                            if (oEntityList != null && oEntityList.Count > 0)
                            {
                                List<int> orderStatusList = oEntityList.Select(x => x.OrderStatusID).ToList();
                                if (orderStatusList.Contains((int)StatusEnum.OrderStatus.Placed))
                                {
                                    cdVM.OrderStatusID = (int)StatusEnum.OrderStatus.Placed;
                                }
                                else if (orderStatusList.Contains((int)StatusEnum.OrderStatus.Shipped))
                                {
                                    cdVM.OrderStatusID = (int)StatusEnum.OrderStatus.Shipped;
                                }
                                else
                                {
                                    cdVM.OrderStatusID = 0;
                                }
                            }
                            cdVM.CustomerID = item1.PartitionKey;
                            cdVM.DesignNumber = item1.DesignNumber;
                            cdVM.UserName = item1.UserName;
                            cdVM.UserID = item1.UserID;

                            cdVMList.Add(cdVM);
                        }
                        if (cdVMList.Count() > 0)
                        {
                            customerVM.CustomerDesignList = new List<CustomerDesignViewModel>();
                            customerVM.CustomerDesignList = cdVMList;
                        }
                    }

                    List<UserFilesEntity> ufEntity = userFileTable.CreateQuery<UserFilesEntity>().Where(x => x.CustomerID == customerID).Select(x => x).ToList();
                    if (ufEntity != null)
                    {
                        List<UserFilesViewModel> ufVMList = new List<UserFilesViewModel>();
                        foreach (var ufl in ufEntity)
                        {
                            UserFilesViewModel ufVM = new UserFilesViewModel();

                            ufVM.CustomerID = ufl.CustomerID;
                            ufVM.FileNumber = ufl.FileNumber;
                            ufVMList.Add(ufVM);
                        }
                        if (ufVMList.Count() > 0)
                        {
                            customerVM.UserFileList = new List<UserFilesViewModel>();
                            customerVM.UserFileList = ufVMList;
                        }
                    }

                    return await Task.FromResult(new InternalOperationResult(Result.Success, "", customerVM));
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Oop's something went wrong. Please try again.", null);
                }
            }
            catch (Exception e)
            {
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", "CustomerDAL", "GetByIDAsync", DateTime.UtcNow, e, null, customerID, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong. Please try again.", null);
            }
        }

        public static async Task<InternalOperationResult> UpdateCustomerAsync(CustomerDetailViewModel vm, string userID, string userName)
        {
            try
            {
                TableBatchOperation updateCustomerOrder = new TableBatchOperation();
                TableBatchOperation deleteCustomerFile = new TableBatchOperation();

                TableOperation updateDesignerApp = null, updateCustomerApp = null, updateApp = null, updateCustomer = null, updateDesignerOrder = null, deleteUserFile = null, updateOrder = null;

                CustomerEntity cEntity = (from c in customerTable.CreateQuery<CustomerEntity>().Where(c => c.PartitionKey == vm.CustomerID) select c).FirstOrDefault();
                if (cEntity != null)
                {
                    if (cEntity.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff") == (vm.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff")))
                    {
                        Mapper.Initialize(a =>
                        {
                            a.CreateMap<CustomerDetailViewModel, CustomerEntity>().ReverseMap();
                        });
                        CustomerEntity cusEntity = Mapper.Map<CustomerEntity>(vm);

                        cusEntity.PartitionKey = cEntity.PartitionKey;
                        cusEntity.RowKey = cEntity.RowKey;
                        cusEntity.CustomerCodeName = cEntity.CustomerCodeName;
                        cusEntity.CreatedBy = cEntity.CreatedBy;
                        cusEntity.CreatedTS = cEntity.CreatedTS;
                        cusEntity.UpdatedBy = userID;
                        cusEntity.UpdatedTS = DateTime.UtcNow;
                        cusEntity.UploadedFileSize = cEntity.UploadedFileSize;
                        cusEntity.UploadedFileCount = cEntity.UploadedFileCount;

                        if (userID != null && userID != "" && vm.RemoveUserFileList != null && vm.RemoveUserFileList.Count > 0)
                        {
                            foreach (var item in vm.RemoveUserFileList)
                            {
                                await BlobStorage.DeleteBlob(StatusEnum.Blob.userfiles.ToString(), item.FileNumber);

                                UserFilesEntity userfileEntity = userFileTable.ExecuteQuery(new TableQuery<UserFilesEntity>()).Where(c => c.CustomerID == vm.CustomerID && c.FileNumber == item.FileNumber).FirstOrDefault();
                                if (userfileEntity != null)
                                {
                                    deleteUserFile = TableOperation.Delete(userfileEntity);
                                    await userFileTable.ExecuteAsync(deleteUserFile);
                                }
                            }
                        }

                        if (vm.RemoveCustomerFileList != null && vm.RemoveCustomerFileList.Count > 0)
                        {
                            foreach (var item in vm.RemoveCustomerFileList)
                            {
                                cusEntity.UploadedFileSize = cusEntity.UploadedFileSize - BlobStorage.GetFileOrImageSize(StatusEnum.Blob.customerfiles.ToString(), item.FileNumber);
                                cusEntity.UploadedFileCount = cusEntity.UploadedFileCount - 1;

                                await BlobStorage.DeleteBlob(StatusEnum.Blob.customerfiles.ToString(), item.FileNumber);

                                FilesEntity fileEntity = FilesDAL.fileTable.ExecuteQuery(new TableQuery<FilesEntity>()).Where(c => c.PartitionKey == vm.CustomerID && c.FileNumber == item.FileNumber).FirstOrDefault();
                                if (fileEntity != null)
                                {
                                    deleteCustomerFile.Delete(fileEntity);
                                }
                            }
                        }

                        if (vm.RemoveCustomerDesignList != null && vm.RemoveCustomerDesignList.Count > 0)
                        {
                            foreach (var item in vm.RemoveCustomerDesignList)
                            {
                                DesignerAppointmentEntity dAppEntity = CalendarAppointmentDAL.designerAppointmentTable.CreateQuery<DesignerAppointmentEntity>().Where(c => c.DesignNumber == item.DesignNumber).FirstOrDefault();

                                if (dAppEntity != null)
                                {
                                    dAppEntity.DesignNumber = null;
                                    dAppEntity.UpdatedBy = userID;
                                    dAppEntity.UpdatedTS = DateTime.UtcNow;
                                    updateDesignerApp = TableOperation.InsertOrReplace(dAppEntity);
                                    await CalendarAppointmentDAL.designerAppointmentTable.ExecuteAsync(updateDesignerApp);
                                }

                                CustomerAppointmentEntity cappEntity = CalendarAppointmentDAL.customerAppointmentTable.CreateQuery<CustomerAppointmentEntity>().Where(c => c.PartitionKey == vm.CustomerID && c.DesignNumber == item.DesignNumber).FirstOrDefault();

                                if (cappEntity != null)
                                {
                                    cappEntity.DesignNumber = null;
                                    cappEntity.UpdatedBy = userID;
                                    cappEntity.UpdatedTS = DateTime.UtcNow;
                                    updateCustomerApp = TableOperation.InsertOrReplace(cappEntity);
                                    await CalendarAppointmentDAL.customerAppointmentTable.ExecuteAsync(updateCustomerApp);
                                }

                                AppointmentEntity appEntity = CalendarAppointmentDAL.appointmentTable.CreateQuery<AppointmentEntity>().Where(c => c.CustomerID == vm.CustomerID && c.DesignNumber == item.DesignNumber).FirstOrDefault();

                                if (appEntity != null)
                                {
                                    appEntity.DesignNumber = null;
                                    appEntity.UpdatedBy = userID;
                                    appEntity.UpdatedTS = DateTime.UtcNow;
                                    updateApp = TableOperation.InsertOrReplace(appEntity);
                                    await CalendarAppointmentDAL.appointmentTable.ExecuteAsync(updateApp);
                                }

                                List<CustomerOrderEntity> coEntityList = OrderDAL.customerOrderTable.CreateQuery<CustomerOrderEntity>().Where(x => x.PartitionKey == vm.CustomerID && x.DesignNumber == item.DesignNumber).ToList();

                                if (coEntityList != null && coEntityList.Count > 0)
                                {
                                    foreach (var item2 in coEntityList)
                                    {
                                        item2.DesignNumber = null;
                                        item2.UpdatedBy = userID;
                                        item2.UpdatedTS = DateTime.UtcNow;
                                        updateCustomerOrder.InsertOrReplace(item2);
                                    }
                                }

                                List<DesignerOrderEntity> doEntityList = OrderDAL.designerOrderTable.CreateQuery<DesignerOrderEntity>().Where(x => x.DesignNumber == item.DesignNumber).ToList();

                                if (doEntityList != null && doEntityList.Count > 0)
                                {
                                    foreach (var item3 in doEntityList)
                                    {
                                        item3.DesignNumber = null;
                                        item3.UpdatedBy = userID;
                                        item3.UpdatedTS = DateTime.UtcNow;
                                        updateDesignerOrder = TableOperation.InsertOrReplace(item3);
                                        await OrderDAL.designerOrderTable.ExecuteAsync(updateDesignerOrder);
                                    }
                                }

                                List<OrderEntity> oEntityList = OrderDAL.orderTable.ExecuteQuery(new TableQuery<OrderEntity>()).Where(x => x.CustomerID == vm.CustomerID && x.DesignNumber == item.DesignNumber).Select(c => c).OrderByDescending(x => x.UpdatedTS).ToList();

                                if (oEntityList != null && oEntityList.Count > 0)
                                {
                                    foreach (var item1 in oEntityList)
                                    {
                                        if (item1 != null)
                                        {
                                            item1.DesignNumber = null;
                                            item1.UpdatedBy = userID;
                                            item1.UpdatedTS = DateTime.UtcNow;
                                            updateOrder = TableOperation.InsertOrReplace(item1);
                                            await OrderDAL.orderTable.ExecuteAsync(updateOrder);
                                        }
                                    }
                                }
                                await BlobStorage.DeleteBlob(StatusEnum.Blob.designimage.ToString(), item.DesignNumber);
                                await BlobStorage.DeleteBlob(StatusEnum.Blob.vectorimage.ToString(), item.DesignNumber);
                            }
                        }

                        updateCustomer = TableOperation.InsertOrReplace(cusEntity);
                        await customerTable.ExecuteAsync(updateCustomer);

                        if (deleteCustomerFile != null && deleteCustomerFile.Count > 0)
                        {
                            await FilesDAL.fileTable.ExecuteBatchAsync(deleteCustomerFile);
                        }
                        if (updateCustomerOrder != null && updateCustomerOrder.Count > 0)
                        {
                            await OrderDAL.customerOrderTable.ExecuteBatchAsync(updateCustomerOrder);
                        }
                        if (vm.FullName != cEntity.FullName)
                        {
                            await UpdateCustomerNameAsync(vm, cEntity.CustomerCodeName, userID, userName);
                        }
                        TransactionLogDAL.InsertTransactionLog(cEntity.PartitionKey, "CustomerDetail", DateTime.UtcNow.Date, DateTime.UtcNow, cEntity.CreatedBy, "Edited", userName);
                        return new InternalOperationResult(Result.Success, "Customer detail updated successfully.", cEntity.PartitionKey);
                    }
                    else
                    {
                        return new InternalOperationResult(Result.Concurrency, "Data is already changed by someone else. Please try again.", null);
                    }
                }
                else
                {
                    return new InternalOperationResult(Result.UDError, "Customer detail doesn't exists.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = Newtonsoft.Json.JsonConvert.SerializeObject(vm);
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", "CustomerDAL", "UpdateCustomerAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> UploadUserFileAsync(UserFilesViewModel userFileVM, string userID, string userName, double fileSize)
        {
            try
            {
                if (userFileVM != null)
                {
                    Mapper.Initialize(a =>
                    {
                        a.CreateMap<UserFilesEntity, UserFilesViewModel>().ReverseMap();
                    });

                    UserFilesEntity userFileEntity = new UserFilesEntity();

                    userFileEntity = Mapper.Map<UserFilesEntity>(userFileVM);

                    userFileEntity.PartitionKey = userID;
                    userFileEntity.CreatedTS = DateTime.UtcNow;
                    userFileEntity.UpdatedTS = DateTime.UtcNow;
                    userFileEntity.FileNumber = userFileVM.FileNumber;
                    userFileEntity.CustomerID = userFileVM.CustomerID;
                    userFileEntity.AppointmentNumber = userFileVM.CustomerID;
                    userFileEntity.UploadedFileSize = fileSize;
                    TableOperation insert = TableOperation.Insert(userFileEntity);
                    await userFileTable.ExecuteAsync(insert);

                    List<UserFilesEntity> userFileEntityList = userFileTable.CreateQuery<UserFilesEntity>().Where(c => c.CustomerID == userFileVM.CustomerID).ToList();

                    Mapper.Initialize(a =>
                    {
                        a.CreateMap<UserFilesViewModel, UserFilesEntity>().ReverseMap().ForMember(x => x.FileNumber, y => y.MapFrom(z => z.FileNumber)); ;
                    });
                    List<UserFilesViewModel> list = Mapper.Map<List<UserFilesViewModel>>(userFileEntityList);

                    TransactionLogDAL.InsertTransactionLog(userFileEntity.PartitionKey, "CustomerDetail", DateTime.UtcNow.Date, DateTime.UtcNow, userName, "Added", userName);

                    return new InternalOperationResult(Result.Success, "File uploaded successfully.", list);
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Oop's something went wrong. Please try again.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = JsonConvert.SerializeObject(userFileVM);
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", "CustomerDAL", "UploadUserFileAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> DeleteAsync(string customerID, DateTime updatedTS, string userID, string userName)
        {
            try
            {
                TableOperation updateCustomer = null, updateDappOperation = null, updateDesignerOrderOperation = null, updateCappOperation = null, updateCustomerOrderOperation = null, updateOrderOperation = null, updateAppOperation = null, deleteappreqOperation = null, deleteUserFileOperation = null;

                CustomerEntity cEntity = (from e in customerTable.CreateQuery<CustomerEntity>().Where(e => e.PartitionKey == customerID) select e).FirstOrDefault();

                if (cEntity != null && cEntity.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff") == (updatedTS.ToString("dd-MM-yyyHH:mm:ss.fff")))
                {
                    cEntity.Active = false;
                    cEntity.UpdatedBy = userID;
                    cEntity.UpdatedTS = DateTime.UtcNow;

                    updateCustomer = TableOperation.InsertOrReplace(cEntity);

                    List<FilesEntity> customerFilesList = (from c in FilesDAL.fileTable.CreateQuery<FilesEntity>().Where(c => c.PartitionKey == customerID) select c).ToList();
                    if (customerFilesList != null && customerFilesList.Count > 0)
                    {
                        foreach (var item in customerFilesList)
                        {
                            var batchOperation = new TableBatchOperation();
                            var projectionQuery = new TableQuery<DynamicTableEntity>()
                          .Where(TableQuery.GenerateFilterCondition("PartitionKey",
                              QueryComparisons.Equal, item.PartitionKey));

                            foreach (var ft in FilesDAL.fileTable.ExecuteQuery(projectionQuery))
                                batchOperation.Delete(ft);

                            if (batchOperation.Count() > 0)
                            {
                                FilesDAL.fileTable.ExecuteBatch(batchOperation);
                            }
                        }
                    }

                    List<UserFilesEntity> userFilesList = (from c in userFileTable.CreateQuery<UserFilesEntity>().Where(c => c.CustomerID == customerID) select c).ToList();
                    if (userFilesList != null && userFilesList.Count > 0)
                    {
                        foreach (var item in userFilesList)
                        {
                            deleteUserFileOperation = TableOperation.Delete(item);
                            await CustomerDAL.userFileTable.ExecuteAsync(deleteUserFileOperation);
                        }
                    }

                    AppointmentRequestEntity appointmentReqEntity = CustomerAppointmentDAL.appointmentRequestTable.CreateQuery<AppointmentRequestEntity>().Where(c => c.CustomerID == customerID && c.RequestStatusID == 1).Select(c => c).FirstOrDefault();
                    if (appointmentReqEntity != null)
                    {
                        deleteappreqOperation = TableOperation.Delete(appointmentReqEntity);
                        await CustomerAppointmentDAL.appointmentRequestTable.ExecuteAsync(deleteappreqOperation);
                    }

                    List<AppointmentEntity> AppList = CalendarAppointmentDAL.appointmentTable.CreateQuery<AppointmentEntity>().Where(c => c.CustomerID == customerID && c.StatusID != (int)StatusEnum.Status.OrderCreated).ToList();

                    if (AppList != null && AppList.Count() > 0)
                    {
                        foreach (var item in AppList)
                        {
                            CustomerAppointmentEntity customerApp = CalendarAppointmentDAL.customerAppointmentTable.ExecuteQuery(new TableQuery<CustomerAppointmentEntity>()).Where(c => c.PartitionKey == customerID && c.RowKey == item.RowKey).Select(c => c).FirstOrDefault();

                            if (customerApp != null)
                            {
                                customerApp.DesignNumber = null;
                                customerApp.UpdatedBy = userID;
                                customerApp.UpdatedTS = DateTime.UtcNow;
                                updateCappOperation = TableOperation.InsertOrReplace(customerApp);
                                await CalendarAppointmentDAL.customerAppointmentTable.ExecuteAsync(updateCappOperation);
                            }

                            DesignerAppointmentEntity designerApp = CalendarAppointmentDAL.designerAppointmentTable.ExecuteQuery(new TableQuery<DesignerAppointmentEntity>()).Where(c => c.RowKey == item.RowKey).FirstOrDefault();

                            if (designerApp != null)
                            {
                                designerApp.DesignNumber = null;
                                designerApp.UpdatedBy = userID;
                                designerApp.UpdatedTS = DateTime.UtcNow;
                                updateDappOperation = TableOperation.InsertOrReplace(designerApp);
                                await CalendarAppointmentDAL.designerAppointmentTable.ExecuteAsync(updateDappOperation);
                            }

                            if (item.StatusID == (int)StatusEnum.Status.OrderShipped)
                            {
                                List<CustomerOrderEntity> coEntityList = OrderDAL.customerOrderTable.CreateQuery<CustomerOrderEntity>().Where(x => x.PartitionKey == customerID && x.DesignNumber == item.DesignNumber).ToList();

                                if (coEntityList != null && coEntityList.Count() > 0)
                                {
                                    foreach (var item1 in coEntityList)
                                    {
                                        item1.DesignNumber = null;
                                        item1.UpdatedBy = userID;
                                        item1.UpdatedTS = DateTime.UtcNow;
                                        updateCustomerOrderOperation = TableOperation.InsertOrReplace(item1);
                                    }
                                    await OrderDAL.customerOrderTable.ExecuteAsync(updateCustomerOrderOperation);
                                }

                                List<DesignerOrderEntity> doEntityList = OrderDAL.designerOrderTable.CreateQuery<DesignerOrderEntity>().Where(x => x.DesignNumber == item.DesignNumber).ToList();

                                if (doEntityList != null && doEntityList.Count() > 0)
                                {
                                    foreach (var item2 in doEntityList)
                                    {
                                        item2.DesignNumber = null;
                                        item2.UpdatedBy = userID;
                                        item2.UpdatedTS = DateTime.UtcNow;
                                        updateDesignerOrderOperation = TableOperation.InsertOrReplace(item2);
                                    }
                                    await OrderDAL.designerOrderTable.ExecuteAsync(updateDesignerOrderOperation);
                                }

                                List<OrderEntity> oEntityList = OrderDAL.orderTable.CreateQuery<OrderEntity>().Where(x => x.CustomerID == customerID && x.DesignNumber == item.DesignNumber).ToList();
                                if (oEntityList != null && oEntityList.Count() > 0)
                                {
                                    foreach (var item3 in oEntityList)
                                    {
                                        item3.DesignNumber = null;
                                        item3.UpdatedBy = userID;
                                        item3.UpdatedTS = DateTime.UtcNow;
                                        updateOrderOperation = TableOperation.InsertOrReplace(item3);
                                    }
                                    await OrderDAL.orderTable.ExecuteAsync(updateOrderOperation);
                                }
                            }

                            if (item.DesignNumber != null)
                            {
                                await BlobStorage.DeleteBlob(StatusEnum.Blob.designimage.ToString(), item.DesignNumber);
                                await BlobStorage.DeleteBlob(StatusEnum.Blob.vectorimage.ToString(), item.DesignNumber);
                                item.DesignNumber = null;
                            }
                            updateAppOperation = TableOperation.InsertOrReplace(item);
                        }
                        await CalendarAppointmentDAL.appointmentTable.ExecuteAsync(updateAppOperation);
                    }
                    await customerTable.ExecuteAsync(updateCustomer);

                    TransactionLogDAL.InsertTransactionLog(cEntity.PartitionKey, "CustomerProfile", DateTime.UtcNow.Date, DateTime.UtcNow, cEntity.CreatedBy, "Deleted", userName);

                    return new InternalOperationResult(Result.Success, "Customer deleted successfully.", null);
                }
                else
                {
                    return new InternalOperationResult(Result.Concurrency, "Data is already changed by someone else. Please try again.", null);
                }
            }
            catch (Exception e)
            {
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", "CustomerDAL", "DeleteAsync", DateTime.UtcNow, e, null, null, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task UpdateCustomerNameAsync(CustomerDetailViewModel vm, string customerCodeName, string userID, string userName)
        {
            List<AppointmentRequestEntity> appRequestEntityList = CustomerAppointmentDAL.appointmentRequestTable.CreateQuery<AppointmentRequestEntity>().Where(x => x.CustomerID == vm.CustomerID).Select(c => c).ToList();
            if (appRequestEntityList != null && appRequestEntityList.Count > 0)
            {
                foreach (var item in appRequestEntityList)
                {
                    item.CustomerName = vm.FullName;
                    item.CustomerCodeName = customerCodeName;

                    TableOperation uarOperation = TableOperation.InsertOrReplace(item);
                    await CustomerAppointmentDAL.appointmentRequestTable.ExecuteAsync(uarOperation);
                }
            }

            List<DesignerOrderEntity> dOrderEntityList = OrderDAL.designerOrderTable.CreateQuery<DesignerOrderEntity>().Where(x => x.CustomerID == vm.CustomerID).Select(c => c).ToList();
            if (dOrderEntityList != null && dOrderEntityList.Count > 0)
            {
                foreach (var item in dOrderEntityList)
                {
                    item.CustomerName = vm.FullName;
                    item.CustomerCodeName = customerCodeName;
                    item.UpdatedBy = userID;
                    item.UpdatedTS = DateTime.UtcNow;
                    TableOperation udoOperation = TableOperation.InsertOrReplace(item);
                    await OrderDAL.designerOrderTable.ExecuteAsync(udoOperation);
                }
            }

            List<FeedbackEntity> feedbackEntityList = FeedbackDAL.feedbackTable.CreateQuery<FeedbackEntity>().Where(x => x.CustomerID == vm.CustomerID).Select(c => c).ToList();
            if (feedbackEntityList != null && feedbackEntityList.Count > 0)
            {
                foreach (var item in feedbackEntityList)
                {
                    item.CustomerName = vm.FullName;
                    item.CustomerCodeName = customerCodeName;
                    item.UpdatedBy = userID;
                    item.UpdatedTS = DateTime.UtcNow;
                    TableOperation ufbOperation = TableOperation.InsertOrReplace(item);
                    await FeedbackDAL.feedbackTable.ExecuteAsync(ufbOperation);
                }
            }

            List<DesignerAppointmentEntity> designerAppEntityList = CalendarAppointmentDAL.designerAppointmentTable.CreateQuery<DesignerAppointmentEntity>().Where(x => x.CustomerID == vm.CustomerID).Select(c => c).ToList();
            if (designerAppEntityList != null && designerAppEntityList.Count > 0)
            {
                foreach (var item in designerAppEntityList)
                {
                    item.CustomerName = vm.FullName;
                    item.CustomerCodeName = customerCodeName;
                    item.UpdatedBy = userID;
                    item.UpdatedTS = DateTime.UtcNow;
                    TableOperation udaOperation = TableOperation.InsertOrReplace(item);
                    await CalendarAppointmentDAL.designerAppointmentTable.ExecuteAsync(udaOperation);
                }
            }
        }

        #endregion

        #region  Customer Helper Methods 

        public static bool CheckCustomerEmailIDExists(string emailID, bool isFacebookUser, string customerID)
        {
            List<string> customerList = customerTable.CreateQuery<CustomerEntity>().Where(c => c.PartitionKey != customerID && c.IsFacebookUser == isFacebookUser).Select(c => c.EmailID).ToList();
            string ID = customerList.Where(c => c.Split(new string[] { "@" }, StringSplitOptions.None).FirstOrDefault().Replace(".", "") == emailID.Split(new string[] { "@" }, StringSplitOptions.None).FirstOrDefault().Replace(".", "") && c.Split(new string[] { "@" }, StringSplitOptions.None)[1] == emailID.Split(new string[] { "@" }, StringSplitOptions.None)[1]).Select(c => c).FirstOrDefault();
            if (ID != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region User Helper Methods

        public static async Task<List<ListItem>> GetCustomerListAsync(string codeName)
        {
            List<ListItem> customerlist = customerTable.CreateQuery<CustomerEntity>().Where(x => x.Active == true).Select(c => new ListItem
            {
                Text = c.CustomerCodeName,
                Value = c.PartitionKey
            }).ToList();

            customerlist = customerlist.Where(c => c.Text != null && c.Text.ToLower().Contains(codeName.ToLower())).GroupBy(c => c.Text).Select(c => c.First()).Distinct().OrderBy(c => c.Text).ToList();

            return await Task.FromResult(customerlist);
        }

        public static bool ValidateUserFileSize(string userID, string customerID, double fileSize)
        {
            fileSize = fileSize + userFileTable.ExecuteQuery(new TableQuery<UserFilesEntity>()).Where(x => x.CustomerID == customerID).Select(x => x.UploadedFileSize).Sum();
            if (fileSize / 1024f < 20)
                return true;
            else
                return false;
        }

        #endregion
    }
}

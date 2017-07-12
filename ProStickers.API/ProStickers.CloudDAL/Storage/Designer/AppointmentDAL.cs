using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Globalization;
using AutoMapper;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using ProStickers.CloudDAL.Entity.Customer;
using ProStickers.CloudDAL.Storage.Customer;
using ProStickers.ViewModel.Designer;
using ProStickers.ViewModel.Core;
using ProStickers.CloudDAL.Entity.Designer;
using ProStickers.CloudDAL.Storage.ExceptionStorage;
using ProStickers.CloudDAL.Entity.Master;
using ProStickers.ViewModel.Customer;

namespace ProStickers.CloudDAL.Storage.Designer
{
    public class AppointmentDAL
    {
        public static async Task<int> GetAppointmentRequestCountAsync(string userID, string userName)
        {
            double currentSec = Utility.ConvertTimeToSec(Utility.GetCSTDateTime());

            currentSec = currentSec - 45;
            List<AppointmentRequestEntity> oldList = CustomerAppointmentDAL.appointmentRequestTable.CreateQuery<AppointmentRequestEntity>().Where(c => c.StartTime <= currentSec && c.StartTime > 0 && c.RequestStatusID == (int)StatusEnum.AppointmentRequestStatus.Processed).Select(c => c).ToList();
            if (oldList != null && oldList.Count > 0)
            {
                foreach (var item in oldList)
                {
                    item.RequestStatusID = (int)StatusEnum.AppointmentRequestStatus.Created;
                    TableOperation insertOperation = TableOperation.InsertOrReplace(item);
                    await CustomerAppointmentDAL.appointmentRequestTable.ExecuteAsync(insertOperation);
                }
            }
            List<AppointmentRequestEntity> list = CustomerAppointmentDAL.appointmentRequestTable.CreateQuery<AppointmentRequestEntity>().Where(c => c.RequestStatusID == (int)StatusEnum.AppointmentRequestStatus.Created).Select(c => c).ToList();
            return await Task.FromResult(list.Count);
        }

        public static async Task<AppointmentRequestPickViewModel> GetAppointmentRequestAsync()
        {
            AppointmentRequestPickViewModel vm = new AppointmentRequestPickViewModel();
            AppointmentRequestEntity apRequestEntity = CustomerAppointmentDAL.appointmentRequestTable.CreateQuery<AppointmentRequestEntity>().Where(x => x.RequestStatusID == (int)StatusEnum.AppointmentRequestStatus.Created).Select(c => c).FirstOrDefault();
            if (apRequestEntity != null)
            {
                vm.CustomerID = apRequestEntity.CustomerID;
                vm.CustomerName = apRequestEntity.CustomerName;
                vm.RequestDateTime = apRequestEntity.RequestedDateTime;
                vm.RequestStatusID = apRequestEntity.RequestStatusID;
                vm.PhoneNumber = apRequestEntity.PhoneNumber;
                vm.CustomerCodeName = apRequestEntity.CustomerCodeName;

                apRequestEntity.RequestStatusID = (int)StatusEnum.AppointmentRequestStatus.Processed;
                apRequestEntity.StartTime = Utility.ConvertTimeToSec(Utility.GetCSTDateTime());

                TableOperation insertOperation = TableOperation.InsertOrReplace(apRequestEntity);
                await CustomerAppointmentDAL.appointmentRequestTable.ExecuteAsync(insertOperation);
                return vm;
            }
            else
            {
                return vm = null;
            }
        }

        public static async Task<InternalOperationResult> CancelAppointmentRequestAsync(string userID, string userName, AppointmentRequestViewModel vm)
        {
            try
            {
                AppointmentRequestEntity appRequestEntity = CustomerAppointmentDAL.appointmentRequestTable.CreateQuery<AppointmentRequestEntity>().Where(c => c.PartitionKey == Convert.ToDateTime(vm.RequestDateTime).ToString("yyyy-MM-dd") && c.CustomerID == vm.CustomerID).FirstOrDefault();
                if (appRequestEntity != null)
                {
                    appRequestEntity.RequestStatusID = (int)StatusEnum.AppointmentRequestStatus.Created;

                    TableOperation cancelOperation = TableOperation.InsertOrReplace(appRequestEntity);
                    await CustomerAppointmentDAL.appointmentRequestTable.ExecuteAsync(cancelOperation);

                    return new InternalOperationResult(Result.Success, "Appointment request cancelled successfully.", userID.ToString());
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = JsonConvert.SerializeObject(vm);
                ExceptionTableStorage.InsertOrReplaceEntity("Designer", "AppointmentDAL", "CancelAppointmentRequestAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> CreateAsync(string userID, string userName, AppointmentRequestPickViewModel vm)
        {
            try
            {
                AppointmentEntity apEntity = new AppointmentEntity();
                if (vm != null)
                {
                    TableOperation uOperation = null; TableOperation updateOperation = null;
                    DesignerTimeSlotEntity dTimeSlotEntity = DesignerTimeSlotDAL.designerTimeSlotTable.CreateQuery<DesignerTimeSlotEntity>().
                                                           Where(c => c.PartitionKey == userID && c.TimeSlotID == vm.TimeSlotID &&
                                                           c.RowKey == (vm.AppointmentDate.ToString("yyyyMMdd") + "_" + vm.TimeSlotID)
                                                           ).FirstOrDefault();

                    if (dTimeSlotEntity == null)
                    {
                        return new InternalOperationResult(Result.SDError, "There is no appointment slot present for this date.", null);
                    }
                    else if (dTimeSlotEntity != null && dTimeSlotEntity.TimeSlotStatus == (int)StatusEnum.TimeSlotStatus.Booked)
                    {
                        return new InternalOperationResult(Result.SDError, "Appointment already exists.", null);
                    }
                    else if (dTimeSlotEntity != null)
                    {
                        apEntity.PartitionKey = vm.AppointmentDate.ToString("yyyy-MM-dd");

                        string time = TimeSpan.FromMinutes(vm.StartTime).ToString().TrimEnd('0');
                        time = time.Replace(":", string.Empty);

                        apEntity.RowKey = (userID + "-" + vm.AppointmentDate.ToString("yyyyMMdd") + Utility.GetCSTDateTime().ToString("HHmmss") + "-" + vm.TimeSlotID + "-" + time).Trim();
                        apEntity.TimeSlotID = vm.TimeSlotID;
                        apEntity.TimeSlot = vm.TimeSlot;
                        apEntity.CustomerID = vm.CustomerID;
                        apEntity.UserID = userID;
                        apEntity.UserName = userName;
                        apEntity.AppointmentStatusID = (int)StatusEnum.AppointmentStatus.Scheduled;
                        apEntity.AppointmentStatus = StatusEnum.AppointmentStatus.Scheduled.ToString();
                        apEntity.StatusID = (int)StatusEnum.Status.AppointmentScheduled;
                        apEntity.IsCalendarRequest = false;
                        apEntity.StartTime = vm.StartTime;
                        apEntity.EndTime = vm.EndTime;
                        apEntity.RequestDateTime = vm.RequestDateTime;

                        string rDateTime = vm.RequestDateTime.Replace('/', '-');

                        DateTime requestDateTime = DateTime.ParseExact(rDateTime, "MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                        apEntity.RequestDate = requestDateTime.ToString("yyyy-MM-dd");
                        apEntity.RequestTime = Utility.ConvertTime(Convert.ToDateTime(requestDateTime));
                        apEntity.AppointmentDateTime = vm.AppointmentDate.ToString("MM/dd/yyyy ").Replace('-', '/') + Convert.ToDateTime(TimeSpan.FromMinutes(vm.StartTime).ToString()).ToString("HH:mm:ss");
                        apEntity.CreatedBy = userID;
                        apEntity.CreatedTS = DateTime.UtcNow;
                        apEntity.UpdatedBy = userID;
                        apEntity.UpdatedTS = DateTime.UtcNow;

                        TableOperation insertOperation = TableOperation.InsertOrReplace(apEntity);

                        TableOperation iOperation = await FillCustomerAppointmentEntityAsync(vm, userID, userName, apEntity.RowKey);

                        TableOperation _insOperation = await FillDesignerAppointmentEntityAsync(vm, userID, userName, apEntity.RowKey);

                        uOperation = await UpdateDesignerTimeSlotEntityAsync(vm, userID, userName, apEntity.AppointmentStatusID);

                        updateOperation = await UpdateDesignerAvailabilityEntityAsync(vm, userID, userName);

                        await CalendarAppointmentDAL.appointmentTable.ExecuteAsync(insertOperation);
                        await CalendarAppointmentDAL.customerAppointmentTable.ExecuteAsync(iOperation);
                        await CalendarAppointmentDAL.designerAppointmentTable.ExecuteAsync(_insOperation);
                        if (uOperation != null)
                        {
                            await DesignerTimeSlotDAL.designerTimeSlotTable.ExecuteAsync(uOperation);
                        }
                        if (updateOperation != null)
                        {
                            await DesignerTimeSlotDAL.designerAvailabilityTable.ExecuteAsync(updateOperation);
                        }

                        AppointmentRequestEntity appRequestEntity = CustomerAppointmentDAL.appointmentRequestTable.CreateQuery<AppointmentRequestEntity>().Where(c => c.PartitionKey == requestDateTime.ToString("yyyy-MM-dd") && c.CustomerID == vm.CustomerID && c.RequestStatusID == 2).FirstOrDefault();
                        if (appRequestEntity != null)
                        {
                            TableOperation delete = TableOperation.Delete(appRequestEntity);
                            await CustomerAppointmentDAL.appointmentRequestTable.ExecuteAsync(delete);
                        }
                    }
                    TransactionLogDAL.InsertTransactionLog(userID, "Designer", DateTime.UtcNow.Date, DateTime.UtcNow, userID, "Added", userName);

                    return new InternalOperationResult(Result.Success, "Appointment scheduled successfully.", apEntity.RowKey);
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = JsonConvert.SerializeObject(vm);
                ExceptionTableStorage.InsertOrReplaceEntity("Designer", "AppointmentDAL", "CreateAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task GetListAsync<AppointmentListViewModel>(Pager<AppointmentListViewModel> pagerVM, string userID)
        {
            string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userID);

            if (pagerVM.Sort == "CurrentUpdatedTS")
            {
                filter = TableQuery.CombineFilters(filter, TableOperators.And, (TableQuery.GenerateFilterCondition("AppointmentDateTime", QueryComparisons.GreaterThanOrEqual, Utility.GetCSTDateTime().ToString("MM/dd/yyyy ").Replace('-', '/') + Utility.GetCSTDateTime().ToString("HH:mm:ss"))));
            } 

            if (pagerVM.SearchList != null && pagerVM.SearchList.Count() > 0)
            {
                foreach (var item in pagerVM.SearchList.Where(s => s.Value != null && s.Value != ""))
                {
                    if (item.DisplayName == "FromDate")
                    {
                        filter = TableQuery.CombineFilters(filter, TableOperators.And, (TableQuery.GenerateFilterCondition(item.Name, QueryComparisons.GreaterThanOrEqual, DateTime.Parse(item.Value).ToString("yyyy-MM-dd"))));
                    }
                    else if (item.DisplayName == "ToDate")
                    {
                        filter = TableQuery.CombineFilters(filter, TableOperators.And, (TableQuery.GenerateFilterCondition(item.Name, QueryComparisons.LessThanOrEqual, DateTime.Parse(item.Value).ToString("yyyy-MM-dd"))));
                    }
                    else if (item.Value != "All")
                    {
                        filter = TableQuery.CombineFilters(filter, TableOperators.And, (TableQuery.GenerateFilterCondition(item.Name, QueryComparisons.Equal, item.Value)));
                    }  
                }
            }

            TableQuery<DynamicTableEntity> projectionQuery = new TableQuery<DynamicTableEntity>().Select(new string[] { "AppointmentNumber", "AppointmentDateTime", "Date", "CustomerCodeName", "ContactNumber", "RequestDateTime", "AppointmentStatus", "UpdatedTS" }).Where(filter);

            EntityResolver<ViewModel.Designer.AppointmentListViewModel> resolver = (pk, rk, ts, props, etag) => new ViewModel.Designer.AppointmentListViewModel
            {
                AppointmentNumber = props["AppointmentNumber"].StringValue,
                AppointmentDate = props["Date"].StringValue,
                AppointmentDateTime = props["AppointmentDateTime"].StringValue,
                Customer = props["CustomerCodeName"].StringValue,
                ContactNumber = props["ContactNumber"].StringValue,
                RequestDateTime = props["RequestDateTime"].StringValue,
                Status = props["AppointmentStatus"].StringValue,
                UpdatedTS = props["UpdatedTS"].DateTime.Value,  
            };

            List<ViewModel.Designer.AppointmentListViewModel> list = CalendarAppointmentDAL.designerAppointmentTable.ExecuteQuery(projectionQuery, resolver, null, null).ToList();

            if (list != null && list.Count() > 0)
            {
                pagerVM.RecordsCount = list.Count();

                if (pagerVM.Sort == "UpdatedTS")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        list = list.OrderByDescending(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }

                if (pagerVM.Sort == "CurrentUpdatedTS")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        list = list.OrderByDescending(s => s.AppointmentDateTime).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(s => DateTime.ParseExact(s.AppointmentDateTime, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) >= Utility.GetCSTDateTime()).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    pagerVM.RecordsCount = list.Count;
                }

                else if (pagerVM.Sort == "AppointmentDateTime")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        list = list.OrderByDescending(s => s.AppointmentDateTime).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(s => s.AppointmentDateTime).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }

                else if (pagerVM.Sort == "AppointmentNumber")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        list = list.OrderByDescending(s => s.AppointmentNumber).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(s => s.AppointmentNumber).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "CustomerName")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        list = list.OrderByDescending(s => s.Customer).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(s => s.Customer).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "ContactNumber")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        list = list.OrderByDescending(s => s.ContactNumber).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(s => s.ContactNumber).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "AppointmentStatus")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        list = list.OrderByDescending(s => s.Status).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(s => s.Status).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "RequestDateTime")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        list = list.OrderByDescending(s => s.RequestDateTime).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(s => s.RequestDateTime).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
            }

            Mapper.Initialize(a =>
            {
                a.CreateMap<ViewModel.Designer.AppointmentListViewModel, AppointmentListViewModel>().ReverseMap();
            });

            await Task.FromResult(pagerVM.Data = Mapper.Map<List<AppointmentListViewModel>>(list));
        }

        public static async Task<InternalOperationResult> GetByIDAsync(string userID, string userName, string appointmentNumber, DateTime appointmentDate)
        {
            try
            {
                AppointmentEntity appEntity = CalendarAppointmentDAL.appointmentTable.CreateQuery<AppointmentEntity>().Where(c => c.PartitionKey == appointmentDate.ToString("yyyy-MM-dd") && c.RowKey == appointmentNumber.ToString()).FirstOrDefault();

                if (appEntity != null)
                {
                    AppointmentViewModel appointmentVM = new AppointmentViewModel();

                    appointmentVM.DesignerAppointmentFileList = new List<UserFilesViewModel>();
                    appointmentVM.DesignImageList = new List<UserFilesViewModel>();

                    CustomerEntity cEntity = CustomerDAL.customerTable.CreateQuery<CustomerEntity>().Where(c => c.PartitionKey == appEntity.CustomerID).FirstOrDefault();
                    if (cEntity != null)
                    {
                        Mapper.Initialize(a =>
                        {
                            a.CreateMap<AppointmentViewModel, AppointmentEntity>().ReverseMap();
                        });

                        appointmentVM = Mapper.Map<AppointmentViewModel>(appEntity);

                        appointmentVM.AppointmentNumber = appEntity.RowKey;
                        appointmentVM.AppointmentDateTime = appEntity.AppointmentDateTime;
                        appointmentVM.CustomerName = cEntity.FullName;
                        appointmentVM.CustomerContactNumber = cEntity.ContactNo;
                        appointmentVM.FirstName = cEntity.FirstName;
                        appointmentVM.MiddleName = (cEntity.MiddleName != null && cEntity.MiddleName != "") ? cEntity.MiddleName : null;
                        appointmentVM.LastName = cEntity.LastName;
                        appointmentVM.ContactNumber = cEntity.ContactNo;
                        appointmentVM.EmailID = cEntity.EmailID;
                        appointmentVM.SkypeID = (cEntity.SkypeID != null && cEntity.SkypeID != "") ? cEntity.SkypeID : null;
                        appointmentVM.Notes = cEntity.FileNote;
                        appointmentVM.AppointmentDate = appointmentDate;
                        appointmentVM.RequestDateTime = appEntity.RequestDateTime;
                        if (appEntity.AppointmentStatusID != (int)StatusEnum.AppointmentStatus.Scheduled)
                        {
                            appointmentVM.CallStartTime = Convert.ToDateTime(TimeSpan.FromMinutes(appEntity.StartTime).ToString());
                        }
                        appointmentVM.MeetingLink = appEntity.MeetingLink;
                        appointmentVM.DesignerNote = appEntity.DesignerNote;

                        if (appointmentVM.AppointmentStatus == StatusEnum.AppointmentStatus.Completed.ToString())
                        {
                            appointmentVM.CallEndTime = Convert.ToDateTime(TimeSpan.FromMinutes(appEntity.EndTime).ToString());
                        }

                        if (appEntity.AppointmentStatusID == (int)StatusEnum.AppointmentStatus.Cancelled)
                        {
                            appointmentVM.IsCancel = true;
                        }

                        StringBuilder s = new StringBuilder();                      
                        s.Append(cEntity.Address1 != null && cEntity.Address1 != "" ? cEntity.Address1 : "");
                        s.Append(cEntity.Address2 != null && cEntity.Address2 != "" ? ", " + cEntity.Address2 : "");
                        s.Append(cEntity.City != null && cEntity.City != "" ? ", " + cEntity.City : "");                      
                        s.Append(cEntity.StateName != null && cEntity.StateName != "" ? ", " + cEntity.StateName : "");
                        s.Append(cEntity.PostalCode != null && cEntity.PostalCode != "" ? ", " + cEntity.PostalCode : "");
                        s.Append(cEntity.CountryName != null && cEntity.CountryName != "" ? ", " + cEntity.CountryName : "");

                        appointmentVM.BillingAddress = s.ToString() != null && s.ToString() != "" ? s.ToString() : null;

                        List<FilesEntity> fileEntityList = FilesDAL.fileTable.CreateQuery<FilesEntity>().Where(c => c.PartitionKey == appEntity.CustomerID).ToList();
                        appointmentVM.FileList = new List<CustomerFileViewModel>();

                        if (fileEntityList != null && fileEntityList.Count > 0)
                        {
                            foreach (var item in fileEntityList.Where(x => x.FileNumber != null))
                            {
                                CustomerFileViewModel vm = new CustomerFileViewModel();
                                vm.FileNumber = item.FileNumber;
                                vm.Image = null;
                                appointmentVM.FileList.Add(vm);
                            }
                        }

                        List<UserFilesEntity> userFileEntityList = CustomerDAL.userFileTable.CreateQuery<UserFilesEntity>().Where(c => c.PartitionKey == userID && c.AppointmentNumber == appEntity.RowKey && c.CustomerID == appointmentVM.CustomerID).ToList();

                        if (userFileEntityList != null && userFileEntityList.Count > 0)
                        {
                            Mapper.Initialize(a =>
                            {
                                a.CreateMap<UserFilesViewModel, UserFilesEntity>().ReverseMap();
                            });
                            appointmentVM.DesignerAppointmentFileList = Mapper.Map<List<UserFilesViewModel>>(userFileEntityList);
                        }
                        List<CustomerAppointmentEntity> list = CalendarAppointmentDAL.customerAppointmentTable.CreateQuery<CustomerAppointmentEntity>().Where(c => c.PartitionKey == appEntity.CustomerID).ToList();

                        if (list != null && list.Count > 0)
                        {
                            Mapper.Initialize(a =>
                            {
                                a.CreateMap<CustomerAppointmentEntity, UserFilesViewModel>().
                                 ForMember(x => x.DesignerName, y => y.MapFrom(z => z.UserName)).
                                 ForMember(x => x.FileNumber, y => y.MapFrom(z => z.DesignNumber)).
                                 ForMember(x => x.UserID, y => y.MapFrom(z => z.UserID));
                            });
                            appointmentVM.DesignImageList = Mapper.Map<List<UserFilesViewModel>>(list.Where(x => x.DesignNumber != null));
                        }
                        return await Task.FromResult(new InternalOperationResult(Result.Success, "", appointmentVM));
                    }
                    else
                    {
                        return new InternalOperationResult(Result.SDError, "Oop's something went wrong. Please try again.", null);
                    }
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Oop's something went wrong. Please try again.", null);
                }
            }
            catch (Exception e)
            {
                ExceptionTableStorage.InsertOrReplaceEntity("Designer", "AppointmentDAL", "GetByIDAsync", DateTime.UtcNow, e, null, appointmentNumber, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong. Please try again.", null);
            }
        }

        public static async Task<InternalOperationResult> UpdateAsync(string userID, string userName, AppointmentViewModel vm)
        {
            try
            {
                AppointmentEntity appEntity = (from c in CalendarAppointmentDAL.appointmentTable.CreateQuery<AppointmentEntity>().Where(c => c.PartitionKey == vm.AppointmentDate.ToString("yyyy-MM-dd") && c.RowKey == vm.AppointmentNumber) select c).FirstOrDefault();
                TableOperation insOperation = null; TableOperation iOperation = null;
                if (appEntity != null && vm.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff") == (appEntity.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff")))
                {
                    appEntity.UpdatedBy = userID;
                    appEntity.UpdatedTS = DateTime.UtcNow;
                    appEntity.ImageStatusID = vm.ImageStatusID;
                    appEntity.DesignerNote = vm.DesignerNote;
                    appEntity.MeetingLink = vm.MeetingLink;
                    appEntity.AspectRatio = vm.AspectRatio;

                    if (vm.DesignImageBuffer != null || vm.VectorImageBuffer != null)
                    {
                        if (appEntity.DesignNumber == null)
                        {
                            appEntity.DesignNumber = (vm.CustomerID + "-" + Utility.GetCSTDateTime().ToString("yyyyMMdd") + "-" + Utility.GetCSTDateTime().TimeOfDay.ToString("hhmmss")).Trim();
                        }
                    }
                    if (vm.DesignImageBuffer != null)
                    {
                        await BlobStorage.UploadPrivateImage(StatusEnum.Blob.designimage.ToString(), vm.DesignImageBuffer, appEntity.DesignNumber, await CommonDAL.GetContentType(vm.DesignImageExtension));
                    }
                    if (vm.VectorImageBuffer != null)
                    {
                        await BlobStorage.UploadPrivateImage(StatusEnum.Blob.vectorimage.ToString(), vm.VectorImageBuffer, appEntity.DesignNumber, await CommonDAL.GetContentType(vm.VectorImageExtension));
                    }

                    iOperation = await UpdateCustomerAppointmentEntityAsync(vm, userID, userName, appEntity.DesignNumber);

                    insOperation = await UpdateDesignerAppointmentEntityAsync(vm, userID, userName, appEntity.DesignNumber);

                    TableOperation insertOperation = TableOperation.InsertOrReplace(appEntity);
                    await CalendarAppointmentDAL.appointmentTable.ExecuteAsync(insertOperation);

                    if (iOperation != null)
                    {
                        await CalendarAppointmentDAL.customerAppointmentTable.ExecuteAsync(iOperation);
                    }
                    if (insOperation != null)
                    {
                        await CalendarAppointmentDAL.designerAppointmentTable.ExecuteAsync(insOperation);
                    }
                    TransactionLogDAL.InsertTransactionLog(userID, "Designer", DateTime.UtcNow.Date, DateTime.UtcNow, userID, "Edited", userName);

                    return new InternalOperationResult(Result.Success, "Customer appointment updated successfully.", userID.ToString());
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Data is already changed by someone else.Please try again.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = JsonConvert.SerializeObject(vm);
                ExceptionTableStorage.InsertOrReplaceEntity("Designer", "AppointmentDAL", "UpdateAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> UpdateAppointmentStatusAsync(string userID, string userName, AppointmentViewModel vm)
        {
            try
            {
                if (vm != null)
                {
                    AppointmentEntity appEntity = (from c in CalendarAppointmentDAL.appointmentTable.CreateQuery<AppointmentEntity>().Where(c => c.PartitionKey == vm.AppointmentDate.ToString("yyyy-MM-dd") && c.RowKey == vm.AppointmentNumber) select c).FirstOrDefault();

                    if (appEntity.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff") == (vm.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff")))
                    {
                        TableOperation uOperation = null; TableOperation updateOperation = null; TableOperation insOperation = null; TableOperation iOperation = null;

                        TableOperation _uDTimeSlotOperation = null;

                        if (vm.CallStartTime != DateTime.MinValue && vm.IsCancel == false && vm.CancellationReason == null)
                        {
                            AppointmentEntity _appointmentEntity = CalendarAppointmentDAL.appointmentTable.CreateQuery<AppointmentEntity>().Where(c => c.PartitionKey == vm.AppointmentDate.ToString("yyyy-MM-dd") && c.TimeSlotID == (vm.TimeSlotID - 1) && c.AppointmentStatusID == (int)StatusEnum.AppointmentStatus.Completed).Select(c => c).FirstOrDefault();
                            if (_appointmentEntity != null && Utility.ConvertTime(vm.CallStartTime) < _appointmentEntity.EndTime)
                            {
                                return new InternalOperationResult(Result.UDError, "Appointment start time should be greater than or equal to previous appointment end time.", userID.ToString());
                            }
                            appEntity.StartTime = vm.CallStartTime != DateTime.MinValue ? Utility.ConvertTime(vm.CallStartTime) : appEntity.StartTime; ;
                            appEntity.AppointmentDateTime = vm.AppointmentDate.ToString("MM/dd/yyyy ").Replace('-', '/') + vm.CallStartTime.ToString("HH:mm:ss");
                            appEntity.AppointmentStatusID = (int)StatusEnum.AppointmentStatus.Initiated;
                            appEntity.AppointmentStatus = StatusEnum.AppointmentStatus.Initiated.ToString();
                        }

                        if (vm.CallEndTime != DateTime.MinValue)
                        {
                            AppointmentEntity _aEntity = CalendarAppointmentDAL.appointmentTable.CreateQuery<AppointmentEntity>().Where(c => c.PartitionKey == vm.AppointmentDate.ToString("yyyy-MM-dd") && c.TimeSlotID == (vm.TimeSlotID + 1) && c.AppointmentStatusID == (int)StatusEnum.AppointmentStatus.Completed).Select(c => c).FirstOrDefault();
                            if (vm.CallEndTime > vm.CallStartTime && vm.CallEndTime <= vm.CallStartTime.AddMinutes(25))
                            {
                                appEntity.EndTime = vm.CallEndTime != DateTime.MinValue ? Utility.ConvertTime(vm.CallEndTime) : appEntity.EndTime;
                                appEntity.AppointmentStatusID = (int)StatusEnum.AppointmentStatus.Completed;
                                appEntity.AppointmentStatus = StatusEnum.AppointmentStatus.Completed.ToString();
                                appEntity.StatusID = (int)StatusEnum.Status.AppointmentCompleted;

                                if (Utility.ConvertTime(vm.CallEndTime) < vm.EndTime)
                                {
                                    DesignerTimeSlotEntity dTimeSlotEntity = DesignerTimeSlotDAL.designerTimeSlotTable.CreateQuery<DesignerTimeSlotEntity>().Where(c => c.PartitionKey == userID && c.TimeSlotID == vm.TimeSlotID && c.RowKey == (vm.AppointmentDate.ToString("yyyyMMdd") + "_" + vm.TimeSlotID)).FirstOrDefault();
                                    if (dTimeSlotEntity != null)
                                    {
                                        dTimeSlotEntity.IsUserAvailable = true;
                                        dTimeSlotEntity.TimeSlotStatus = (int)StatusEnum.TimeSlotStatus.AvailableAfterBooked;
                                        uOperation = TableOperation.InsertOrReplace(dTimeSlotEntity);
                                    }
                                    updateOperation = UpdateDesignerAvailability(userID, userName, vm);
                                }
                            }
                            else
                            {
                                return new InternalOperationResult(Result.UDError, "Appointment end time should be greater than appointment start time.", userID.ToString());
                            }
                        }
                        if (vm.CancellationReason != null && vm.IsCancel == true)
                        {
                            appEntity.AppointmentStatusID = (int)StatusEnum.AppointmentStatus.Cancelled;
                            appEntity.AppointmentStatus = StatusEnum.AppointmentStatus.Cancelled.ToString();
                            appEntity.CancellationReason = vm.CancellationReason;
                            appEntity.StatusID = (int)StatusEnum.Status.AppointmentCancelled;

                            DesignerTimeSlotEntity dTimeSlotEntity = DesignerTimeSlotDAL.designerTimeSlotTable.CreateQuery<DesignerTimeSlotEntity>().Where(c => c.PartitionKey == userID && c.TimeSlotID == vm.TimeSlotID && c.RowKey == (vm.AppointmentDate.ToString("yyyyMMdd") + "_" + vm.TimeSlotID)).FirstOrDefault();
                            if (dTimeSlotEntity != null)
                            {
                                dTimeSlotEntity.IsUserAvailable = true;
                                dTimeSlotEntity.TimeSlotStatus = (int)StatusEnum.TimeSlotStatus.Available;
                                dTimeSlotEntity.AppointmentStatus = (int)StatusEnum.AppointmentStatus.Cancelled;
                                uOperation = TableOperation.InsertOrReplace(dTimeSlotEntity);
                            }
                            updateOperation = UpdateDesignerAvailability(userID, userName, vm);
                        }
                        appEntity.UpdatedBy = userID;
                        appEntity.UpdatedTS = DateTime.UtcNow;
                        appEntity.AspectRatio = vm.AspectRatio;
                        appEntity.DesignerNote = vm.DesignerNote;
                        appEntity.MeetingLink = vm.MeetingLink;

                        TableOperation insertOperation = TableOperation.InsertOrReplace(appEntity);

                        iOperation = await UpdateCustomerAppointmentEntityAsync(vm, userID, userName, appEntity.DesignNumber);

                        insOperation = await UpdateDesignerAppointmentEntityAsync(vm, userID, userName, appEntity.DesignNumber);

                        if (uOperation == null)
                        {
                            _uDTimeSlotOperation = await UpdateDesignerTimeSlotStatusAsync(vm, userID, userName, appEntity.AppointmentStatusID);
                        }
                        await CalendarAppointmentDAL.appointmentTable.ExecuteAsync(insertOperation);

                        if (iOperation != null)
                        {
                            await CalendarAppointmentDAL.customerAppointmentTable.ExecuteAsync(iOperation);
                        }
                        if (insOperation != null)
                        {
                            await CalendarAppointmentDAL.designerAppointmentTable.ExecuteAsync(insOperation);
                        }
                        if (uOperation != null)
                        {
                            await DesignerTimeSlotDAL.designerTimeSlotTable.ExecuteAsync(uOperation);
                        }
                        if (updateOperation != null)
                        {
                            await DesignerTimeSlotDAL.designerAvailabilityTable.ExecuteAsync(updateOperation);
                        }
                        if (_uDTimeSlotOperation != null)
                        {
                            await DesignerTimeSlotDAL.designerTimeSlotTable.ExecuteAsync(_uDTimeSlotOperation);
                        }
                        return new InternalOperationResult(Result.Success, "Appointment " + vm.AppointmentStatus.ToLower() + " successfully.", appEntity.UpdatedTS);
                    }
                    else
                    {
                        return new InternalOperationResult(Result.SDError, "Data is already changed by someone else. Please try again.", null);
                    }
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = JsonConvert.SerializeObject(vm);
                ExceptionTableStorage.InsertOrReplaceEntity("Designer", "AppointmentDAL", "UpdateAppointmentStatusAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> UploadUserFileAsync(string userID, string userName, UserFilesViewModel dAppFileVM)
        {
            try
            {
                if (dAppFileVM != null)
                {
                    dAppFileVM.FileNumber = userID + "-" + Utility.GetCSTDateTime().ToString("yyyyMMdd") + "-" + Utility.GetCSTDateTime().TimeOfDay.ToString("hhmmss") + "-" + dAppFileVM.FileName + "." + dAppFileVM.FileExtension;
                    await BlobStorage.UploadPrivateImage(StatusEnum.Blob.userfiles.ToString(), dAppFileVM.FileBuffer, dAppFileVM.FileNumber, await CommonDAL.GetContentType(dAppFileVM.FileExtension));

                    UserFilesEntity userFileEntity = new UserFilesEntity(userID);

                    userFileEntity.AppointmentNumber = dAppFileVM.AppointmentNumber;
                    userFileEntity.FileNumber = dAppFileVM.FileNumber;
                    userFileEntity.CustomerID = dAppFileVM.CustomerID;
                    userFileEntity.CreatedTS = DateTime.UtcNow;
                    userFileEntity.UpdatedTS = DateTime.UtcNow;

                    TableOperation insert = TableOperation.Insert(userFileEntity);
                    await CustomerDAL.userFileTable.ExecuteAsync(insert);

                    List<UserFilesEntity> userFileEntityList = CustomerDAL.userFileTable.CreateQuery<UserFilesEntity>().
                                                               Where(c => c.PartitionKey == userID && c.AppointmentNumber == userFileEntity.AppointmentNumber && c.CustomerID == dAppFileVM.CustomerID).ToList();

                    Mapper.Initialize(a =>
                    {
                        a.CreateMap<UserFilesViewModel, UserFilesEntity>().ReverseMap().ForMember(x => x.FileNumber, y => y.MapFrom(z => z.FileNumber)); ;
                    });
                    List<UserFilesViewModel> list = Mapper.Map<List<UserFilesViewModel>>(userFileEntityList);

                    TransactionLogDAL.InsertTransactionLog(userFileEntity.PartitionKey, "Customer Appointment", DateTime.UtcNow.Date, DateTime.UtcNow, userName, "Added", userName);

                    return new InternalOperationResult(Result.Success, "File Uploaded successfully.", list);
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Oop's something went wrong. Please try again.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = JsonConvert.SerializeObject(dAppFileVM);
                ExceptionTableStorage.InsertOrReplaceEntity("Designer", "AppointmentDAL", "UploadUserFileAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<CustomerDesignViewModel> GetDesignerNoteAsync(string designNumber, string userID)
        {
            DesignerAppointmentEntity dAppEntity = CalendarAppointmentDAL.designerAppointmentTable.CreateQuery<DesignerAppointmentEntity>().Where(c => c.PartitionKey == userID && c.DesignNumber == designNumber).FirstOrDefault();

            CustomerDesignViewModel vm = new CustomerDesignViewModel();
            if (dAppEntity != null)
            {
                Mapper.Initialize(a =>
                {
                    a.CreateMap<DesignerAppointmentEntity, CustomerDesignViewModel>().
                    ForMember(x => x.UserID, y => y.MapFrom(z => z.PartitionKey));
                });
                vm = Mapper.Map<CustomerDesignViewModel>(dAppEntity);
            }
            return await Task.FromResult(vm);
        }

        public static async Task<InternalOperationResult> UpdateDesignerNoteAsync(string userID, CustomerDesignViewModel customerDVM)
        {
            try
            {
                DesignerAppointmentEntity designerEntity = CalendarAppointmentDAL.designerAppointmentTable.ExecuteQuery(new TableQuery<DesignerAppointmentEntity>()).
                                                               Where(x => x.DesignNumber == customerDVM.DesignNumber && x.PartitionKey == customerDVM.UserID).FirstOrDefault();

                TableOperation appointmentInsert = null; TableOperation customerAppointmentInsert = null;
                if (designerEntity != null && designerEntity.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff") == (customerDVM.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff")))
                {
                    AppointmentEntity appointmentEntity = CalendarAppointmentDAL.appointmentTable.ExecuteQuery(new TableQuery<AppointmentEntity>()).
                                                          Where(x => x.DesignNumber == customerDVM.DesignNumber && x.UserID == customerDVM.UserID).FirstOrDefault();

                    if (appointmentEntity != null)
                    {
                        appointmentEntity.DesignerNote = customerDVM.DesignerNote;
                        appointmentEntity.UpdatedBy = userID;
                        appointmentEntity.UpdatedTS = DateTime.UtcNow;
                        appointmentInsert = TableOperation.InsertOrReplace(appointmentEntity);
                    }

                    CustomerAppointmentEntity customerEntity = CalendarAppointmentDAL.customerAppointmentTable.ExecuteQuery(new TableQuery<CustomerAppointmentEntity>()).
                                                               Where(x => x.DesignNumber == customerDVM.DesignNumber && x.UserID == customerDVM.UserID).FirstOrDefault();

                    if (customerEntity != null)
                    {
                        customerEntity.DesignerNote = customerDVM.DesignerNote;
                        customerEntity.UpdatedBy = userID;
                        customerEntity.UpdatedTS = DateTime.UtcNow;
                        customerAppointmentInsert = TableOperation.InsertOrReplace(customerEntity);
                    }

                    designerEntity.DesignerNote = customerDVM.DesignerNote;
                    designerEntity.UpdatedBy = userID;
                    designerEntity.UpdatedTS = DateTime.UtcNow;
                    TableOperation designerAppointmentInsert = TableOperation.InsertOrReplace(designerEntity);

                    if (appointmentInsert != null)
                    {
                        await CalendarAppointmentDAL.appointmentTable.ExecuteAsync(appointmentInsert);
                    }
                    if (customerAppointmentInsert != null)
                    {
                        await CalendarAppointmentDAL.customerAppointmentTable.ExecuteAsync(customerAppointmentInsert);
                    }
                    await CalendarAppointmentDAL.designerAppointmentTable.ExecuteAsync(designerAppointmentInsert);
                    return new InternalOperationResult(Result.Success, "Designer note updated successfully.", designerEntity.AppointmentNumber);
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Data is already changed by someone else. Please try again.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = JsonConvert.SerializeObject(customerDVM);
                ExceptionTableStorage.InsertOrReplaceEntity("Designer", "AppointmentDAL", "UpdateDesignerNoteAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        #region HelperMethods

        public static async Task<List<TimeSlotViewModel>> GetAppointmentTimeSlotListAsync(string userID, DateTime date)
        {
            DateTime d = Utility.GetCSTDateTime();

            List<TimeSlotViewModel> list = new List<TimeSlotViewModel>();
            if (d.Date == date)
            {
                int time = Utility.ConvertTime(d);

                list = DesignerTimeSlotDAL.designerTimeSlotTable.CreateQuery<DesignerTimeSlotEntity>().
                    Where(c => c.UserID == userID && c.Date == d.ToString("yyyy-MM-dd") && c.EndTime >= time && (c.TimeSlotStatus == (int)StatusEnum.TimeSlotStatus.Available || c.TimeSlotStatus == (int)StatusEnum.TimeSlotStatus.AvailableAfterBooked)).
                     Select(c => new TimeSlotViewModel
                     {
                         Text = c.Name,
                         Value = c.TimeSlotID,
                         StartTime = c.StartTime,
                         EndTime = c.EndTime
                     }).ToList();

                TimeSlotViewModel timeSlot = list.Where(x => x.StartTime <= time && time <= x.EndTime).Select(c => c).FirstOrDefault();

                if (timeSlot != null)
                {
                    AppointmentEntity oldAppointmentEntity = CalendarAppointmentDAL.appointmentTable.CreateQuery<AppointmentEntity>().Where(c => c.PartitionKey == d.ToString("yyyy-MM-dd") && c.UserID == userID && c.AppointmentStatusID == (int)StatusEnum.AppointmentStatus.Completed && c.TimeSlotID == (int)timeSlot.Value).FirstOrDefault();
                    if (oldAppointmentEntity != null)
                    {
                        foreach (var item in list)
                        {
                            if (item.Value == oldAppointmentEntity.TimeSlotID)
                            {
                                item.StartTime = oldAppointmentEntity.EndTime + 1;
                            }
                        }
                    }
                }
                list = list.Where(c => c.Text != null && c.Text != "").Select(c => c).ToList();
            }
            else
            {
                list = DesignerTimeSlotDAL.designerTimeSlotTable.CreateQuery<DesignerTimeSlotEntity>().
                       Where(c => c.UserID == userID && c.Date == date.ToString("yyyy-MM-dd") && (c.TimeSlotStatus == (int)StatusEnum.TimeSlotStatus.Available || c.TimeSlotStatus == (int)StatusEnum.TimeSlotStatus.AvailableAfterBooked)).
                        Select(c => new TimeSlotViewModel
                        {
                            Text = c.Name,
                            Value = c.TimeSlotID,
                            StartTime = c.StartTime,
                            EndTime = c.EndTime
                        }).ToList();
                list = list.Where(c => c.Text != null && c.Text != "").Select(c => c).ToList();
            }

            list = list.OrderBy(x => x.Value).ToList();
            return await Task.FromResult(list);
        }

        public static async Task<TableOperation> FillCustomerAppointmentEntityAsync(AppointmentRequestPickViewModel vm, string userID, string userName, string rowKey)
        {
            CustomerAppointmentEntity cAppointmentEntity = new CustomerAppointmentEntity();

            cAppointmentEntity.PartitionKey = vm.CustomerID;
            cAppointmentEntity.RowKey = rowKey;
            cAppointmentEntity.AppointmentNumber = rowKey;
            cAppointmentEntity.AppointmentStatus = StatusEnum.AppointmentStatus.Scheduled.ToString();
            cAppointmentEntity.AppointmentStatusID = (int)StatusEnum.AppointmentStatus.Scheduled;
            cAppointmentEntity.Date = vm.AppointmentDate.ToString("yyyy-MM-dd");
            cAppointmentEntity.RequestDateTime = vm.RequestDateTime;

            string rDateTime = vm.RequestDateTime.Replace('/', '-');
            DateTime requestDateTime = DateTime.ParseExact(rDateTime, "MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            cAppointmentEntity.RequestDate = requestDateTime.ToString("yyyy-MM-dd");
            cAppointmentEntity.RequestTime = Utility.ConvertTime(Convert.ToDateTime(requestDateTime));
            cAppointmentEntity.StartTime = vm.StartTime;
            cAppointmentEntity.EndTime = vm.EndTime;
            cAppointmentEntity.UserID = userID;
            cAppointmentEntity.StatusID = (int)StatusEnum.Status.AppointmentScheduled;
            cAppointmentEntity.AppointmentDateTime = vm.AppointmentDate.ToString("MM/dd/yyyy ").Replace('-', '/') + Convert.ToDateTime(TimeSpan.FromMinutes(vm.StartTime).ToString()).ToString("HH:mm:ss");
            cAppointmentEntity.UserName = userName;
            cAppointmentEntity.CreatedBy = userID;
            cAppointmentEntity.CreatedTS = DateTime.UtcNow;
            cAppointmentEntity.UpdatedBy = userID;
            cAppointmentEntity.UpdatedTS = DateTime.UtcNow;
          
            TableOperation insertOperation = TableOperation.InsertOrReplace(cAppointmentEntity);

            return await Task.FromResult(insertOperation);
        }

        public static async Task<TableOperation> FillDesignerAppointmentEntityAsync(AppointmentRequestPickViewModel vm, string userID, string userName, string rowKey)
        {
            DesignerAppointmentEntity dAppointmentEntity = new DesignerAppointmentEntity();

            dAppointmentEntity.PartitionKey = userID;
            dAppointmentEntity.RowKey = rowKey;
            dAppointmentEntity.AppointmentNumber = rowKey;
            dAppointmentEntity.AppointmentStatus = StatusEnum.AppointmentStatus.Scheduled.ToString();
            dAppointmentEntity.AppointmentStatusID = (int)StatusEnum.AppointmentStatus.Scheduled;
            dAppointmentEntity.CustomerID = vm.CustomerID;
            dAppointmentEntity.CustomerCodeName = vm.CustomerCodeName;
            dAppointmentEntity.CustomerName = vm.CustomerName;
            dAppointmentEntity.ContactNumber = vm.PhoneNumber;
            dAppointmentEntity.Date = vm.AppointmentDate.Date.ToString("yyyy-MM-dd");
            dAppointmentEntity.RequestDateTime = vm.RequestDateTime.ToString();

            string rDateTime = vm.RequestDateTime.Replace('/', '-');
            DateTime requestDateTime = DateTime.ParseExact(rDateTime, "MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            dAppointmentEntity.RequestDate = requestDateTime.ToString("yyyy-MM-dd");
            dAppointmentEntity.RequestTime = Utility.ConvertTime(Convert.ToDateTime(requestDateTime));
            dAppointmentEntity.StartTime = vm.StartTime;
            dAppointmentEntity.EndTime = vm.EndTime;
            dAppointmentEntity.UserName = userName;
            dAppointmentEntity.StatusID = (int)StatusEnum.Status.AppointmentScheduled;
            dAppointmentEntity.AppointmentDateTime = vm.AppointmentDate.ToString("MM/dd/yyyy ").Replace('-', '/') + Convert.ToDateTime(TimeSpan.FromMinutes(vm.StartTime).ToString()).ToString("HH:mm:ss");
            dAppointmentEntity.CreatedBy = userID;
            dAppointmentEntity.CreatedTS = DateTime.UtcNow;
            dAppointmentEntity.UpdatedBy = userID;
            dAppointmentEntity.UpdatedTS = DateTime.UtcNow;

            TableOperation insertOperation = TableOperation.InsertOrReplace(dAppointmentEntity);

            return await Task.FromResult(insertOperation);
        }

        public static async Task<TableOperation> UpdateCustomerAppointmentEntityAsync(AppointmentViewModel vm, string userID, string userName, string designNumber)
        {
            CustomerAppointmentEntity cAppEntity = CalendarAppointmentDAL.customerAppointmentTable.CreateQuery<CustomerAppointmentEntity>().Where(c => c.PartitionKey == vm.CustomerID && c.AppointmentNumber == vm.AppointmentNumber).FirstOrDefault();
            TableOperation iCustomerAppOperation = null;
            if (cAppEntity != null)
            {
                cAppEntity.StartTime = vm.CallStartTime != DateTime.MinValue ? Utility.ConvertTime(vm.CallStartTime) : cAppEntity.StartTime; ;
                cAppEntity.EndTime = vm.CallEndTime != DateTime.MinValue ? Utility.ConvertTime(vm.CallEndTime) : cAppEntity.EndTime; ;
                cAppEntity.Date = vm.AppointmentDate.ToString("yyyy-MM-dd");
                cAppEntity.DesignNumber = designNumber != null ? designNumber : null;
                cAppEntity.UpdatedBy = userID;
                cAppEntity.UpdatedTS = DateTime.UtcNow;
                cAppEntity.ImageStatusID = vm.ImageStatusID;
                cAppEntity.DesignerNote = vm.DesignerNote;
                cAppEntity.MeetingLink = vm.MeetingLink;
                cAppEntity.AspectRatio = vm.AspectRatio;

                if (vm.CallStartTime != DateTime.MinValue)
                {
                    cAppEntity.AppointmentDateTime = vm.AppointmentDate.ToString("MM/dd/yyyy ").Replace('-', '/') + vm.CallStartTime.ToString("HH:mm:ss");
                    cAppEntity.AppointmentStatusID = (int)StatusEnum.AppointmentStatus.Initiated;
                    cAppEntity.AppointmentStatus = StatusEnum.AppointmentStatus.Initiated.ToString();
                }
                if (vm.CallEndTime != DateTime.MinValue)
                {
                    cAppEntity.AppointmentStatusID = (int)StatusEnum.AppointmentStatus.Completed;
                    cAppEntity.AppointmentStatus = StatusEnum.AppointmentStatus.Completed.ToString();
                    cAppEntity.StatusID = (int)StatusEnum.Status.AppointmentCompleted;
                }
                if (vm.CancellationReason != null && vm.IsCancel == true)
                {
                    cAppEntity.AppointmentStatusID = (int)StatusEnum.AppointmentStatus.Cancelled;
                    cAppEntity.AppointmentStatus = StatusEnum.AppointmentStatus.Cancelled.ToString();
                    cAppEntity.CancellationReason = vm.CancellationReason;
                    cAppEntity.StatusID = (int)StatusEnum.Status.AppointmentCancelled;
                }
                iCustomerAppOperation = TableOperation.InsertOrReplace(cAppEntity);
            }
            return await Task.FromResult(iCustomerAppOperation);
        }

        public static async Task<TableOperation> UpdateDesignerAppointmentEntityAsync(AppointmentViewModel vm, string userID, string userName, string designNumber)
        {
            DesignerAppointmentEntity dAppEntity = CalendarAppointmentDAL.designerAppointmentTable.CreateQuery<DesignerAppointmentEntity>().Where(c => c.PartitionKey == vm.UserID && c.AppointmentNumber == vm.AppointmentNumber).FirstOrDefault();

            TableOperation idesignerAppOperation = null;
            if (dAppEntity != null)
            {
                dAppEntity.StartTime = vm.CallStartTime != DateTime.MinValue ? Utility.ConvertTime(vm.CallStartTime) : dAppEntity.StartTime; ;
                dAppEntity.EndTime = vm.CallEndTime != DateTime.MinValue ? Utility.ConvertTime(vm.CallEndTime) : dAppEntity.EndTime; ;
                dAppEntity.Date = vm.AppointmentDate.ToString("yyyy-MM-dd");
                dAppEntity.AspectRatio = vm.AspectRatio;
                dAppEntity.DesignNumber = designNumber != null ? designNumber : null;
                dAppEntity.UpdatedBy = userID;
                dAppEntity.UpdatedTS = DateTime.UtcNow;
                dAppEntity.ImageStatusID = vm.ImageStatusID;
                dAppEntity.DesignerNote = vm.DesignerNote;
                dAppEntity.MeetingLink = vm.MeetingLink;

                if (vm.CallStartTime != DateTime.MinValue)
                {
                    dAppEntity.AppointmentDateTime = vm.AppointmentDate.ToString("MM/dd/yyyy ").Replace('-', '/') + vm.CallStartTime.ToString("HH:mm:ss");
                    dAppEntity.AppointmentStatusID = (int)StatusEnum.AppointmentStatus.Initiated;
                    dAppEntity.AppointmentStatus = StatusEnum.AppointmentStatus.Initiated.ToString();
                }
                if (vm.CallEndTime != DateTime.MinValue)
                {
                    dAppEntity.AppointmentStatusID = (int)StatusEnum.AppointmentStatus.Completed;
                    dAppEntity.AppointmentStatus = StatusEnum.AppointmentStatus.Completed.ToString();
                    dAppEntity.StatusID = (int)StatusEnum.Status.AppointmentCompleted;
                }
                if (vm.CancellationReason != null && vm.IsCancel == true)
                {
                    dAppEntity.AppointmentStatusID = (int)StatusEnum.AppointmentStatus.Cancelled;
                    dAppEntity.AppointmentStatus = StatusEnum.AppointmentStatus.Cancelled.ToString();
                    dAppEntity.StatusID = (int)StatusEnum.Status.AppointmentCancelled;
                }
                idesignerAppOperation = TableOperation.InsertOrReplace(dAppEntity);
            }
            return await Task.FromResult(idesignerAppOperation);
        }

        public static async Task<TableOperation> UpdateDesignerTimeSlotEntityAsync(AppointmentRequestPickViewModel vm, string userID, string userName, int appointmentStatus)
        {
            DesignerTimeSlotEntity dTimeSlotEntity = DesignerTimeSlotDAL.designerTimeSlotTable.CreateQuery<DesignerTimeSlotEntity>().Where(c => c.PartitionKey == userID && c.TimeSlotID == vm.TimeSlotID && c.RowKey == (vm.AppointmentDate.ToString("yyyyMMdd") + "_" + vm.TimeSlotID)).FirstOrDefault();
            TableOperation uOperation = null;
            if (dTimeSlotEntity != null)
            {
                dTimeSlotEntity.AppointmentCount = dTimeSlotEntity.AppointmentCount + 1;
                dTimeSlotEntity.AppointmentStatus = appointmentStatus;
                dTimeSlotEntity.TimeSlotStatus = (int)StatusEnum.TimeSlotStatus.Booked;

                uOperation = TableOperation.InsertOrReplace(dTimeSlotEntity);
            }
            return await Task.FromResult(uOperation);
        }

        public static async Task<TableOperation> UpdateDesignerTimeSlotStatusAsync(AppointmentViewModel vm, string userID, string userName, int appointmentStatus)
        {
            TableOperation uOperation = null;
            DesignerTimeSlotEntity dTimeSlotEntity = DesignerTimeSlotDAL.designerTimeSlotTable.CreateQuery<DesignerTimeSlotEntity>().Where(c => c.PartitionKey == userID && c.TimeSlotID == vm.TimeSlotID && c.RowKey == (vm.AppointmentDate.ToString("yyyyMMdd") + "_" + vm.TimeSlotID)).FirstOrDefault();
            if (dTimeSlotEntity != null)
            {
                dTimeSlotEntity.AppointmentStatus = appointmentStatus;
                uOperation = TableOperation.InsertOrReplace(dTimeSlotEntity);
            }
            return await Task.FromResult(uOperation);
        }

        public static TableOperation UpdateDesignerAvailability(string userID, string userName, AppointmentViewModel vm)
        {
            TableOperation updateOperation = null;
            DesignerAvailabilityEntity desAvailabilityEntity = DesignerTimeSlotDAL.designerAvailabilityTable.CreateQuery<DesignerAvailabilityEntity>().Where(c => c.RowKey == vm.UserID && c.PartitionKey == vm.AppointmentDate.ToString("yyyy-MM-dd")).FirstOrDefault();

            if (desAvailabilityEntity != null)
            {
                string _vm = vm.TimeSlotID.ToString();
                _vm = "TimeSlot" + _vm;

                if (vm.CancellationReason != null && vm.IsCancel == true)
                {
                    PropertyInfo prop = desAvailabilityEntity.GetType().GetProperty(_vm, BindingFlags.Public | BindingFlags.Instance);
                    prop.SetValue(desAvailabilityEntity, (int)StatusEnum.TimeSlotStatus.Available, null);
                }
                else
                {
                    PropertyInfo prop = desAvailabilityEntity.GetType().GetProperty(_vm, BindingFlags.Public | BindingFlags.Instance);
                    prop.SetValue(desAvailabilityEntity, (int)StatusEnum.TimeSlotStatus.AvailableAfterBooked, null);
                }
                desAvailabilityEntity.UpdatedBy = userID;
                desAvailabilityEntity.UpdatedTS = DateTime.UtcNow;

                updateOperation = TableOperation.InsertOrReplace(desAvailabilityEntity);
            }
            return updateOperation;
        }

        public static async Task<TableOperation> UpdateDesignerAvailabilityEntityAsync(AppointmentRequestPickViewModel vm, string userID, string userName)
        {
            TableOperation updateOperation = null;
            DesignerAvailabilityEntity desAvailabilityEntity = DesignerTimeSlotDAL.designerAvailabilityTable.CreateQuery<DesignerAvailabilityEntity>().Where(c => c.RowKey == userID && c.PartitionKey == vm.AppointmentDate.ToString("yyyy-MM-dd")).FirstOrDefault();

            if (desAvailabilityEntity != null)
            {
                string _vm = vm.TimeSlotID.ToString();
                _vm = "TimeSlot" + _vm;

                PropertyInfo prop = desAvailabilityEntity.GetType().GetProperty(_vm, BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(desAvailabilityEntity, (int)StatusEnum.TimeSlotStatus.Booked, null);

                desAvailabilityEntity.UpdatedBy = userID;
                desAvailabilityEntity.UpdatedTS = DateTime.UtcNow;

                updateOperation = TableOperation.InsertOrReplace(desAvailabilityEntity);
            }
            return await Task.FromResult(updateOperation);
        }

        #endregion
    }
}


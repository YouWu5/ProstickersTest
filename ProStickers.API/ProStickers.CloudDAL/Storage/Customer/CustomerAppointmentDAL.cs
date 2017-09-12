using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ProStickers.ViewModel.Core.StatusEnum;
using AutoMapper;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using ProStickers.CloudDAL.Entity.Customer;
using ProStickers.CloudDAL.Storage.ExceptionStorage;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using ProStickers.CloudDAL.Entity.Designer;
using ProStickers.CloudDAL.Storage.Designer;
using System.Reflection;

namespace ProStickers.CloudDAL.Storage.Customer
{
    public class CustomerAppointmentDAL
    {
        public static CloudTable appointmentRequestTable;

        static CustomerAppointmentDAL()
        {
            appointmentRequestTable = Utility.GetStorageTable("AppointmentRequest");
        }

        public static async Task GetListAsync<CustomerAppointmentListViewModel>(Pager<CustomerAppointmentListViewModel> pagerVM, string customerID)
        {
            string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, customerID);

            TableQuery<DynamicTableEntity> projectionQuery = new TableQuery<DynamicTableEntity>().Select(new string[] { "MeetingLink", "AppointmentNumber", "AppointmentDateTime", "RequestDateTime", "AppointmentStatus", "UpdatedTS" }).Where(filter);

            EntityResolver<ViewModel.Customer.CustomerAppointmentListViewModel> resolver = (pk, rk, ts, props, etag) => new ViewModel.Customer.CustomerAppointmentListViewModel
            {
                MeetingLink = props["MeetingLink"].StringValue,
                AppointmentNumber = props["AppointmentNumber"].StringValue,
                AppointmentDateTime = props["AppointmentDateTime"].StringValue,
                RequestDateTime = props["RequestDateTime"].StringValue,
                AppointmentStatus = props["AppointmentStatus"].StringValue,
                UpdatedTS = props["UpdatedTS"].DateTime.Value
            };

            List<ViewModel.Customer.CustomerAppointmentListViewModel> appointmentList = CalendarAppointmentDAL.customerAppointmentTable.ExecuteQuery(projectionQuery, resolver, null, null).
                                                                                       ToList();

            if (appointmentList != null && appointmentList.Count() > 0)
            {
                pagerVM.RecordsCount = appointmentList.Count();

                if (pagerVM.Sort == "AppointmentNumber")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        appointmentList = appointmentList.OrderByDescending(s => s.AppointmentNumber).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        appointmentList = appointmentList.OrderBy(s => s.AppointmentNumber).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "AppointmentDateTime")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        appointmentList = appointmentList.OrderByDescending(s => s.AppointmentDateTime).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        appointmentList = appointmentList.OrderBy(s => s.AppointmentDateTime).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "RequestDateTime")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        appointmentList = appointmentList.OrderByDescending(s => s.RequestDateTime).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        appointmentList = appointmentList.OrderBy(s => s.RequestDateTime).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }

                else if (pagerVM.Sort == "AppointmentStatus")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        appointmentList = appointmentList.OrderByDescending(s => s.AppointmentStatus).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        appointmentList = appointmentList.OrderBy(s => s.AppointmentStatus).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "UpdatedTS")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        appointmentList = appointmentList.OrderByDescending(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        appointmentList = appointmentList.OrderBy(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
            }

            Mapper.Initialize(a =>
            {
                a.CreateMap<ViewModel.Customer.CustomerAppointmentListViewModel, CustomerAppointmentEntity>().ReverseMap();
            });

            await Task.FromResult(pagerVM.Data = Mapper.Map<List<CustomerAppointmentListViewModel>>(appointmentList));
        }

        public static async Task<InternalOperationResult> GetAppointmentListAsync(string customerID)
        {
            List<CustomerAppointmentEntity> appointmentList = CalendarAppointmentDAL.customerAppointmentTable.ExecuteQuery(new TableQuery<CustomerAppointmentEntity>()).
                                                              Where(x => x.PartitionKey == customerID).ToList();
            Mapper.Initialize(a =>
            {
                a.CreateMap<CustomerAppointmentListViewModel, CustomerAppointmentEntity>().ReverseMap();
            });

            List<CustomerAppointmentListViewModel> appList = Mapper.Map<List<CustomerAppointmentEntity>, List<CustomerAppointmentListViewModel>>(appointmentList);
            return await Task.FromResult(new InternalOperationResult(Result.Success, "", appList));
        }

        public static async Task<InternalOperationResult> GetByIDAsync(string customerID, string appointmentNo)
        {
            try
            {
                CustomerAppointmentEntity appointmentEntity = CalendarAppointmentDAL.customerAppointmentTable.ExecuteQuery(new TableQuery<CustomerAppointmentEntity>()).
                                                                  Where(x => x.PartitionKey == customerID && x.RowKey == appointmentNo).FirstOrDefault();
                if (appointmentEntity != null)
                {
                    Mapper.Initialize(a =>
                    {
                        a.CreateMap<CustomerAppointmentViewModel, CustomerAppointmentEntity>().ReverseMap().
                        ForMember(x => x.CustomerID, y => y.MapFrom(z => z.PartitionKey)).
                        ForMember(x => x.AppointmentDate, y => y.MapFrom(z => z.Date));
                    });

                    CustomerAppointmentViewModel appointmentVM = Mapper.Map<CustomerAppointmentEntity, CustomerAppointmentViewModel>(appointmentEntity);

                    appointmentVM.AppointmentTime = TimeSpan.FromMinutes(appointmentEntity.StartTime).ToString();
                    appointmentVM.RequstTime = TimeSpan.FromMinutes(appointmentEntity.RequestTime).ToString();

                    if (appointmentVM.DesignNumber != null)
                    {
                        DownloadImageViewModel vm = BlobStorage.DownloadBlobByteArray(Blob.designimage.ToString(), appointmentVM.DesignNumber);
                        if (vm != null)
                        {
                            appointmentVM.ImageBuffer = vm.ImageBuffer;
                            appointmentVM.ImageExtension = vm.FileExtension;
                        }
                    }
                    if (appointmentVM.AppointmentStatusID == (int)AppointmentStatus.Completed && appointmentVM.DesignNumber != null)
                    {
                        if (appointmentEntity.ImageStatusID == 2 || appointmentEntity.ImageStatusID == 3)
                        {
                            appointmentVM.IsBuyEnable = true;
                        }

                        OrderEntity order = OrderDAL.orderTable.CreateQuery<OrderEntity>().Where(x => x.AppointmentNumber == appointmentVM.AppointmentNumber
                                            && x.DesignNumber == appointmentVM.DesignNumber).FirstOrDefault();

                        if (order != null)
                        {
                            if (order.PurchaseTypeID == (int)PurchaseType.VectorFile || order.PurchaseTypeID == (int)PurchaseType.Both)
                            {
                                appointmentVM.IsDownloadEnable = true;
                            }
                            if (order.OrderStatusID == (int)OrderStatus.Shipped)
                            {
                                appointmentVM.IsDeleteEnable = true;
                            }
                        }
                    }
                    return await Task.FromResult(new InternalOperationResult(Result.Success, "", appointmentVM));
                }
                else
                {
                    return await Task.FromResult(new InternalOperationResult(Result.UDError, "Appointment record doesn't exists.", null));
                }
            }
            catch (Exception e)
            {
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", "CustomerAppointmentDAL", "GetByIDAsync", DateTime.UtcNow, e, null, appointmentNo, customerID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> CancelAppointmentAsync(string customerID, string customerName, CustomerAppointmentViewModel appointmentVM)
        {
            try
            {
                TableOperation updateAppointmentStatus = null;
                TableOperation updateDesignerAppointmentStatus = null;
                TableOperation uOperation = null;
                CustomerAppointmentEntity appointmentEntity = CalendarAppointmentDAL.customerAppointmentTable.ExecuteQuery(new TableQuery<CustomerAppointmentEntity>()).
                                                              Where(x => x.PartitionKey == customerID && x.RowKey == appointmentVM.AppointmentNumber).FirstOrDefault();

                if (appointmentEntity != null)
                {
                    if (appointmentEntity.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff") == appointmentVM.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff"))
                    {
                        appointmentEntity.AppointmentStatusID = (int)AppointmentStatus.Cancelled;
                        appointmentEntity.StatusID = (int)AppointmentStatus.Cancelled;
                        appointmentEntity.AppointmentStatus = AppointmentStatus.Cancelled.ToString();
                        appointmentEntity.CancellationReason = appointmentVM.CancellationReason;
                        appointmentEntity.UpdatedTS = DateTime.UtcNow;
                        appointmentEntity.UpdatedBy = customerID;

                        TableOperation updateStatus = TableOperation.InsertOrReplace(appointmentEntity);

                        AppointmentEntity aEntity = CalendarAppointmentDAL.appointmentTable.ExecuteQuery(new TableQuery<AppointmentEntity>()).
                                                                  Where(x => x.RowKey == appointmentVM.AppointmentNumber && x.CustomerID == customerID).FirstOrDefault();
                        if (aEntity != null)
                        {
                            aEntity.AppointmentStatusID = (int)AppointmentStatus.Cancelled;
                            aEntity.StatusID = (int)AppointmentStatus.Cancelled;
                            aEntity.AppointmentStatus = AppointmentStatus.Cancelled.ToString();
                            aEntity.CancellationReason = appointmentVM.CancellationReason;
                            aEntity.UpdatedBy = customerID;
                            aEntity.UpdatedTS = DateTime.UtcNow;

                            updateAppointmentStatus = TableOperation.InsertOrReplace(aEntity);
                        }

                        DesignerAppointmentEntity designerEntity = CalendarAppointmentDAL.designerAppointmentTable.ExecuteQuery(new TableQuery<DesignerAppointmentEntity>()).
                                                                   Where(x => x.RowKey == appointmentVM.AppointmentNumber && x.CustomerID == customerID).FirstOrDefault();
                        if (designerEntity != null)
                        {
                            designerEntity.AppointmentStatusID = (int)AppointmentStatus.Cancelled;
                            designerEntity.StatusID = (int)AppointmentStatus.Cancelled;
                            designerEntity.AppointmentStatus = AppointmentStatus.Cancelled.ToString();
                            designerEntity.UpdatedBy = customerID;
                            designerEntity.UpdatedTS = DateTime.UtcNow;
                            updateDesignerAppointmentStatus = TableOperation.InsertOrReplace(designerEntity);
                        }

                        DesignerTimeSlotEntity dTimeSlotEntity = DesignerTimeSlotDAL.designerTimeSlotTable.CreateQuery<DesignerTimeSlotEntity>().Where(c => c.PartitionKey == appointmentEntity.UserID && c.TimeSlotID == aEntity.TimeSlotID && c.RowKey == Convert.ToDateTime(appointmentEntity.Date).ToString("yyyyMMdd") + "_" + aEntity.TimeSlotID).FirstOrDefault();
                        if (dTimeSlotEntity != null)
                        {
                            dTimeSlotEntity.IsUserAvailable = true;
                            dTimeSlotEntity.TimeSlotStatus = (int)TimeSlotStatus.Available;
                            dTimeSlotEntity.AppointmentStatus = (int)AppointmentStatus.Cancelled;
                            uOperation = TableOperation.InsertOrReplace(dTimeSlotEntity);
                        }
                        TableOperation updateOperation = null;
                        updateOperation = UpdateDesignerAvailability(appointmentEntity.UserID, appointmentEntity.UserName, appointmentVM, aEntity.TimeSlotID);

                        await CalendarAppointmentDAL.customerAppointmentTable.ExecuteAsync(updateStatus);
                        await CalendarAppointmentDAL.appointmentTable.ExecuteAsync(updateAppointmentStatus);
                        await CalendarAppointmentDAL.designerAppointmentTable.ExecuteAsync(updateDesignerAppointmentStatus);
                        if (uOperation != null)
                        {
                            await DesignerTimeSlotDAL.designerTimeSlotTable.ExecuteAsync(uOperation);
                        }
                        if (updateOperation != null)
                        {
                            await DesignerTimeSlotDAL.designerAvailabilityTable.ExecuteAsync(updateOperation);
                        }

                        TransactionLogDAL.InsertTransactionLog(appointmentEntity.PartitionKey, "CustomerAppointment", DateTime.UtcNow.Date, DateTime.UtcNow, customerID, "Cancelled", customerName);
                        return new InternalOperationResult(Result.Success, "Appointment cancelled successfully.", appointmentEntity.PartitionKey);
                    }
                    else
                    {
                        return new InternalOperationResult(Result.Concurrency, "Data is already changed by someone else. Please try again.", null);
                    }
                }
                else
                {
                    return new InternalOperationResult(Result.Success, "Appointment record doesn't exists.", null);

                }
            }
            catch (Exception e)
            {
                string requestJson = JsonConvert.SerializeObject(appointmentVM);
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", "CustomerAppointmentDAL", "CancelAppointmentAsync", DateTime.UtcNow, e, null, requestJson, customerID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> CreateCustomerCallRequestAsync(string userID, string userName, AppointmentRequestViewModel vm)
        {
            try
            {
                CustomerEntity cEntity = CustomerDAL.customerTable.CreateQuery<CustomerEntity>().Where(c => c.PartitionKey == vm.CustomerID).Select(c => c).FirstOrDefault();

                List<AppointmentRequestEntity> appRequestList = appointmentRequestTable.CreateQuery<AppointmentRequestEntity>().Select(c => c).ToList();

                string customerID = appRequestList.Where(c => c.CustomerID == vm.CustomerID && (c.RequestStatusID == 1 || c.RequestStatusID == 2)).Select(c => c.CustomerID).FirstOrDefault();
                if (cEntity != null)
                {
                    if (customerID == null)
                    {
                        AppointmentRequestEntity aRequestEntity = new AppointmentRequestEntity();

                        aRequestEntity.PartitionKey = Utility.GetCSTDateTime().Date.ToString("yyyy-MM-dd");
                        aRequestEntity.RowKey = (DateTime.UtcNow.Ticks).ToString("d19"); ;
                        aRequestEntity.CustomerID = vm.CustomerID;
                        aRequestEntity.CustomerName = cEntity.FullName;
                        aRequestEntity.PhoneNumber = cEntity.ContactNo;
                        aRequestEntity.EmailID = cEntity.EmailID;
                        aRequestEntity.RequestStatusID = (int)AppointmentRequestStatus.Created;
                        aRequestEntity.RequestedDateTime = Utility.GetCSTDateTime().ToString("MM/dd/yyyy HH:mm:ss").Replace('-', '/');
                        aRequestEntity.RequestNumber = appRequestList.Count() + 1;
                        aRequestEntity.CustomerCodeName = cEntity.CustomerCodeName;
                        aRequestEntity.StartTime = 0.00;
                        aRequestEntity.CreatedBy = userID;
                        aRequestEntity.CreatedTS = DateTime.UtcNow;

                        TableOperation insertOperation = TableOperation.InsertOrReplace(aRequestEntity);
                        await appointmentRequestTable.ExecuteAsync(insertOperation);

                        TransactionLogDAL.InsertTransactionLog(userID, "CustomerAppointment", DateTime.UtcNow.Date, DateTime.UtcNow, userID, "Added", userName);

                        return new InternalOperationResult(Result.Success, "You have been added to the queue. You are number " + aRequestEntity.RequestNumber + " in the queue. Someone will contact you soon.", userID.ToString());
                    }
                    else
                    {
                        return new InternalOperationResult(Result.Success, "You are already in the queue.", userID.ToString());
                    }
                }
                else
                {
                    return new InternalOperationResult(Result.UDError, "Customer doesn't exists or deleted.", userID.ToString());
                }
            }
            catch (Exception e)
            {
                string _requestJSON = JsonConvert.SerializeObject(vm);
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", "CustomerAppointmentDAL", "CustomerRequestCreateAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static TableOperation UpdateDesignerAvailability(string userID, string userName, CustomerAppointmentViewModel vm, int timeSlotID)
        {
            TableOperation updateOperation = null;
            DesignerAvailabilityEntity desAvailabilityEntity = DesignerTimeSlotDAL.designerAvailabilityTable.CreateQuery<DesignerAvailabilityEntity>().Where(c => c.RowKey == userID && c.PartitionKey == vm.AppointmentDate.ToString("yyyy-MM-dd")).FirstOrDefault();

            if (desAvailabilityEntity != null)
            {
                string _vm = timeSlotID.ToString();
                _vm = "TimeSlot" + _vm;

                PropertyInfo prop = desAvailabilityEntity.GetType().GetProperty(_vm, BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(desAvailabilityEntity, (int)TimeSlotStatus.Available, null);

                desAvailabilityEntity.UpdatedBy = userID;
                desAvailabilityEntity.UpdatedTS = DateTime.UtcNow;

                updateOperation = TableOperation.InsertOrReplace(desAvailabilityEntity);
            }
            return updateOperation;
        }
    }
}

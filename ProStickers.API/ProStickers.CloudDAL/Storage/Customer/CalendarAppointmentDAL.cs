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
using ProStickers.ViewModel.Designer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProStickers.CloudDAL.Storage.Customer
{
    public class CalendarAppointmentDAL
    {
        public static CloudTable appointmentTable;
        public static CloudTable customerAppointmentTable;
        public static CloudTable designerAppointmentTable;

        static CalendarAppointmentDAL()
        {
            appointmentTable = Utility.GetStorageTable("Appointment");
            customerAppointmentTable = Utility.GetStorageTable("CustomerAppointment");
            designerAppointmentTable = Utility.GetStorageTable("DesignerAppointment");
        }

        public static async Task<ListItem> GetAvailableDesignerAsync(DateTime date, int timeslotID)
        {
            if (date != null && timeslotID > 0)
            {
                ListItem designer = new ListItem();
                string tsID = "TimeSlot" + timeslotID;

                StringBuilder filter = new StringBuilder();

                filter.Append("(" + TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, date.Date.ToString("yyyy-MM-dd")) + ")");

                filter.Append(" and ((" + TableQuery.GenerateFilterConditionForInt(tsID, QueryComparisons.Equal, Convert.ToInt32(StatusEnum.TimeSlotStatus.Available)) + ")");

                filter.Append(" or (" + TableQuery.GenerateFilterConditionForInt(tsID, QueryComparisons.Equal, Convert.ToInt32(StatusEnum.TimeSlotStatus.AvailableAfterBooked)) + "))");

                DesignerAvailabilityEntity daEntity = DesignerTimeSlotDAL.designerAvailabilityTable.ExecuteQuery(new TableQuery<DesignerAvailabilityEntity>().Where(filter.ToString())).Select(c => c).FirstOrDefault();

                if (daEntity != null)
                {
                    UserEntity uEntity = UserDAL.userTable.CreateQuery<UserEntity>().Where(u => u.PartitionKey == daEntity.UserID && u.Active == true && u.UserTypeID == 2).Select(u => u).FirstOrDefault();

                    if (uEntity != null)
                    {
                        designer.Text = uEntity.FullName;
                        designer.Value = uEntity.PartitionKey;
                    }
                }
                return await Task.FromResult(designer);
            }
            else
            {
                return null;
            }
        }

        public static async Task<List<ListItem>> GetAvailableDesignerListAsync(DateTime date, int timeslotID)
        {
            if (date != null && timeslotID > 0)
            {
                List<ListItem> DesignerList = new List<ListItem>();

                string tsID = "TimeSlot" + timeslotID;

                StringBuilder filter = new StringBuilder();

                filter.Append("(" + TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, date.Date.ToString("yyyy-MM-dd")) + ")");

                filter.Append(" and ((" + TableQuery.GenerateFilterConditionForInt(tsID, QueryComparisons.Equal, Convert.ToInt32(StatusEnum.TimeSlotStatus.Available)) + ")");

                filter.Append(" or (" + TableQuery.GenerateFilterConditionForInt(tsID, QueryComparisons.Equal, Convert.ToInt32(StatusEnum.TimeSlotStatus.AvailableAfterBooked)) + "))");

                List<DesignerAvailabilityEntity> dAEntityList = DesignerTimeSlotDAL.designerAvailabilityTable.ExecuteQuery(new TableQuery<DesignerAvailabilityEntity>().Where(filter.ToString())).Select(c => c).ToList();

                if (dAEntityList != null && dAEntityList.Count() > 0)
                {
                    foreach (var item in dAEntityList)
                    {
                        List<UserEntity> uEntityList = UserDAL.userTable.CreateQuery<UserEntity>().Where(u => u.PartitionKey == item.UserID && u.Active == true && u.UserTypeID == 2).ToList();

                        if (uEntityList != null && uEntityList.Count() > 0)
                        {
                            uEntityList.ForEach(l => DesignerList.Add(new ListItem { Text = l.FullName, Value = l.PartitionKey }));
                        }
                    }
                }
                return await Task.FromResult(DesignerList);
            }
            else
            {
                return null;
            }
        }

        public static async Task<InternalOperationResult> CreateAsync(CalendarAppointmentViewModel vm, string userID, string userName)
        {
            try
            {
                if (vm != null)
                {
                    TableOperation aInsert = null, caInsert = null, daInsert = null, dtsInsert = null, davailInsert = null;

                    AppointmentEntity aEntity = new AppointmentEntity();

                    string time = TimeSpan.FromMinutes(vm.StartTime).ToString().TrimEnd('0');
                    time = time.Replace(":", string.Empty);

                    aEntity.PartitionKey = vm.Date.ToString("yyyy-MM-dd");
                    aEntity.RowKey = vm.UserID + "-" + vm.Date.ToString("yyyyMMdd") + Utility.GetCSTDateTime().ToString("HHmmss") + "-" + vm.TimeSlotID + "-" + time;
                    aEntity.TimeSlotID = vm.TimeSlotID;
                    aEntity.TimeSlot = vm.TimeSlot;
                    aEntity.AppointmentStatus = StatusEnum.AppointmentStatus.Scheduled.ToString();
                    aEntity.AppointmentStatusID = Convert.ToInt32(StatusEnum.AppointmentStatus.Scheduled);
                    aEntity.CustomerID = userID;
                    aEntity.AppointmentDateTime = vm.Date.ToString("MM/dd/yyyy ").Replace('-', '/') + Convert.ToDateTime(TimeSpan.FromMinutes(vm.StartTime).ToString()).ToString("HH:mm:ss");
                    aEntity.UserID = vm.UserID;
                    aEntity.UserName = vm.UserName;
                    aEntity.StatusID = Convert.ToInt32(StatusEnum.Status.AppointmentScheduled);
                    aEntity.StartTime = vm.StartTime;
                    aEntity.EndTime = vm.EndTime;
                    aEntity.IsCalendarRequest = true;
                    aEntity.RequestDate = vm.Date.ToString("yyyy-MM-dd");
                    aEntity.RequestTime = Utility.ConvertTime(vm.RequestDateTime);
                    aEntity.CreatedBy = userID;
                    aEntity.CreatedTS = DateTime.UtcNow;
                    aEntity.UpdatedBy = userID;
                    aEntity.UpdatedTS = DateTime.UtcNow;
                    aEntity.RequestDateTime = vm.RequestDateTime.ToString("MM/dd/yyyy HH:mm:ss").Replace('-', '/');

                    aInsert = TableOperation.InsertOrReplace(aEntity);
                    caInsert = await CreateCustomerAppointmentAsync(vm, userID, userName, aEntity.RowKey);
                    daInsert = await CreateDesignerAppointmentAsync(vm, userID, userName, aEntity.RowKey);
                    dtsInsert = await UpdateDesignerTimeSlotAsync(vm, userID, userName);
                    davailInsert = await UpdateDesignerAvailabilityEntity(vm, userID, userName);

                    if (aInsert != null)
                    {
                        await appointmentTable.ExecuteAsync(aInsert);
                    }
                    if (caInsert != null)
                    {
                        await customerAppointmentTable.ExecuteAsync(caInsert);
                    }
                    if (daInsert != null)
                    {
                        await designerAppointmentTable.ExecuteAsync(daInsert);
                    }
                    if (dtsInsert != null)
                    {
                        await DesignerTimeSlotDAL.designerTimeSlotTable.ExecuteAsync(dtsInsert);
                    }
                    if (davailInsert != null)
                    {
                        await DesignerTimeSlotDAL.designerAvailabilityTable.ExecuteAsync(davailInsert);
                    }

                    TransactionLogDAL.InsertTransactionLog(aEntity.PartitionKey, "CalendarAppointment", DateTime.UtcNow.Date, DateTime.UtcNow, aEntity.CreatedBy, "Added", userName);

                    return new InternalOperationResult(Result.Success, "Appointment scheduled successfully.", aEntity.PartitionKey);
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = JsonConvert.SerializeObject(vm);
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", "CalendarAppointmentDAL", "CreateAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        #region Helper Methods

        public static async Task<List<TimeSlotViewModel>> GetTimeSlotListAsync(DateTime date)
        {
            var list = JsonConvert.DeserializeObject<List<TimeSlotEntity>>(await MasterDAL.RetriveEntity("TimeSlot")) as List<TimeSlotEntity>;

            List<TimeSlotViewModel> timeslotList = new List<TimeSlotViewModel>();
            if (list != null && list.Count() > 0)
            {
                list.ForEach(l => timeslotList.Add(new TimeSlotViewModel { Text = l.Name, Value = l.TimeSlotID, StartTime = l.StartTime, EndTime = l.EndTime }));

                DateTime d = Utility.GetCSTDateTime();
                if (d.Date == date)
                {
                    int time = Utility.ConvertTime(d);
                    timeslotList = timeslotList.Where(c => c.EndTime >= time).ToList();
                }
                else
                {
                    timeslotList = timeslotList.ToList();
                }
            }
            return timeslotList;
        }

        public static async Task<TableOperation> CreateDesignerAppointmentAsync(CalendarAppointmentViewModel vm, string userID, string userName, string rowKey)
        {
            TableOperation daInsert = null;
            DesignerAppointmentEntity daEntity = new DesignerAppointmentEntity();

            CustomerEntity cEntity = (from c in CustomerDAL.customerTable.CreateQuery<CustomerEntity>().Where(c => c.PartitionKey == userID) select c).FirstOrDefault();

            daEntity.PartitionKey = vm.UserID;
            daEntity.RowKey = rowKey;
            daEntity.AppointmentNumber = rowKey;
            daEntity.Date = vm.Date.ToString("yyyy-MM-dd");
            daEntity.StartTime = vm.StartTime;
            daEntity.EndTime = vm.EndTime;
            daEntity.CustomerID = userID;
            daEntity.CustomerName = userName;
            daEntity.CustomerCodeName = cEntity.CustomerCodeName;
            daEntity.UserName = vm.UserName;
            daEntity.ContactNumber = cEntity.ContactNo;
            daEntity.RequestDate = vm.Date.ToString("yyyy-MM-dd");
            daEntity.RequestTime = Utility.ConvertTime(vm.RequestDateTime);
            daEntity.AppointmentStatus = StatusEnum.AppointmentStatus.Scheduled.ToString();
            daEntity.AppointmentStatusID = Convert.ToInt32(StatusEnum.AppointmentStatus.Scheduled);
            daEntity.RequestDateTime = vm.RequestDateTime.ToString("MM/dd/yyyy HH:mm:ss").Replace('-', '/');
            daEntity.AppointmentDateTime = vm.Date.ToString("MM/dd/yyyy ").Replace('-', '/') + Convert.ToDateTime(TimeSpan.FromMinutes(vm.StartTime).ToString()).ToString("HH:mm:ss");
            daEntity.StatusID = (int)StatusEnum.Status.AppointmentScheduled;
            daEntity.CreatedBy = vm.UserID;
            daEntity.CreatedTS = DateTime.UtcNow;
            daEntity.UpdatedBy = vm.UserID;
            daEntity.UpdatedTS = DateTime.UtcNow;

            daInsert = TableOperation.InsertOrReplace(daEntity);
            return await Task.FromResult(daInsert);
        }

        public static async Task<TableOperation> CreateCustomerAppointmentAsync(CalendarAppointmentViewModel vm, string userID, string userName, string rowKey)
        {
            TableOperation caInsert = null;
            CustomerAppointmentEntity caEntity = new CustomerAppointmentEntity();

            caEntity.PartitionKey = userID;
            caEntity.RowKey = rowKey;
            caEntity.AppointmentNumber = rowKey;
            caEntity.Date = vm.Date.ToString("yyyy-MM-dd");
            caEntity.StartTime = vm.StartTime;
            caEntity.EndTime = vm.EndTime;
            caEntity.UserID = vm.UserID;
            caEntity.UserName = vm.UserName;
            caEntity.RequestDate = vm.Date.ToString("yyyy-MM-dd");
            caEntity.RequestTime = Utility.ConvertTime(vm.RequestDateTime);
            caEntity.AppointmentStatus = StatusEnum.AppointmentStatus.Scheduled.ToString();
            caEntity.AppointmentStatusID = Convert.ToInt32(StatusEnum.AppointmentStatus.Scheduled);
            caEntity.RequestDateTime = vm.RequestDateTime.ToString("MM/dd/yyyy HH:mm:ss").Replace('-', '/');
            caEntity.StatusID = (int)StatusEnum.Status.AppointmentScheduled;
            caEntity.AppointmentDateTime = vm.Date.ToString("MM/dd/yyyy ").Replace('-', '/') + Convert.ToDateTime(TimeSpan.FromMinutes(vm.StartTime).ToString()).ToString("HH:mm:ss");
            caEntity.CreatedBy = userID;
            caEntity.CreatedTS = DateTime.UtcNow;
            caEntity.UpdatedBy = userID;
            caEntity.UpdatedTS = DateTime.UtcNow;

            caInsert = TableOperation.InsertOrReplace(caEntity);
            return await Task.FromResult(caInsert);
        }

        public static async Task<TableOperation> UpdateDesignerTimeSlotAsync(CalendarAppointmentViewModel vm, string userID, string userName)
        {
            TableOperation dtsInsert = null;

            DesignerTimeSlotEntity dtsEntity = DesignerTimeSlotDAL.designerTimeSlotTable.CreateQuery<DesignerTimeSlotEntity>().Where(c => c.PartitionKey == vm.UserID && c.TimeSlotID == vm.TimeSlotID && c.Date == vm.Date.ToString("yyyy-MM-dd")).FirstOrDefault();

            if (dtsEntity != null)
            {
                dtsEntity.TimeSlotStatus = (int)StatusEnum.TimeSlotStatus.Booked;
                dtsEntity.IsUserAvailable = false;
                dtsEntity.Date = vm.Date.ToString("yyyy-MM-dd");
                dtsEntity.UpdatedBy = userID;
                dtsEntity.UpdatedTS = DateTime.UtcNow;

                dtsInsert = TableOperation.InsertOrReplace(dtsEntity);
            }
            return await Task.FromResult(dtsInsert);
        }

        public static async Task<TableOperation> UpdateDesignerAvailabilityEntity(CalendarAppointmentViewModel vm, string userID, string userName)
        {
            TableOperation davailInsert = null;

            DesignerAvailabilityEntity dAvailEntity = DesignerTimeSlotDAL.designerAvailabilityTable.CreateQuery<DesignerAvailabilityEntity>().Where(c => c.RowKey == vm.UserID && c.PartitionKey == vm.Date.ToString("yyyy-MM-dd")).FirstOrDefault();

            if (dAvailEntity != null)
            {
                string _vm = vm.TimeSlotID.ToString();
                _vm = "TimeSlot" + _vm;

                PropertyInfo prop = dAvailEntity.GetType().GetProperty(_vm, BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(dAvailEntity, (int)StatusEnum.TimeSlotStatus.Booked, null);

                dAvailEntity.UpdatedBy = userID;
                dAvailEntity.UpdatedTS = DateTime.UtcNow;

                davailInsert = TableOperation.InsertOrReplace(dAvailEntity);
            }
            return await Task.FromResult(davailInsert);
        }

        #endregion
    }
}



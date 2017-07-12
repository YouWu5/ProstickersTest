using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using ProStickers.ViewModel.Designer;
using ProStickers.CloudDAL.Storage.Master;
using ProStickers.ViewModel.Core;
using ProStickers.CloudDAL.Entity.Designer;
using ProStickers.CloudDAL.Storage.ExceptionStorage;

namespace ProStickers.CloudDAL.Storage.Designer
{
    public class DesignerTimeSlotDAL
    {
        public static CloudTable designerTimeSlotTable;
        public static CloudTable designerAvailabilityTable;

        static DesignerTimeSlotDAL()
        {
            designerTimeSlotTable = Utility.GetStorageTable("DesignerTimeSlot");
            designerAvailabilityTable = Utility.GetStorageTable("DesignerAvailability");
        }

        public static async Task<DesignerTimeSlotViewModel> GetListAsync(string userID, string userName, DateTime date, bool IsAllTimeSlots)
        {
            DesignerTimeSlotViewModel vm = new DesignerTimeSlotViewModel();

            if (date != null)
            {
                vm.TimeSlotList = new List<TimeSlotListViewModel>();

                StringBuilder sb = new StringBuilder();
                sb.Append("((" + TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userID) + ")");
                if (IsAllTimeSlots == true)
                {
                    sb.Append(" and (" + TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, date.ToString("yyyyMMdd") + "_1") + ")");
                    sb.Append(" and (" + TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, date.ToString("yyyyMMdd") + "_96") + "))");
                }
                else
                {
                    sb.Append(" and (" + TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, date.ToString("yyyyMMdd") + "_41") + ")");
                    sb.Append(" and (" + TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, date.ToString("yyyyMMdd") + "_80") + "))");
                }

                TableQuery<DynamicTableEntity> projectionQuery = new TableQuery<DynamicTableEntity>().Select(new string[] { "TimeSlotID", "Name", "UserID", "TimeSlotStatus", "UserName", "UpdatedTS", "StartTime", "EndTime" }).Where(sb.ToString());
                EntityResolver<TimeSlotListViewModel> resolver = (pk, rk, ts, props, etag) => new TimeSlotListViewModel
                {
                    TimeSlotID = props["TimeSlotID"].Int32Value.Value,
                    Name = props["Name"].StringValue,
                    TimeSlotStatus = props["TimeSlotStatus"].Int32Value.Value,
                    UserID = props["UserID"].StringValue,
                    UserName = props["UserName"].StringValue,
                    UpdatedTS = props["UpdatedTS"].DateTime.Value,
                    StartTime = props["StartTime"].Int32Value.Value,
                    EndTime = props["EndTime"].Int32Value.Value,
                };

                List<TimeSlotListViewModel> desTimeSlotList = designerTimeSlotTable.ExecuteQuery(projectionQuery, resolver, null, null).ToList();

                List<TimeSlotListViewModel> tsEntityList = await GetTimeSlotListAsync();
                if (tsEntityList != null && tsEntityList.Count > 0)
                {
                    if (IsAllTimeSlots == true)
                    {
                        tsEntityList = tsEntityList.ToList();
                    }
                    else
                    {
                        tsEntityList = tsEntityList.Where(x => x.TimeSlotID >= 41 && x.TimeSlotID <= 80).Select(x => x).ToList();
                    }

                    if (desTimeSlotList != null && desTimeSlotList.Count() > 0)
                    {
                        vm.TimeSlotList = (from d in tsEntityList
                                           from e in desTimeSlotList.Where(mapping => mapping.TimeSlotID == d.TimeSlotID).DefaultIfEmpty()
                                           select new TimeSlotListViewModel
                                           {
                                               TimeSlotID = d.TimeSlotID,
                                               Name = d.Name,
                                               UserName = userName,
                                               UserID = userID,
                                               IsUserAvailable = true,
                                               TimeSlotStatus = e == null ? 0 : e.TimeSlotStatus,
                                               StartTime = d.StartTime,
                                               EndTime = d.EndTime
                                           }).ToList();
                    }
                    else
                    {
                        vm.TimeSlotList = tsEntityList.ToList();
                    }
                }
                vm.Date = date;
                vm.IsAllTimeSlots = IsAllTimeSlots;
            }
            return vm;
        }

        public static async Task<InternalOperationResult> CreateAsync(string userID, string userName, DesignerTimeSlotViewModel vm)
        {
            try
            {
                if (vm != null)
                {
                    DesignerAvailabilityEntity desAvailabilityEntity = new DesignerAvailabilityEntity();

                    bool isUpdate = false; bool isCreate = false;
                    TableOperation insOperation = FillDesignerAvailabilityEntity(desAvailabilityEntity, vm, userID, userName);

                    foreach (var item in vm.TimeSlotList)
                    {
                        DesignerTimeSlotEntity designerTimeSlotEntity = designerTimeSlotTable.CreateQuery<DesignerTimeSlotEntity>().Where(c => c.PartitionKey == userID && c.RowKey == (vm.Date.ToString("yyyyMMdd") + "_" + item.TimeSlotID).ToString()).Select(c => c).FirstOrDefault();

                        if (designerTimeSlotEntity != null)
                        {
                            designerTimeSlotEntity.TimeSlotID = item.TimeSlotID;
                            designerTimeSlotEntity.TimeSlotStatus = item.TimeSlotStatus;
                            designerTimeSlotEntity.UpdatedTS = DateTime.UtcNow;
                            isUpdate = true;

                            TableOperation updateOperation = TableOperation.InsertOrReplace(designerTimeSlotEntity);
                            await designerTimeSlotTable.ExecuteAsync(updateOperation);
                        }
                        else
                        {
                            if (item.TimeSlotStatus > 0)
                            {
                                DesignerTimeSlotEntity dTimeSlotEntity = new DesignerTimeSlotEntity();

                                dTimeSlotEntity.PartitionKey = userID;
                                dTimeSlotEntity.RowKey = vm.Date.ToString("yyyyMMdd") + "_" + item.TimeSlotID.ToString();
                                dTimeSlotEntity.TimeSlotID = item.TimeSlotID;
                                dTimeSlotEntity.Name = item.Name;
                                dTimeSlotEntity.UserID = userID;
                                dTimeSlotEntity.UserName = userName;
                                dTimeSlotEntity.TimeSlotStatus = item.TimeSlotStatus;
                                dTimeSlotEntity.IsUserAvailable = true;
                                dTimeSlotEntity.Date = vm.Date.ToString("yyyy-MM-dd");
                                dTimeSlotEntity.CreatedBy = userID;
                                dTimeSlotEntity.CreatedTS = DateTime.UtcNow;
                                dTimeSlotEntity.UpdatedBy = userID;
                                dTimeSlotEntity.UpdatedTS = DateTime.UtcNow;
                                dTimeSlotEntity.StartTime = item.StartTime;
                                dTimeSlotEntity.EndTime = item.EndTime;
                                isCreate = true;

                                TableOperation insertOperation = TableOperation.InsertOrReplace(dTimeSlotEntity);
                                await designerTimeSlotTable.ExecuteAsync(insertOperation);
                            }
                        }
                    }
                    if (insOperation != null)
                    {
                        await designerAvailabilityTable.ExecuteAsync(insOperation);
                    }

                    TransactionLogDAL.InsertTransactionLog(userID, "Designer", DateTime.UtcNow.Date, DateTime.UtcNow, userID, "Added", userName);

                    if (isUpdate == true)
                    {
                        return new InternalOperationResult(Result.Success, "Availability time updated successfully.", userID.ToString());
                    }
                    else if (isCreate == true)
                    {
                        return new InternalOperationResult(Result.Success, "Availability time added successfully.", userID.ToString());
                    }
                    else
                    {
                        return new InternalOperationResult(Result.Success, "There is nothing to save.", userID.ToString());
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
                ExceptionTableStorage.InsertOrReplaceEntity("Designer", "DesignerTimeSlotDAL", "CreateAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static TableOperation FillDesignerAvailabilityEntity(DesignerAvailabilityEntity dAvailabilityEntity, DesignerTimeSlotViewModel vm, string userID, string userName)
        {
            DesignerAvailabilityEntity desAvailabilityEntity = new DesignerAvailabilityEntity(); TableOperation updateOperation = null;

            DesignerAvailabilityEntity _desAvailabilityEntity = DesignerTimeSlotDAL.designerAvailabilityTable.CreateQuery<DesignerAvailabilityEntity>().Where(c => c.RowKey == userID && c.PartitionKey == vm.Date.ToString("yyyy-MM-dd")).FirstOrDefault();

            bool isCreate = false; TableOperation insertOperation = null;
            if (_desAvailabilityEntity != null)
            {
                foreach (var item in vm.TimeSlotList)
                {
                    string _vm = item.TimeSlotID.ToString();

                    _vm = "TimeSlot" + _vm;
                    PropertyInfo prop = _desAvailabilityEntity.GetType().GetProperty(_vm, BindingFlags.Public | BindingFlags.Instance);
                    prop.SetValue(_desAvailabilityEntity, item.TimeSlotStatus, null);
                }
                _desAvailabilityEntity.UpdatedBy = userID;
                _desAvailabilityEntity.UpdatedTS = DateTime.UtcNow;

                updateOperation = TableOperation.InsertOrReplace(_desAvailabilityEntity);
                return updateOperation;
            }
            else
            {
                foreach (var item in vm.TimeSlotList.Where(x => x.TimeSlotStatus == 1))
                {
                    string _vm = item.TimeSlotID.ToString();

                    _vm = "TimeSlot" + _vm;

                    PropertyInfo prop = desAvailabilityEntity.GetType().GetProperty(_vm, BindingFlags.Public | BindingFlags.Instance);
                    prop.SetValue(dAvailabilityEntity, item.TimeSlotStatus, null);
                    isCreate = true;
                }
                if (isCreate == true)
                {
                    dAvailabilityEntity.PartitionKey = vm.Date.ToString("yyyy-MM-dd");
                    dAvailabilityEntity.RowKey = userID;
                    dAvailabilityEntity.UserID = userID;
                    dAvailabilityEntity.UserName = userName;
                    dAvailabilityEntity.CreatedBy = userID;
                    dAvailabilityEntity.CreatedTS = DateTime.UtcNow;
                    dAvailabilityEntity.UpdatedBy = userID;
                    dAvailabilityEntity.UpdatedTS = DateTime.UtcNow;

                    insertOperation = TableOperation.InsertOrReplace(dAvailabilityEntity);
                }
                return insertOperation;
            }
        }

        #region Helper Methods

        public static async Task<List<TimeSlotListViewModel>> GetTimeSlotListAsync()
        {
            return JsonConvert.DeserializeObject<List<TimeSlotListViewModel>>(await MasterDAL.RetriveEntity("TimeSlot")) as List<TimeSlotListViewModel>;
        }

        #endregion
    }
}


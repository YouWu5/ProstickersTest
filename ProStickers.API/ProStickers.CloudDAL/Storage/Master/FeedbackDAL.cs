using AutoMapper;
using Microsoft.WindowsAzure.Storage.Table;
using ProStickers.CloudDAL.Entity.Customer;
using ProStickers.CloudDAL.Entity.Master;
using ProStickers.CloudDAL.Storage.Customer;
using ProStickers.CloudDAL.Storage.ExceptionStorage;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProStickers.CloudDAL.Storage.Master
{
    public class FeedbackDAL
    {
        public static CloudTable feedbackTable;

        static FeedbackDAL()
        {
            feedbackTable = Utility.GetStorageTable("Feedback");
        }

        public static async Task<InternalOperationResult> CreateCustomeFeedbackAsync(string userName, string userID, FeedbackViewModel feedbackVM)
        {
            try
            {
                bool isFeedbackDone = OrderDAL.orderTable.ExecuteQuery(new TableQuery<OrderEntity>()).Where(x => x.OrderNumber == feedbackVM.OrderNumber).Select(x => x.IsfeedbackDone).FirstOrDefault();
                if (!isFeedbackDone)
                {
                    Mapper.Initialize(a =>
                    {
                        a.CreateMap<FeedbackViewModel, FeedbackEntity>().ReverseMap().
                        ForMember(x => x.UserID, yu => yu.MapFrom(z => z.PartitionKey));
                    });

                    string customerCodeName = CustomerDAL.customerTable.ExecuteQuery(new TableQuery<CustomerEntity>()).
                                              Where(x => x.PartitionKey == userID).Select(x => x.CustomerCodeName).FirstOrDefault();

                    FeedbackEntity feedbackEntity = Mapper.Map<FeedbackEntity>(feedbackVM);
                    feedbackEntity.PartitionKey = feedbackVM.UserID;
                    feedbackEntity.CustomerID = userID;
                    feedbackEntity.CreatedBy = userID;
                    feedbackEntity.CreatedTS = DateTime.UtcNow;
                    feedbackEntity.UpdatedBy = userID;
                    feedbackEntity.UpdatedTS = DateTime.UtcNow;
                    feedbackEntity.CustomerCodeName = customerCodeName;

                    TableOperation insert = TableOperation.Insert(feedbackEntity);
                    await feedbackTable.ExecuteAsync(insert);

                    OrderEntity orderEntity = OrderDAL.orderTable.ExecuteQuery(new TableQuery<OrderEntity>()).Where(x => x.OrderNumber == feedbackVM.OrderNumber).FirstOrDefault();
                    orderEntity.IsfeedbackDone = true;
                    TableOperation updateOrderDetails = TableOperation.InsertOrReplace(orderEntity);
                    await OrderDAL.orderTable.ExecuteAsync(updateOrderDetails);

                    TransactionLogDAL.InsertTransactionLog(feedbackEntity.PartitionKey, "FeedbackDAL", DateTime.UtcNow.Date, DateTime.UtcNow, userID, "Added", userName);
                    return new InternalOperationResult(Result.Success, "Feedback saved successfully.", feedbackEntity.PartitionKey);
                }
                else
                {
                    return new InternalOperationResult(Result.UDError, "Feedback record already exist.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = Newtonsoft.Json.JsonConvert.SerializeObject(feedbackVM);
                ExceptionTableStorage.InsertOrReplaceEntity("Master", "FeedbackDAL", "CreateCustomeFeedback", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task GetListAsync<FeedbackListViewModel>(Pager<FeedbackListViewModel> pagerVM)
        {
            string filter = "";

            if (pagerVM.SearchList != null && pagerVM.SearchList.Count > 0)
            {
                foreach (var item in pagerVM.SearchList.Where(c => c.Value != null && c.Value != ""))
                {
                    if (item.Name == "DateFrom")
                    {
                        filter = TableQuery.GenerateFilterCondition("FeedbackDate", QueryComparisons.GreaterThanOrEqual, item.Value);
                    }
                    else if (item.Name == "DateTo")
                    {
                        filter = TableQuery.CombineFilters(filter, TableOperators.And, TableQuery.GenerateFilterCondition("FeedbackDate", QueryComparisons.LessThanOrEqual, item.Value));
                    }
                    else if (item.Name == "CustomerID")
                    {
                        filter = TableQuery.CombineFilters(filter, TableOperators.And, TableQuery.GenerateFilterCondition("CustomerID", QueryComparisons.Equal, item.Value));
                    }
                }
            }

            TableQuery<DynamicTableEntity> projectionQuery = new TableQuery<DynamicTableEntity>().Select(new string[] { "DesignNo", "CustomerID", "CustomerCodeName", "FeedbackDate", "UpdatedTS" }).Where(filter);

            EntityResolver<ViewModel.Master.FeedbackListViewModel> resolver = (pk, rk, ts, props, etag) => new ViewModel.Master.FeedbackListViewModel
            {
                DesignNo = props["DesignNo"].StringValue,
                CustomerID = props["CustomerID"].StringValue,
                CustomerName = props["CustomerCodeName"].StringValue,
                FeedbackDate = props["FeedbackDate"].StringValue,
                UpdatedTS = props["UpdatedTS"].DateTime.Value
            };

            List<ViewModel.Master.FeedbackListViewModel> feedbackList = feedbackTable.ExecuteQuery(projectionQuery, resolver, null, null).ToList();

            if (feedbackList != null && feedbackList.Count() > 0)
            {
                pagerVM.RecordsCount = feedbackList.Count();

                if (pagerVM.Sort == "DesignNo")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        feedbackList = feedbackList.OrderByDescending(s => s.DesignNo).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        feedbackList = feedbackList.OrderBy(s => s.DesignNo).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "CustomerName")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        feedbackList = feedbackList.OrderByDescending(s => s.CustomerName).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        feedbackList = feedbackList.OrderBy(s => s.CustomerName).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "FeedbackDate")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        feedbackList = feedbackList.OrderByDescending(s => s.FeedbackDate).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        feedbackList = feedbackList.OrderBy(s => s.FeedbackDate).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }

                else if (pagerVM.Sort == "UpdatedTS")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        feedbackList = feedbackList.OrderByDescending(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        feedbackList = feedbackList.OrderBy(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
            }

            Mapper.Initialize(a =>
            {
                a.CreateMap<ViewModel.Master.FeedbackListViewModel, FeedbackListViewModel>().ReverseMap();
            });
            feedbackList.Select(x => { x.FeedbackDate = Convert.ToDateTime(x.FeedbackDate).ToString("MM/dd/yyyy").Replace('-', '/'); return x; }).ToList();
            await Task.FromResult(pagerVM.Data = Mapper.Map<List<FeedbackListViewModel>>(feedbackList));
        }

        public static async Task<InternalOperationResult> GetByIDAsync(string userName, string customerID, string DesignNo)
        {
            FeedbackEntity feedbackEntity = feedbackTable.ExecuteQuery(new TableQuery<FeedbackEntity>()).
                                            Where(x => x.CustomerID == customerID && x.DesignNo == DesignNo).FirstOrDefault();
            if (feedbackEntity != null)
            {
                Mapper.Initialize(a =>
                {
                    a.CreateMap<FeedbackEntity, FeedbackViewModel>().
                    ForMember(x => x.UserID, y => y.MapFrom(z => z.PartitionKey));
                });

                FeedbackViewModel feedbackVM = Mapper.Map<FeedbackViewModel>(feedbackEntity);
                return await Task.FromResult(new InternalOperationResult(Result.Success, "", feedbackVM));
            }
            else
            {
                return await Task.FromResult(new InternalOperationResult(Result.Success, "Feedback record not found.", null));
            }
        }

        public static async Task<InternalOperationResult> CreateMasterReplyAsync(string userName, string userID, FeedbackViewModel feedbackVM)
        {
            try
            {
                FeedbackEntity fEntity = feedbackTable.ExecuteQuery(new TableQuery<FeedbackEntity>()).
                                         Where(x => x.DesignNo == feedbackVM.DesignNo && x.CustomerID == feedbackVM.CustomerID).FirstOrDefault();
                if (fEntity != null)
                {
                    fEntity.MasterReply = feedbackVM.MasterReply;
                    fEntity.IsDisplayInProfile = feedbackVM.IsDisplayInProfile;
                    fEntity.UpdatedBy = userID;
                    fEntity.UpdatedTS = DateTime.UtcNow;

                    TableOperation update = TableOperation.InsertOrReplace(fEntity);
                    await feedbackTable.ExecuteAsync(update);

                    TransactionLogDAL.InsertTransactionLog(fEntity.PartitionKey, "FeedbackDAL", DateTime.UtcNow.Date, DateTime.UtcNow, userID, "Added", userName);
                    return new InternalOperationResult(Result.Success, "Feedback saved successfully.", fEntity.PartitionKey);
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Feedback record not found.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = Newtonsoft.Json.JsonConvert.SerializeObject(feedbackVM);
                ExceptionTableStorage.InsertOrReplaceEntity("Master", "FeedbackDAL", "CreateMasterReplyAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> DeleteAsync(string userName, string userID, string customerID, string designNo)
        {
            try
            {
                FeedbackEntity fEntity = feedbackTable.ExecuteQuery(new TableQuery<FeedbackEntity>()).
                                         Where(x => x.CustomerID == customerID && x.DesignNo == designNo).FirstOrDefault();
                if (fEntity != null)
                {
                    TableOperation delete = TableOperation.Delete(fEntity);
                    await feedbackTable.ExecuteAsync(delete);

                    TransactionLogDAL.InsertTransactionLog(fEntity.PartitionKey, "FeedbackDAL", DateTime.UtcNow.Date, DateTime.UtcNow, userID, "Delete", userName);
                    return new InternalOperationResult(Result.Success, "Feedback deleted successfully.", fEntity.PartitionKey);
                }
                else
                {
                    return new InternalOperationResult(Result.UDError, "Feedback record not found.", fEntity.PartitionKey);
                }
            }
            catch (Exception e)
            {
                ExceptionTableStorage.InsertOrReplaceEntity("Master", "FeedbackDAL", "DeleteAsync", DateTime.UtcNow, e, null, designNo, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> GetOrderByIDAsync(string customerID, int orderNo)
        {
            OrderEntity orderEntity = OrderDAL.orderTable.ExecuteQuery(new TableQuery<OrderEntity>()).
                                      Where(x => x.CustomerID == customerID && x.RowKey == orderNo.ToString().PadLeft(8, '0')).FirstOrDefault();
            if (orderEntity != null)
            {
                if (orderEntity.IsfeedbackDone == false)
                {
                    Mapper.Initialize(a =>
                    {
                        a.CreateMap<FeedbackViewModel, OrderEntity>().ReverseMap().
                        ForMember(x => x.OrderDate, y => y.MapFrom(z => z.PartitionKey)).
                        ForMember(x => x.DesignNo, y => y.MapFrom(z => z.DesignNumber));
                    });

                    FeedbackViewModel orderVM = Mapper.Map<OrderEntity, FeedbackViewModel>(orderEntity);
                    if (orderVM.DesignNo != null)
                    {
                        DownloadImageViewModel vm = BlobStorage.DownloadBlobByteArray(StatusEnum.Blob.designimage.ToString(), orderVM.DesignNo);
                        if (vm != null)
                        {
                            orderVM.ImageBuffer = vm.ImageBuffer;
                            orderVM.ImageExtension = vm.FileExtension;
                        }
                    }
                    return await Task.FromResult(new InternalOperationResult(Result.Success, "", orderVM));
                }
                else
                {
                    return await Task.FromResult(new InternalOperationResult(Result.Success, "Feedback already submitted for Order No: " + orderNo, null));
                }
            }
            else
            {
                return await Task.FromResult(new InternalOperationResult(Result.UDError, "Order detail does not exists.", null));
            }
        }

        public static async Task<List<ListItem>> GetCustomerListAsync(string codeName)
        {
            List<ListItem> customerlist = feedbackTable.CreateQuery<FeedbackEntity>().Select(c => new ListItem
            {
                Text = c.CustomerCodeName,
                Value = c.CustomerID
            }).ToList();

            customerlist = customerlist.Where(c => c.Text.ToLower().Contains(codeName.ToLower())).GroupBy(c => c.Text).Select(c => c.First()).Distinct().OrderBy(c => c.Text).ToList();
            return await Task.FromResult(customerlist);
        }
    }
}

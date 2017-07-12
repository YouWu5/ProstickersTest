using ProStickers.BL.Core;
using ProStickers.CloudDAL;
using ProStickers.CloudDAL.Storage.Master;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Master;
using ProStickers.ViewModel.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProStickers.BL.Master
{
    public class FeedbackBL
    {
        public static async Task<Pager<FeedbackListViewModel>> GetListAsync<FeedbackListViewModel>()
        {
            Pager<FeedbackListViewModel> pager = PagerOperations.InitializePager<FeedbackListViewModel>
                                                        ("UpdatedTS", SortDirection.DESC.ToString(),
                                                          true, true, true, 30, true);

            pager.SearchList = new List<SearchItem>
            {
               new SearchItem { DisplayName = "Date From" ,Name = "DateFrom",Value = Utility.GetCSTDateTime().ToString("yyyy-MM-dd") },
                new SearchItem { DisplayName = "Date To" ,Name = "DateTo",Value =Utility.GetCSTDateTime().ToString("yyyy-MM-dd") },
                new SearchItem { DisplayName = "Customer" ,Name = "CustomerID",Value ="" }
            };

            pager.ColumnList = new List<ListItem>()
            {
                new ListItem { Text = "Design number", Value = "DesignNo" },
                new ListItem { Text = "Customer name", Value = "CustomerName" },
                new ListItem { Text = "Feedback date", Value = "FeedbackDate" }
            };
            await FeedbackDAL.GetListAsync(pager);
            return pager;
        }

        public static async Task<OperationResult> GetListAsync<FeedbackListViewModel>(Pager<FeedbackListViewModel> pager)
        {
            if (pager != null)
            {
                PagerOperations.UpdatePager(pager);
                await FeedbackDAL.GetListAsync(pager);
                return new OperationResult(Result.Success, "", pager);
            }
            else
            {
                return new OperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<OperationResult> GetByIDAsync(string userName, string customerID, string designNo)
        {
            if (userName != null && userName != "" && customerID != null && customerID != "" && designNo != null && designNo != "")
            {
                InternalOperationResult result = await FeedbackDAL.GetByIDAsync(userName, customerID, designNo);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> CreateCustomerFeedbackAsync(UserInfo userInfo, FeedbackViewModel feedbackVM)
        {
            if (userInfo != null && feedbackVM != null)
            {
                InternalOperationResult result = await FeedbackDAL.CreateCustomeFeedbackAsync(userInfo.UserName, userInfo.UserID, feedbackVM);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> CreateMasterReplyAsync(UserInfo userInfo, FeedbackViewModel feedbackVM)
        {
            if (userInfo != null && feedbackVM != null)
            {
                InternalOperationResult result = await FeedbackDAL.CreateMasterReplyAsync(userInfo.UserName, userInfo.UserID, feedbackVM);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> DeleteAsync(UserInfo userInfo, string customerID, string designNo)
        {
            if (userInfo != null && customerID != null && customerID != "" && designNo != null && designNo != "")
            {
                InternalOperationResult result = await FeedbackDAL.DeleteAsync(userInfo.UserName, userInfo.UserID, customerID, designNo);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> GetOrderByIDAsync(UserInfo currentUserInfo, int orderNo)
        {
            if (currentUserInfo != null && orderNo > 0)
            {
                InternalOperationResult result = await FeedbackDAL.GetOrderByIDAsync(currentUserInfo.UserID, orderNo);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<Pager<FeedbackListViewModel>> GetFeedbackListAsync<FeedbackListViewModel>()
        {
            Pager<FeedbackListViewModel> pager = PagerOperations.InitializePager<FeedbackListViewModel>
                                                        ("UpdatedTS", SortDirection.DESC.ToString(),
                                                          true, true, true, 5, true);

            pager.ColumnList = new List<ListItem>()
            {
                new ListItem { Text = "Design Number", Value = "DesignNo" },
                new ListItem { Text = "Customer name", Value = "CustomerName" },
                new ListItem { Text = "Feedback date", Value = "FeedbackDate" }
            };
            await FeedbackDAL.GetListAsync(pager);
            return pager;
        }

        public static async Task<List<ListItem>> GetCustomerListAsync(string codeName)
        {
            return await FeedbackDAL.GetCustomerListAsync(codeName);
        }
    }
}


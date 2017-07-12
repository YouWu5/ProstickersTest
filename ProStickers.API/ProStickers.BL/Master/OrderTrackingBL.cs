using System.Collections.Generic;
using System.Threading.Tasks;
using ProStickers.BL.Core;
using ProStickers.CloudDAL;
using ProStickers.CloudDAL.Storage.Master;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using ProStickers.ViewModel.Security;
using ProStickers.BL.Customer;
using ProStickers.CloudDAL.Storage;

namespace ProStickers.BL.Master
{
    public class OrderTrackingBL
    {
        public static async Task<Pager<OrderTrackingViewModel>> GetListAsync<OrderTrackingViewModel>()
        {
            Pager<OrderTrackingViewModel> pager = PagerOperations.InitializePager<OrderTrackingViewModel>
                                                        ("UpdatedTS", SortDirection.DESC.ToString(),
                                                          true, true, true, 30, true);
            pager.SearchList = new List<SearchItem>
            {
                new SearchItem { DisplayName = "Date From" ,Name = "DateFrom",Value =Utility.GetCSTDateTime().ToString("yyyy-MM-dd") },
                new SearchItem { DisplayName = "Date To" ,Name = "DateTo",Value =Utility.GetCSTDateTime().ToString("yyyy-MM-dd") },
                new SearchItem { DisplayName = "Status" ,Name = "Status",Value ="1" },
                new SearchItem { DisplayName = "Customer Name" ,Name = "CustomerCodeName",Value ="" }
            };
            pager.ColumnList = new List<ListItem>()
            {
                new ListItem { Text = "Order number", Value = "OrderNumber" },
                new ListItem { Text = "Order date", Value = "OrderDate" },
                new ListItem { Text = "Customer", Value = "CustomerName" } ,
                new ListItem { Text = "Design number", Value = "DesignNumber" },
                new ListItem { Text = "Status", Value = "OrderStatus" },
                new ListItem { Text = "Amount", Value = "Amount" }
            };
            await OrderTrackingDAL.GetListAsync(pager);
            return pager;
        }

        public static async Task<OperationResult> GetListAsync<OrderViewModel>(Pager<OrderViewModel> pager)
        {
            if (pager != null)
            {
                PagerOperations.UpdatePager(pager);
                await OrderTrackingDAL.GetListAsync(pager);
                return new OperationResult(Result.Success, "", pager);
            }
            else
            {
                return new OperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<OperationResult> GetByIDAsync(int orderNo)
        {
            if (orderNo > 0)
            {
                InternalOperationResult result = await OrderTrackingDAL.GetByIDAsync(orderNo);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> UpdateTrackingNumberAsync(UserInfo userInfo, OrderTrackingViewModel orderVM)
        {
            if (userInfo != null && orderVM != null)
            {
                InternalOperationResult result = await OrderTrackingDAL.UpdateTrackingNumberAsync(userInfo.UserName, userInfo.UserID, orderVM);
                await SendEmail.OrderDeliveryConformationEmailAsync(OrderBL.senderAddress, OrderBL.displayName, orderVM);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<List<ListItemTypes>> GetStatusListAsync()
        {
            return await OrderTrackingDAL.GetStatusListAsync();
        }

        public static async Task<List<ListItem>> GetCustomerNameListAsync(string codeName)
        {
            return await OrderTrackingDAL.GetCustomerNameListAsync(codeName);
        }

        public static async Task<OperationResult> DownloadVectorFileAsync(string userID, string designNumber)
        {
            DownloadImageViewModel imageVM = await CommonDAL.DownloadVectorFileAsync(userID, designNumber);
            if (imageVM != null)
            {
                return new OperationResult(Result.Success, "", imageVM);
            }
            else
            {
                return new OperationResult(Result.UDError, "Design file doesn't exists.", imageVM);
            }
        }

        #region Designer Order Tracking

        public static async Task<Pager<OrderTrackingViewModel>> GetListAsync<OrderTrackingViewModel>(string userID)
        {
            Pager<OrderTrackingViewModel> pager = PagerOperations.InitializePager<OrderTrackingViewModel>
                                                        ("UpdatedTS", SortDirection.DESC.ToString(),
                                                          true, true, true, 30, true);
            pager.SearchList = new List<SearchItem>
            {
                new SearchItem {DisplayName = "Date From" , Name = "DateFrom" , Value = Utility.GetCSTDateTime().ToString("yyyy-MM-dd") },
                new SearchItem {DisplayName = "Date To" , Name = "DateTo" , Value = Utility.GetCSTDateTime().ToString("yyyy-MM-dd") },
                new SearchItem { DisplayName = "Status" ,Name = "Status",Value ="1" },
                new SearchItem { DisplayName = "Display all designer order" ,Name = "IsDisplayAllDesignerOrder",Value ="false" },
                new SearchItem {DisplayName = "Customer Name" , Name = "CustomerCodeName" , Value = "" },
            };
            pager.ColumnList = new List<ListItem>()
            {
                new ListItem { Text = "Order number", Value = "OrderNumber" },
                new ListItem { Text = "Order date", Value = "OrderDate" },
                new ListItem { Text = "Customer", Value = "CustomerName" } ,
                new ListItem { Text = "Design number", Value = "DesignNumber" },
                new ListItem { Text = "Status", Value = "OrderStatus" },
                new ListItem { Text = "Amount", Value = "Amount" }
            };
            await OrderTrackingDAL.GetListAsync(pager, userID);
            return pager;
        }

        public static async Task<OperationResult> GetListAsync<OrderViewModel>(Pager<OrderViewModel> pager, string userID)
        {
            if (pager != null)
            {
                PagerOperations.UpdatePager(pager);
                await OrderTrackingDAL.GetListAsync(pager, userID);
                return new OperationResult(Result.Success, "", pager);
            }
            else
            {
                return new OperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        #endregion
    }
}

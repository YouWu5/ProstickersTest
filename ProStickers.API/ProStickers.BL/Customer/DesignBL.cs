using ProStickers.BL.Core;
using ProStickers.CloudDAL.Storage.Customer;
using ProStickers.ViewModel.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProStickers.BL.Customer
{
    public class DesignBL
    {
        public static async Task<Pager<DesignViewModel>> GetListAsync<DesignViewModel>(string customerID)
        {
            Pager<DesignViewModel> pager = PagerOperations.InitializePager<DesignViewModel>
                                                        ("UpdatedTS", SortDirection.DESC.ToString(),
                                                          true, true, true, 30, true);

            pager.ColumnList = new List<ListItem>()
            {
                new ListItem { Text = "Design image", Value = "DesignImage" },
                new ListItem { Text = "Design number", Value = "DesignNumber" }
            };
            await DesignDAL.GetListAsync(pager, customerID);
            return pager;
        }

        public static async Task<OperationResult> GetListAsync<DesignViewModel>(Pager<DesignViewModel> pager, string customerID)
        {
            if (pager != null)
            {
                PagerOperations.UpdatePager(pager);
                await DesignDAL.GetListAsync(pager, customerID);
                return new OperationResult(Result.Success, "", pager);
            }
            else
            {
                return new OperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<OperationResult> GetByIDAsync(string customerID, string designNo)
        {
            if (customerID != null && customerID != "" && designNo != null && designNo != "")
            {
                InternalOperationResult result = await DesignDAL.GetByIDAsync(customerID, designNo);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> DeleteAsync(string customerID, string designNo, DateTime updatedTS)
        {
            if (customerID != null && customerID != "" && designNo != null && designNo != "" && updatedTS != null)
            {
                InternalOperationResult result = await DesignDAL.DeleteAsync(customerID, designNo, updatedTS);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }
    }
}

using ProStickers.BL.Core;
using ProStickers.CloudDAL.Storage;
using ProStickers.ViewModel.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProStickers.BL
{
    public class DesignerProfileBL
    {
        public static async Task<OperationResult> GetListAsync()
        {
            InternalOperationResult result = await DesignerProfileDAL.GetListAsync();
            return new OperationResult(result.Result, result.Message, result.ReturnedData);
        }

        public static async Task<Pager<AppointmentListViewModel>> GetDesignerFeedbackListAsync<AppointmentListViewModel>(string userID)
        {
            Pager<AppointmentListViewModel> pager = PagerOperations.InitializePager<AppointmentListViewModel>
                                                        ("RowKey", SortDirection.DESC.ToString(),
                                                          true, true, true, 30, true);

            pager.ColumnList = new List<ListItem>()
            {
                new ListItem { Text = "Feedback", Value = "CustomerFeedback" },
               new ListItem { Text = "Customer name", Value = "CustomerName"},
            };

            await DesignerProfileDAL.GetListAsync(pager, userID);
            return pager;
        }

        public static async Task<OperationResult> GetDesignerFeedbackListAsync<AppointmentListViewModel>(Pager<AppointmentListViewModel> pager, string userID)
        {
            PagerOperations.UpdatePager<AppointmentListViewModel>(pager);
            await DesignerProfileDAL.GetListAsync(pager, userID);
            return new OperationResult(Result.Success, "", pager);
        }
    }
}

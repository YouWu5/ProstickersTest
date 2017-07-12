using System.Collections.Generic;
using System.Threading.Tasks;
using ProStickers.BL.Core;
using ProStickers.CloudDAL;
using ProStickers.ViewModel.Core;

namespace ProStickers.BL
{
    public class ReportBL
    {
        public static async Task<Pager<SalesReportViewModel>> GetListAsync<SalesReportViewModel>()
        {
            Pager<SalesReportViewModel> pager = PagerOperations.InitializePager<SalesReportViewModel>
                                                        ("RowKey", SortDirection.DESC.ToString(),
                                                          true, true, true, 30, true);

            pager.SearchList = new List<SearchItem>
            {
                new SearchItem { DisplayName = "DateFrom", Name = "PartitionKey", Value ="" },
                new SearchItem { DisplayName = "DateTo", Name = "PartitionKey", Value ="" },
                new SearchItem { DisplayName = "Designer", Name = "UserName", Value ="" },
            };

            pager.ColumnList = new List<ListItem>()
            {
                new ListItem { Text = "Designer", Value = "UserName" },
                new ListItem { Text = "Number of designs created", Value = "NoOfDesigns" },
                new ListItem { Text = "Number of designs order", Value = "NoOfOrder" },
                new ListItem { Text = "Total sales amount", Value = "Amount" },
            };

            await ReportDAL.GetListAsync(pager);
            return pager;
        }

        public static async Task<OperationResult> GetListAsync<SalesReportViewModel>(Pager<SalesReportViewModel> pager)
        {
            PagerOperations.UpdatePager<SalesReportViewModel>(pager);
            await ReportDAL.GetListAsync(pager);
            return new OperationResult(Result.Success, "", pager);
        }

        public static async Task<List<ListItem>> GetDesignerListAsync()
        {
            return await ReportDAL.GetDesignerListAsync();
        }
    }
}

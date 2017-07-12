using ProStickers.ViewModel.Core;
using System.Collections.Generic;

namespace ProStickers.BL.Core
{
    public class PagerOperations
    {
        public static Pager<T> InitializePager<T>(string previousSortedColumn,
                                                  string sortDir, bool showPager = false,
                                                  bool showPageSize = false, bool showTotalRows = false,
                                                  byte defaultPageSize = 0, bool isSortable = false)
        {
            Pager<T> pager = new Pager<T>();
            pager.PreviousSortedColumn = previousSortedColumn;
            pager.Sort = previousSortedColumn;
            pager.SortDir = sortDir;
            pager.ShowPager = showPager;
            pager.ShowPageSize = showPageSize;
            pager.ShowTotalRows = showTotalRows;
            pager.PageNumber = 1;
            pager.PageSize = defaultPageSize;
            pager.IsSortable = isSortable;
            pager.CurrentOperation = GridOperations.None;
            pager.PageSizeOptions = new List<int> { 30, 50, 100 };
            return pager;
        }

        public static void UpdatePager<T>(Pager<T> pager)
        {
            if (pager.CurrentOperation == GridOperations.SortOrderChanged)
            {
                ApplySorting(ref pager);
            }

            if (pager.CurrentOperation == GridOperations.SearchParamChanged)
            {
                pager.PageNumber = 1;
            }
        }

        private static void ApplySorting<T>(ref Pager<T> pager)
        {
            pager.SortDir = pager.PreviousSortedColumn == pager.Sort ? (pager.SortDir == SortDirection.DESC.ToString() ? SortDirection.ASC.ToString() : SortDirection.DESC.ToString()) : SortDirection.DESC.ToString();
            pager.PreviousSortedColumn = pager.Sort;
        }

    }
}

using System;
using System.Collections.Generic;

namespace ProStickers.ViewModel.Core
{
    public class Pager<T>
    {
        public string PreviousSortedColumn { get; set; }
        public int RecordsCount { get; set; }
        public string Sort { get; set; }
        public string SortDir { get; set; }
        public byte PageSize { get; set; }
        public int PageNumber { get; set; }
        public bool ShowPager { get; set; }
        public bool IsSortable { get; set; }
        public bool ShowPageSize { get; set; }
        public bool ShowTotalRows { get; set; }

        private List<int> _pageSizeOptions = new List<int>() { };
        public List<int> PageSizeOptions
        {
            get
            {
                return _pageSizeOptions;
            }
            set
            {
                _pageSizeOptions = value;
            }
        }

        private GridOperations _currentOperations = GridOperations.None;
        public GridOperations CurrentOperation
        {
            get
            {
                return _currentOperations;
            }
            set
            {
                _currentOperations = value;
            }
        }

        private int _currentStartPage = 1;
        public int CurrentStartPage
        {
            get
            {
                return _currentStartPage;
            }

            set
            {
                _currentStartPage = value;
                OnCurrentStartPageChange();
            }
        }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                _currentPage = value;
            }
        }

        private int _currentLastPage = 0;
        public int CurrentLastPage
        {
            get
            {
                return _currentLastPage;
            }

            set
            {
                _currentLastPage = value;
            }
        }

        private int _pagerLimit = 5;
        public int PagerLimit
        {
            get
            {
                return _pagerLimit;
            }

            set
            {
                _pagerLimit = value;
            }
        }

        public List<SearchItem> SearchList { get; set; }
        public List<ListItem> ColumnList { get; set; }
        public List<T> Data { get; set; }

        public void OnCurrentStartPageChange()
        {
            decimal totalPages = Math.Ceiling((decimal)this.RecordsCount / (decimal)this.PageSize);
            if ((this.CurrentPage + (this.PagerLimit - 1)) <= totalPages)
            {
                this.CurrentLastPage = this.CurrentStartPage + (this.PagerLimit - 1);
            }
            else
            {
                this.CurrentLastPage = (int)totalPages;
            }
        }
    }

    public enum SortDirection
    {
        ASC = 1,
        DESC = 2
    }

    public enum GridOperations
    {
        PageSizeChanged,
        CurrentPageChanged,
        SortOrderChanged,
        SearchParamChanged,
        None
    }
}

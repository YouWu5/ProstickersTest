using System;

namespace ProStickers.ViewModel.Core
{
    public class ListItem
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }

    public class ListItemTypes
    {
        public string Text { get; set; }
        public long? Value { get; set; }
    }

    public class SearchItem
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class DeleteViewModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public DateTime UpdatedTS { get; set; }
    }

    public class DateFilter
    {
        public byte SNo { get; set; }
        public string Name { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class DownloadImageViewModel
    {
        public byte[] ImageBuffer { get; set; }
        public string FileExtension { get; set; }
    }

    public class CustomerFileViewModel
    {
        public string FileNumber { get; set; }
        public byte[] Image { get; set; }
    }

    public class CountryListViewModel
    {
        public long? ID { get; set; }
        public string Name { get; set; }
        public string Regax { get; set; }
    }
}

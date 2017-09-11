using System.Collections.Generic;

namespace ProStickers.ViewModel.Security
{
    public class UserSession
    {
        public string UserID { get; set; }
        public string Name { get; set; }
        public int UserTypeID { get; set; }
        public string UtcDateTimeOffset { get; set; }
        public bool IsPolicyAccepted { get; set; }
        public bool HaveSkype { get; set; }
        public List<UserPageListItem> AssignedPageList { get; set; }
    }

    public class UserPageListItem
    {
        public int? PageID { get; set; }
        public string Name { get; set; }
        public string ApiUrl { get; set; }
        public string Url { get; set; }
        public int OrderSequence { get; set; }
    }
}

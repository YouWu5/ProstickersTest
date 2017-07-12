namespace ProStickers.ViewModel
{
    public class DesignerProfileViewModel
    {
        public string UserID { get; set; }
        public string FullName { get; set; }
        public string ImageGUID { get; set; }
        public string ImageURL { get; set; }
    }

    public class DesignerFeedbackListViewModel
    {
        public string UserID { get; set; }
        public string CustomerName { get; set; }
        public string Feedback { get; set; }
    }
}

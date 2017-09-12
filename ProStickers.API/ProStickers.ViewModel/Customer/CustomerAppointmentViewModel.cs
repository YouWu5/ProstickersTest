using ProStickers.ViewModel.Core;
using System;

namespace ProStickers.ViewModel.Customer
{
    #region CustomerAppointmentViewModel
    public class CustomerAppointmentViewModel
    {
        public string CustomerID { get; set; }
        public string AppointmentNumber { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
        public string RequstTime { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string RequestDate { get; set; }
        public int AppointmentStatusID { get; set; }
        public string AppointmentStatus { get; set; }
        public string DesignNumber { get; set; }
        public string CancellationReason { get; set; }
        public DateTime UpdatedTS { get; set; }
        public byte[] ImageBuffer { get; set; }
        public bool IsDeleteEnable { get; set; }
        public bool IsBuyEnable { get; set; }
        public bool IsDownloadEnable { get; set; }
        public string ImageExtension { get; set; }
        public string MeetingLink { get; set; }
    }
    #endregion

    #region CustomerAppointmentListViewModel
    public class CustomerAppointmentListViewModel
    {
        public string MeetingLink { get; set; }
        public string AppointmentNumber { get; set; }
        public string AppointmentDateTime { get; set; }
        public string RequestDateTime { get; set; }
        public string AppointmentStatus { get; set; }
        public DateTime UpdatedTS { get; set; }
    }
    #endregion

    #region AppointmentRequestViewModel
    public class AppointmentRequestViewModel
    {
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public int RequestStatusID { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailID { get; set; }
        public DateTime RequestDateTime { get; set; }
    }
    #endregion

    #region Design ViewModel

    public class DesignViewModel
    {
        public string DesignNumber { get; set; }
        public bool IsAllowForPurchase { get; set; }
        public DateTime UpdatedTS { get; set; }
        public string AppointmentNumber { get; set; }
        public DownloadImageViewModel DesignImage { get; set; }
        public bool IsBuyEnable { get; set; }
    }

    #endregion
}

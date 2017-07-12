using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Attributes;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;

namespace ProStickers.ViewModel.Designer
{
    #region AppointmentViewModel

    [Validator(typeof(AppointmentViewModelValidator))]
    public class AppointmentViewModel
    {
        public string AppointmentNumber { get; set; }
        public string AppointmentDateTime { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int TimeSlotID { get; set; }
        public string TimeSlot { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerContactNumber { get; set; }
        public string RequestDateTime { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ContactNumber { get; set; }
        public string EmailID { get; set; }
        public string BillingAddress { get; set; }
        public DateTime CallStartTime { get; set; }
        public DateTime CallEndTime { get; set; }
        public string Notes { get; set; }
        public string CancellationReason { get; set; }
        public string AppointmentStatus { get; set; }
        public int AppointmentStatusID { get; set; }
        public string DesignNumber { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public DateTime UpdatedTS { get; set; }
        public double AspectRatio { get; set; }
        public bool IsCancel { get; set; }
        public byte[] DesignImageBuffer { get; set; }
        public byte[] VectorImageBuffer { get; set; }
        public string DesignImageExtension { get; set; }
        public string VectorImageExtension { get; set; }
        public int StatusID { get; set; }
        public int ImageStatusID { get; set; }
        public string MeetingLink { get; set; }
        public string DesignerNote { get; set; }
        public string SkypeID { get; set; }

        public List<CustomerFileViewModel> FileList { get; set; }
        public List<UserFilesViewModel> DesignerAppointmentFileList { get; set; }
        public List<UserFilesViewModel> DesignImageList { get; set; }
    }
    #endregion

    #region AppointmentListViewModel
    public class AppointmentListViewModel
    {
        public string AppointmentNumber { get; set; }
        public string AppointmentDateTime { get; set; }
        public string AppointmentDate { get; set; }
        public string Customer { get; set; }
        public string ContactNumber { get; set; }
        public string RequestDateTime { get; set; }
        public string Status { get; set; }
        public DateTime UpdatedTS { get; set; }
    }
    #endregion

    #region AppointmentRequestPickViewModel

    [Validator(typeof(AppointmentRequestPickViewModelValidator))]
    public class AppointmentRequestPickViewModel
    {
        public int TimeSlotID { get; set; }

        [Display(Name = "Time Slot")]
        public string TimeSlot { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCodeName { get; set; }
        public string PhoneNumber { get; set; }
        public int RequestStatusID { get; set; }
        public string RequestDateTime { get; set; }
        public string EmailID { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }

        [Display(Name = "Appointment Date")]
        public DateTime AppointmentDate { get; set; }
    }
    #endregion

    #region TimeSlotViewModel
    public class TimeSlotViewModel
    {
        public string Text { get; set; }
        public long? Value { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
    }
    #endregion

    #region AppointmentViewModelValidator

    public class AppointmentViewModelValidator : AbstractValidator<AppointmentViewModel>
    {
        public AppointmentViewModelValidator()
        {
            RuleFor(x => x.CancellationReason).NotEmpty().WithMessage(CommonValidationMessage.Required).When(x => x.IsCancel == true);

            RuleFor(x => x.CallStartTime).NotEmpty().WithMessage(CommonValidationMessage.Required).When(x => x.AppointmentStatus == "Initiated");

            RuleFor(x => x.CallEndTime).NotEmpty().WithMessage(CommonValidationMessage.Required).When(x => x.AppointmentStatus == "Completed");

            RuleFor(x => x.AspectRatio).NotEmpty().WithMessage(CommonValidationMessage.Required).InclusiveBetween(0.001, 99.999).WithMessage(CommonValidationMessage.AspectRatio).When(x => x.DesignImageBuffer != null || x.VectorImageBuffer != null);

            RuleFor(x => x.DesignerNote).Length(0, 1500).WithMessage(CommonValidationMessage.DesignerNote).When(x => x.DesignerNote != null);
        }
    }
    #endregion

    #region AppointmentRequestPickViewModelValidator

    public class AppointmentRequestPickViewModelValidator : AbstractValidator<AppointmentRequestPickViewModel>
    {
        public AppointmentRequestPickViewModelValidator()
        {
            RuleFor(x => x.AppointmentDate).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.TimeSlotID).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.TimeSlot).NotEmpty().WithMessage(CommonValidationMessage.Required);
        }
    }
    #endregion
}

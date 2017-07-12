using FluentValidation;
using FluentValidation.Attributes;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Designer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProStickers.ViewModel.Customer
{
    #region CalendarAppoimtmentViewModel
    [Validator(typeof(CalendarAppointmentViewModelValidator))]
    public class CalendarAppointmentViewModel
    {
        [Display(Name = "TimeSlot")]
        public int TimeSlotID { get; set; }
        public string TimeSlot { get; set; }
        [Display(Name = "Designer")]
        public string UserID { get; set; }
        public string UserName { get; set; }
        public DateTime UpdatedTS { get; set; }
        public DateTime Date { get; set; }
        public DateTime RequestDateTime { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }

        public List<TimeSlotViewModel> TimeSlotList { get; set; }
    }
    #endregion

    #region CalendarAppointmentViewModelValidator
    public class CalendarAppointmentViewModelValidator : AbstractValidator<CalendarAppointmentViewModel>
    {
        public CalendarAppointmentViewModelValidator()
        {
            RuleFor(x => x.TimeSlotID).NotNull().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.Date).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.RequestDateTime).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.UserID).NotEmpty().WithMessage(CommonValidationMessage.Required);
        }
    }
    #endregion

}

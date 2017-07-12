using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Attributes;
using ProStickers.ViewModel.Core;

namespace ProStickers.ViewModel.Designer
{
    #region DesignerTimeSlotViewModel

    [Validator(typeof(DesignerTimeSlotViewModelValidator))]
    public class DesignerTimeSlotViewModel
    {
        public bool IsAllTimeSlots { get; set; }
        public DateTime Date { get; set; }
        public List<TimeSlotListViewModel> TimeSlotList { get; set; }
    }

    #endregion

    #region TimeSlotListViewModel

    public class TimeSlotListViewModel
    {
        public int TimeSlotID { get; set; }
        public string Name { get; set; }
        public bool IsUserAvailable { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public int TimeSlotStatus { get; set; }
        public DateTime UpdatedTS { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
    }

    #endregion

    #region DesignerTimeSlotViewModelValidator

    public class DesignerTimeSlotViewModelValidator : AbstractValidator<DesignerTimeSlotViewModel>
    {
        public DesignerTimeSlotViewModelValidator()
        {
            RuleFor(x => x.Date).NotEmpty().WithMessage(CommonValidationMessage.Required);
        }
    }
    #endregion
}

using FluentValidation;
using FluentValidation.Attributes;
using ProStickers.ViewModel.Core;
using System;

namespace ProStickers.ViewModel.Master
{
    #region Predefined Size ViewModel
    [Validator(typeof(PredefinedSizeViewModelValidator))]
    public class PredefinedSizeViewModel
    {
        public string PredefinedSizeID { get; set; }
        public double Size { get; set; }
        public double OneColorPrice { get; set; }
        public double TwoColorPrice { get; set; }
        public double MoreColorPrice { get; set; }
        public DateTime UpdatedTS { get; set; }
    }
    #endregion

    #region Predefined Size ViewModel Validator
    public class PredefinedSizeViewModelValidator : AbstractValidator<PredefinedSizeViewModel>
    {
        public PredefinedSizeViewModelValidator()
        {
            RuleFor(x => x.Size).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.OneColorPrice).NotNull().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.TwoColorPrice).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.MoreColorPrice).NotEmpty().WithMessage(CommonValidationMessage.Required);
        }
    }
    #endregion
}

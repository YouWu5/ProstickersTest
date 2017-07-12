using FluentValidation;
using FluentValidation.Attributes;
using ProStickers.ViewModel.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProStickers.ViewModel.Master
{
    #region ColorChartViewModel
    [Validator(typeof(ColorChartViewModelValidator))]
    public class ColorChartViewModel
    {
        public string ColorID { get; set; }
        public string Name { get; set; }
        public string ImageGUID { get; set; }
        public string ImageExtension { get; set; }
        public bool IsAllowForSale { get; set; }
        public DateTime UpdatedTS { get; set; }

        public byte[] ImageBuffer { get; set; }
        public bool IsNew { get; set; }
    }
    #endregion

    #region ColorChartListViewModel
    public class ColorChartListViewModel
    {
        public string ColorID { get; set; }
        public string Name { get; set; }
        public bool IsAllowForSale { get; set; }
        public DateTime UpdatedTS { get; set; }
    }

    #endregion

    #region ColorChartViewModelValidator
    public class ColorChartViewModelValidator : AbstractValidator<ColorChartViewModel>
    {
        public ColorChartViewModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(CommonValidationMessage.Required);
        }
    }
    #endregion
}

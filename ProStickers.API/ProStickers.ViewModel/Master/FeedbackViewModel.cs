using FluentValidation;
using FluentValidation.Attributes;
using ProStickers.ViewModel.Core;
using System;

namespace ProStickers.ViewModel.Master
{
    #region Feedback Viewmodel
    [Validator(typeof(FeedbackViewModelValidator))]
    public class FeedbackViewModel
    {
        public string UserID { get; set; }
        public string CustomerID { get; set; }
        public int OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string FeedbackDate { get; set; }
        public string FeedbackDateTime { get; set; }
        public string CustomerFeedback { get; set; }
        public string MasterReply { get; set; }
        public string UserName { get; set; }
        public string DesignNo { get; set; }
        public string CustomerName { get; set; }
        public bool IsDisplayInProfile { get; set; }
        public byte[] ImageBuffer { get; set; }
        public DateTime CreatedTS { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTS { get; set; }
        public string ImageExtension { get; set; }
    }
    #endregion

    #region Feedback List ViewModel

    public class FeedbackListViewModel
    {
        public string DesignNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerID { get; set; }
        public string FeedbackDate { get; set; }
        public DateTime UpdatedTS { get; set; }
    }
    #endregion

    #region Feedback ViewModel Validator
    public class FeedbackViewModelValidator : AbstractValidator<FeedbackViewModel>
    {
        public FeedbackViewModelValidator()
        {
            RuleFor(x => x.CustomerID).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.FeedbackDate).NotNull().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.CustomerFeedback).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.DesignNo).NotEmpty().WithMessage(CommonValidationMessage.Required);

        }
    }
    #endregion
}

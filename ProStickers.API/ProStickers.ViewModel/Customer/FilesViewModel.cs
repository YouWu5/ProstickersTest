using FluentValidation;
using FluentValidation.Attributes;
using ProStickers.ViewModel.Core;
using System;
using System.Collections.Generic;

namespace ProStickers.ViewModel.Customer
{
    #region FilesViewModel
    [Validator(typeof(FilesViewModelValidator))]
    public class FilesViewModel
    {
        public string CustomerID { get; set; }
        public string FileName { get; set; }
        public string FileNumber { get; set; } // CustomerID + FileName
        public byte[] FileBuffer { get; set; }
        public string FileExtension { get; set; }
        public string FileNote { get; set; }
        public double FileSize { get; set; }
        public DateTime UpdatedTS { get; set; }
    }
    #endregion

    #region FilesListViewModel
    public class FilesListViewModel
    {
        public string FileNote { get; set; }
        public List<FilesViewModel> filesList { get; set; }
    }
    #endregion

    #region Files View model validator
    public class FilesViewModelValidator : AbstractValidator<FilesViewModel>
    {
        public FilesViewModelValidator()
        {
            RuleFor(x => x.CustomerID).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.FileName).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.FileExtension).NotEmpty().WithMessage(CommonValidationMessage.Required);
        }
    }
    #endregion
}

using FluentValidation;
using FluentValidation.Attributes;
using ProStickers.ViewModel.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProStickers.ViewModel.Customer
{
    #region CustomerViewModel
    [Validator(typeof(CustomerViewModelValidator))]
    public class CustomerViewModel
    {
        public string CustomerID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        [Display(Name = "Contact Number")]
        public string ContactNo { get; set; }
        [Display(Name = "Email")]
        public string EmailID { get; set; }
        public string SkypeID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        [Display(Name = "State")]
        public int StateID { get; set; }
        public string StateName { get; set; }
        public string PostalCode { get; set; }
        [Display(Name = "Country")]
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public bool IsFacebookUser { get; set; }
        public string ImageGUID { get; set; }
        public string FileNote { get; set; }
        public bool IsPolicyAccepted { get; set; }
        public DateTime? PolicyAcceptedDate { get; set; }
        public bool Active { get; set; }
        public DateTime UpdatedTS { get; set; }

        public string ImageURL { get; set; }
        public byte[] ImageBuffer { get; set; }
        private string _fullName;
        public string FullName
        {
            get
            {
                StringBuilder s = new StringBuilder();
                s.Append(FirstName);
                s.Append(MiddleName != null && MiddleName != "" ? " " + MiddleName + " " : " ");
                s.Append(LastName);

                this._fullName = s.ToString();
                return _fullName;
            }
            set
            {
                StringBuilder s = new StringBuilder();
                s.Append(FirstName);
                s.Append(MiddleName != null && MiddleName != "" ? " " + MiddleName + " " : " ");
                s.Append(LastName);

                this._fullName = s.ToString();
            }
        }
        public string ID { get; set; }

        public List<ListItemTypes> CountryList { get; set; }
    }
    #endregion

    #region CustomerDetailListViewModel
    public class CustomerDetailListViewModel
    {
        public List<CustomerAppointmentListViewModel> AppointmentList { get; set; }
        public List<OrderListViewModel> OrderList { get; set; }
        public List<CustomerDetailDesignViewModel> DesignList { get; set; }
        public List<string> FilesList { get; set; }
    }
    #endregion

    #region CustomerDetailDesignViewMdel
    public class CustomerDetailDesignViewModel
    {
        public string DesignNumber { get; set; }
        public DownloadImageViewModel DesignImage { get; set; }
    }
    #endregion

    #region CustomerListViewModel
    public class CustomerListViewModel
    {
        public string CustomerID { get; set; }
        public string Name { get; set; }
        public string ContactNo { get; set; }
        public string EmailID { get; set; }
        public DateTime UpdatesTS { get; set; }
    }
    #endregion

    #region CustomerDetailViewModel
    [Validator(typeof(CustomerDetailViewModelValidator))]
    public class CustomerDetailViewModel
    {
        public string UserID { get; set; }
        public string CustomerID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        [Display(Name = "Contact Number")]
        public string ContactNo { get; set; }
        [Display(Name = "Email")]
        public string EmailID { get; set; }
        public string SkypeID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        [Display(Name = "State")]
        public int StateID { get; set; }
        public string StateName { get; set; }
        public string PostalCode { get; set; }
        [Display(Name = "Country")]
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public bool IsFacebookUser { get; set; }
        public string ImageGUID { get; set; }
        public string FileNote { get; set; }
        public bool IsPolicyAccepted { get; set; }
        public DateTime? PolicyAcceptedDate { get; set; }
        public bool Active { get; set; }
        public DateTime UpdatedTS { get; set; }

        public string ImageURL { get; set; }
        public byte[] ImageBuffer { get; set; }
        private string _fullName;
        public string FullName
        {
            get
            {
                StringBuilder s = new StringBuilder();
                s.Append(FirstName);
                s.Append(MiddleName != null && MiddleName != "" ? " " + MiddleName + " " : " ");
                s.Append(LastName);

                this._fullName = s.ToString();
                return _fullName;
            }
            set
            {
                StringBuilder s = new StringBuilder();
                s.Append(FirstName);
                s.Append(MiddleName != null && MiddleName != "" ? " " + MiddleName + " " : " ");
                s.Append(LastName);

                this._fullName = s.ToString();
            }
        }

        public List<ListItemTypes> CountryList { get; set; }
        public List<CustomerFileViewModel> CustomerFileList { get; set; }
        public List<UserFilesViewModel> UserFileList { get; set; }
        public List<CustomerDesignViewModel> CustomerDesignList { get; set; }
        public List<CustomerFileViewModel> RemoveCustomerFileList { get; set; }
        public List<UserFilesViewModel> RemoveUserFileList { get; set; }
        public List<CustomerDesignViewModel> RemoveCustomerDesignList { get; set; }
        public DeleteViewModel Delete { get; set; }
    }
    #endregion  

    #region UserFilesViewModel
    public class UserFilesViewModel
    {
        public string CustomerID { get; set; }
        public string UserID { get; set; }
        public string DesignerName { get; set; }
        public string FileName { get; set; }
        public string FileNumber { get; set; } // UserID_FileName
        public byte[] FileBuffer { get; set; }
        public string FileExtension { get; set; }
        public string AppointmentNumber { get; set; }
        public int ImageStatusID { get; set; }            
    }
    #endregion

    #region CustomerDesignViewModel
    [Validator(typeof(CustomerDesignViewModelValidator))]
    public class CustomerDesignViewModel
    {
        public string CustomerID { get; set; }
        public string DesignNumber { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string DesignerNote { get; set; }
        public int OrderStatusID { get; set; }
        public DateTime UpdatedTS { get; set; }
    }
    #endregion

    #region CustomerViewModelValidator
    public class CustomerViewModelValidator : AbstractValidator<CustomerViewModel>
    {
        public CustomerViewModelValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.LastName).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.ContactNo).NotEmpty().WithMessage(CommonValidationMessage.Required).Length(0, 15).WithMessage(CommonValidationMessage.Max15);

            RuleFor(x => x.EmailID).NotEmpty().WithMessage(CommonValidationMessage.Required).EmailAddress().WithMessage(CommonValidationMessage.CodeInfo).Length(0, 99).WithMessage(CommonValidationMessage.AlphaNumericMax100);

            RuleFor(x => x.Address1).NotEmpty().WithMessage(CommonValidationMessage.Required).When(x => !string.IsNullOrEmpty(x.City) || x.StateID > 0 || !string.IsNullOrEmpty(x.PostalCode) || x.CountryID > 0);

            RuleFor(x => x.City).NotEmpty().WithMessage(CommonValidationMessage.Required).When(x => !string.IsNullOrEmpty(x.Address1) || x.StateID > 0 || !string.IsNullOrEmpty(x.PostalCode) || x.CountryID > 0);

            RuleFor(x => x.StateID).NotNull().WithMessage(CommonValidationMessage.Required).When(x => !string.IsNullOrEmpty(x.Address1) || !string.IsNullOrEmpty(x.City) || !string.IsNullOrEmpty(x.PostalCode) || x.CountryID > 0);

            RuleFor(x => x.PostalCode).NotEmpty().WithMessage(CommonValidationMessage.Required).When(x => !string.IsNullOrEmpty(x.Address1) || !string.IsNullOrEmpty(x.City) || x.StateID > 0 || x.CountryID > 0);

            RuleFor(x => x.CountryID).NotNull().WithMessage(CommonValidationMessage.Required).When(x => !string.IsNullOrEmpty(x.Address1) || !string.IsNullOrEmpty(x.City) || x.StateID > 0 || !string.IsNullOrEmpty(x.PostalCode));
        }
    }
    #endregion

    #region CustomerDetailViewModelValidator
    public class CustomerDetailViewModelValidator : AbstractValidator<CustomerDetailViewModel>
    {
        public CustomerDetailViewModelValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.LastName).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.ContactNo).NotEmpty().WithMessage(CommonValidationMessage.Required).Length(0, 15).WithMessage(CommonValidationMessage.Max15);

            RuleFor(x => x.EmailID).NotEmpty().WithMessage(CommonValidationMessage.Required).EmailAddress().WithMessage(CommonValidationMessage.CodeInfo).Length(0, 99).WithMessage(CommonValidationMessage.AlphaNumericMax100);

            RuleFor(x => x.Address1).NotEmpty().WithMessage(CommonValidationMessage.Required).When(x => !string.IsNullOrEmpty(x.City) || x.StateID > 0 || !string.IsNullOrEmpty(x.PostalCode) || x.CountryID > 0);

            RuleFor(x => x.City).NotEmpty().WithMessage(CommonValidationMessage.Required).When(x => !string.IsNullOrEmpty(x.Address1) || x.StateID > 0 || !string.IsNullOrEmpty(x.PostalCode) || x.CountryID > 0);

            RuleFor(x => x.StateID).NotNull().WithMessage(CommonValidationMessage.Required).When(x => !string.IsNullOrEmpty(x.Address1) || !string.IsNullOrEmpty(x.City) || !string.IsNullOrEmpty(x.PostalCode) || x.CountryID > 0);

            RuleFor(x => x.PostalCode).NotEmpty().WithMessage(CommonValidationMessage.Required).When(x => !string.IsNullOrEmpty(x.Address1) || !string.IsNullOrEmpty(x.City) || x.StateID > 0 || x.CountryID > 0);

            RuleFor(x => x.CountryID).NotNull().WithMessage(CommonValidationMessage.Required).When(x => !string.IsNullOrEmpty(x.Address1) || !string.IsNullOrEmpty(x.City) || x.StateID > 0 || !string.IsNullOrEmpty(x.PostalCode));
        }
    }
    #endregion

    #region CustomerDesignViewModelValidator
    public class CustomerDesignViewModelValidator : AbstractValidator<CustomerDesignViewModel>
    {
        public CustomerDesignViewModelValidator()
        {
            RuleFor(x => x.DesignerNote).Length(0, 1500).WithMessage(CommonValidationMessage.DesignerNote).When(x => x.DesignerNote != null);
        }
    }
    #endregion
}

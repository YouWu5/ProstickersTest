using FluentValidation;
using FluentValidation.Attributes;
using ProStickers.ViewModel.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProStickers.ViewModel.Master
{
    #region User ViewModel
    [Validator(typeof(UserViewModelValidator))]
    public class UserViewModel
    {
        public string UserID { get; set; }
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        public string ImageGUID { get; set; }
        [Display(Name = "Google id")]
        public string GoogleID { get; set; }
        public string SkypeID { get; set; }
        public string Description { get; set; }
        [Display(Name = "Role")]
        public int UserTypeID { get; set; }
        public string UserType { get; set; }
        public DateTime UpdatedTS { get; set; }
        public byte[] ImageBuffer { get; set; }
        public string ID { get; set; }
        private string _fullName;
        public string FullName
        {
            get
            {
                StringBuilder s = new StringBuilder();
                s.Append(FirstName);
                s.Append(MiddleName != null ? " " + MiddleName : "");
                s.Append(" ");
                s.Append(LastName);

                this._fullName = s.ToString();
                return _fullName;
            }
            set
            {
                StringBuilder s = new StringBuilder();
                s.Append(FirstName);
                s.Append(MiddleName != null ? " " + MiddleName : "");
                s.Append(LastName);

                this._fullName = s.ToString();
            }
        }
        public bool IsNew { get; set; }
        public bool Active { get; set; }
        public bool IsDeleteEnable { get; set; }
    }
    #endregion

    public class UserListViewModel
    {
        public string Name { get; set; }
        public string GoogleID { get; set; }
        public string UserType { get; set; }
        public bool IsDeleteEnable { get; set; }
        public string UserID { get; set; }
        public DateTime UpdatedTS { get; set; }
        public bool Active { get; set; }
    }

    public class UserStatusChangeViewModel
    {
        public string UserID { get; set; }
        public DateTime UpdatedTS { get; set; }
        public bool Active { get; set; }
    }

    #region User ViewModel Validator
    public class UserViewModelValidator : AbstractValidator<UserViewModel>
    {
        public UserViewModelValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.LastName).NotNull().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.GoogleID).EmailAddress().WithMessage(CommonValidationMessage.CodeInfo);

            RuleFor(x => x.UserTypeID).NotEmpty().WithMessage(CommonValidationMessage.Required);
        }
    }
    #endregion
}

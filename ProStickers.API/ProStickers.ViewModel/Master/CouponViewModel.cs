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
    #region Coupon ViewModel
    [Validator(typeof(CouponViewModelValidator))]
    public class CouponViewModel
    {
        public string CouponID { get; set; }
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
        public int CouponTypeID { get; set; }
        public string CouponType { get; set; }
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

    public class CouponListViewModel
    {
        public string Name { get; set; }
        public string GoogleID { get; set; }
        public string CouponType { get; set; }
        public bool IsDeleteEnable { get; set; }
        public string CouponID { get; set; }
        public DateTime UpdatedTS { get; set; }
        public bool Active { get; set; }
    }

    public class CouponStatusChangeViewModel
    {
        public string CouponID { get; set; }
        public DateTime UpdatedTS { get; set; }
        public bool Active { get; set; }
    }

    #region Coupon ViewModel Validator
    public class CouponViewModelValidator : AbstractValidator<CouponViewModel>
    {
        public CouponViewModelValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.LastName).NotNull().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.GoogleID).EmailAddress().WithMessage(CommonValidationMessage.CodeInfo);

            RuleFor(x => x.CouponTypeID).NotEmpty().WithMessage(CommonValidationMessage.Required);
        }
    }
    #endregion
}

using FluentValidation;
using FluentValidation.Attributes;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProStickers.ViewModel.Customer
{
    #region OrderViewModel
    [Validator(typeof(OrderViewModelValidator))]
    public class OrderViewModel
    {
        public int OrderNumber { get; set; }
        public string DesignNumber { get; set; }
        public string AppointmentNumber { get; set; }
        public bool PurchaseVectorFile { get; set; }
        public bool PurchaseDesignImage { get; set; }
        public double ShippingPrice { get; set; }
        public double VectorFilePrice { get; set; }
        public double DesignImagePrice { get; set; }
        public double Amount { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        [Display(Name = "State")]
        public int StateID { get; set; }
        public string StateName { get; set; }
        [Display(Name = "Country")]
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public string PostalCode { get; set; }
        public string EmailID { get; set; }
        public int OrderStatusID { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public string TrackingNumber { get; set; }
        public bool IsfeedbackDone { get; set; }
        public int StatusID { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public DateTime UpdatedTS { get; set; }

        public double AspectRatio { get; set; }
        public double OneColorPrice { get; set; }
        public double TwoColorPrice { get; set; }
        public double MoreColorPrice { get; set; }
        public byte[] DesignImageBuffer { get; set; }
        public string DesignImageExtension { get; set; }

        public string CardNo { get; set; }
        public string NameOnCard { get; set; }
        public int ExpiryYear { get; set; }
        public int ExpiryMonth { get; set; }
        public int CVV { get; set; }

        public int PurchaseTypeID { get; set; }
        public int Quantity { get; set; }

        [Display(Name = "Color")]
        public List<ColorViewModel> ColorList { get; set; }
    }
    #endregion

    #region ColorViewModel
    public class ColorViewModel
    {
        public string ColorID { get; set; }
        public string Name { get; set; }
        public string ImageGUID { get; set; }
        public string ImageURL { get; set; }
        public bool IsSelected { get; set; }
        public int ColorSequence { get; set; }
    }
    #endregion

    #region StripeResult

    public class StripeResult
    {
        public string ID { get; set; }
        public bool IsSuccess { get; set; }
    }
    #endregion

    #region Order List View Model

    public class OrderListViewModel
    {
        public string OrderNumber { get; set; }
        public string DesignNumber { get; set; }
        public string OrderDate { get; set; }
        public int OrderStatusID { get; set; }
        public string OrderStatus { get; set; }
        public int PurchaseTypeID { get; set; }
        public double ShippingPrice { get; set; }
        public double VectorFilePrice { get; set; }
        public double DesignImagePrice { get; set; }
        public double Amount { get; set; }
        public byte[] ImageBuffer { get; set; }      
        public string TrackingNumber { get; set; }
        public string UserName { get; set; }
        public string UserID { get; set; }
        public string ImageExtension { get; set; }
        public string USPSLink { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public int Quantity { get; set; }
        public List<ColorViewModel> ColorList { get; set; }
    }

    #endregion

    #region Order Tracking View Model
    [Validator(typeof(OrderTrackingViewModelValidator))]
    public class OrderTrackingViewModel
    {
        public int OrderNumber { get; set; }
        public string AppointmentNumber { get; set; }
        public string DesignNumber { get; set; }
        public string OrderDate { get; set; }
        public double Amount { get; set; }
        public string TrackingNumber { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string OrderStatus { get; set; }
        public string UserID { get; set; }
        public DownloadImageViewModel ImageBuffer { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public List<ColorViewModel> ColorList { get; set; }
        public string ColorJSON { get; set; }
        public string ShippingAddress { get; set; }
        public int PurchaseTypeID { get; set; }
        public double ShippingPrice { get; set; }
        public double VectorFilePrice { get; set; }
        public double DesignImagePrice { get; set; }
        public int Quantity { get; set; }
    }

    #endregion

    #region OrderViewModelValidator
    public class OrderViewModelValidator : AbstractValidator<OrderViewModel>
    {
        public OrderViewModelValidator()
        {
            RuleFor(x => x.Length).NotEmpty().WithMessage(CommonValidationMessage.Required).When(x => x.Width == 0.0);

            RuleFor(x => x.Width).NotEmpty().WithMessage(CommonValidationMessage.Required).When(x => x.Length == 0.0);

            RuleFor(x => x.ColorList).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.Address1).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.City).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.StateID).NotNull().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.PostalCode).NotEmpty().WithMessage(CommonValidationMessage.Required);

            RuleFor(x => x.CountryID).NotNull().WithMessage(CommonValidationMessage.Required);
        }
    }
    #endregion

    #region OrderTrackingViewModelValidator
    public class OrderTrackingViewModelValidator : AbstractValidator<OrderTrackingViewModel>
    {
        public OrderTrackingViewModelValidator()
        {
            RuleFor(x => x.TrackingNumber).NotEmpty().WithMessage(CommonValidationMessage.Required).Length(0, 40).WithMessage(CommonValidationMessage.Required);
        }
    }
    #endregion

    #region EmailCustomerDetailViewModel

    public class EmailCustomerDetailViewModel
    {
        public string CustomerID { get; set; }
        public string FullName { get; set; }
        public string ContactNo { get; set; }
        public string EmailID { get; set; }
        public string BillingAddress { get; set; }
        public string DeliveryAdderss { get; set; }
    }

    #endregion
}

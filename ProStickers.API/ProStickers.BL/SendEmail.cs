using ProStickers.CloudDAL.Storage;
using ProStickers.CloudDAL.Storage.Customer;
using ProStickers.ViewModel.Customer;
using ProStickers.ViewModel.Designer;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace ProStickers.BL
{
    public class SendEmail
    {
        static string apiKey = WebConfigurationManager.AppSettings["SENDGRID_APIKEY"].ToString();
        static string publicWebsiteUrl = CommonDAL.GetPublicWebsiteURL();
        static string prostickerLogoUrl = CommonDAL.GetProstickerLogoURL();

        #region Send Feedback Request Email On Order Placed

        public static async Task FeedbackRequestEmailAsync(string senderAddress, string displayName, OrderViewModel vm)
        {          
            string feedbacklink = CommonDAL.GetFeedbackLink();
            string customerportalurl = CommonDAL.GetCustomerPortalURL();

            double itemSubTotal = vm.DesignImagePrice + vm.VectorFilePrice;
            string day = vm.OrderDate.Date.DayOfWeek.ToString();
            int year = vm.OrderDate.Date.Year;
            int date = vm.OrderDate.Date.Day;
            string month = vm.OrderDate.ToString("MMMM");

            var client = new SendGridClient(apiKey);

            // Send a Single Email using the Mail Helper
            var from = new EmailAddress(senderAddress, displayName);
            var subject = "Your ProStickersLive.com Order - " + vm.OrderNumber;
            var to = new EmailAddress(vm.EmailID.ToString(), vm.CustomerName);
            var htmlcontent =
"<body style='margin:0; padding:20px;'>" +
     "<div style='height:100%;font-family:Roboto,sans-serif;font-size:14px;line-height:1.6em;color:#564c4c;'>" +
          "<div style='width:100%;overflow:visible; width:auto;margin:0 auto;border:1px solid #333; padding:10px;'>" +
               "<div style='min-height:10px;text-align: right;'>" +
                    "<div><p> <a href='" + customerportalurl + "' target='_blank' alt='Your Account'><span>Your Account</span></a></p></div>" +
                    "<div style='min-height:10px;'>" +
                         "<div><p><b>Order Confirmation #" + vm.OrderNumber + "</b></p></div>" +
                         "</div>" +
                    "</div>" +
                    "<div style='min-height:10px;'>" +
                         "<div><p>Hello " + vm.CustomerName + ",</p></div>" +
                         "<div style='padding - left: 25px;'><p>Thank you for order. We'll send you a confirmation email once your order is shipped.</p></div>" +
                            "</div>" +
                       "<div style='min-height:20px;'>" +
                            "<div><p><b>Order Detail:</b></p></div>" +
                            "</div>" +
                       "<div style='min-height:10px;'>" +
                            "<div style='padding - left: 25px;'><p>Placed on " + day + ", " + month + " " + date + ", " + year + "</p></div>" +
                               "</div>" +
                          "<div style='min-height:10px;padding-bottom:10px;'>" +
                               "<div><p>Your order will be send to: </p></div>" +
                               "<div><b>" + vm.CustomerName + "</b></div>" +
                               "<div>" + vm.Address1 + "</div>" +
                               "<div>" + vm.Address2 + "</div>" +
                               "<div>" + vm.City + ", " + vm.StateName + ", " + vm.PostalCode + "</div>" +
                               "<div>" + vm.CountryName + "</div>" +
                   "</div>" +                 
                   "<div style='min-height:20px;padding-top:25px;padding-bottom:25px;width:100%;display: block;overflow: auto;'>" +
                   "<div style='width:100%; float:left;'>" +
                          "<div style='width:100%; float:left; font-weight:bold; padding-bottom:10px; font-size:18px;'>Design No.: "+ vm.DesignNumber +"</div>" +
                                                 "<div style='width:50%; float:left;'>Decal Price</div>" +
                                                    "<div style='width:50%; float:left; text-align:right;'>" + Convert.ToDouble(vm.DesignImagePrice).ToString("C", new CultureInfo("en-US")) + "</div>" +
                                                         "<div style='width:50%; float:left;'>Design File Price</div>" +
                                                            "<div style='width:50%; float:left; text-align:right;  padding-bottom:10px;'>" + Convert.ToDouble(vm.VectorFilePrice).ToString("C", new CultureInfo("en-US")) + "</div>" +
                  "<div style='width:25%; float:left;'>&nbsp;</div>" +
                     "<div style='width:74%; float:right; text-align:right; border-top:1px solid #333;'>Item Sub Total: " + Convert.ToDouble(itemSubTotal).ToString("C", new CultureInfo("en-US")) + "</div>" +
                          "<div style='width:25%; float:left;'>&nbsp;</div>" +
                             "<div style='width:74%; float:right; text-align:right; border-bottom:1px solid #333; padding-bottom:10px;'>Shipping charges:  " + Convert.ToDouble(vm.ShippingPrice).ToString("C", new CultureInfo("en-US")) + "</div>" +
                  "<div style='width:25%; float:left; '>&nbsp;</div>" +
                     "<div style='width:75%; float:left; text-align:right; font-weight:bold;'>Order Total: " + Convert.ToDouble(vm.Amount).ToString("C", new CultureInfo("en-US")) + "</div>" +
                      "</div>" +
                      "</div>" +
                      "<div>&nbsp;</div>" +
                      "<div style='min-height:20px;'>" +
                           "<div><p><b>Feedback Request:</b></p></div>" +
                           "</div>" +
                             "<div style='min-height:10px;'>" +
                                  "<div><p>We want your feedback for your recent purchase/design experience:</p></div>" +
                                  "</div>" +
                             "<div style='min-height:10px;'>" +
                                  "<div><p>We would love to hear what you think about your custom decal experience.</p></div>" +
                                  "</div>" +
                             "<div style='min-height:10px;'>" +
                                  "<div><p>Start your feedback:<a href='" + feedbacklink + vm.OrderNumber + "' target='_blank' alt='Feedback Link'><span>Feedback Link</span></a></p></div>" +
                      "</div>" +
                      "<div>Thanks,</div>" +
                      "<div>Pro Sticker Live</div>" +
                      "<div>&nbsp;</div>" +
                      "<div>" +
                           "<a href='" + publicWebsiteUrl + "' target='_blank' alt='Pro Sticker Public Website URL'>" +
                                "<img style='padding:10px;' src='" + prostickerLogoUrl + "' alt='Pro Sticker Logo' ; Height='100' ; Width='200' />" +
                                "</a>" +
                           "</div>" +
                             "<div style='min-height:10px;'>" +
                                  "<div><p>If Clicking the “feedback link” link fails to load feedback form, Copy the following URL <a href='" + feedbacklink + vm.OrderNumber + "' target='_blank' alt='Feedback Link'><span>Feedback Link</span></a> and paste it into a browser address bar to submit your feedback.</p></div>" +
                                  "</div>" +
                             "</div>" +
                        "</div>" +
                   "</body>";
          
            var message = MailHelper.CreateSingleEmail(from, to, subject, null, htmlcontent);
            var response = await client.SendEmailAsync(message);
        }

        #endregion

        #region Send Order Delivery Conformation Email On Order Shipped

        public static async Task OrderDeliveryConformationEmailAsync(string senderAddress, string displayName, OrderTrackingViewModel otVM)
        {
            EmailCustomerDetailViewModel ecdVM = await OrderDAL.GetCustomerDetailOnSendEmailAsync(otVM.CustomerID, otVM.OrderNumber);

            string uspslink = CommonDAL.GetUSPSLink();

            var client = new SendGridClient(apiKey);

            // Send a Single Email using the Mail Helper
            var from = new EmailAddress(senderAddress, displayName);
            var subject = "Pro Sticker Live – Order Shipping Confirmation";
            var to = new EmailAddress(ecdVM.EmailID, ecdVM.FullName);

            var htmlcontent = "<body style = 'margin:0;background-color:#f5f5f5; padding:50px;' > " +
    "<div style='padding:10px;height:100%;font-family:Roboto,sans-serif;font-size:14px;line-height:1.6em;color:#564c4c;'>" +
        "<div style='max-width:800px;width:auto;margin:0 auto;border:1px solid #333;min-width:400px; padding:10px;'>" +
            "<div style='min-height:10px;'>" +
                "<div><p>Your Custom Decal order has shipped!</p></div>" +
            "</div>" +
             "<div style='min-height:20px;'>" +
                "<div><p><b>Thank you for your order</b></p></div>" +
            "</div>" +
            "<div style='min-height:10px;'>" +
                "<div><p>Your Order no. " + otVM.OrderNumber + " has shipped. Your order is on its way, and can no longer be changed.</p></div>" +
            "</div>" +
             "<div style='min-height:10px;'>" +
                "<div><p>Here is your tracking information:</p></div>" +
            "</div>" +
             "<div style='min-height:20px;'>" +
                "<div><p><b>Shipping to USPS:</b></p></div>" +
            "</div>" +
            "<div style='min-height:10px;'>" +
                "<div><p>Here is your USPS tracking number " + otVM.TrackingNumber + " You can also track through USPS here:<a href='" + uspslink + otVM.TrackingNumber + "' target='_blank' alt='Tracking Link'><span>Tracking Link</span></a></p></div>" +
            "</div>" +
             "<div style='min-height:20px;'>" +
                "<div><p><b>Customer Details:</b></p></div>" +
                "<div><p><b>Name:</b>" + ecdVM.FullName + "</p></div>" +
                "<div><p><b>Contact No:</b>" + ecdVM.ContactNo + "</p></div>" +
                "<div><p><b>EmailID:</b>" + ecdVM.EmailID + "</p></div>" +
            "</div>" +
            "<div style='min-height:20px;'>" +
                "<div><p><b>Billing Address:</b></p></div>" +
                 "<div><p>" + ecdVM.BillingAddress + "</p></div>" +
            "</div>" +
             "<div style='min-height:20px;'>" +
                "<div><p><b>Delivery Address:</b></p></div>" +
                 "<div><p>" + ecdVM.DeliveryAdderss + "</p></div>" +
            "</div>" +
            "<div>Thanks,</div> " +
            "<div>Pro Sticker Live</div>" +
            "<div>&nbsp;</div>" +
            "<div>" +
                "<a href='" + publicWebsiteUrl + "' target='_blank' alt='Pro Sticker Public Website URL'>" +
                    "<img style='padding:10px;' src='" + prostickerLogoUrl + "' alt='Pro Sticker Logo'; Height = '100'; Width = '200'/>" +
                "</a>" +
            "</div> " +
        "</div>" +
    "</div>" +
"</body>";

            var message = MailHelper.CreateSingleEmail(from, to, subject, null, htmlcontent);
            var response = await client.SendEmailAsync(message);
        }

        #endregion

        #region Send Designer Cancelled Appointment Email

        public static async Task DesignerCancelledAppointmentEmailAsync(string senderAddress, string displayName, AppointmentViewModel vm)
        {
            string customerportalurl = CommonDAL.GetCustomerPortalURL();

            var client = new SendGridClient(apiKey);

            // Send a Single Email using the Mail Helper
            var from = new EmailAddress(senderAddress, displayName);
            var subject = "Pro Sticker Live – Appointment Cancellation";
            var to = new EmailAddress(vm.EmailID, vm.CustomerName);

            var htmlcontent = "<body style='margin:0;background-color:#f5f5f5; padding:50px;'>" +
      "<div style='padding:10px;height:100%;font-family:Roboto,sans-serif;font-size:14px;line-height:1.6em;color:#564c4c;'>" +
          "<div style='max-width:800px;width:auto;margin:0 auto;border:1px solid #333;min-width:400px; padding:10px;'>" +
              "<div style='min-height:10px;'>" +
                  "<div><p>Hello from Pro Sticker Live,</p></div>" +
              "</div>" +
              "<div style='min-height:10px;'>" +
                  "<div><p>Your appointment " + vm.AppointmentNumber + " on " + vm.AppointmentDate.ToShortDateString() + " and " + vm.TimeSlot + " slot with " + vm.UserName + " has been cancelled.</p></div> " +
               "</div>" +
                 "<div style='min-height:20px;'>" +
                  "<div><p><b>Designer’s Cancellation Reason:</b>" + vm.CancellationReason + "</p></div> " +
               "</div>" +
               "<div style='min-height:10px;'>" +
                   "<div><p>You can schedule a new appointment with us at: <a href='" + customerportalurl + "' target='_blank' alt='Customer Portal URL'><span>Customer Portal URL</span></a></p></div>" +
               "</div>" +
               "<div>Thanks,</div> " +
               "<div>Pro Sticker Live</div>" +
               "<div>&nbsp;</div>" +
               "<div>" +
                   "<a href='" + publicWebsiteUrl + "' target='_blank' alt='Pro Sticker Public Website URL'>" +
                       "<img style='padding:10px;' src='" + prostickerLogoUrl + "' alt='Pro Sticker Logo'; Height = '100'; Width = '200'/>" +
                   "</a>" +
               "</div> " +
           "</div>" +
       "</div>" +
   "</body>";

            var message = MailHelper.CreateSingleEmail(from, to, subject, null, htmlcontent);
            var response = await client.SendEmailAsync(message);
        }

        #endregion
    }
}

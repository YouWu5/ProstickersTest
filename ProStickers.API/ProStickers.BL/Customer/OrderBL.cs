using ProStickers.BL.Core;
using ProStickers.CloudDAL.Storage.Customer;
using ProStickers.CloudDAL.Storage.ExceptionStorage;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using ProStickers.ViewModel.Security;
using Stripe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace ProStickers.BL.Customer
{
    public class OrderBL
    {
        static string publicKey = WebConfigurationManager.AppSettings["StripeKey"];
        public static string senderAddress = WebConfigurationManager.AppSettings["SENDER_ID"].ToString();
        public static string displayName = WebConfigurationManager.AppSettings["SENDER_DisplayName"].ToString();

        public static async Task<OperationResult> GetDetailAsync(string designNo, string appointmentNo, UserInfo userInfo)
        {
            if (designNo != null && designNo != "" && appointmentNo != null && appointmentNo != "" && userInfo != null)
            {
                InternalOperationResult result = await OrderDAL.GetDetailAsync(designNo, appointmentNo, userInfo.UserID);
                if (result.Result == Result.Success)
                {
                    OrderViewModel orderVM = (result.ReturnedData as OrderViewModel);
                    DownloadImageViewModel vm = await CommonBL.DownloadDesignImageAsync(userInfo.UserID, designNo);
                    if (vm != null)
                    {
                        orderVM.DesignImageBuffer = vm.ImageBuffer;
                        orderVM.DesignImageExtension = vm.FileExtension;
                    }
                }
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> CreateAsync(OrderViewModel vm, UserInfo currentUserInfo)
        {
            if (vm != null)
            {
                if (vm.Amount < (50 / 100))
                {
                    return new OperationResult(Result.SDError, "Order amount should be greater than 50 cents.", null);
                }
                InternalOperationResult result = new InternalOperationResult();
                result = await OrderDAL.CreateAsync(vm, currentUserInfo.UserID, currentUserInfo.UserName);
                if (result.Result == Result.Success)
                {
                    try
                    {
                        StripeResult customerResult = Create();
                        StripeResult cardResult;
                        StripeResult chargeResult;
                        if (customerResult.IsSuccess)
                        {
                            cardResult = Create(customerResult.ID, vm.NameOnCard, vm.CardNo, vm.ExpiryYear.ToString(), vm.ExpiryMonth.ToString(), vm.CVV.ToString());
                            if (cardResult.IsSuccess)
                            {
                                chargeResult = Create(customerResult.ID, vm.Amount);
                                if (chargeResult.IsSuccess)
                                {
                                    await OrderDAL.UpdateTransactionDetail(cardResult.ID, chargeResult.ID, customerResult.ID, vm.OrderDate, vm.OrderNumber);
                                    await SendEmail.FeedbackRequestEmailAsync(senderAddress, displayName, vm);
                                    return new OperationResult(result.Result, result.Message, result.ReturnedData);
                                }
                                else
                                {
                                    result = await OrderDAL.DeleteAsync(vm, currentUserInfo.UserID);
                                    return new OperationResult(Result.SDError, "Order can't be placed due to transaction failure.", null);
                                }
                            }
                            else
                            {
                                result = await OrderDAL.DeleteAsync(vm, currentUserInfo.UserID);
                                return new OperationResult(Result.SDError, "Order can't be placed due to transaction failure", null);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        result = await OrderDAL.DeleteAsync(vm, currentUserInfo.UserID);
                        string _requestJSON = Newtonsoft.Json.JsonConvert.SerializeObject(vm);
                        ExceptionTableStorage.InsertOrReplaceEntity("Customer", "OrderDAL", "CreateAsync", DateTime.UtcNow, e, null, _requestJSON, currentUserInfo.UserID);
                        return new OperationResult(Result.SDError, "Order can't be placed due to transaction failure", null);
                    }
                }
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        #region Order Detail

        public static async Task<Pager<OrderListViewModel>> GetListAsync<OrderListViewModel>(string userID)
        {
            Pager<OrderListViewModel> pager = PagerOperations.InitializePager<OrderListViewModel>
                                                        ("UpdatedTS", SortDirection.DESC.ToString(),
                                                          true, true, true, 30, true);

            pager.ColumnList = new List<ListItem>()
            {
                new ListItem { Text = "Order number", Value = "OrderNumber" },
                new ListItem { Text = "Order date", Value = "Date" },
                new ListItem { Text = "Design number", Value = "DesignNumber" },
                new ListItem { Text = "Status", Value = "OrderStatus" },
                new ListItem { Text = "Amount", Value = "Amount" },
            };

            await OrderDAL.GetListAsync(pager, userID);
            return pager;
        }

        public static async Task<OperationResult> GetListAsync<OrderListViewModel>(Pager<OrderListViewModel> pager, string userID)
        {
            if (pager != null && userID != null && userID != "")
            {
                PagerOperations.UpdatePager<OrderListViewModel>(pager);
                await OrderDAL.GetListAsync(pager, userID);
                return new OperationResult(Result.Success, "", pager);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> GetByIDAsync(UserInfo currentUserInfo, int orderNo)
        {
            if (orderNo > 0)
            {
                InternalOperationResult result = await OrderDAL.GetByIDAsync(currentUserInfo.UserID, orderNo);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        #endregion

        #region Payment 

        public static StripeResult Create()
        {
            var customer = new StripeCustomerCreateOptions();

            // set these properties if it makes you happy
            // customer.Email = email;
            //customer.Description = "Punit kabran (pk@email.com)";

            var customerService = new StripeCustomerService(publicKey);
            var stripeCustomer = customerService.Create(customer);

            StripeResult result = new StripeResult();
            result.ID = stripeCustomer.Id;
            result.IsSuccess = true;
            return result;
        }

        public static StripeResult Create(string customerID, string cardHolderName, string cardNumber, string expiryYear, string expiryMonth, string cvc)
        {
            var myCard = new StripeCardCreateOptions();

            // setting up the card
            myCard.SourceCard = new SourceCard()
            {
                // set these properties if passing full card details (do not
                // set these properties if you set TokenId)
                Number = cardNumber,
                ExpirationYear = expiryYear,
                ExpirationMonth = expiryMonth,
                Name = cardHolderName,   // optional
                Cvc = cvc                // optional
            };

            StripeResult chargeResult = new StripeResult();
            var cardService = new StripeCardService(publicKey);
            StripeCard stripeCard = cardService.Create(customerID, myCard);
            chargeResult.ID = stripeCard.Id;
            chargeResult.IsSuccess = true;
            return chargeResult;
        }

        public static StripeResult Create(string cardID, double amount)
        {
            var charge = new StripeChargeCreateOptions();
            charge.Currency = "USD";
            charge.Amount = (int)amount * 100;
            charge.CustomerId = cardID;

            var chargeService = new StripeChargeService(publicKey);
            var stripeCharge = chargeService.Create(charge);
            StripeResult chargeResult = new StripeResult();
            chargeResult.ID = stripeCharge.BalanceTransactionId;
            if (stripeCharge.Status == "succeeded")
            {
                chargeResult.IsSuccess = true;
            }
            else
            {
                chargeResult.IsSuccess = false;
            }
            return chargeResult;
        }

        #endregion
    }
}

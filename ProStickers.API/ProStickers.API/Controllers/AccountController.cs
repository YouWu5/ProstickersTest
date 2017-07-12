using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProStickers.BL.Security;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;

namespace ProStickers.API.Controllers
{
    [RoutePrefix("User/Account")]
    public class AccountController : ApiController
    {
        #region Post Create
        [HttpPost]
        [Route("CustomerSession")]
        public async Task<IHttpActionResult> CreateCustomerAsync(CustomerViewModel vm)
        {
            OperationResult result = await UserAuthenticationBL.CreateCustomerAsync(vm);
            if (result.Result != Result.Success)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.PreconditionFailed, result));
            }
            else
            {
                return Ok(result);
            }
        }
        #endregion

        #region Get UserSession
        [HttpGet]
        [Route("{userID}/{ID}/UserSession")]
        public async Task<IHttpActionResult> GetUserSessionAsync(string userID, string ID)
        {
            OperationResult result = await UserAuthenticationBL.GetUserSessionAsync(userID, ID);
            if (result.Result != Result.Success)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.PreconditionFailed, result));
            }
            else
            {
                return Ok(result);
            }
        }
        #endregion

        #region Get LoginToken
        [HttpPost]
        [Route("ObtainAccessToken")]
        public async Task<IHttpActionResult> GetLoginTokenAsync(LoginViewModel vm)
        {
            try
            {
                if (vm.UserTypeID == 1)
                {
                    OperationResult result = UserAuthenticationBL.CheckMasterOrDesignerExistsAsync(vm.UserID);
                    if (result.Result != Result.Success)
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, result));
                    }   
                }
              
                else if (vm.UserTypeID == 3 && UserAuthenticationBL.CheckUserExistsAsync(vm.UserID))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, new OperationResult { Result = Result.UDError, Message = "User is added to Master/Designer group and is not authorized for customer portal. Please use different ID to login.", ReturnedData = null }));
                }

                string param;
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(WebConfigurationManager.AppSettings["APIURL"], UriKind.Absolute);
                if (vm.Provider.ToLower() == "facebook")
                {
                    param = "access_token";
                }
                else
                {
                    param = "id_token";
                }
                var jsonToPost = new JObject(new JProperty(param, vm.Token));

                var contentToPost = new StringContent(JsonConvert.SerializeObject(jsonToPost), Encoding.UTF8, "application/json");
                string uri = ".auth/login/" + vm.Provider;
                var asyncResult = await httpClient.PostAsync(uri, contentToPost);
                if (asyncResult.IsSuccessStatusCode)
                {
                    var resultContentAsString = await asyncResult.Content.ReadAsStringAsync();
                    dynamic jObj = (JObject)JsonConvert.DeserializeObject(resultContentAsString);
                    string apiToken = jObj["authenticationToken"];

                    return Ok(new OperationResult { Result = Result.Success, Message = "User is authorized", ReturnedData = apiToken });
                }
                else
                {
                    return Ok(new OperationResult { Result = Result.UDError, Message = "User is not authorized", ReturnedData = null });
                }

            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, new OperationResult { Result = Result.UDError, Message = "User is Unauthorized", ReturnedData = null }));
            }
        }

        #endregion
    }

    #region LoginViewModel
    public class LoginViewModel
    {
        public string Token { get; set; }
        public string Provider { get; set; }
        public string UserID { get; set; }
        public int UserTypeID { get; set; }
    }
    #endregion
}

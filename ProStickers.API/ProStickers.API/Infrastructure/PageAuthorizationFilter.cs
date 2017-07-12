using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using ProStickers.BL.Security;
using ProStickers.ViewModel.Security;
using ProStickers.ViewModel.Core;

namespace ProStickers.API.Infrastructure
{
    public class PageAuthorizationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            UserSession _currentUser = new UserSession();

            string checkpath = actionContext.Request.RequestUri.AbsolutePath.Split('/')[2];
            string apiroute = actionContext.Request.RequestUri.AbsolutePath;

            if (apiroute.ToLower() != "/customer/customer/customersession" && apiroute.ToLower() != "/customer/customer/acceptsigninpolicy" && apiroute.ToLower() != "/customer/customer/default")
            {
                if (WebApiApplication.IsDevEnv)
                {
                    var viewModel = UserAuthenticationBL.LoadUserSessionAsync("bhgupta05@gmail.com", 1).Result;
                    _currentUser = viewModel;
                }
                else
                {
                    var session = actionContext.Request.Headers.GetValues("usersession");
                    if (session.FirstOrDefault() != null)
                    {
                        _currentUser = JsonConvert.DeserializeObject<UserSession>(session.FirstOrDefault()) as UserSession;
                    }
                }

                string path = "/";
                path = path + actionContext.Request.RequestUri.AbsolutePath.Split('/')[1] + "/" + actionContext.Request.RequestUri.AbsolutePath.Split('/')[2];

                if (!(WebApiApplication.list != null && WebApiApplication.list.Count > 0 && WebApiApplication.list.Where(x => x.Value == path && x.Text == _currentUser.UserTypeID.ToString()).Count() > 0))
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new OperationResult { Result = Result.UDError, Message = "User is unauthorized.Please login again.", ReturnedData = null });
                }
            }
        }
    }
}

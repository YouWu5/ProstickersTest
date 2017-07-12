using Newtonsoft.Json;
using ProStickers.BL.Infrastructure;
using ProStickers.ViewModel.Security;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace ProStickers.API.Infrastructure
{
    public class NotImplExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            string controllerName = context.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
            var json = JsonConvert.SerializeObject(context.ActionContext.ActionArguments);
            string actionName = context.ActionContext.ActionDescriptor.ActionName;
            string requestURL = context.Request.RequestUri.ToString();
            if (WebApiApplication.IsDevEnv == false)
            {
                if (context.Request.Headers.Contains("usersession"))
                {
                    var session = context.Request.Headers.GetValues("usersession");
                    if (session.FirstOrDefault() != null)
                    {
                        UserSession user = JsonConvert.DeserializeObject<UserSession>(session.FirstOrDefault()) as UserSession;
                        ExceptionUtility.InsertAPIException(controllerName, controllerName, actionName, requestURL, context.Exception, json, user.UserID);
                    }
                }
            }
            else
            {
                ExceptionUtility.InsertAPIException(controllerName, controllerName, actionName, requestURL, context.Exception, json, "admin@gmail.com");
            }
            context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }

    }
}
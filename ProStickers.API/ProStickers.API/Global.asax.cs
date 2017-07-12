using System.Collections.Generic;
using System.Web.Http;
using FluentValidation.WebApi;
using Newtonsoft.Json;
using ProStickers.API.Infrastructure;
using ProStickers.BL.Security;
using ProStickers.ViewModel.Core;

namespace ProStickers.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static bool IsDevEnv = false;
        public static List<ListItem> list = null;
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FluentValidationModelValidatorProvider.Configure(GlobalConfiguration.Configuration);
            GlobalConfiguration.Configuration.Filters.Add(new NotImplExceptionFilterAttribute());
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings =
            new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Unspecified,
            };
            list = UserAuthenticationBL.GetPageList();
        }
    }
}

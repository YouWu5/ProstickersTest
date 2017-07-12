using System.Linq;
using System.Web.Http;
using System.Net;
using System.Web.Http.Controllers;
using Newtonsoft.Json;
using ProStickers.API.Infrastructure;
using ProStickers.BL.Security;
using ProStickers.ViewModel.Security;

namespace ProStickers.API.Controllers
{
    [PageAuthorizationFilter]
    [Authorize]
    public class BaseController : ApiController
    {
        #region Fields & Property

        private UserInfo _userInfo;
        private UserSession _currentUser;

        protected UserSession CurrentUser { get { return _currentUser; } }

        protected UserInfo CurrentUserInfo
        {
            get
            {
                if (_currentUser != null && _userInfo == null)
                {
                    _userInfo = new UserInfo(_currentUser.UserID, _currentUser.Name);
                }
                return _userInfo;
            }
        }
        #endregion

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            string path = controllerContext.Request.RequestUri.AbsolutePath;

            if (!WebApiApplication.IsDevEnv)
            {
                if (controllerContext.Request.Headers.Contains("usersession"))
                {
                    var session = controllerContext.Request.Headers.GetValues("usersession");
                    if (session.FirstOrDefault() != null)
                    {
                        _currentUser = JsonConvert.DeserializeObject<UserSession>(session.FirstOrDefault()) as UserSession;
                    }
                }
                else if (path.ToLower() == "/customer/customer/customersession" || path.ToLower() == "/customer/customer/acceptsigninpolicy" || path.ToLower() == "/customer/customer/default")
                {

                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.Gone);
                }
            }
            else
            {
                var viewModel = UserAuthenticationBL.LoadUserSessionAsync("AbhisheckGupta@gmail.com", 1).Result;
                _currentUser = viewModel;
            }
        }
    }
}
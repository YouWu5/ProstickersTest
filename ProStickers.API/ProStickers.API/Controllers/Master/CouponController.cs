using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ProStickers.API.Infrastructure;
using ProStickers.BL.Master;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Master;

namespace ProStickers.API.Controllers.Master
{
    [RoutePrefix("Master/User")]
    public class UserController : BaseController
    {
        #region Get UserList

        [HttpGet]
        [Route("GetList")]
        public async Task<IHttpActionResult> GetListAsync()
        {
            return Ok(await UserBL.GetListAsync<UserListViewModel>(CurrentUserInfo.UserID));
        }

        #endregion

        #region Post UserList
        [HttpPost]
        [Route("List")]
        public async Task<IHttpActionResult> GetListAsync(Pager<UserListViewModel> pagerVM)
        {
            OperationResult result = await UserBL.GetListAsync(CurrentUserInfo.UserID, pagerVM);
            if (result.Result != Result.Success)
            {
                return ResponseMessage(Request.CreateResponse<OperationResult>(HttpStatusCode.PreconditionFailed, result));
            }
            else
            {
                return Ok(result);
            }
        }

        #endregion

        #region GetByID

        [HttpGet]
        [Route("{userID}/GetByID")]
        public async Task<IHttpActionResult> GetByIDAsync(string userID)
        {
            OperationResult result = await UserBL.GetByIDAsync(CurrentUserInfo.UserID, userID);
            if (result.Result != Result.Success)
            {
                return ResponseMessage(Request.CreateResponse<OperationResult>(HttpStatusCode.PreconditionFailed, result));
            }
            else
            {
                return Ok(result);
            }
        }

        #endregion

        #region Create 
        [ValidateFilter]
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> CreateAsync(UserViewModel userVM)
        {
            OperationResult result = await UserBL.CreateAsync(userVM, CurrentUserInfo);
            if (result.Result != Result.Success)
            {
                return ResponseMessage(Request.CreateResponse<OperationResult>(HttpStatusCode.PreconditionFailed, result));
            }
            else
            {
                return Ok(result);
            }
        }

        #endregion

        #region Update 
        [ValidateFilter]
        [HttpPut]
        [Route("")]
        public async Task<IHttpActionResult> UpdateAsync(UserViewModel userVM)
        {
            OperationResult result = await UserBL.UpdateAsync(userVM, CurrentUserInfo);
            if (result.Result != Result.Success)
            {
                return ResponseMessage(Request.CreateResponse<OperationResult>(HttpStatusCode.PreconditionFailed, result));
            }
            else
            {
                return Ok(result);
            }
        }

        #endregion

        #region Get UserTypeList
        [HttpGet]
        [Route("UserTypeList")]
        public async Task<IHttpActionResult> GetUserTypeListAsync()
        {
            return Ok(await UserBL.GetUserTypeListAsync());
        }
        #endregion

        #region Get DefaultViewModel
        [HttpGet]
        [Route("Default")]
        public async Task<IHttpActionResult> GetDefaultViewModelAsync()
        {
            return Ok(await UserBL.GetDefaultViewModelAsync());
        }
        #endregion

        #region Delete 
        [ValidateFilter]
        [HttpPut]
        [Route("Inactive")]
        public async Task<IHttpActionResult> InactiveAsync(UserStatusChangeViewModel userVM)
        {
            OperationResult result = await UserBL.InactiveAsync(CurrentUserInfo, userVM);
            if (result.Result != Result.Success)
            {
                return ResponseMessage(Request.CreateResponse<OperationResult>(HttpStatusCode.PreconditionFailed, result));
            }
            else
            {
                return Ok(result);
            }
        }

        #endregion
    }
}
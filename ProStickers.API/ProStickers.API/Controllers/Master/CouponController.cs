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
    [RoutePrefix("Master/Coupon")]
    public class CouponController : BaseController
    {
        #region Get CouponList

        [HttpGet]
        [Route("GetList")]
        public async Task<IHttpActionResult> GetListAsync()
        {
            return Ok(await CouponBL.GetListAsync<CouponListViewModel>(CurrentCouponInfo.CouponID));
        }

        #endregion

        #region Post CouponList
        [HttpPost]
        [Route("List")]
        public async Task<IHttpActionResult> GetListAsync(Pager<CouponListViewModel> pagerVM)
        {
            OperationResult result = await CouponBL.GetListAsync(CurrentCouponInfo.CouponID, pagerVM);
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
        [Route("{couponID}/GetByID")]
        public async Task<IHttpActionResult> GetByIDAsync(string couponID)
        {
            OperationResult result = await CouponBL.GetByIDAsync(CurrentCouponInfo.CouponID, couponID);
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
        public async Task<IHttpActionResult> CreateAsync(CouponViewModel couponVM)
        {
            OperationResult result = await CouponBL.CreateAsync(couponVM, CurrentCouponInfo);
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
        public async Task<IHttpActionResult> UpdateAsync(CouponViewModel couponVM)
        {
            OperationResult result = await CouponBL.UpdateAsync(couponVM, CurrentCouponInfo);
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

        #region Get CouponTypeList
        [HttpGet]
        [Route("CouponTypeList")]
        public async Task<IHttpActionResult> GetCouponTypeListAsync()
        {
            return Ok(await CouponBL.GetCouponTypeListAsync());
        }
        #endregion

        #region Get DefaultViewModel
        [HttpGet]
        [Route("Default")]
        public async Task<IHttpActionResult> GetDefaultViewModelAsync()
        {
            return Ok(await CouponBL.GetDefaultViewModelAsync());
        }
        #endregion

        #region Delete 
        [ValidateFilter]
        [HttpPut]
        [Route("Inactive")]
        public async Task<IHttpActionResult> InactiveAsync(CouponStatusChangeViewModel couponVM)
        {
            OperationResult result = await CouponBL.InactiveAsync(CurrentCouponInfo, couponVM);
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

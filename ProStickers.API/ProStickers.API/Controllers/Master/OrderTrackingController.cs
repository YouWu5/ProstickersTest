using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ProStickers.BL.Master;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using ProStickers.API.Infrastructure;

namespace ProStickers.API.Controllers.Master
{
    [RoutePrefix("Master/OrderTracking")]
    public class OrderTrackingController : BaseController
    {
        #region Get OrderList

        [HttpGet]
        [Route("GetList")]
        public async Task<IHttpActionResult> GetListAsync()
        {
            return Ok(await OrderTrackingBL.GetListAsync<OrderTrackingViewModel>());
        }

        #endregion

        #region Post OrderList
        [HttpPost]
        [Route("List")]
        public async Task<IHttpActionResult> GetListAsync(Pager<OrderTrackingViewModel> pagerVM)
        {
            OperationResult result = await OrderTrackingBL.GetListAsync(pagerVM);
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
        [Route("{orderNo}/GetByID")]
        public async Task<IHttpActionResult> GetByIDAsync(int orderNo)
        {
            OperationResult result = await OrderTrackingBL.GetByIDAsync(orderNo);
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

        #region Update TrackingNumber

        [HttpPut]
        [Route("")]
        [ValidateFilter]
        public async Task<IHttpActionResult> UpdateTrackingNumberAsync(OrderTrackingViewModel orderVM)
        {
            OperationResult result = await OrderTrackingBL.UpdateTrackingNumberAsync(CurrentUserInfo, orderVM);
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

        #region Get StatusList

        [HttpGet]
        [Route("GetStatusList")]
        public async Task<IHttpActionResult> GetStatusListAsync()
        {
            return Ok(await OrderTrackingBL.GetStatusListAsync());
        }
        #endregion

        #region Get CustomerNameList
        [HttpGet]
        [Route("{codeName}/GetCustomerNameList")]
        public async Task<IHttpActionResult> GetCustomerNameListAsync(string codeName)
        {
            return Ok(await OrderTrackingBL.GetCustomerNameListAsync(codeName));
        }

        #endregion

        #region Download  vector file

        [HttpGet]
        [Route("{designNumber}/Download")]
        public async Task<IHttpActionResult> DownloadVectorFileAsyncAsync(string designNumber)
        {
            OperationResult result = await OrderTrackingBL.DownloadVectorFileAsync(CurrentUserInfo.UserID , designNumber);
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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ProStickers.BL.Master;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Master;

namespace ProStickers.API.Controllers.Customer
{
    [RoutePrefix("Customer/CustomerFeedback")]
    public class CustomerFeedbackController:BaseController
    {
        #region Create CustomerFeedback
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> CreateCustomerFeedbackAsync(FeedbackViewModel feedbackVM)
        {
            OperationResult result = await FeedbackBL.CreateCustomerFeedbackAsync(CurrentUserInfo, feedbackVM);
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

        #region Get OrderDetail
        [HttpGet]
        [Route("{orderNo}/GetByID")]
        public async Task<IHttpActionResult> GetOrderByIDAsync(int orderNo)
        {
            OperationResult result = await FeedbackBL.GetOrderByIDAsync(CurrentUserInfo, orderNo);
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
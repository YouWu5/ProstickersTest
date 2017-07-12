using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ProStickers.BL.Master;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Master;

namespace ProStickers.API.Controllers.Master
{
    [RoutePrefix("Master/Feedback")]
    public class FeedbackController : BaseController
    {
        #region Create MasterReply
        [HttpPut]
        [Route("")]
        public async Task<IHttpActionResult> CreateMasterReplyAsync(FeedbackViewModel feedbackVM)
        {
            OperationResult result = await FeedbackBL.CreateMasterReplyAsync(CurrentUserInfo, feedbackVM);
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

        #region Post FeedbackList
        [HttpPost]
        [Route("List")]
        public async Task<IHttpActionResult> GetListAsync(Pager<FeedbackListViewModel> pagerVM)
        {
            OperationResult result = await FeedbackBL.GetListAsync(pagerVM);
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

        #region Get FeedbackList
        [HttpGet]
        [Route("GetList")]
        public async Task<IHttpActionResult> GetListAsync()
        {
            return Ok(await FeedbackBL.GetListAsync<FeedbackListViewModel>());
        }

        #endregion

        #region GetByID
        [HttpGet]
        [Route("{customerID}/{designNo}/GetByID")]
        public async Task<IHttpActionResult> GetByIDAsync(string customerID, string designNo)
        {
            return Ok(await FeedbackBL.GetByIDAsync(CurrentUserInfo.UserName, customerID, designNo));
        }

        #endregion

        #region Delete Feedback
        [HttpDelete]
        [Route("{customerID}/{designNo}/DeleteFeedback")]
        public async Task<IHttpActionResult> DeleteAsync(string customerID, string designNo)
        {
            OperationResult result = await FeedbackBL.DeleteAsync(CurrentUserInfo, customerID, designNo);
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

        #region Get LandingPageFeedbackList
        [HttpGet]
        [Route("GetFeedbackList")]
        public async Task<IHttpActionResult> GetFeedbackListAsync()
        {
            return Ok(await FeedbackBL.GetFeedbackListAsync<FeedbackListViewModel>());
        }

        #endregion

        #region Get CustomerList
        [HttpGet]
        [Route("{codeName}/GetCustomerList")]
        public async Task<IHttpActionResult> GetCustomerListAsync(string codeName)
        {
            return Ok(await FeedbackBL.GetCustomerListAsync(codeName));
        }

        #endregion
    }
}
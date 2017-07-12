using ProStickers.BL;
using ProStickers.ViewModel;
using ProStickers.ViewModel.Core;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ProStickers.API.Controllers
{
    [RoutePrefix("DesignerProfile")]
    public class DesignerProfileController : BaseController
    {
        #region Get Designer's List

        [HttpGet]
        [Route("GetList")]
        public async Task<IHttpActionResult> GetListAsync()
        {
            OperationResult result = await DesignerProfileBL.GetListAsync();
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

        #region Get List
        [HttpGet]
        [Route("GetDesignerFeedbackListList")]
        public async Task<IHttpActionResult> GetDesignerFeedbackListAsync()
        {
            return Ok(await DesignerProfileBL.GetDesignerFeedbackListAsync<DesignerFeedbackListViewModel>(CurrentUserInfo.UserID));
        }
        #endregion

        #region Post List
        [HttpPost]
        [Route("List")]
        public async Task<IHttpActionResult> GetListAsync(Pager<DesignerFeedbackListViewModel> pagerVM)
        {
            OperationResult result = await DesignerProfileBL.GetDesignerFeedbackListAsync<DesignerFeedbackListViewModel>(pagerVM, CurrentUserInfo.UserID);
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
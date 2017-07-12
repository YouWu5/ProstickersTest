using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net;
using ProStickers.BL;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Master;

namespace ProStickers.API.Controllers
{
    [RoutePrefix("Report/Report")]
    public class ReportController : BaseController
    {
        #region Get List
        [HttpGet]
        [Route("GetList")]
        public async Task<IHttpActionResult> GetListAsync()
        {
            return Ok(await ReportBL.GetListAsync<SalesReportViewModel>());
        }
        #endregion

        #region Post List
        [HttpPost]
        [Route("List")]
        public async Task<IHttpActionResult> GetListAsync(Pager<SalesReportViewModel> pagerVM)
        {
            OperationResult result = await ReportBL.GetListAsync<SalesReportViewModel>(pagerVM);
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
        [Route("DesignerList")]
        public async Task<IHttpActionResult> GetDesignerListAsync()
        {
            return Ok(await ReportBL.GetDesignerListAsync());
        }
        #endregion
    }
}
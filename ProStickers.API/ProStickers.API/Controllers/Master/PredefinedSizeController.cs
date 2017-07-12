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
    [RoutePrefix("Master/PredefinedSize")]
    public class PredefinedSizeController : BaseController
    {
        #region Get By ID

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetByIDAsync()
        {
            OperationResult result = await PredefinedSizeBL.GetByIDAsync();
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
        public async Task<IHttpActionResult> UpdateAsync(PredefinedSizeViewModel sizeVM)
        {
            OperationResult result = await PredefinedSizeBL.UpdateAsync(sizeVM, CurrentUserInfo);
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

        #region Get DefaultViewModel
        [HttpGet]
        [Route("Default")]
        public async Task<IHttpActionResult> GetDefaultViewModelAsync()
        {
            return Ok(await PredefinedSizeBL.GetDefaultViewModelAsync());
        }
        #endregion
    }
}
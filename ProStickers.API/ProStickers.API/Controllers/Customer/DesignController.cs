using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ProStickers.BL;
using ProStickers.BL.Customer;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;

namespace ProStickers.API.Controllers.Customer
{
    [RoutePrefix("Customer/Design")]
    public class DesignController : BaseController
    {
        #region Get DesignNumberList

        [HttpGet]
        [Route("GetList")]
        public async Task<IHttpActionResult> GetListAsync()
        {
            return Ok(await DesignBL.GetListAsync<DesignViewModel>(CurrentUserInfo.UserID));
        }

        #endregion

        #region Post DesignNumberList
        [HttpPost]
        [Route("List")]
        public async Task<IHttpActionResult> GetListAsync(Pager<DesignViewModel> pagerVM)
        {
            OperationResult result = await DesignBL.GetListAsync(pagerVM , CurrentUserInfo.UserID);
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

        #region Get DesignByID
        [HttpGet]
        [Route("{designNo}/GetByID")]
        public async Task<IHttpActionResult> GetByIDAsync(string designNo)
        {
            OperationResult result = await DesignBL.GetByIDAsync(CurrentUserInfo.UserID, designNo);
            if (result.Result != Result.Success)
            {
                return ResponseMessage(Request.CreateResponse<OperationResult>(HttpStatusCode.PreconditionFailed, result));
            }
            return Ok(result);
        }

        #endregion

        #region Delete Design
        [HttpPut]
        [Route("{designNo}/{updatedTS}/DeleteDesign")]
        public async Task<IHttpActionResult> DeleteAsync(string designNo, DateTime updatedTS)
        {
            OperationResult result = await DesignBL.DeleteAsync(CurrentUserInfo.UserID, designNo, updatedTS);
            if (result.Result != Result.Success)
            {
                return ResponseMessage(Request.CreateResponse<OperationResult>(HttpStatusCode.PreconditionFailed, result));
            }
            return Ok(result);
        }

        #endregion

        #region Download VectorDesign
        [HttpGet]
        [Route("{designNumber}/DownloadVectorFile")]
        public async Task<IHttpActionResult> DownLoadVectorFileAsync(string designNumber)
        {
            return Ok(await CommonBL.DownloadVectorFileAsync(CurrentUserInfo.UserID, designNumber));
        }

        #endregion
    }
}
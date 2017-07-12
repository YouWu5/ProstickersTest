using ProStickers.API.Infrastructure;
using ProStickers.BL.Master;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Master;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ProStickers.API.Controllers.Master
{
    [RoutePrefix("Master/ColorChart")]
    public class ColorChartController : BaseController
    {
        #region Get List
        [HttpGet]
        [Route("List")]
        public async Task<IHttpActionResult> GetListAsync()
        {
            return Ok(await ColorChartBL.GetListAsync<ColorChartListViewModel>());
        }
        #endregion

        #region Post List
        [HttpPost]
        [Route("List")]
        public async Task<IHttpActionResult> GetListAsync(Pager<ColorChartListViewModel> pagerVM)
        {
            OperationResult result = await ColorChartBL.GetListAsync(pagerVM);
            if (result.Result != Result.Success)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.PreconditionFailed, result));
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
            return Ok(await ColorChartBL.GetDefaultViewModelAsync());
        }
        #endregion

        #region Get ByID
        [HttpGet]
        [Route("{colorID}")]
        public async Task<IHttpActionResult> GetByIDAsync(string colorID)
        {
            OperationResult result = await ColorChartBL.GetByIDAsync(colorID, CurrentUserInfo);
            if (result.Result != Result.Success)
            {
                return ResponseMessage(Request.CreateResponse<OperationResult>(HttpStatusCode.PreconditionFailed, result));
            }
            else
            {
                ColorChartViewModel viewModel = result.ReturnedData as ColorChartViewModel;
                return Ok(viewModel);
            }
        }
        #endregion

        #region Post CreateColor
        [HttpPost]
        [Route("")]
        [ValidateFilter]
        public async Task<IHttpActionResult> CreateAsync(ColorChartViewModel vm)
        {
            OperationResult result = await ColorChartBL.CreateAsync(vm, CurrentUserInfo);
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

        #region Put UpdateColor
        [HttpPut]
        [Route("")]
        [ValidateFilter]
        public async Task<IHttpActionResult> UpdateAsync(ColorChartViewModel vm)
        {
            OperationResult result = await ColorChartBL.UpdateAsync(vm, CurrentUserInfo);
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

        #region Delete Color
        [HttpDelete]
        [Route("{colorID}/{name}/{updatedTS}/DeleteColor")]
        public async Task<IHttpActionResult> DeleteAsync(string colorID, string name, DateTime updatedTS)
        {
            OperationResult result = await ColorChartBL.DeleteAsync(colorID, name, updatedTS, CurrentUserInfo);
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
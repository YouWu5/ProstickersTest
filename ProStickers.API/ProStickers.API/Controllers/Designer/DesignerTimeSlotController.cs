using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using ProStickers.BL.Designer;
using ProStickers.ViewModel.Designer;
using ProStickers.ViewModel.Core;

namespace ProStickers.API.Controllers.Designer
{
    [RoutePrefix("Designer/DesignerTimeSlot")]
    public class DesignerTimeSlotController : BaseController
    {
        #region Get List
        [HttpGet]
        [Route("{date}/{isAllTimeSlots}/GetList")]
        public async Task<IHttpActionResult> GetListAsync(DateTime date, bool isAllTimeSlots)
        {
            return Ok(await DesignerTimeSlotBL.GetListAsync(CurrentUserInfo, date, isAllTimeSlots));
        }
        #endregion

        #region Post Create
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> CreateAsync(DesignerTimeSlotViewModel vm)
        {
            OperationResult result = await DesignerTimeSlotBL.CreateAsync(CurrentUserInfo, vm);
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
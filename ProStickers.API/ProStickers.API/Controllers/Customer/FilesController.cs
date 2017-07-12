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
    [RoutePrefix("Customer/Files")]
    public class FilesController : BaseController
    {
        #region Upload File

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> UploadFileAsync(FilesViewModel fileVM)
        {
            OperationResult result = await FilesBL.UploadFileAsync(CurrentUserInfo, fileVM);
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

        #region Download File

        [HttpGet]
        [Route("{fileNumber}/DownloadFile")]
        public async Task<IHttpActionResult> DownloadFileAsync(string fileNumber)
        {
            return Ok(await CommonBL.DownloadCustomerFileAsync(CurrentUserInfo.UserID, fileNumber));
        }

        #endregion

        #region Delete File

        [HttpDelete]
        [Route("{fileNumber}/DeleteFile")]
        public async Task<IHttpActionResult> DeleteFileAsync(string fileNumber)
        {
            OperationResult result = await FilesBL.DeleteFileAsync(CurrentUserInfo.UserID, fileNumber);
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

        #region Create

        [HttpPost]
        [Route("Create")]
        public async Task<IHttpActionResult> CreateAsync(FilesViewModel filesVM)
        {
            OperationResult result = await FilesBL.CreateAsync(CurrentUserInfo.UserID, filesVM);
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

        #region Get FilesList

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetListAsync()
        {
            return Ok(await FilesBL.GetListAsync(CurrentUserInfo.UserID));
        }

        #endregion

        #region Get DefaultViewModel
        [HttpGet]
        [Route("Default")]
        public async Task<IHttpActionResult> GetDefaultViewModelAsync()
        {
            return Ok(await FilesBL.GetDefaultViewModelAsync(CurrentUserInfo));
        }
        #endregion
    }
}
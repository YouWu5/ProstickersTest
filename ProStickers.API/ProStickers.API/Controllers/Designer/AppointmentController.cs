using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using ProStickers.BL.Designer;
using ProStickers.ViewModel.Designer;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using ProStickers.BL;
using ProStickers.API.Infrastructure;

namespace ProStickers.API.Controllers.Designer
{
    [RoutePrefix("Designer/Appointment")]
    public class AppointmentController : BaseController
    {
        #region Get AppointmentRequestCount
        [HttpGet]
        [Route("AppointmentRequestCount")]
        public async Task<IHttpActionResult> GetAppointmentRequestCountAsync()
        {
            return Ok(await AppointmentBL.GetAppointmentRequestCountAsync(CurrentUserInfo));
        }
        #endregion

        #region Get AppointmentRequest
        [HttpGet]
        [Route("AppointmentRequest")]
        public async Task<IHttpActionResult> GetAppointmentRequestAsync()
        {
            return Ok(await AppointmentBL.GetAppointmentRequestAsync());
        }
        #endregion

        #region Post Cancel
        [HttpPost]
        [Route("CancelAppointmentRequest")]
        public async Task<IHttpActionResult> CancelAppointmentRequestAsync(AppointmentRequestViewModel vm)
        {
            OperationResult result = await AppointmentBL.CancelAppointmentRequestAsync(CurrentUserInfo, vm);
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

        #region Post Create
        [HttpPost]
        [Route("")]
        [ValidateFilter]
        public async Task<IHttpActionResult> CreateAsync(AppointmentRequestPickViewModel vm)
        {
            OperationResult result = await AppointmentBL.CreateAsync(CurrentUserInfo, vm);
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
        [Route("GetList")]
        public async Task<IHttpActionResult> GetListAsync()
        {
            return Ok(await AppointmentBL.GetListAsync<AppointmentListViewModel>(CurrentUserInfo.UserID));
        }
        #endregion

        #region Get LandingPage Appointment List
        [HttpGet]
        [Route("GetAppointmentList")]
        public async Task<IHttpActionResult> GetAppointmentListAsync()
        {
            return Ok(await AppointmentBL.GetAppointmentListAsync<AppointmentListViewModel>(CurrentUserInfo.UserID));
        }
        #endregion

        #region Post List
        [HttpPost]
        [Route("List")]
        public async Task<IHttpActionResult> GetListAsync(Pager<AppointmentListViewModel> pagerVM)
        {
            OperationResult result = await AppointmentBL.GetListAsync<AppointmentListViewModel>(pagerVM, CurrentUserInfo.UserID);
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

        #region Get ByID
        [HttpGet]
        [Route("{appointmentNumber}/{date}")]
        public async Task<IHttpActionResult> GetByIDAsync(string appointmentNumber, DateTime date)
        {
            OperationResult result = await AppointmentBL.GetByIDAsync(CurrentUserInfo, appointmentNumber, date);
            if (result.Result != Result.Success)
            {
                return ResponseMessage(Request.CreateResponse<OperationResult>(HttpStatusCode.PreconditionFailed, result));
            }
            else
            {
                AppointmentViewModel viewModel = result.ReturnedData as AppointmentViewModel;
                return Ok(viewModel);
            }
        }
        #endregion

        #region Put Update
        [HttpPut]
        [Route("")]
        [ValidateFilter]
        public async Task<IHttpActionResult> UpdateAsync(AppointmentViewModel vm)
        {
            OperationResult result = await AppointmentBL.UpdateAsync(CurrentUserInfo, vm);
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

        #region Put UpdateAppointentStatus
        [HttpPut]
        [Route("UpdateAppointmentStatus")]
        [ValidateFilter]
        public async Task<IHttpActionResult> UpdateAppointmentStatusAsync(AppointmentViewModel vm)
        {
            OperationResult result = await AppointmentBL.UpdateAppointmentStatusAsync(CurrentUserInfo, vm);
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

        #region Post UploadUserFile
        [HttpPost]
        [Route("UploadUserFile")]
        public async Task<IHttpActionResult> UploadUserFileAsync(UserFilesViewModel dAppFileVM)
        {
            OperationResult result = await AppointmentBL.UploadUserFileAsync(CurrentUserInfo, dAppFileVM);
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

        #region Get DownloadCustomerFile
        [HttpGet]
        [Route("{fileNumber}/DownloadFile")]
        public async Task<IHttpActionResult> DownloadCustomerFileAsync(string fileNumber)
        {
            return Ok(await CommonBL.DownloadCustomerFileAsync(CurrentUserInfo.UserID, fileNumber));
        }
        #endregion

        #region Get DownloadDesignerAppointmentFile
        [HttpGet]
        [Route("{designerAppointmentFileNumber}/DownloadDesignerAppointmentFile")]
        public async Task<IHttpActionResult> DownloadDesignerAppointmentFileAsync(string designerAppointmentFileNumber)
        {
            return Ok(await CommonBL.DownloadUserFileAsync(CurrentUserInfo.UserID, designerAppointmentFileNumber));
        }
        #endregion

        #region Get DownloadDesignImage
        [HttpGet]
        [Route("{designNumber}/DownloadDesignImage")]
        public async Task<IHttpActionResult> DownloadDesignImageAsync(string designNumber)
        {
            return Ok(await CommonBL.DownloadDesignImageAsync(CurrentUserInfo.UserID, designNumber));
        }
        #endregion

        #region Get DownloadVectorFile
        [HttpGet]
        [Route("{designNumber}/DownloadVectorFile")]
        public async Task<IHttpActionResult> DownloadVectorFileAsync(string designNumber)
        {
            return Ok(await CommonBL.DownloadVectorFileAsync(CurrentUserInfo.UserID, designNumber));
        }
        #endregion

        #region Get DesignerNote
        [HttpGet]
        [Route("{designNumber}/{userID}/DesignerNote")]
        public async Task<IHttpActionResult> GetDesignerNoteAsync(string designNumber, string userID)
        {
            return Ok(await AppointmentBL.GetDesignerNoteAsync(designNumber, userID));
        }
        #endregion

        #region Update DesignerNote
        [HttpPut]
        [Route("UpdateDesignerNote")]
        [ValidateFilter]
        public async Task<IHttpActionResult> UpdateDesignerNoteAsync(CustomerDesignViewModel customerDVM)
        {
            OperationResult result = await AppointmentBL.UpdateDesignerNoteAsync(CurrentUserInfo.UserID, customerDVM);
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

        #region Helper Methods

        #region Get AppointmentSlotList
        [HttpGet]
        [Route("{date}/AppointmentSlotList")]
        public async Task<IHttpActionResult> GetAppointmentSlotListAsync(DateTime date)
        {
            return Ok(await AppointmentBL.GetAppointmentSlotListAsync(CurrentUserInfo, date));
        }
        #endregion

        #region Get DateList
        [HttpGet]
        [Route("DateList")]
        public async Task<IHttpActionResult> GetDateListAsync()
        {
            return Ok(await CommonBL.GetDateListAsync());
        }
        #endregion

        #region Get StatusList
        [HttpGet]
        [Route("StatusList")]
        public async Task<IHttpActionResult> GetStatusListAsync()
        {
            return Ok(await AppointmentBL.GetStatusListAsync());
        }
        #endregion

        #endregion
    }
}
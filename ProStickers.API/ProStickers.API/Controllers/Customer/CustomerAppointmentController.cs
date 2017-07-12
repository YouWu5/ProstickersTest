using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ProStickers.BL.Customer;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using System;
using ProStickers.API.Infrastructure;

namespace ProStickers.API.Controllers.Customer
{
    [RoutePrefix("Customer/CustomerAppointment")]
    public class CustomerAppointmentController : BaseController
    {
        #region Post CreateCallRequest
        [HttpPost]
        [Route("CallRequestCreate")]
        public async Task<IHttpActionResult> CreateCustomerCallRequestAsync(AppointmentRequestViewModel vm)
        {
            OperationResult result = await CustomerAppointmentBL.CreateCustomerCallRequestAsync(CurrentUserInfo, vm);
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

        #region Get CustomerAppointmentList

        [HttpGet]
        [Route("GetList")]
        public async Task<IHttpActionResult> GetListAsync()
        {
            return Ok(await CustomerAppointmentBL.GetListAsync<CustomerAppointmentListViewModel>(CurrentUserInfo.UserID));
        }

        #endregion

        #region Post CustomerAppointmentList
        [HttpPost]
        [Route("List")]
        public async Task<IHttpActionResult> GetListAsync(Pager<CustomerAppointmentListViewModel> pagerVM)
        {
            OperationResult result = await CustomerAppointmentBL.GetListAsync(pagerVM, CurrentUserInfo.UserID);
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

        #region GetByID
        [HttpGet]
        [Route("{appointmentNo}/GetByID")]
        public async Task<IHttpActionResult> GetByIDAsync(string appointmentNo)
        {
            OperationResult result = await CustomerAppointmentBL.GetByIDAsync(CurrentUserInfo.UserID, appointmentNo);
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

        #region Cancel Appointment
        [HttpPut]
        [Route("")]
        public async Task<IHttpActionResult> CancelAppointmentAsync(CustomerAppointmentViewModel appointmentVM)
        {
            OperationResult result = await CustomerAppointmentBL.CancelAppointmentAsync(CurrentUserInfo.UserID, CurrentUserInfo.UserName, appointmentVM);
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
            return Ok(await CustomerAppointmentBL.GetDefaultViewModelAsync());
        }
        #endregion

        #region Calendar Appointment Methods

        #region Get DefaultViewModel
        [HttpGet]
        [Route("GetDefault")]
        public async Task<IHttpActionResult> GetCalendarDefaultViewModelAsync()
        {
            return Ok(await CalendarAppointmentBL.GetDefaultViewModelAsync());
        }
        #endregion

        #region Get AvailableDesigner         
        [HttpGet]
        [Route("{date}/{timeslotID}")]
        public async Task<IHttpActionResult> GetAvailableDesignerAsync(DateTime date, int timeslotID)
        {
            return Ok(await CalendarAppointmentBL.GetAvailableDesignerAsync(date, timeslotID));
        }
        #endregion

        #region Get AvailableDesignerList         
        [HttpGet]
        [Route("{date}/{timeslotID}/AvailableDesignerList")]
        public async Task<IHttpActionResult> GetAvailableDesignerListAsync(DateTime date, int timeslotID)
        {
            return Ok(await CalendarAppointmentBL.GetAvailableDesignerListAsync(date, timeslotID));
        }
        #endregion

        #region Post CreateCalendarAppointment
        [HttpPost]
        [Route("")]
        [ValidateFilter]
        public async Task<IHttpActionResult> CreateAsync(CalendarAppointmentViewModel vm)
        {
            OperationResult result = await CalendarAppointmentBL.CreateAsync(vm, CurrentUserInfo);
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

        #region Get TimeSlotList        
        [HttpGet]
        [Route("{date}")]
        public async Task<IHttpActionResult> GetTimeSlotListAsync(DateTime date)
        {
            return Ok(await CalendarAppointmentBL.GetTimeSlotListAsync(date));
        }
        #endregion

        #endregion
    }
}
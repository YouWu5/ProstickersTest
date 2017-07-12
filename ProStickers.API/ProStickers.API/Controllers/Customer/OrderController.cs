using ProStickers.API.Infrastructure;
using ProStickers.BL;
using ProStickers.BL.Customer;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ProStickers.API.Controllers.Customer
{
    [RoutePrefix("Customer/Order")]
    public class OrderController : BaseController
    {
        #region Get Detail
        [HttpGet]
        [Route("{designNo}/{appointmentNo}/Detail")]
        public async Task<IHttpActionResult> GetDetailAsync(string designNo, string appointmentNo)
        {
            OperationResult result = await OrderBL.GetDetailAsync(designNo, appointmentNo, CurrentUserInfo);
            if (result.Result != Result.Success)
            {
                return ResponseMessage(Request.CreateResponse<OperationResult>(HttpStatusCode.PreconditionFailed, result));
            }
            else
            {
                OrderViewModel viewModel = result.ReturnedData as OrderViewModel;
                return Ok(viewModel);
            }
        }
        #endregion

        #region Post CreateOrder
        [HttpPost]
        [Route("")]
        [ValidateFilter]
        public async Task<IHttpActionResult> CreateAsync(OrderViewModel vm)
        {
            OperationResult result = await OrderBL.CreateAsync(vm, CurrentUserInfo);
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
            return Ok(await OrderBL.GetListAsync<OrderListViewModel>(CurrentUserInfo.UserID));
        }
        #endregion

        #region Post List
        [HttpPost]
        [Route("List")]
        public async Task<IHttpActionResult> GetListAsync(Pager<OrderListViewModel> pagerVM)
        {
            OperationResult result = await OrderBL.GetListAsync<OrderListViewModel>(pagerVM, CurrentUserInfo.UserID);
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

        #region Get By ID
        [HttpGet]
        [Route("{orderNo}/GetByID")]
        public async Task<IHttpActionResult> GetByIDAsync(int orderNo)
        {
            OperationResult result = await OrderBL.GetByIDAsync(CurrentUserInfo, orderNo);
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

        #region Get StateListByCountry          
        [HttpGet]
        [Route("{countryID}/StateList")]
        public async Task<IHttpActionResult> GetStateListByCountryAsync(int countryID)
        {
            return Ok(await CommonBL.GetStateListByCountryAsync(countryID));
        }
        #endregion

        #region Get CountryList        
        [HttpGet]
        [Route("CountryList")]
        public async Task<IHttpActionResult> GetCountryListAsync()
        {
            return Ok(await CommonBL.GetCountryListAsync());
        }
        #endregion

        #region Get MonthList        
        [HttpGet]
        [Route("MonthList")]
        public async Task<IHttpActionResult> GetMonthListAsync()
        {
            return Ok(await CommonBL.GetMonthListAsync());
        }
        #endregion

        #region Get YearList        
        [HttpGet]
        [Route("YearList")]
        public async Task<IHttpActionResult> GetYearListAsync()
        {
            return Ok(await CommonBL.GetYearListAsync());
        }
        #endregion     
    }
}
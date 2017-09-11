using ProStickers.API.Infrastructure;
using ProStickers.BL;
using ProStickers.BL.Customer;
using ProStickers.BL.Security;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ProStickers.API.Controllers.Customer
{
    [RoutePrefix("Customer/Customer")]
    public class CustomerController : BaseController
    {
        #region Get DefaultViewModel
        [HttpGet]
        [Route("Default")]
        public async Task<IHttpActionResult> GetDefaultViewModelAsync()
        {
            return Ok(await CustomerBL.GetDefaultViewModelAsync());
        }
        #endregion

        #region Get ByID
        [HttpGet]
        [Route("{customerID}")]
        public async Task<IHttpActionResult> GetByIDAsync(string customerID)
        {
            OperationResult result = await CustomerBL.GetByIDAsync(customerID, CurrentUserInfo);
            if (result.Result != Result.Success)
            {
                return ResponseMessage(Request.CreateResponse<OperationResult>(HttpStatusCode.PreconditionFailed, result));
            }
            else
            {
                CustomerViewModel viewModel = result.ReturnedData as CustomerViewModel;
                return Ok(viewModel);
            }
        }
        #endregion

        #region Get DetailList
        [HttpGet]
        [Route("DetailList")]
        public async Task<IHttpActionResult> GetDetailListAsync()
        {
            OperationResult result = await CustomerBL.GetDetailListAsync(CurrentUserInfo);
            if (result.Result != Result.Success)
            {
                return ResponseMessage(Request.CreateResponse<OperationResult>(HttpStatusCode.PreconditionFailed, result));
            }
            else
            {
                CustomerDetailListViewModel viewModel = result.ReturnedData as CustomerDetailListViewModel;
                return Ok(viewModel);
            }
        }
        #endregion

        #region Post CreateCustomer
        [HttpPost]
        [Route("")]
        [ValidateFilter]
        public async Task<IHttpActionResult> CreateAsync(CustomerViewModel vm)
        {
            OperationResult result = await CustomerBL.CreateAsync(vm, CurrentUserInfo);
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

        #region Put UpdateCustomer
        [HttpPut]
        [Route("")]
        [ValidateFilter]
        public async Task<IHttpActionResult> UpdateAsync(CustomerViewModel vm)
        {
            OperationResult result = await CustomerBL.UpdateAsync(vm, CurrentUserInfo);
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

        #region Helper Methods

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

        #region Post Create
        [HttpPost]
        [Route("CustomerSession")]
        public async Task<IHttpActionResult> CreateCustomerAsync(CustomerViewModel vm)
        {
            OperationResult result = await UserAuthenticationBL.CreateCustomerAsync(vm);
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

        #region Post AcceptSignInPolicy
        [HttpPost]
        [Route("AcceptSignInPolicy")]
        public IHttpActionResult AcceptSignInPolicy(ListItem vm)
        {
            OperationResult result = UserAuthenticationBL.AcceptSignInPolicy(vm.Text);
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

        #region Post HaveSkype
        [HttpPost]
        [Route("HaveSkype")]
        public IHttpActionResult HaveSkype(ListItem vm)
        {
            OperationResult result = UserAuthenticationBL.HaveSkype(vm.Text);
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

        #endregion      
    }
}
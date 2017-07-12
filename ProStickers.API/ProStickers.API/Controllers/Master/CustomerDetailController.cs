using ProStickers.API.Infrastructure;
using ProStickers.BL;
using ProStickers.BL.Customer;
using ProStickers.BL.Designer;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ProStickers.API.Controllers.Master
{
    [RoutePrefix("Master/CustomerDetail")]
    public class CustomerDetailController : BaseController
    {
        #region Get List
        [HttpGet]
        [Route("List")]
        public async Task<IHttpActionResult> GetListAsync()
        {
            return Ok(await CustomerBL.GetListAsync<CustomerListViewModel>());
        }
        #endregion

        #region Post List
        [HttpPost]
        [Route("List")]
        public async Task<IHttpActionResult> GetListAsync(Pager<CustomerListViewModel> pagerVM)
        {
            OperationResult result = await CustomerBL.GetListAsync(pagerVM);
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

        #region Get CustomerDetail
        [HttpGet]
        [Route("{customerID}")]
        public async Task<IHttpActionResult> GetCustomerDetailAsync(string customerID)
        {
            OperationResult result = await CustomerBL.GetCustomerDetailAsync(customerID, CurrentUserInfo);
            if (result.Result != Result.Success)
            {
                return ResponseMessage(Request.CreateResponse<OperationResult>(HttpStatusCode.PreconditionFailed, result));
            }
            else
            {
                CustomerDetailViewModel viewModel = result.ReturnedData as CustomerDetailViewModel;
                return Ok(viewModel);
            }
        }
        #endregion

        #region Post UploadFile
        [HttpPost]
        [Route("UploadFile")]
        public async Task<IHttpActionResult> UploadUserFileAsync(UserFilesViewModel userFileVM)
        {
            OperationResult result = await CustomerBL.UploadUserFileAsync(CurrentUserInfo, userFileVM);
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
        public async Task<IHttpActionResult> UpdateCustomerAsync(CustomerDetailViewModel vm)
        {
            OperationResult result = await CustomerBL.UpdateCustomerAsync(vm, CurrentUserInfo);
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

        #region Delete DeleteCustomer
        [HttpDelete]
        [Route("{customerID}/{updatedTS}/Delete")]
        public async Task<IHttpActionResult> DeleteAsync(string customerID, DateTime updatedTS)
        {
            OperationResult result = await CustomerBL.DeleteAsync(customerID, updatedTS, CurrentUserInfo);
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

        #region Get DownloadVectorFile
        [HttpGet]
        [Route("{designNumber}/DownloadVectorFile")]
        public async Task<IHttpActionResult> DownLoadVectorFileAsync(string designNumber)
        {
            return Ok(await CommonBL.DownloadVectorFileAsync(CurrentUserInfo.UserID, designNumber));
        }
        #endregion

        #region Get DownloadDesignImageFile
        [HttpGet]
        [Route("{designNumber}/DownloadDesignImageFile")]
        public async Task<IHttpActionResult> DownloadDesignImageAsync(string designNumber)
        {
            return Ok(await CustomerBL.DownloadDesignImageAsync(CurrentUserInfo.UserID, designNumber));
        }
        #endregion

        #region Get DownloadUserFileAsync
        [HttpGet]
        [Route("{fileNumber}/DownloadUserFile")]
        public async Task<IHttpActionResult> DownloadUserFileAsync(string fileNumber)
        {
            return Ok(await CommonBL.DownloadUserFileAsync(CurrentUserInfo.UserID, fileNumber));
        }

        #endregion

        #region Get DownloadCustomerFile
        [HttpGet]
        [Route("{fileNumber}/DownloadCustomerFile")]
        public async Task<IHttpActionResult> DownloadCustomerFileAsync(string fileNumber)
        {
            return Ok(await CommonBL.DownloadCustomerFileAsync(CurrentUserInfo.UserID, fileNumber));
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

        #region Get CustomerList        
        [HttpGet]
        [Route("{codeName}/CustomerList")]
        public async Task<IHttpActionResult> GetCustomerListAsync(string codeName)
        {
            return Ok(await CustomerBL.GetCustomerListAsync(codeName));
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
    }
}
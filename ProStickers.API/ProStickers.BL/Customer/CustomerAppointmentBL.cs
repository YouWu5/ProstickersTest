using ProStickers.BL.Core;
using ProStickers.CloudDAL.Storage.Customer;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using ProStickers.ViewModel.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProStickers.BL.Customer
{
    public class CustomerAppointmentBL
    {
        public static async Task<OperationResult> CreateCustomerCallRequestAsync(UserInfo currentUserInfo, AppointmentRequestViewModel vm)
        {
            if (vm != null)
            {
                InternalOperationResult result = await CustomerAppointmentDAL.CreateCustomerCallRequestAsync(currentUserInfo.UserID, currentUserInfo.UserName, vm);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<Pager<CustomerAppointmentListViewModel>> GetListAsync<CustomerAppointmentListViewModel>(string customerID)
        {
            Pager<CustomerAppointmentListViewModel> pager = PagerOperations.InitializePager<CustomerAppointmentListViewModel>
                                                        ("UpdatedTS", SortDirection.DESC.ToString(),
                                                          true, true, true, 30, true);

            pager.ColumnList = new List<ListItem>()
            {
                new ListItem { Text = "Appointment number", Value = "AppointmentNumber" },
                new ListItem { Text = "Appointment date time", Value = "AppointmentDateTime" },
                new ListItem { Text = "Request date time", Value = "RequestDateTime" } ,
                new ListItem { Text = "Status", Value = "AppointmentStatus" }
            };
            await CustomerAppointmentDAL.GetListAsync(pager, customerID);
            return pager;
        }

        public static async Task<OperationResult> GetListAsync<CustomerAppointmentListViewModel>(Pager<CustomerAppointmentListViewModel> pager, string customerID)
        {
            if (pager != null)
            {
                PagerOperations.UpdatePager(pager);
                await CustomerAppointmentDAL.GetListAsync(pager, customerID);
                return new OperationResult(Result.Success, "", pager);
            }
            else
            {
                return new OperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<OperationResult> GetAppointmentListAsync(string customerID)
        {
            if (customerID != null && customerID != "")
            {
                InternalOperationResult result = await CustomerAppointmentDAL.GetAppointmentListAsync(customerID);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> GetByIDAsync(string customerID, string appointmentNo)
        {
            if (customerID != null && customerID != "" && appointmentNo != null && appointmentNo != "")
            {
                InternalOperationResult result = await CustomerAppointmentDAL.GetByIDAsync(customerID, appointmentNo);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> CancelAppointmentAsync(string customerID, string customerName, CustomerAppointmentViewModel appointmentVM)
        {
            if (customerID != null && customerID != "" && customerName != null && customerName != "" && appointmentVM != null)
            {
                InternalOperationResult result = await CustomerAppointmentDAL.CancelAppointmentAsync(customerID, customerName, appointmentVM);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<AppointmentRequestViewModel> GetDefaultViewModelAsync()
        {
            AppointmentRequestViewModel vm = new AppointmentRequestViewModel();
            return await Task.FromResult(vm);
        }
    }
}

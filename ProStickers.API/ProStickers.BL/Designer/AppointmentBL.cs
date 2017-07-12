using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProStickers.CloudDAL.Storage.Designer;
using ProStickers.ViewModel.Designer;
using ProStickers.ViewModel.Core;
using ProStickers.BL.Core;
using ProStickers.ViewModel.Customer;
using ProStickers.ViewModel.Security;
using ProStickers.CloudDAL;
using System.Web.Configuration;
using ProStickers.BL.Customer;

namespace ProStickers.BL.Designer
{
    public class AppointmentBL
    {
        public static async Task<int> GetAppointmentRequestCountAsync(UserInfo currentUserInfo)
        {
            return await AppointmentDAL.GetAppointmentRequestCountAsync(currentUserInfo.UserID, currentUserInfo.UserName);
        }

        public static async Task<AppointmentRequestPickViewModel> GetAppointmentRequestAsync()
        {
            return await AppointmentDAL.GetAppointmentRequestAsync();
        }

        public static async Task<OperationResult> CancelAppointmentRequestAsync(UserInfo currentUserInfo, AppointmentRequestViewModel vm)
        {
            if (vm != null)
            {
                InternalOperationResult result = await AppointmentDAL.CancelAppointmentRequestAsync(currentUserInfo.UserID, currentUserInfo.UserName, vm);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> CreateAsync(UserInfo currentUserInfo, AppointmentRequestPickViewModel vm)
        {
            if (vm != null)
            {
                InternalOperationResult result = await AppointmentDAL.CreateAsync(currentUserInfo.UserID, currentUserInfo.UserName, vm);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<Pager<AppointmentListViewModel>> GetListAsync<AppointmentListViewModel>(string userID)
        {
            Pager<AppointmentListViewModel> pager = PagerOperations.InitializePager<AppointmentListViewModel>
                                                        ("UpdatedTS", SortDirection.DESC.ToString(),
                                                          true, true, true, 30, true);

            pager.SearchList = new List<SearchItem>
            {
                new SearchItem { DisplayName = "FromDate", Name = "Date", Value = Utility.GetCSTDateTime().ToString("yyyy-MM-dd") },
                new SearchItem { DisplayName = "ToDate", Name = "Date", Value = Utility.GetCSTDateTime().ToString("yyyy-MM-dd") },
                new SearchItem { DisplayName = "Status", Name = "AppointmentStatus", Value = "Scheduled" },
            };

            pager.ColumnList = new List<ListItem>()
            {
                new ListItem { Text = "Appointment number", Value = "AppointmentNumber" },
                new ListItem { Text = "Appointment date time", Value = "AppointmentDateTime" },
                new ListItem { Text = "Customer", Value = "CustomerName" },
                new ListItem { Text = "Customer contact number", Value = "ContactNumber" },
                new ListItem { Text = "Request date time", Value = "RequestDateTime" },
                new ListItem { Text = "Status", Value = "AppointmentStatus" },
            };

            await AppointmentDAL.GetListAsync(pager, userID);
            return pager;
        }

        public static async Task<OperationResult> GetListAsync<AppointmentListViewModel>(Pager<AppointmentListViewModel> pager, string userID)
        {
            PagerOperations.UpdatePager<AppointmentListViewModel>(pager);
            await AppointmentDAL.GetListAsync(pager, userID);
            return new OperationResult(Result.Success, "", pager);
        }

        public static async Task<Pager<AppointmentListViewModel>> GetAppointmentListAsync<AppointmentListViewModel>(string userID)
        {
            Pager<AppointmentListViewModel> pager = PagerOperations.InitializePager<AppointmentListViewModel>
                                                        ("CurrentUpdatedTS", SortDirection.DESC.ToString(),
                                                          true, true, true, 5, true);

            pager.SearchList = new List<SearchItem>
            {
                new SearchItem { DisplayName = "Status", Name = "AppointmentStatus", Value = "Scheduled" }
            };

            pager.ColumnList = new List<ListItem>()
            {
                new ListItem { Text = "Appointment number", Value = "AppointmentNumber" },
                new ListItem { Text = "Appointment date time", Value = "AppointmentDateTime" },
                new ListItem { Text = "Customer", Value = "CustomerName" },
                new ListItem { Text = "Customer contact number", Value = "ContactNumber" },
                new ListItem { Text = "Request date time", Value = "RequestDateTime" },
                new ListItem { Text = "Status", Value = "AppointmentStatus" },
            };

            await AppointmentDAL.GetListAsync(pager, userID);
            return pager;
        }

        public static async Task<OperationResult> GetByIDAsync(UserInfo currentUserInfo, string appointmentNumer, DateTime date)
        {
            InternalOperationResult result = await AppointmentDAL.GetByIDAsync(currentUserInfo.UserID, currentUserInfo.UserName, appointmentNumer, date);
            return new OperationResult(result.Result, result.Message, result.ReturnedData);
        }

        public static async Task<OperationResult> UpdateAsync(UserInfo currentUserInfo, AppointmentViewModel vm)
        {
            if (vm != null)
            {
                InternalOperationResult result = await AppointmentDAL.UpdateAsync(currentUserInfo.UserID, currentUserInfo.UserName, vm);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> UpdateAppointmentStatusAsync(UserInfo currentUserInfo, AppointmentViewModel vm)
        {
            if (vm != null)
            {
                InternalOperationResult result = await AppointmentDAL.UpdateAppointmentStatusAsync(currentUserInfo.UserID, currentUserInfo.UserName, vm);
                if(vm.IsCancel == true && vm.CancellationReason != null)
                {
                    await SendEmail.DesignerCancelledAppointmentEmailAsync(OrderBL.senderAddress, OrderBL.displayName, vm);
                }                
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> UploadUserFileAsync(UserInfo currentUserInfo, UserFilesViewModel dAppFileVM)
        {
            if (dAppFileVM != null && dAppFileVM.FileBuffer != null)
            {
                double fileSize = dAppFileVM.FileBuffer.Length;
                if (fileSize / (1024f * 1024f) < 20)
                {
                    InternalOperationResult result = await AppointmentDAL.UploadUserFileAsync(currentUserInfo.UserID, currentUserInfo.UserName, dAppFileVM);
                    return new OperationResult(result.Result, result.Message, result.ReturnedData);
                }
                else
                {
                    return new OperationResult(Result.UDError, "Maximum file size is 20MB for uploaded files.", null);
                }
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad Request, Image doesn't exists.", null);
            }
        }

        public static async Task<CustomerDesignViewModel> GetDesignerNoteAsync(string designNumber, string userID)
        {
            return await AppointmentDAL.GetDesignerNoteAsync(designNumber, userID);
        }

        public static async Task<OperationResult> UpdateDesignerNoteAsync(string userID, CustomerDesignViewModel customerDVM)
        {
            if (customerDVM != null)
            {
                InternalOperationResult result = await AppointmentDAL.UpdateDesignerNoteAsync(userID, customerDVM);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        #region Helper Methods

        public static async Task<List<TimeSlotViewModel>> GetAppointmentSlotListAsync(UserInfo currentUserInfo, DateTime date)
        {
            return await AppointmentDAL.GetAppointmentTimeSlotListAsync(currentUserInfo.UserID, date);
        }

        public static async Task<List<ListItemTypes>> GetStatusListAsync()
        {
            List<ListItemTypes> list = new List<ListItemTypes>();

            list.Add(new ListItemTypes { Text = "All", Value = 0 });
            list.Add(new ListItemTypes { Text = "Scheduled", Value = 1 });
            list.Add(new ListItemTypes { Text = "Initiated", Value = 2 });
            list.Add(new ListItemTypes { Text = "Cancelled", Value = 3 });
            list.Add(new ListItemTypes { Text = "Completed", Value = 4 });

            return await Task.FromResult(list);
        }

        #endregion
    }
}

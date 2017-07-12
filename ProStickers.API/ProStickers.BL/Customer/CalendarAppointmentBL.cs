using ProStickers.CloudDAL.Storage.Customer;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using ProStickers.ViewModel.Designer;
using ProStickers.ViewModel.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProStickers.BL.Customer
{
    public class CalendarAppointmentBL
    {
        public static async Task<CalendarAppointmentViewModel> GetDefaultViewModelAsync()
        {
            CalendarAppointmentViewModel vm = new CalendarAppointmentViewModel();
            return await Task.FromResult(vm);
        }

        public static async Task<ListItem> GetAvailableDesignerAsync(DateTime date, int timeslotID)
        {
            return await CalendarAppointmentDAL.GetAvailableDesignerAsync(date, timeslotID);
        }

        public static async Task<List<ListItem>> GetAvailableDesignerListAsync(DateTime date, int timeslotID)
        {
            return await CalendarAppointmentDAL.GetAvailableDesignerListAsync(date, timeslotID);
        }

        public static async Task<OperationResult> CreateAsync(CalendarAppointmentViewModel vm, UserInfo currentUserInfo)
        {
            if (vm != null)
            {
                InternalOperationResult result = await CalendarAppointmentDAL.CreateAsync(vm, currentUserInfo.UserID, currentUserInfo.UserName);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<List<TimeSlotViewModel>> GetTimeSlotListAsync(DateTime date)
        {
            return await CalendarAppointmentDAL.GetTimeSlotListAsync(date);
        }
    }
}

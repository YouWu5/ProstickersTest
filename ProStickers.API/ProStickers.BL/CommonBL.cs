using ProStickers.CloudDAL;
using ProStickers.CloudDAL.Storage;
using ProStickers.ViewModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProStickers.BL
{
    public class CommonBL
    {
        public static async Task<DownloadImageViewModel> DownloadCustomerFileAsync(string userID, string fileNumber)
        {
            if (fileNumber != null && fileNumber != "")
            {
                return await CommonDAL.DownloadCustomerFileAsync(userID, fileNumber);
            }
            else
            {
                return null;
            }
        }

        public static async Task<DownloadImageViewModel> DownloadUserFileAsync(string userID, string fileNumber)
        {
            if (fileNumber != null && fileNumber != "")
            {
                return await CommonDAL.DownloadUserFileAsync(userID, fileNumber);
            }
            else
            {
                return null;
            }
        }

        public static async Task<DownloadImageViewModel> DownloadDesignImageAsync(string userID, string designNumber)
        {
            if (designNumber != null && designNumber != "")
            {
                return await CommonDAL.DownloadDesignImageAsync(userID, designNumber);
            }
            else
            {
                return null;
            }
        }

        public static async Task<DownloadImageViewModel> DownloadVectorFileAsync(string userID, string designNumber)
        {
            if (designNumber != null && designNumber != "")
            {
                return await CommonDAL.DownloadVectorFileAsync(userID, designNumber);
            }
            else
            {
                return null;
            }
        }

        public static async Task<List<ListItemTypes>> GetStateListByCountryAsync(int countryID)
        {
            return await CommonDAL.GetStateListByCountryAsync(countryID);
        }

        public static async Task<List<CountryListViewModel>> GetCountryListAsync()
        {
            return await CommonDAL.GetCountryListAsync();
        }

        public static async Task<List<DateFilter>> GetDateListAsync()
        {
            List<DateFilter> list = new List<DateFilter>();

            list.Add(new DateFilter { SNo = 1, Name = "Today", FromDate = Utility.GetCSTDateTime().Date, ToDate = Utility.GetCSTDateTime().Date });
            list.Add(new DateFilter { SNo = 2, Name = "Tomorrow", FromDate = Utility.GetCSTDateTime().AddDays(1).Date, ToDate = Utility.GetCSTDateTime().AddDays(1).Date });
            list.Add(new DateFilter { SNo = 3, Name = "Next 7 Days", FromDate = Utility.GetCSTDateTime().Date, ToDate = Utility.GetCSTDateTime().AddDays(6).Date });
            list.Add(new DateFilter { SNo = 4, Name = "Next 14 Days", FromDate = Utility.GetCSTDateTime().Date, ToDate = Utility.GetCSTDateTime().AddDays(14).Date });
            list.Add(new DateFilter { SNo = 5, Name = "Yesterday", FromDate = Utility.GetCSTDateTime().AddDays(-1).Date, ToDate = Utility.GetCSTDateTime().AddDays(-1).Date });
            list.Add(new DateFilter { SNo = 6, Name = "Last 7 Days", FromDate = Utility.GetCSTDateTime().AddDays(-6).Date, ToDate = Utility.GetCSTDateTime().Date });

            return await Task.FromResult(list);
        }

        public static async Task<List<ListItemTypes>> GetMonthListAsync()
        {
            return await CommonDAL.GetMonthListAsync();
        }

        public static async Task<List<int>> GetYearListAsync()
        {
            return await CommonDAL.GetYearListAsync();
        }
    }
}

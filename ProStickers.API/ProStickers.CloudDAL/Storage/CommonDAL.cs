using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using ProStickers.CloudDAL.Entity.Master;
using ProStickers.CloudDAL.Storage.ExceptionStorage;
using ProStickers.CloudDAL.Storage.Master;
using ProStickers.ViewModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ProStickers.ViewModel.Core.StatusEnum;

namespace ProStickers.CloudDAL.Storage
{
    public class CommonDAL
    {
        public static CloudTable configurationtable;

        static CommonDAL()
        {
            configurationtable = Utility.GetStorageTable("Configuration");
        }

        public static async Task<DownloadImageViewModel> DownloadVectorFileAsync(string userID, string designNumber)
        {
            try
            {
                DownloadImageViewModel vm = BlobStorage.DownloadBlobByteArray(Blob.vectorimage.ToString(), designNumber);
                return await Task.FromResult(vm);
            }
            catch (Exception e)
            {
                ExceptionTableStorage.InsertOrReplaceEntity("CustomerAppointment", Blob.vectorimage.ToString(), "DownloadVectorFileAsync", DateTime.UtcNow, e, null, designNumber, userID);
                return null;
            }
        }

        public static async Task<DownloadImageViewModel> DownloadDesignImageAsync(string userID, string designNumber)
        {
            try
            {
                DownloadImageViewModel vm = BlobStorage.DownloadBlobByteArray(Blob.designimage.ToString(), designNumber);
                return await Task.FromResult(vm);
            }
            catch (Exception e)
            {
                ExceptionTableStorage.InsertOrReplaceEntity("CustomerAppointment", Blob.designimage.ToString(), "DownloadDesignImageAsync", DateTime.UtcNow, e, null, designNumber, userID);
                return null;
            }
        }

        public static async Task<DownloadImageViewModel> DownloadCustomerFileAsync(string userID, string fileNumber)
        {
            try
            {
                DownloadImageViewModel vm = BlobStorage.DownloadBlobByteArray(Blob.customerfiles.ToString(), fileNumber);
                return await Task.FromResult(vm);
            }
            catch (Exception e)
            {
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", Blob.customerfiles.ToString(), "DownloadCustomerFileAsync", DateTime.UtcNow, e, null, fileNumber, userID);
                return null;
            }
        }

        public static async Task<DownloadImageViewModel> DownloadUserFileAsync(string userID, string fileNumber)
        {
            try
            {
                DownloadImageViewModel vm = BlobStorage.DownloadBlobByteArray(Blob.userfiles.ToString(), fileNumber);
                return await Task.FromResult(vm);
            }
            catch (Exception e)
            {
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", Blob.userfiles.ToString(), "DownloadUserFileAsync", DateTime.UtcNow, e, null, fileNumber, userID);
                return null;
            }
        }

        public static async Task<List<CountryListViewModel>> GetCountryListAsync()
        {
            var list = JsonConvert.DeserializeObject<List<CountryEntity>>(await MasterDAL.RetriveEntity("Country")) as List<CountryEntity>;
            List<CountryListViewModel> countryList = new List<CountryListViewModel>();
            list.ForEach(l => countryList.Add(new CountryListViewModel { ID = l.CountryID, Name = l.Name, Regax = l.PostalCodeRegax }));
            return countryList;
        }

        public static async Task<List<ListItemTypes>> GetStateListByCountryAsync(int countryID)
        {
            var list = JsonConvert.DeserializeObject<List<StateEntity>>(await MasterDAL.RetriveEntity("State")) as List<StateEntity>;
            List<ListItemTypes> stateList = new List<ListItemTypes>();
            list.Where(c => c.CountryID == countryID).ToList().ForEach(l => stateList.Add(new ListItemTypes { Value = l.StateID, Text = l.Name }));
            return stateList;
        }

        public static async Task<List<ListItemTypes>> GetMonthListAsync()
        {
            var list = JsonConvert.DeserializeObject<List<MonthEntity>>(await MasterDAL.RetriveEntity("Month")) as List<MonthEntity>;
            List<ListItemTypes> monthList = new List<ListItemTypes>();
            list.ToList().ForEach(l => monthList.Add(new ListItemTypes { Value = l.MonthID, Text = l.Name }));
            return monthList;
        }

        public static async Task<List<int>> GetYearListAsync()
        {
            var list = JsonConvert.DeserializeObject<List<YearEntity>>(await MasterDAL.RetriveEntity("Year")) as List<YearEntity>;
            List<int> yearList = list.Select(x => x.YearID).ToList();
            return yearList;
        }

        public static string GetImageURL(string name)
        {
            string URL = configurationtable.CreateQuery<ConfigurationEntity>().Where(c => c.PartitionKey == "imageconf" && c.RowKey == name).Select(c => c.Value).FirstOrDefault();
            return URL;
        }

        public static string GetFeedbackLink()
        {
            string link = configurationtable.CreateQuery<ConfigurationEntity>().Where(c => c.PartitionKey == "feedbacklink" && c.RowKey == "feedbacklink").Select(c => c.Value).FirstOrDefault();
            return link;
        }

        public static string GetPublicWebsiteURL()
        {
            string URL = configurationtable.CreateQuery<ConfigurationEntity>().Where(c => c.PartitionKey == "publicwebsite" && c.RowKey == "publicwebsite").Select(c => c.Value).FirstOrDefault();
            return URL;
        }

        public static string GetProstickerLogoURL()
        {
            string URL = configurationtable.CreateQuery<ConfigurationEntity>().Where(c => c.PartitionKey == "prostickerlogo" && c.RowKey == "prostickerlogo").Select(c => c.Value).FirstOrDefault();
            return URL;
        }

        public static string GetUSPSLink()
        {
            string URL = configurationtable.CreateQuery<ConfigurationEntity>().Where(c => c.PartitionKey == "uspslink" && c.RowKey == "uspslink").Select(c => c.Value).FirstOrDefault();
            return URL;
        }

        public static string GetCustomerPortalURL()
        {
            string URL = configurationtable.CreateQuery<ConfigurationEntity>().Where(c => c.PartitionKey == "customerportalurl" && c.RowKey == "customerportalurl").Select(c => c.Value).FirstOrDefault();
            return URL;
        }

        public static async Task<string> GetContentType(string contentType)
        {
            var list = JsonConvert.DeserializeObject<List<SupportedFileExtensionEntity>>(await MasterDAL.RetriveEntity("SupportedFileExtension")) as List<SupportedFileExtensionEntity>;
            string contentTypeName = list.Where(x => x.Text == contentType).Select(c => c.Value).FirstOrDefault();

            return await Task.FromResult(contentTypeName);
        }
    }
}

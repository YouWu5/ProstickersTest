using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using ProStickers.CloudDAL.Entity.Customer;
using ProStickers.CloudDAL.Entity.Master;
using ProStickers.CloudDAL.Storage.Customer;
using ProStickers.CloudDAL.Storage.ExceptionStorage;
using ProStickers.CloudDAL.Storage.Master;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using ProStickers.ViewModel.Security;

namespace ProStickers.CloudDAL.Storage.Security
{
    public class UserAuthenticationDAL
    {
        static CloudTable userPagetable;
        static CloudTable customerPagetable;

        static UserAuthenticationDAL()
        {
            userPagetable = Utility.GetStorageTable("UserPage");
            customerPagetable = Utility.GetStorageTable("CustomerPage");
        }

        public static async Task<InternalOperationResult> CreateCustomerAsync(CustomerViewModel vm)
        {
            try
            {
                UserSession userSession = new UserSession();
                userSession.AssignedPageList = new List<UserPageListItem>();

                CustomerEntity customerEntity = CustomerDAL.customerTable.CreateQuery<CustomerEntity>().Where(c => c.EmailID == vm.EmailID).Select(c => c).FirstOrDefault();

                if (customerEntity != null)
                {
                    if (customerEntity.Active == false)
                    {
                        customerEntity.Active = true;
                        customerEntity.IsPolicyAccepted = false;
                        TableOperation update = TableOperation.InsertOrReplace(customerEntity);
                        await CustomerDAL.customerTable.ExecuteAsync(update);
                    }
                    userSession.Name = customerEntity.FullName;
                    userSession.UserID = customerEntity.PartitionKey;
                    userSession.IsPolicyAccepted = customerEntity.IsPolicyAccepted;
                    userSession.UserTypeID = 3;
                }
                else
                {
                    InternalOperationResult result = await CustomerDAL.CreateAsync(vm);
                    CustomerEntity cEntity = result.ReturnedData as CustomerEntity;

                    if (result.Result == Result.Success)
                    {
                        userSession.Name = cEntity.FullName;
                        userSession.UserID = cEntity.PartitionKey;
                        userSession.IsPolicyAccepted = false;
                        userSession.UserTypeID = 3;
                    }
                }

                List<CustomerPageEntity> pageEntity = customerPagetable.CreateQuery<CustomerPageEntity>().Where(c => c.Active == true).ToList();

                foreach (var item in pageEntity.OrderBy(c => c.OrderSequence))
                {
                    UserPageListItem page = new UserPageListItem();
                    page.PageID = item.PageID;
                    page.Name = item.Name;
                    page.ApiUrl = item.ApiUrl;
                    page.Url = item.Url;
                    page.OrderSequence = item.OrderSequence;
                    userSession.AssignedPageList.Add(page);
                }
                var utcOffset = new DateTimeOffset(Utility.GetCSTDateTime(), TimeSpan.Zero);
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                DateTimeOffset dateTimeOffset = utcOffset.ToOffset(tz.GetUtcOffset(utcOffset));
                userSession.UtcDateTimeOffset = dateTimeOffset.Offset.ToString();

                return new InternalOperationResult(Result.Success, "Customer added successfully.", userSession);
            }
            catch (Exception e)
            {
                string requestJson = JsonConvert.SerializeObject(vm);
                ExceptionTableStorage.InsertOrReplaceEntity("User", "UserAuthenticationDAL", "CreateCustomerAsync", DateTime.UtcNow, e, null, requestJson, vm.EmailID);
                return new InternalOperationResult(Result.SDError, "Oops something went wrong. Please try again.", null);
            }
        }

        public static async Task<InternalOperationResult> GetUserSessionAsync(string emailID, string ID)
        {
            UserSession userSession = new UserSession();
            List<UserPageEntity> pageEntity;

            UserEntity userEntity = UserDAL.userTable.CreateQuery<UserEntity>().Where(c => c.GoogleID == emailID).Select(c => c).FirstOrDefault();
            if (userEntity != null)
            {
                if (userEntity.ID == null)
                {
                    userEntity.ID = ID;
                    TableOperation insert = TableOperation.InsertOrReplace(userEntity);
                    await UserDAL.userTable.ExecuteAsync(insert);
                }
                userSession.Name = userEntity.FullName;
                userSession.UserID = userEntity.PartitionKey;
                userSession.UserTypeID = userEntity.UserTypeID;
                userSession.AssignedPageList = new List<UserPageListItem>();
                if (userEntity.UserTypeID == 1)
                {
                    pageEntity = userPagetable.CreateQuery<UserPageEntity>().Where(c => c.PartitionKey == "TRUE" && c.Active == true).ToList();
                }
                else
                {
                    pageEntity = userPagetable.CreateQuery<UserPageEntity>().Where(c => c.PartitionKey == "FALSE" && c.Active == true).ToList();
                }

                foreach (var item in pageEntity.OrderBy(c => c.OrderSequence))
                {
                    UserPageListItem page = new UserPageListItem();
                    page.PageID = item.PageID;
                    page.Name = item.Name;
                    page.ApiUrl = item.ApiUrl;
                    page.Url = item.Url;
                    page.OrderSequence = item.OrderSequence;
                    userSession.AssignedPageList.Add(page);
                }
                var utcOffset = new DateTimeOffset(Utility.GetCSTDateTime(), TimeSpan.Zero);
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                DateTimeOffset dateTimeOffset = utcOffset.ToOffset(tz.GetUtcOffset(utcOffset));
                userSession.UtcDateTimeOffset = dateTimeOffset.Offset.ToString();
                return new InternalOperationResult(Result.Success, "User login successfully.", userSession);
            }
            else
            {
                return new InternalOperationResult(Result.Success, "User ID doesn't exists.", null);
            }
        }

        public static async Task<UserSession> LoadUserSessionAsync(string emailID)
        {
            UserSession userSession = new UserSession();
            List<UserPageEntity> pageEntity;
            UserEntity userEntity = UserDAL.userTable.CreateQuery<UserEntity>().Where(c => c.GoogleID == emailID).Select(c => c).FirstOrDefault();
            if (userEntity != null)
            {
                userSession.Name = userEntity.FullName;
                userSession.UserID = userEntity.PartitionKey;
                userSession.UserTypeID = userEntity.UserTypeID;
                userSession.AssignedPageList = new List<UserPageListItem>();
                if (userEntity.UserTypeID == 1)
                {
                    pageEntity = userPagetable.CreateQuery<UserPageEntity>().Where(c => c.PartitionKey == "TRUE" && c.Active == true).ToList();
                }
                else
                {
                    pageEntity = userPagetable.CreateQuery<UserPageEntity>().Where(c => c.PartitionKey == "FALSE" && c.Active == true).ToList();
                }

                foreach (var item in pageEntity.OrderBy(c => c.OrderSequence))
                {
                    UserPageListItem page = new UserPageListItem();
                    page.PageID = item.PageID;
                    page.Name = item.Name;
                    page.ApiUrl = item.ApiUrl;
                    page.Url = item.Url;
                    page.OrderSequence = item.OrderSequence;
                    userSession.AssignedPageList.Add(page);
                }
                var utcOffset = new DateTimeOffset(Utility.GetCSTDateTime(), TimeSpan.Zero);
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                DateTimeOffset dateTimeOffset = utcOffset.ToOffset(tz.GetUtcOffset(utcOffset));
                userSession.UtcDateTimeOffset = dateTimeOffset.Offset.ToString();
            }
            return await Task.FromResult(userSession);
        }

        public static async Task<UserSession> LoadCustomerSessionAsync(string emailID)
        {
            UserSession userSession = new UserSession();
            CustomerEntity customerEntity = CustomerDAL.customerTable.CreateQuery<CustomerEntity>().Where(c => c.EmailID == emailID).Select(c => c).FirstOrDefault();
            if (customerEntity != null)
            {
                userSession.Name = customerEntity.FullName;
                userSession.UserID = customerEntity.PartitionKey;
                userSession.UserTypeID = 3;
                userSession.AssignedPageList = new List<UserPageListItem>();

                List<CustomerPageEntity> pageEntity = customerPagetable.CreateQuery<CustomerPageEntity>().Where(c => c.Active == true).ToList();

                foreach (var item in pageEntity.OrderBy(c => c.OrderSequence))
                {
                    UserPageListItem page = new UserPageListItem();
                    page.PageID = item.PageID;
                    page.Name = item.Name;
                    page.ApiUrl = item.ApiUrl;
                    page.Url = item.Url;
                    page.OrderSequence = item.OrderSequence;
                    userSession.AssignedPageList.Add(page);
                }
                var utcOffset = new DateTimeOffset(Utility.GetCSTDateTime(), TimeSpan.Zero);
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                DateTimeOffset dateTimeOffset = utcOffset.ToOffset(tz.GetUtcOffset(utcOffset));
                userSession.UtcDateTimeOffset = dateTimeOffset.Offset.ToString();
            }
            return await Task.FromResult(userSession);
        }

        public static InternalOperationResult AcceptSignInPolicy(string userID)
        {
            CustomerEntity customerEntity = CustomerDAL.customerTable.CreateQuery<CustomerEntity>().Where(c => c.PartitionKey == userID).FirstOrDefault();
            if (customerEntity != null)
            {
                customerEntity.IsPolicyAccepted = true;
                customerEntity.PolicyAcceptedDate = DateTime.UtcNow.Date;
                TableOperation insertOperation = TableOperation.Replace(customerEntity);
                CustomerDAL.customerTable.ExecuteAsync(insertOperation);
                return new InternalOperationResult(Result.Success, "Saved successfully.", null);
            }
            else
            {
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static bool CheckUserExistsAsync(string userID)
        {
            List<string> userList = UserDAL.userTable.CreateQuery<UserEntity>().Select(c => c.GoogleID).ToList();
            string ID = userList.Where(c => c.Split(new string[] { "@" }, StringSplitOptions.None).FirstOrDefault().Replace(".", "") == userID.Split(new string[] { "@" }, StringSplitOptions.None).FirstOrDefault().Replace(".", "") && c.Split(new string[] { "@" }, StringSplitOptions.None)[1] == userID.Split(new string[] { "@" }, StringSplitOptions.None)[1]).Select(c => c).FirstOrDefault();
            if (ID != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static InternalOperationResult CheckMasterOrDesignerExistsAsync(string userID)
        {
            List<UserEntity> userList = UserDAL.userTable.CreateQuery<UserEntity>().Select(c => c).ToList();
            UserEntity userEntity = userList.Where(c => c.GoogleID.Split(new string[] { "@" }, StringSplitOptions.None).FirstOrDefault().Replace(".", "") == userID.Split(new string[] { "@" }, StringSplitOptions.None).FirstOrDefault().Replace(".", "") && c.GoogleID.Split(new string[] { "@" }, StringSplitOptions.None)[1] == userID.Split(new string[] { "@" }, StringSplitOptions.None)[1]).Select(c => c).FirstOrDefault();

            if (userEntity == null)
            {
                return new InternalOperationResult(Result.UDError, "User is not authorized for Prosticker login.", null);
            }
            else if (userEntity != null && userEntity.Active == false)
            {
                return new InternalOperationResult(Result.UDError, "This User has been inactive now, you cannot login anymore.", null);
            }
            else
            {
                return new InternalOperationResult(Result.Success, "Valid User.", null);
            }
        }

        public static List<ListItem> GetPageList()
        {
            List<ListItem> pageList = new List<ListItem>();
            List<CustomerPageEntity> pageEntity = customerPagetable.CreateQuery<CustomerPageEntity>().ToList();

            foreach (var item in pageEntity.OrderBy(c => c.OrderSequence))
            {
                ListItem list = new ListItem();
                list.Text = "3";
                list.Value = item.ApiUrl;
                pageList.Add(list);
            }

            List<ListItem> userPageList = new List<ListItem>();
            List<UserPageEntity> userPageEntity = userPagetable.CreateQuery<UserPageEntity>().Where(c => c.Active == true).ToList();

            foreach (var item in userPageEntity.OrderBy(c => c.OrderSequence))
            {
                ListItem list = new ListItem();
                if (item.PartitionKey == "TRUE")
                {
                    list.Text = "1";
                }
                else if (item.PartitionKey == "FALSE")
                {
                    list.Text = "2";
                }
                list.Value = item.ApiUrl;
                userPageList.Add(list);
            }
            if (userPageList != null)
            {
                pageList.AddRange(userPageList);
            }
            return pageList;
        }
    }
}

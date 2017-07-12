using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.WindowsAzure.Storage.Table;
using ProStickers.CloudDAL.Entity.Customer;
using ProStickers.CloudDAL.Entity.Master;
using ProStickers.CloudDAL.Storage.Customer;
using ProStickers.CloudDAL.Storage.ExceptionStorage;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Master;
using ProStickers.CloudDAL.Entity.Designer;

namespace ProStickers.CloudDAL.Storage.Master
{
    public class UserDAL
    {
        public static CloudTable userTable;

        static UserDAL()
        {
            userTable = Utility.GetStorageTable("User");
        }

        public static async Task GetListAsync<UserListViewModel>(string userID, Pager<UserListViewModel> pagerVM)
        {
            TableQuery<DynamicTableEntity> projectionQuery = new TableQuery<DynamicTableEntity>().Select(new string[] { "UserID", "FullName", "GoogleID", "UserType", "UpdatedTS", "Active" });

            EntityResolver<ViewModel.Master.UserListViewModel> resolver = (pk, rk, ts, props, etag) => new ViewModel.Master.UserListViewModel
            {
                UserID = pk,
                Name = props["FullName"].StringValue,
                GoogleID = props["GoogleID"].StringValue,
                UserType = props["UserType"].StringValue,
                UpdatedTS = props["UpdatedTS"].DateTime.Value,
                Active = props["Active"].BooleanValue.Value,
                IsDeleteEnable = true
            };

            List<ViewModel.Master.UserListViewModel> userList = userTable.ExecuteQuery(projectionQuery, resolver, null, null).ToList();

            if (userList != null && userList.Count() > 0)
            {
                pagerVM.RecordsCount = userList.Count();

                if (pagerVM.Sort == "FullName")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        userList = userList.OrderByDescending(s => s.Name).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        userList = userList.OrderBy(s => s.Name).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "GoogleID")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        userList = userList.OrderByDescending(s => s.GoogleID).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        userList = userList.OrderBy(s => s.GoogleID).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "UserType")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        userList = userList.OrderByDescending(s => s.UserType).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        userList = userList.OrderBy(s => s.UserType).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "UpdatedTS")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        userList = userList.OrderByDescending(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        userList = userList.OrderBy(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
            }

            for (int i = 0; i < userList.Count(); i++)
            {
                if (userID == userList[i].UserID || userList[i].UserID == "10000001")
                {
                    userList[i].IsDeleteEnable = false;
                }
            }

            Mapper.Initialize(a =>
            {
                a.CreateMap<ViewModel.Master.UserListViewModel, UserListViewModel>();
            });

            await Task.FromResult(pagerVM.Data = Mapper.Map<List<UserListViewModel>>(userList));
        }

        public static async Task<InternalOperationResult> GetByIDAsync(string loggedInUserID, string userID)
        {
            UserEntity userEntity = userTable.ExecuteQuery(new TableQuery<UserEntity>()).Where(c => c.PartitionKey == userID).FirstOrDefault();
            if (userEntity != null)
            {
                Mapper.Initialize(a =>
                {
                    a.CreateMap<UserViewModel, UserEntity>().ReverseMap().
                    ForMember(x => x.UserID, y => y.MapFrom(z => z.PartitionKey));
                });

                UserViewModel userVM = Mapper.Map<UserViewModel>(userEntity);
                userVM.IsDeleteEnable = true;
                if (userVM.UserID == "10000001" || userVM.UserID == loggedInUserID)
                {
                    userVM.IsDeleteEnable = false;
                }

                return await Task.FromResult(new InternalOperationResult(Result.Success, "", userVM));
            }
            else
            {
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong. Please try again.", null);
            }
        }

        public static async Task<InternalOperationResult> CreateAsync(UserViewModel userVM, string userID, string userName)
        {
            try
            {
                if (userVM.ImageBuffer != null)
                {
                    string imageGUID = Guid.NewGuid().ToString();
                    userVM.ImageGUID = imageGUID;

                    await BlobStorage.UploadPublicImage(StatusEnum.Blob.userimage.ToString(), userVM.ImageBuffer, imageGUID, "image/jpg");
                }
                Mapper.Initialize(a =>
                {
                    a.CreateMap<UserEntity, UserViewModel>().ReverseMap();
                });

                string nextUserID = Utility.GetNextUserId("UserID");
                if (nextUserID != null && nextUserID != "")
                {
                    UserEntity userEntity = new UserEntity();
                    userEntity = Mapper.Map<UserEntity>(userVM);
                    userEntity.PartitionKey = nextUserID;
                    userEntity.Active = true;
                    userEntity.CreatedBy = userID;
                    userEntity.CreatedTS = DateTime.UtcNow;
                    userEntity.UpdatedBy = userID;
                    userEntity.UpdatedTS = DateTime.UtcNow;

                    TableOperation insert = TableOperation.Insert(userEntity);
                    await userTable.ExecuteAsync(insert);

                    TransactionLogDAL.InsertTransactionLog(userEntity.PartitionKey, "User", DateTime.UtcNow.Date, DateTime.UtcNow, userID, "Added", userName);
                    return new InternalOperationResult(Result.Success, "User added successfully.", userEntity.PartitionKey);
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = Newtonsoft.Json.JsonConvert.SerializeObject(userVM);
                ExceptionTableStorage.InsertOrReplaceEntity("Master", "UserDAL", "CreateAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> UpdateAsync(UserViewModel userVM, string userName, string userID)
        {
            try
            {
                UserEntity userEntity = (from c in userTable.CreateQuery<UserEntity>().Where(c => c.PartitionKey == userVM.UserID) select c).FirstOrDefault();
                if (userEntity != null)
                {
                    if (userVM.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff") == (userEntity.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff")))
                    {
                        if (userVM.ImageBuffer != null && userVM.IsNew == true)
                        {
                            string imageGUID = Guid.NewGuid().ToString();
                            userVM.ImageGUID = imageGUID;
                            await BlobStorage.UploadPublicImage(StatusEnum.Blob.userimage.ToString(), userVM.ImageBuffer, imageGUID, "image/jpg");
                        }
                        else
                        {
                            userVM.ImageGUID = userEntity.ImageGUID;
                        }

                        if (userVM.IsNew == true && userEntity.ImageGUID != null)
                        {
                            await BlobStorage.DeleteBlob(StatusEnum.Blob.userimage.ToString(), userEntity.ImageGUID);
                        }

                        Mapper.Initialize(a =>
                        {
                            a.CreateMap<UserEntity, UserViewModel>().ReverseMap();
                        });

                        UserEntity uEntity = new UserEntity(userVM.UserID);
                        uEntity = Mapper.Map<UserEntity>(userVM);
                        uEntity.UpdatedBy = userID;
                        uEntity.UpdatedTS = DateTime.UtcNow;
                        uEntity.CreatedBy = userEntity.CreatedBy;
                        uEntity.CreatedTS = userEntity.CreatedTS;
                        uEntity.RowKey = userEntity.RowKey;

                        TableOperation update = TableOperation.InsertOrReplace(uEntity);
                        await userTable.ExecuteAsync(update);
                        TransactionLogDAL.InsertTransactionLog(uEntity.PartitionKey, "User", DateTime.UtcNow.Date, DateTime.UtcNow, userID, "Edited", userName);
                        return new InternalOperationResult(Result.Success, "User updated successfully.", uEntity.PartitionKey);
                    }
                    else
                    {
                        return new InternalOperationResult(Result.Concurrency, "Data is already changed by someone else. Please try again.", null);
                    }
                }
                else
                {
                    return new InternalOperationResult(Result.UDError, "User doesn't exist or is deleted.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = Newtonsoft.Json.JsonConvert.SerializeObject(userVM);
                ExceptionTableStorage.InsertOrReplaceEntity("Master", "UserDAL", "UpdateAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> InactiveAsync(string userID, UserStatusChangeViewModel userVM)
        {
            try
            {
                UserEntity userEntity = (from c in userTable.CreateQuery<UserEntity>().Where(c => c.PartitionKey == userVM.UserID) select c).FirstOrDefault();
                if (userEntity != null)
                {
                    if (userVM.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff") == (userEntity.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff")))
                    {
                        List<int> designerEntity = CalendarAppointmentDAL.designerAppointmentTable.ExecuteQuery(new TableQuery<DesignerAppointmentEntity>()).
                                                                     Where(x => x.PartitionKey == userEntity.PartitionKey).Select(x => x.AppointmentStatusID).ToList();

                        if (!designerEntity.Contains(1) && !designerEntity.Contains(2))
                        {

                            userEntity.Active = userVM.Active;
                            userEntity.UpdatedBy = userID;
                            userEntity.UpdatedTS = DateTime.UtcNow;
                            TableOperation update = TableOperation.InsertOrReplace(userEntity);
                            await userTable.ExecuteAsync(update);
                            TransactionLogDAL.InsertTransactionLog(userEntity.PartitionKey, "User", DateTime.UtcNow.Date, DateTime.UtcNow, userEntity.CreatedBy, "Delete", userID);
                            return new InternalOperationResult(Result.Success, "User updated successfully.", userEntity.PartitionKey);
                        }
                        else
                        {
                            return new InternalOperationResult(Result.Success, "This user has scheduled appointments, hence this user can’t be inactive.", null);
                        }
                    }
                    else
                    {
                        return new InternalOperationResult(Result.Concurrency, "Data is already changed by someone else. Please try again.", null);
                    }

                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "User does not exist.", null);
                }

            }
            catch (Exception e)
            {
                ExceptionTableStorage.InsertOrReplaceEntity("Master", "UserDAL", "DeleteAsync", DateTime.UtcNow, e, null, null, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        #region Helper Method

        public static bool CheckGoogleID(string googleID)
        {
            List<string> userList = userTable.CreateQuery<UserEntity>().Select(c => c.GoogleID).ToList();
            string userID = userList.Where(c => c.Split(new string[] { "@" }, StringSplitOptions.None).FirstOrDefault().Replace(".", "") == googleID.Split(new string[] { "@" }, StringSplitOptions.None).FirstOrDefault().Replace(".", "") && c.Split(new string[] { "@" }, StringSplitOptions.None)[1] == googleID.Split(new string[] { "@" }, StringSplitOptions.None)[1]).Select(c => c).FirstOrDefault();

            List<string> customerList = CustomerDAL.customerTable.CreateQuery<CustomerEntity>().Where(c => c.IsFacebookUser == false).Select(c => c.EmailID).ToList();
            string customerID = customerList.Where(c => c.Split(new string[] { "@" }, StringSplitOptions.None).FirstOrDefault().Replace(".", "") == googleID.Split(new string[] { "@" }, StringSplitOptions.None).FirstOrDefault().Replace(".", "") && c.Split(new string[] { "@" }, StringSplitOptions.None)[1] == googleID.Split(new string[] { "@" }, StringSplitOptions.None)[1]).Select(c => c).FirstOrDefault();

            if (userID == null && customerID == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}



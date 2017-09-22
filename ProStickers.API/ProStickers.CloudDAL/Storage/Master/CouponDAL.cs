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
    public class CouponDAL
    {
        public static CloudTable couponTable;

        static CouponDAL()
        {
            couponTable = Utility.GetStorageTable("Coupon");
        }

        public static async Task GetListAsync<CouponListViewModel>(string couponID, Pager<CouponListViewModel> pagerVM)
        {
            TableQuery<DynamicTableEntity> projectionQuery = new TableQuery<DynamicTableEntity>().Select(new string[] { "CouponID", "FullName", "GoogleID", "CouponType", "UpdatedTS", "Active" });

            EntityResolver<ViewModel.Master.CouponListViewModel> resolver = (pk, rk, ts, props, etag) => new ViewModel.Master.CouponListViewModel
            {
                CouponID = pk,
                Name = props["FullName"].StringValue,
                GoogleID = props["GoogleID"].StringValue,
                CouponType = props["CouponType"].StringValue,
                UpdatedTS = props["UpdatedTS"].DateTime.Value,
                Active = props["Active"].BooleanValue.Value,
                IsDeleteEnable = true
            };

            List<ViewModel.Master.CouponListViewModel> couponList = couponTable.ExecuteQuery(projectionQuery, resolver, null, null).ToList();

            if (couponList != null && couponList.Count() > 0)
            {
                pagerVM.RecordsCount = couponList.Count();

                if (pagerVM.Sort == "FullName")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        couponList = couponList.OrderByDescending(s => s.Name).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        couponList = couponList.OrderBy(s => s.Name).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "GoogleID")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        couponList = couponList.OrderByDescending(s => s.GoogleID).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        couponList = couponList.OrderBy(s => s.GoogleID).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "CouponType")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        couponList = couponList.OrderByDescending(s => s.CouponType).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        couponList = couponList.OrderBy(s => s.CouponType).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "UpdatedTS")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        couponList = couponList.OrderByDescending(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        couponList = couponList.OrderBy(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
            }

            for (int i = 0; i < couponList.Count(); i++)
            {
                if (couponID == couponList[i].CouponID || couponList[i].CouponID == "10000001")
                {
                    couponList[i].IsDeleteEnable = false;
                }
            }

            Mapper.Initialize(a =>
            {
                a.CreateMap<ViewModel.Master.CouponListViewModel, CouponListViewModel>();
            });

            await Task.FromResult(pagerVM.Data = Mapper.Map<List<CouponListViewModel>>(couponList));
        }

        public static async Task<InternalOperationResult> GetByIDAsync(string loggedInCouponID, string couponID)
        {
            CouponEntity couponEntity = couponTable.ExecuteQuery(new TableQuery<CouponEntity>()).Where(c => c.PartitionKey == couponID).FirstOrDefault();
            if (couponEntity != null)
            {
                Mapper.Initialize(a =>
                {
                    a.CreateMap<CouponViewModel, CouponEntity>().ReverseMap().
                    ForMember(x => x.CouponID, y => y.MapFrom(z => z.PartitionKey));
                });

                CouponViewModel couponVM = Mapper.Map<CouponViewModel>(couponEntity);
                couponVM.IsDeleteEnable = true;
                if (couponVM.CouponID == "10000001" || couponVM.CouponID == loggedInCouponID)
                {
                    couponVM.IsDeleteEnable = false;
                }

                return await Task.FromResult(new InternalOperationResult(Result.Success, "", couponVM));
            }
            else
            {
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong. Please try again.", null);
            }
        }

        public static async Task<InternalOperationResult> CreateAsync(CouponViewModel couponVM, string couponID, string couponName)
        {
            try
            {
                if (couponVM.ImageBuffer != null)
                {
                    string imageGUID = Guid.NewGuid().ToString();
                    couponVM.ImageGUID = imageGUID;

                    await BlobStorage.UploadPublicImage(StatusEnum.Blob.couponimage.ToString(), couponVM.ImageBuffer, imageGUID, "image/jpg");
                }
                Mapper.Initialize(a =>
                {
                    a.CreateMap<CouponEntity, CouponViewModel>().ReverseMap();
                });

                string nextCouponID = Utility.GetNextCouponId("CouponID");
                if (nextCouponID != null && nextCouponID != "")
                {
                    CouponEntity couponEntity = new CouponEntity();
                    couponEntity = Mapper.Map<CouponEntity>(couponVM);
                    couponEntity.PartitionKey = nextCouponID;
                    couponEntity.Active = true;
                    couponEntity.CreatedBy = couponID;
                    couponEntity.CreatedTS = DateTime.UtcNow;
                    couponEntity.UpdatedBy = couponID;
                    couponEntity.UpdatedTS = DateTime.UtcNow;

                    TableOperation insert = TableOperation.Insert(couponEntity);
                    await couponTable.ExecuteAsync(insert);

                    TransactionLogDAL.InsertTransactionLog(couponEntity.PartitionKey, "Coupon", DateTime.UtcNow.Date, DateTime.UtcNow, couponID, "Added", couponName);
                    return new InternalOperationResult(Result.Success, "Coupon added successfully.", couponEntity.PartitionKey);
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = Newtonsoft.Json.JsonConvert.SerializeObject(couponVM);
                ExceptionTableStorage.InsertOrReplaceEntity("Master", "CouponDAL", "CreateAsync", DateTime.UtcNow, e, null, _requestJSON, couponID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> UpdateAsync(CouponViewModel couponVM, string couponName, string couponID)
        {
            try
            {
                CouponEntity couponEntity = (from c in couponTable.CreateQuery<CouponEntity>().Where(c => c.PartitionKey == couponVM.CouponID) select c).FirstOrDefault();
                if (couponEntity != null)
                {
                    if (couponVM.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff") == (couponEntity.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff")))
                    {
                        if (couponVM.ImageBuffer != null && couponVM.IsNew == true)
                        {
                            string imageGUID = Guid.NewGuid().ToString();
                            couponVM.ImageGUID = imageGUID;
                            await BlobStorage.UploadPublicImage(StatusEnum.Blob.couponimage.ToString(), couponVM.ImageBuffer, imageGUID, "image/jpg");
                        }
                        else
                        {
                            couponVM.ImageGUID = couponEntity.ImageGUID;
                        }

                        if (couponVM.IsNew == true && couponEntity.ImageGUID != null)
                        {
                            await BlobStorage.DeleteBlob(StatusEnum.Blob.couponimage.ToString(), couponEntity.ImageGUID);
                        }

                        Mapper.Initialize(a =>
                        {
                            a.CreateMap<CouponEntity, CouponViewModel>().ReverseMap();
                        });

                        CouponEntity uEntity = new CouponEntity(couponVM.CouponID);
                        uEntity = Mapper.Map<CouponEntity>(couponVM);
                        uEntity.UpdatedBy = couponID;
                        uEntity.UpdatedTS = DateTime.UtcNow;
                        uEntity.CreatedBy = couponEntity.CreatedBy;
                        uEntity.CreatedTS = couponEntity.CreatedTS;
                        uEntity.RowKey = couponEntity.RowKey;

                        TableOperation update = TableOperation.InsertOrReplace(uEntity);
                        await couponTable.ExecuteAsync(update);
                        TransactionLogDAL.InsertTransactionLog(uEntity.PartitionKey, "Coupon", DateTime.UtcNow.Date, DateTime.UtcNow, couponID, "Edited", couponName);
                        return new InternalOperationResult(Result.Success, "Coupon updated successfully.", uEntity.PartitionKey);
                    }
                    else
                    {
                        return new InternalOperationResult(Result.Concurrency, "Data is already changed by someone else. Please try again.", null);
                    }
                }
                else
                {
                    return new InternalOperationResult(Result.UDError, "Coupon doesn't exist or is deleted.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = Newtonsoft.Json.JsonConvert.SerializeObject(couponVM);
                ExceptionTableStorage.InsertOrReplaceEntity("Master", "CouponDAL", "UpdateAsync", DateTime.UtcNow, e, null, _requestJSON, couponID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> InactiveAsync(string couponID, CouponStatusChangeViewModel couponVM)
        {
            try
            {
                CouponEntity couponEntity = (from c in couponTable.CreateQuery<CouponEntity>().Where(c => c.PartitionKey == couponVM.CouponID) select c).FirstOrDefault();
                if (couponEntity != null)
                {
                    if (couponVM.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff") == (couponEntity.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff")))
                    {
                        List<int> designerEntity = CalendarAppointmentDAL.designerAppointmentTable.ExecuteQuery(new TableQuery<DesignerAppointmentEntity>()).
                                                                     Where(x => x.PartitionKey == couponEntity.PartitionKey).Select(x => x.AppointmentStatusID).ToList();

                        if (!designerEntity.Contains(1) && !designerEntity.Contains(2))
                        {

                            couponEntity.Active = couponVM.Active;
                            couponEntity.UpdatedBy = couponID;
                            couponEntity.UpdatedTS = DateTime.UtcNow;
                            TableOperation update = TableOperation.InsertOrReplace(couponEntity);
                            await couponTable.ExecuteAsync(update);
                            TransactionLogDAL.InsertTransactionLog(couponEntity.PartitionKey, "Coupon", DateTime.UtcNow.Date, DateTime.UtcNow, couponEntity.CreatedBy, "Delete", couponID);
                            return new InternalOperationResult(Result.Success, "Coupon updated successfully.", couponEntity.PartitionKey);
                        }
                        else
                        {
                            return new InternalOperationResult(Result.Success, "This coupon has scheduled appointments, hence this coupon can’t be inactive.", null);
                        }
                    }
                    else
                    {
                        return new InternalOperationResult(Result.Concurrency, "Data is already changed by someone else. Please try again.", null);
                    }

                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Coupon does not exist.", null);
                }

            }
            catch (Exception e)
            {
                ExceptionTableStorage.InsertOrReplaceEntity("Master", "CouponDAL", "DeleteAsync", DateTime.UtcNow, e, null, null, couponID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        #region Helper Method

        public static bool CheckGoogleID(string googleID)
        {
            List<string> couponList = couponTable.CreateQuery<CouponEntity>().Select(c => c.GoogleID).ToList();
            string couponID = couponList.Where(c => c.Split(new string[] { "@" }, StringSplitOptions.None).FirstOrDefault().Replace(".", "") == googleID.Split(new string[] { "@" }, StringSplitOptions.None).FirstOrDefault().Replace(".", "") && c.Split(new string[] { "@" }, StringSplitOptions.None)[1] == googleID.Split(new string[] { "@" }, StringSplitOptions.None)[1]).Select(c => c).FirstOrDefault();

            List<string> customerList = CustomerDAL.customerTable.CreateQuery<CustomerEntity>().Where(c => c.IsFacebookCoupon == false).Select(c => c.EmailID).ToList();
            string customerID = customerList.Where(c => c.Split(new string[] { "@" }, StringSplitOptions.None).FirstOrDefault().Replace(".", "") == googleID.Split(new string[] { "@" }, StringSplitOptions.None).FirstOrDefault().Replace(".", "") && c.Split(new string[] { "@" }, StringSplitOptions.None)[1] == googleID.Split(new string[] { "@" }, StringSplitOptions.None)[1]).Select(c => c).FirstOrDefault();

            if (couponID == null && customerID == null)
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



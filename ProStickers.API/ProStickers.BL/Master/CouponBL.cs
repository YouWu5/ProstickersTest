using ProStickers.BL.Core;
using ProStickers.CloudDAL.Storage;
using ProStickers.CloudDAL.Storage.Master;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Master;
using ProStickers.ViewModel.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProStickers.BL.Master
{
    public class CouponBL
    {
        public static async Task<Pager<CouponListViewModel>> GetListAsync<CouponListViewModel>(string couponID)
        {
            Pager<CouponListViewModel> pager = PagerOperations.InitializePager<CouponListViewModel>
                                                        ("UpdatedTS", SortDirection.DESC.ToString(),
                                                          true, true, true, 30, true);

            pager.ColumnList = new List<ListItem>()
            {
                new ListItem { Text = "Name", Value = "FullName" },
                new ListItem { Text = "Google id", Value = "GoogleID" },
                new ListItem { Text = "Role", Value = "CouponType" }
            };
            await CouponDAL.GetListAsync(couponID, pager);
            return pager;
        }

        public static async Task<OperationResult> GetListAsync<CouponListViewModel>(string couponID, Pager<CouponListViewModel> pager)
        {
            if (pager != null)
            {
                PagerOperations.UpdatePager(pager);
                await CouponDAL.GetListAsync(couponID, pager);
                return new OperationResult(Result.Success, "", pager);
            }
            else
            {
                return new OperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<OperationResult> GetByIDAsync(string loggedInCouponID, string couponID)
        {
            if (couponID != null)
            {
                InternalOperationResult result = await CouponDAL.GetByIDAsync(loggedInCouponID, couponID);
                if (result.Result == Result.Success)
                {
                    CouponViewModel couponVM = (result.ReturnedData as CouponViewModel);
                    if (couponVM.ImageGUID != null && couponVM.ImageGUID != "")
                    {
                        couponVM.ImageGUID = BlobStorage.DownloadBlobUri(StatusEnum.Blob.couponimage.ToString(), couponVM.ImageGUID);
                    }
                }
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> CreateAsync(CouponViewModel couponVM, CouponInfo couponInfo)
        {
            if (couponVM != null && couponInfo != null)
            {
                if (CouponDAL.CheckGoogleID(couponVM.GoogleID))
                {
                    InternalOperationResult result = await CouponDAL.CreateAsync(couponVM, couponInfo.CouponID, couponInfo.CouponName);
                    return new OperationResult(result.Result, result.Message, result.ReturnedData);
                }
                else
                {
                    return new OperationResult(Result.UDError, "Google ID already exist.", null);
                }
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> UpdateAsync(CouponViewModel couponVM, CouponInfo couponInfo)
        {
            if (couponVM != null && couponInfo != null)
            {
                InternalOperationResult result = await CouponDAL.UpdateAsync(couponVM, couponInfo.CouponName, couponInfo.CouponID);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<List<ListItemTypes>> GetCouponTypeListAsync()
        {
            List<ListItemTypes> list = new List<ListItemTypes>();

            list.Add(new ListItemTypes { Text = "Master", Value = 1 });
            list.Add(new ListItemTypes { Text = "Designer", Value = 2 });
            return await Task.FromResult(list);
        }

        public static async Task<CouponViewModel> GetDefaultViewModelAsync()
        {
            CouponViewModel vm = new CouponViewModel();
            return await Task.FromResult(vm);
        }

        public static async Task<OperationResult> InactiveAsync(CouponInfo couponInfo, CouponStatusChangeViewModel couponVM)
        {
            if (couponInfo != null && couponVM != null)
            {
                InternalOperationResult result = await CouponDAL.InactiveAsync(couponInfo.CouponID, couponVM);
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }
    }
}

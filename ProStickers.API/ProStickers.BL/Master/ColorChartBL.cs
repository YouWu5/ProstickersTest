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
    public class ColorChartBL
    {
        public static async Task<Pager<ColorChartListViewModel>> GetListAsync<ColorChartListViewModel>()
        {
            Pager<ColorChartListViewModel> pager = PagerOperations.InitializePager<ColorChartListViewModel>
                                                        ("UpdatedTS", SortDirection.DESC.ToString(),
                                                          true, true, true, 30, true);

            pager.ColumnList = new List<ListItem>()
            {
                new ListItem { Text = "Name", Value = "Name" },
                new ListItem { Text = "Allow taking order", Value = "IsAllowForSale" },
            };

            await ColorChartDAL.GetListAsync(pager);
            return pager;
        }

        public static async Task<OperationResult> GetListAsync<ColorChartListViewModel>(Pager<ColorChartListViewModel> pager)
        {
            if (pager != null)
            {
                PagerOperations.UpdatePager<ColorChartListViewModel>(pager);
                await ColorChartDAL.GetListAsync(pager);
                return new OperationResult(Result.Success, "", pager);
            }
            else
            {
                return new OperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<ColorChartViewModel> GetDefaultViewModelAsync()
        {
            ColorChartViewModel vm = new ColorChartViewModel();
            return await Task.FromResult(vm);
        }

        public static async Task<OperationResult> GetByIDAsync(string colorID, UserInfo currentUserInfo)
        {
            if (colorID != null && colorID != "")
            {
                InternalOperationResult result = await ColorChartDAL.GetByIDAsync(colorID, currentUserInfo.UserID);
                if (result.Result == Result.Success)
                {
                    ColorChartViewModel colorChartVM = (result.ReturnedData as ColorChartViewModel);
                    if (colorChartVM.ImageGUID != null && colorChartVM.ImageGUID != "")
                    {
                        DownloadImageViewModel vm = BlobStorage.DownloadBlobByteArray(StatusEnum.Blob.colorimage.ToString(), colorChartVM.ImageGUID);
                        if (vm != null)
                        {
                            colorChartVM.ImageBuffer = vm.ImageBuffer;
                            colorChartVM.ImageExtension = vm.FileExtension;
                        }
                    }
                }
                return new OperationResult(result.Result, result.Message, result.ReturnedData);
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request,color doesn't exist.", null);
            }
        }

        public static async Task<OperationResult> CreateAsync(ColorChartViewModel vm, UserInfo currentUserInfo)
        {
            if (vm != null)
            {
                bool res = await ColorChartDAL.ValidateColorOnCreateAsync(vm.Name);
                if (res == true)
                {
                    InternalOperationResult result = await ColorChartDAL.CreateAsync(vm, currentUserInfo.UserID, currentUserInfo.UserName);
                    return new OperationResult(result.Result, result.Message, result.ReturnedData);
                }
                else
                {
                    return new OperationResult(Result.UDError, "Color name should be unique.", null);
                }
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request.", null);
            }
        }

        public static async Task<OperationResult> UpdateAsync(ColorChartViewModel vm, UserInfo currentUserInfo)
        {
            if (vm != null)
            {
                bool res = await ColorChartDAL.ValidateColorOnUpdateAsync(vm.ColorID, vm.Name);
                if (res == true)
                {
                    InternalOperationResult result = await ColorChartDAL.UpdateAsync(vm, currentUserInfo.UserID, currentUserInfo.UserName);
                    return new OperationResult(result.Result, result.Message, result.ReturnedData);
                }
                else
                {
                    return new OperationResult(Result.UDError, "Color name should be unique.", null);
                }
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request,color doesn't exist.", null);
            }
        }

        public static async Task<OperationResult> DeleteAsync(string colorID, string name, DateTime updatedTS, UserInfo currentUserInfo)
        {
            if (colorID != null)
            {
                bool res = await ColorChartDAL.ValidateColorOnDeleteAsync(name);
                if (!res)
                {
                    InternalOperationResult result = await ColorChartDAL.DeleteAsync(colorID, name, updatedTS, currentUserInfo.UserID, currentUserInfo.UserName);
                    return new OperationResult(result.Result, result.Message, result.ReturnedData);
                }
                else
                {
                    return new OperationResult(Result.UDError, "This color is used in order, can't be deleted.", null);
                }
            }
            else
            {
                return new OperationResult(Result.UDError, "Bad request,color doesn't exist.", null);
            }
        }
    }
}

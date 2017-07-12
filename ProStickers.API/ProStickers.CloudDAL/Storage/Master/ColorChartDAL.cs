using AutoMapper;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using ProStickers.CloudDAL.Entity.Customer;
using ProStickers.CloudDAL.Entity.Master;
using ProStickers.CloudDAL.Storage.Customer;
using ProStickers.CloudDAL.Storage.ExceptionStorage;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using ProStickers.ViewModel.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProStickers.CloudDAL.Storage.Master
{
    public class ColorChartDAL
    {
        public static CloudTable colorChartTable;

        static ColorChartDAL()
        {
            colorChartTable = Utility.GetStorageTable("ColorChart");
        }

        public static async Task GetListAsync<ColorChartListViewModel>(Pager<ColorChartListViewModel> pagerVM)
        {
            TableQuery<ColorChartEntity> listQuery = new TableQuery<ColorChartEntity>();

            List<ColorChartEntity> colorChartList = colorChartTable.ExecuteQuery(listQuery).ToList();

            if (colorChartList != null && colorChartList.Count() > 0)
            {
                pagerVM.RecordsCount = colorChartList.Count();

                if (pagerVM.Sort == "Name")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        colorChartList = colorChartList.OrderByDescending(s => s.Name).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<ColorChartEntity>();
                    }
                    else
                    {
                        colorChartList = colorChartList.OrderBy(s => s.Name).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<ColorChartEntity>();
                    }
                }
                else if (pagerVM.Sort == "IsAllowForSale")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        colorChartList = colorChartList.OrderByDescending(s => s.IsAllowForSale).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<ColorChartEntity>();
                    }
                    else
                    {
                        colorChartList = colorChartList.OrderBy(s => s.IsAllowForSale).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<ColorChartEntity>();
                    }
                }
                else if (pagerVM.Sort == "UpdatedTS")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        colorChartList = colorChartList.OrderByDescending(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<ColorChartEntity>();
                    }
                    else
                    {
                        colorChartList = colorChartList.OrderBy(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<ColorChartEntity>();
                    }
                }
                else
                {
                    colorChartList = colorChartList.Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<ColorChartEntity>();
                }
            }

            Mapper.Initialize(a =>
            {
                a.CreateMap<ViewModel.Master.ColorChartListViewModel, ColorChartEntity>().ReverseMap()
                 .ForMember(x => x.ColorID, y => y.MapFrom(z => z.PartitionKey));
            });

            await Task.FromResult(pagerVM.Data = Mapper.Map<List<ColorChartListViewModel>>(colorChartList));
        }

        public static async Task<InternalOperationResult> GetByIDAsync(string colorID, string userID)
        {
            try
            {
                ColorChartViewModel colorChartVM = new ColorChartViewModel();

                Mapper.Initialize(a =>
                {
                    a.CreateMap<ColorChartViewModel, ColorChartEntity>().ReverseMap()
                     .ForMember(x => x.ColorID, y => y.MapFrom(z => z.PartitionKey));
                });

                colorChartVM = Mapper.Map<ColorChartViewModel>(colorChartTable.CreateQuery<ColorChartEntity>().Where(e => e.PartitionKey == colorID).FirstOrDefault());

                if (colorChartVM != null)
                {
                    return await Task.FromResult(new InternalOperationResult(Result.Success, "", colorChartVM));
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Oop's something went wrong. Please try again.", null);
                }
            }
            catch (Exception e)
            {
                ExceptionTableStorage.InsertOrReplaceEntity("Master", "ColorChartDAL", "GetByIDAsync", DateTime.UtcNow, e, null, colorID, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong. Please try again.", null);
            }
        }

        public static async Task<InternalOperationResult> CreateAsync(ColorChartViewModel vm, string userID, string userName)
        {
            try
            {
                if (vm.ImageBuffer != null)
                {
                    string imageGUID = Guid.NewGuid().ToString();
                    vm.ImageGUID = imageGUID;
                    await BlobStorage.UploadPublicImage(StatusEnum.Blob.colorimage.ToString(), vm.ImageBuffer, imageGUID, await CommonDAL.GetContentType(vm.ImageExtension));
                }

                Mapper.Initialize(a =>
                {
                    a.CreateMap<ColorChartViewModel, ColorChartEntity>().ReverseMap();
                });

                ColorChartEntity colorChartEntity = Mapper.Map<ColorChartEntity>(vm);

                string colorID = Utility.GetNextId("ColorID");

                if (colorID != null && colorID != "")
                {
                    colorChartEntity.PartitionKey = colorID;
                    colorChartEntity.RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
                    colorChartEntity.CreatedBy = userID;
                    colorChartEntity.CreatedTS = DateTime.UtcNow;
                    colorChartEntity.UpdatedBy = userID;
                    colorChartEntity.UpdatedTS = DateTime.UtcNow;

                    TableOperation insert = TableOperation.InsertOrReplace(colorChartEntity);
                    await colorChartTable.ExecuteAsync(insert);

                    TransactionLogDAL.InsertTransactionLog(colorChartEntity.PartitionKey, "ColorChart", DateTime.UtcNow.Date, DateTime.UtcNow, colorChartEntity.CreatedBy, "Added", userName);

                    return new InternalOperationResult(Result.Success, "Color added successfully in color chart.", colorChartEntity.PartitionKey);
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = Newtonsoft.Json.JsonConvert.SerializeObject(vm);
                ExceptionTableStorage.InsertOrReplaceEntity("Master", "ColorChartDAL", "CreateAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> UpdateAsync(ColorChartViewModel vm, string userID, string userName)
        {
            try
            {
                ColorChartEntity ccEntity = (from c in colorChartTable.CreateQuery<ColorChartEntity>().Where(c => c.PartitionKey == vm.ColorID) select c).FirstOrDefault();
                if (ccEntity != null)
                {
                    if (ccEntity.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff") == (vm.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff")))
                    {
                        if (vm.ImageBuffer != null && vm.IsNew == true)
                        {
                            string imageGUID = Guid.NewGuid().ToString();
                            vm.ImageGUID = imageGUID;
                            await BlobStorage.UploadPublicImage(StatusEnum.Blob.colorimage.ToString(), vm.ImageBuffer, imageGUID, await CommonDAL.GetContentType(vm.ImageExtension));
                        }
                        else
                        {
                            vm.ImageGUID = ccEntity.ImageGUID;
                        }

                        if (vm.IsNew == true && ccEntity.ImageGUID != null)
                        {
                            await BlobStorage.DeleteBlob(StatusEnum.Blob.colorimage.ToString(), ccEntity.ImageGUID);
                        }

                        Mapper.Initialize(a =>
                        {
                            a.CreateMap<ColorChartViewModel, ColorChartEntity>().ReverseMap();
                        });

                        ColorChartEntity colorChartEntity = Mapper.Map<ColorChartEntity>(vm);

                        colorChartEntity.PartitionKey = ccEntity.PartitionKey;
                        colorChartEntity.RowKey = ccEntity.RowKey;
                        colorChartEntity.CreatedBy = ccEntity.CreatedBy;
                        colorChartEntity.CreatedTS = ccEntity.CreatedTS;
                        colorChartEntity.UpdatedBy = userID;
                        colorChartEntity.UpdatedTS = DateTime.UtcNow;

                        TableOperation update = TableOperation.InsertOrReplace(colorChartEntity);
                        await colorChartTable.ExecuteAsync(update);

                        TransactionLogDAL.InsertTransactionLog(colorChartEntity.PartitionKey, "ColorChart", DateTime.UtcNow.Date, DateTime.UtcNow, colorChartEntity.CreatedBy, "Edited", userName);

                        return new InternalOperationResult(Result.Success, "Color Updated successfully in color chart.", colorChartEntity.PartitionKey);
                    }
                    else
                    {
                        return new InternalOperationResult(Result.Concurrency, "Data is already changed by someone else. Please try again.", null);
                    }
                }
                else
                {
                    return new InternalOperationResult(Result.UDError, "Color doesn't exist or is deleted.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = Newtonsoft.Json.JsonConvert.SerializeObject(vm);
                ExceptionTableStorage.InsertOrReplaceEntity("Master", "ColorChartDAL", "UpdateAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> DeleteAsync(string colorID, string name, DateTime updatedTS, string userID, string userName)
        {
            try
            {
                TableOperation delete = null;
                ColorChartEntity ccEntity = (from c in colorChartTable.CreateQuery<ColorChartEntity>().Where(c => c.PartitionKey == colorID) select c).FirstOrDefault();
                if (ccEntity != null)
                {
                    if (ccEntity.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff") == (updatedTS.ToString("dd-MM-yyyHH:mm:ss.fff")))
                    {
                        delete = TableOperation.Delete(ccEntity);
                        if (ccEntity.ImageGUID != null)
                        {
                            await BlobStorage.DeleteBlob(StatusEnum.Blob.colorimage.ToString(), ccEntity.ImageGUID);
                        }

                        await colorChartTable.ExecuteAsync(delete);

                        TransactionLogDAL.InsertTransactionLog(colorID, "ColorChart", DateTime.UtcNow.Date, DateTime.UtcNow, ccEntity.CreatedBy, "Deleted", userName);

                        return new InternalOperationResult(Result.Success, "Color deleted successfully from color chart.", ccEntity.PartitionKey);
                    }
                    else
                    {
                        return new InternalOperationResult(Result.Concurrency, "Data is already changed by someone else. Please try again.", null);
                    }
                }
                else
                {
                    return new InternalOperationResult(Result.UDError, "Color doesn't exist or is deleted.", null);
                }
            }
            catch (Exception e)
            {
                ExceptionTableStorage.InsertOrReplaceEntity("Master", "ColorChartDAL", "DeleteAsync", DateTime.UtcNow, e, null, colorID, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        #region Helper Methods

        public static async Task<bool> ValidateColorOnCreateAsync(string name)
        {
            ColorChartEntity cEntity = colorChartTable.CreateQuery<ColorChartEntity>().Where(x => x.Name == name).Select(x => x).FirstOrDefault();

            if (cEntity == null)
            {
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }

        public static async Task<bool> ValidateColorOnUpdateAsync(string colorID, string name)
        {
            ColorChartEntity cEntity = colorChartTable.CreateQuery<ColorChartEntity>().Where(x => x.PartitionKey != colorID && x.Name == name).Select(x => x).FirstOrDefault();

            if (cEntity == null)
            {
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }

        public static async Task<bool> ValidateColorOnDeleteAsync(string name)
        {
            bool res = false;
            List<string> ColorJsonList = OrderDAL.orderTable.CreateQuery<OrderEntity>().Select(o => o.ColorJSON).ToList();

            foreach (var item in ColorJsonList)
            {
                List<ColorViewModel> Color = JsonConvert.DeserializeObject<List<ColorViewModel>>(item) as List<ColorViewModel>;
                if (Color.Select(c => c.Name).Contains(name))
                {
                    res = true;
                    break;
                }
            }
            return await Task.FromResult(res);
        }

        #endregion
    }
}

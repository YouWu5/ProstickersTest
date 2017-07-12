using System;
using System.Linq;
using System.Threading.Tasks;
using ProStickers.ViewModel.Master;
using ProStickers.CloudDAL.Entity.Master;
using AutoMapper;
using Microsoft.WindowsAzure.Storage.Table;
using ProStickers.ViewModel.Core;
using ProStickers.CloudDAL.Storage.ExceptionStorage;

namespace ProStickers.CloudDAL.Storage.Master
{
    public class PredefinedSizeDAL
    {
        public static CloudTable predefinedSizeTable;

        static PredefinedSizeDAL()
        {
            predefinedSizeTable = Utility.GetStorageTable("PredefinedSize");
        }

        public static async Task<InternalOperationResult> GetByIDAsync()
        {
            PredefinedSizeEntity sizeEntity = predefinedSizeTable.ExecuteQuery(new TableQuery<PredefinedSizeEntity>()).FirstOrDefault();
            if (sizeEntity != null)
            {
                Mapper.Initialize(a =>
            {
                a.CreateMap<PredefinedSizeViewModel, PredefinedSizeEntity>().ReverseMap().
                 ForMember(x => x.PredefinedSizeID, y => y.MapFrom(z => z.PartitionKey));
            });

                PredefinedSizeViewModel sizeVM = Mapper.Map<PredefinedSizeViewModel>(sizeEntity);
                return await Task.FromResult(new InternalOperationResult(Result.Success, "", sizeVM));
            }
            else
            {
                return new InternalOperationResult(Result.UDError, "Predefined size record does'nt exists.", null);
            }
        }

        public static async Task<InternalOperationResult> UpdateAsync(PredefinedSizeViewModel sizeVM, string userName, string userID)
        {
            try
            {
                PredefinedSizeEntity sizeEntity = (from c in predefinedSizeTable.CreateQuery<PredefinedSizeEntity>().
                                                   Where(c => c.PartitionKey == sizeVM.PredefinedSizeID) select c).FirstOrDefault();
                if (sizeEntity != null)
                {
                    if (sizeVM.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff") == (sizeEntity.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff")))
                    {
                        Mapper.Initialize(a =>
                        {
                            a.CreateMap<PredefinedSizeEntity, PredefinedSizeViewModel>().ReverseMap();
                        });

                        PredefinedSizeEntity sEntity = Mapper.Map<PredefinedSizeEntity>(sizeVM);
                        sEntity.PartitionKey = sizeEntity.PartitionKey;
                        sEntity.RowKey = sizeEntity.RowKey;
                        sEntity.CreatedBy = sizeEntity.CreatedBy;
                        sEntity.CreatedTS = sizeEntity.CreatedTS;
                        sEntity.UpdatedBy = userID;
                        sEntity.UpdatedTS = DateTime.UtcNow;

                        TableOperation update = TableOperation.InsertOrReplace(sEntity);
                        await predefinedSizeTable.ExecuteAsync(update);

                        TransactionLogDAL.InsertTransactionLog(sizeEntity.PartitionKey, "PredefinedSize", DateTime.UtcNow.Date, DateTime.UtcNow, sizeEntity.CreatedBy, "Edited", userName);
                        return new InternalOperationResult(Result.Success, "Pre defined size updated successfully.", sizeEntity.PartitionKey);
                    }
                    else
                    {
                        return new InternalOperationResult(Result.Concurrency, "Data is already changed by someone else. Please try again.", null);
                    }
                }
                else
                {
                    return new InternalOperationResult(Result.UDError, "Data doesn't exist or is deleted.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = Newtonsoft.Json.JsonConvert.SerializeObject(sizeVM);
                ExceptionTableStorage.InsertOrReplaceEntity("Master", "PredefinedSizeDAL", "UpdateAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }
    }
}

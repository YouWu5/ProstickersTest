using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.WindowsAzure.Storage.Table;
using ProStickers.CloudDAL.Entity.Customer;
using ProStickers.CloudDAL.Storage.ExceptionStorage;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using static ProStickers.ViewModel.Core.StatusEnum;

namespace ProStickers.CloudDAL.Storage.Customer
{
    public class DesignDAL
    {
        public static async Task GetListAsync<DesignViewModel>(Pager<DesignViewModel> pagerVM, string customerID)
        {
            List<ViewModel.Customer.DesignViewModel> designList = CalendarAppointmentDAL.customerAppointmentTable.ExecuteQuery(new TableQuery<CustomerAppointmentEntity>()).
                                               Where(x => x.PartitionKey == customerID && x.AppointmentStatusID == (int)StatusEnum.AppointmentStatus.Completed && (x.DesignNumber != null && x.DesignNumber != "")).
                                               Select(c => new ViewModel.Customer.DesignViewModel
                                               {
                                                   DesignNumber = c.DesignNumber,
                                                   UpdatedTS = c.UpdatedTS,
                                                   IsAllowForPurchase = c.StatusID > 0 && c.StatusID < (int)StatusEnum.Status.OrderCreated ? true : false,
                                                   AppointmentNumber = c.AppointmentNumber,
                                                   DesignImage = BlobStorage.DownloadBlobByteArray(StatusEnum.Blob.designimage.ToString(), c.DesignNumber),
                                                   IsBuyEnable = c.ImageStatusID == 2 || c.ImageStatusID == 3 ? true : false
                                               }).ToList();

            if (designList != null && designList.Count() > 0)
            {
                pagerVM.RecordsCount = designList.Count();

                if (pagerVM.Sort == "UpdatedTS")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        designList = designList.OrderByDescending(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        designList = designList.OrderBy(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
            }
            Mapper.Initialize(a =>
            {
                a.CreateMap<ViewModel.Customer.DesignViewModel, DesignViewModel>().ReverseMap();
            });

            await Task.FromResult(pagerVM.Data = Mapper.Map<List<DesignViewModel>>(designList));
        }

        public static async Task<InternalOperationResult> GetByIDAsync(string customerID, string designNo)
        {
            CustomerAppointmentEntity customerEntity = CalendarAppointmentDAL.customerAppointmentTable.ExecuteQuery(new TableQuery<CustomerAppointmentEntity>()).
                                                       Where(x => x.PartitionKey == customerID && x.DesignNumber == designNo).
                                                       FirstOrDefault();
            if (customerEntity != null)
            {
                Mapper.Initialize(a =>
                {
                    a.CreateMap<CustomerAppointmentViewModel, CustomerAppointmentEntity>().ReverseMap();
                });

                CustomerAppointmentViewModel appointmentVM = Mapper.Map<CustomerAppointmentViewModel>(customerEntity);

                DownloadImageViewModel vm = BlobStorage.DownloadBlobByteArray(Blob.designimage.ToString(), appointmentVM.DesignNumber);
                if (vm != null)
                {
                    appointmentVM.ImageBuffer = vm.ImageBuffer;
                    appointmentVM.ImageExtension = vm.FileExtension;
                }
                
                if (customerEntity.ImageStatusID == 2 || customerEntity.ImageStatusID == 3)
                {
                    appointmentVM.IsBuyEnable = true;
                }

                List<OrderEntity> orderList = OrderDAL.orderTable.ExecuteQuery(new TableQuery<OrderEntity>()).Where(x => x.AppointmentNumber == appointmentVM.AppointmentNumber && x.DesignNumber == appointmentVM.DesignNumber).ToList();

                if (orderList != null && orderList.Count() > 0)
                {
                    if (orderList.Select(x => x.PurchaseTypeID).Contains((int)StatusEnum.PurchaseType.VectorFile) || orderList.Select(x => x.PurchaseTypeID).Contains((int)StatusEnum.PurchaseType.Both))
                    {
                        appointmentVM.IsDownloadEnable = true;
                    }

                    if (orderList.Select(x => x.OrderStatusID).Contains((int)StatusEnum.OrderStatus.Placed))
                    {
                        appointmentVM.IsDeleteEnable = false;
                    }
                    else if (orderList.Select(x => x.PurchaseTypeID).Contains((int)StatusEnum.PurchaseType.DesignSticker) || orderList.Select(x => x.PurchaseTypeID).Contains((int)StatusEnum.PurchaseType.Both))
                    {
                        if (vm != null)
                        {
                            appointmentVM.IsDeleteEnable = true;
                        }
                        else
                        {
                            appointmentVM.IsDeleteEnable = false;
                        }
                    }
                    else
                    {
                        appointmentVM.IsDeleteEnable = false;
                    }
                }

                return await Task.FromResult(new InternalOperationResult(Result.Success, "", appointmentVM));
            }
            else
            {
                return await Task.FromResult(new InternalOperationResult(Result.UDError, "Design does not exists.", null));
            }
        }

        public static async Task<InternalOperationResult> DeleteAsync(string customerID, string designNo, DateTime updatedTS)
        {
            try
            {
                CustomerAppointmentEntity customerEntity = CalendarAppointmentDAL.customerAppointmentTable.ExecuteQuery(new TableQuery<CustomerAppointmentEntity>()).
                                                        Where(x => x.PartitionKey == customerID && x.DesignNumber == designNo).
                                                        FirstOrDefault();

                if (updatedTS.ToString("dd-MM-yyyHH:mm:ss.fff") == (customerEntity.UpdatedTS.ToString("dd-MM-yyyHH:mm:ss.fff")))
                {
                    List<OrderEntity> orderList = OrderDAL.orderTable.ExecuteQuery(new TableQuery<OrderEntity>()).Where(x => x.AppointmentNumber == customerEntity.AppointmentNumber && x.DesignNumber == customerEntity.DesignNumber).ToList();

                    if (!orderList.Select(x => x.OrderStatusID).Contains((int)StatusEnum.OrderStatus.Placed))
                    {
                        if (orderList.Select(x => x.PurchaseTypeID).Contains((int)StatusEnum.PurchaseType.DesignSticker) || orderList.Select(x => x.PurchaseTypeID).Contains((int)StatusEnum.PurchaseType.Both))
                        {
                            await BlobStorage.DeleteBlob(StatusEnum.Blob.designimage.ToString(), customerEntity.DesignNumber);
                            return new InternalOperationResult(Result.Success, "Design deleted successfully.", customerEntity.DesignNumber);
                        }
                        else
                        {
                            return new InternalOperationResult(Result.UDError, "Design does not exists.", null);
                        }
                    }
                    else
                    {
                        return new InternalOperationResult(Result.SDError, "Design order in process , can not delete.", null);
                    }
                }
                else
                {
                    return new InternalOperationResult(Result.Concurrency, "Data is already changed by someone else. Please try again.", null);
                }
            }
            catch (Exception e)
            {
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", "DesignDAL", "DeleteAsync", DateTime.UtcNow, e, null, designNo, customerID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }
    }
}

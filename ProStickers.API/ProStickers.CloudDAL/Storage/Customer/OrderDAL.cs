using AutoMapper;
using Microsoft.WindowsAzure.Storage.Table;
using ProStickers.CloudDAL.Entity.Customer;
using ProStickers.CloudDAL.Entity.Designer;
using ProStickers.CloudDAL.Entity.Master;
using ProStickers.CloudDAL.Storage.ExceptionStorage;
using ProStickers.CloudDAL.Storage.Master;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProStickers.CloudDAL.Storage.Customer
{
    public class OrderDAL
    {
        public static CloudTable orderTable;
        public static CloudTable customerOrderTable;
        public static CloudTable designerOrderTable;
        public static CloudTable designerAppointmentDetailTable;

        static OrderDAL()
        {
            orderTable = Utility.GetStorageTable("Order");
            customerOrderTable = Utility.GetStorageTable("CustomerOrder");
            designerOrderTable = Utility.GetStorageTable("DesignerOrder");
            designerAppointmentDetailTable = Utility.GetStorageTable("DesignerAppointmentDetail");
        }

        public static async Task<InternalOperationResult> GetDetailAsync(string designNo, string appointmentNo, string userID)
        {
            try
            {
                OrderViewModel orderVM = new OrderViewModel();

                CustomerEntity cEntity = CustomerDAL.customerTable.ExecuteQuery(new TableQuery<CustomerEntity>()).Where(x => x.PartitionKey == userID).FirstOrDefault();

                if (cEntity != null)
                {
                    orderVM.Address1 = (cEntity.Address1 != null && cEntity.Address1 != "") ? cEntity.Address1 : null;
                    orderVM.Address2 = (cEntity.Address2 != null && cEntity.Address2 != "") ? cEntity.Address2 : null;
                    orderVM.City = (cEntity.City != null && cEntity.City != "") ? cEntity.City : null;
                    orderVM.StateID = cEntity.StateID;
                    orderVM.StateName = (cEntity.StateName != null && cEntity.StateName != "") ? cEntity.StateName : null;
                    orderVM.CountryID = cEntity.CountryID;
                    orderVM.CountryName = (cEntity.CountryName != null && cEntity.CountryName != "") ? cEntity.CountryName : null;
                    orderVM.PostalCode = (cEntity.PostalCode != null && cEntity.PostalCode != "") ? cEntity.PostalCode : null;
                    orderVM.EmailID = (cEntity.EmailID != null && cEntity.EmailID != "") ? cEntity.EmailID : null;
                }

                CustomerAppointmentEntity cappEntity = CalendarAppointmentDAL.customerAppointmentTable.ExecuteQuery(new TableQuery<CustomerAppointmentEntity>()).Where(x => x.PartitionKey == userID && x.RowKey == appointmentNo && x.DesignNumber == designNo && x.AppointmentStatusID != (int)StatusEnum.AppointmentStatus.Cancelled)
                                                      .Select(x => x).FirstOrDefault();

                if (cappEntity != null)
                {
                    orderVM.UserID = cappEntity.UserID;
                    orderVM.UserName = cappEntity.UserName;
                    orderVM.AppointmentNumber = cappEntity.RowKey;
                    orderVM.DesignNumber = cappEntity.DesignNumber;
                    orderVM.AspectRatio = cappEntity.AspectRatio;
                }

                PredefinedSizeEntity psEntity = PredefinedSizeDAL.predefinedSizeTable.ExecuteQuery(new TableQuery<PredefinedSizeEntity>()).Select(x => x).FirstOrDefault();

                if (psEntity != null)
                {
                    orderVM.OneColorPrice = psEntity.OneColorPrice;
                    orderVM.TwoColorPrice = psEntity.TwoColorPrice;
                    orderVM.MoreColorPrice = psEntity.MoreColorPrice;
                }

                orderVM.ColorList = await GetColorListAsync();

                OrderEntity oEntity = orderTable.CreateQuery<OrderEntity>().Where(o => o.DesignNumber == designNo && o.AppointmentNumber == appointmentNo).Select(o
                   => o).FirstOrDefault();

                if (oEntity != null)
                {
                    orderVM.PurchaseTypeID = oEntity.PurchaseTypeID == (int)StatusEnum.PurchaseType.Both ? (int)StatusEnum.PurchaseType.Both : (oEntity.PurchaseTypeID == (int)StatusEnum.PurchaseType.DesignSticker ? (int)StatusEnum.PurchaseType.DesignSticker : (int)StatusEnum.PurchaseType.VectorFile);
                }

                if (orderVM != null)
                {
                    return await Task.FromResult(new InternalOperationResult(Result.Success, "", orderVM));
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Oop's something went wrong. Please try again.", null);
                }
            }
            catch (Exception e)
            {
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", "OrderDAL", "GetDetailAsync", DateTime.UtcNow, e, null, null, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong. Please try again.", null);
            }
        }

        public static async Task<InternalOperationResult> CreateAsync(OrderViewModel vm, string userID, string userName)
        {
            try
            {
                if (vm != null && vm.ColorList != null && vm.ColorList.Count > 0)
                {
                    int _colorCount = 0;
                    TableOperation insertOrder = null, appointmentUpdate = null, customerAppointmentUpdate = null, designerAppointmentUpdate = null;

                    Mapper.Initialize(a =>
                    {
                        a.CreateMap<OrderViewModel, OrderEntity>();
                    });
                    OrderEntity orderEntity = Mapper.Map<OrderEntity>(vm);

                    List<ColorViewModel> clist = new List<ColorViewModel>();
                    clist = vm.ColorList.Where(x => x.IsSelected == true).Select(x => x).ToList();
                    foreach (var item in clist)
                    {
                        item.ImageURL = null;
                    }
                    _colorCount = clist.Count();
                    if (_colorCount <= 5 && _colorCount > 0)
                    {
                        orderEntity.ColorJSON = Newtonsoft.Json.JsonConvert.SerializeObject(clist).ToString();

                        CustomerAppointmentEntity caEntity = CalendarAppointmentDAL.customerAppointmentTable.ExecuteQuery(new TableQuery<CustomerAppointmentEntity>()).Where(x => x.PartitionKey == userID && x.DesignNumber == vm.DesignNumber && x.AppointmentNumber == vm.AppointmentNumber && x.AppointmentStatusID != (int)StatusEnum.AppointmentStatus.Cancelled).Select(x => x).FirstOrDefault();

                        if (caEntity != null)
                        {
                            string orderNumber = Utility.GetNextId("OrderNumber");
                            double lengthorwidth = 0.0, predefinedDimension = 0.0, vectorFilePrice = 0.0, designImagePrice = 0.0, dimension = 0.0;
                            TableOperation icustomerOrderOperation = null, idesignerOrderOperation = null, insertOperation = null;

                            if (orderNumber != null && orderNumber != "")
                            {
                                string customerCodeName = CustomerDAL.customerTable.ExecuteQuery(new TableQuery<CustomerEntity>()).Where(x => x.PartitionKey == userID).Select(x => x.CustomerCodeName).FirstOrDefault();

                                orderEntity.PartitionKey = vm.OrderDate.ToString("yyyy-MM-dd");
                                orderEntity.RowKey = orderNumber;
                                orderEntity.CreatedBy = userID;
                                orderEntity.CreatedTS = DateTime.UtcNow;
                                orderEntity.UpdatedBy = userID;
                                orderEntity.UpdatedTS = DateTime.UtcNow;
                                orderEntity.CustomerID = userID;
                                vm.CustomerID = orderEntity.CustomerID;
                                orderEntity.CustomerName = userName;
                                vm.CustomerName = orderEntity.CustomerName;
                                orderEntity.CustomerCodeName = customerCodeName;
                                orderEntity.OrderStatusID = Convert.ToInt32(StatusEnum.OrderStatus.Placed);
                                orderEntity.OrderStatus = StatusEnum.OrderStatus.Placed.ToString();
                                orderEntity.StatusID = Convert.ToInt32(StatusEnum.Status.OrderCreated);
                                orderEntity.OrderNumber = Convert.ToInt32(orderNumber);
                                vm.OrderNumber = orderEntity.OrderNumber;

                                if (vm.PurchaseVectorFile == true)
                                {
                                    vectorFilePrice = 30;
                                    orderEntity.VectorFilePrice = vectorFilePrice;
                                    vm.VectorFilePrice = orderEntity.VectorFilePrice;
                                }
                                if (vm.PurchaseDesignImage == true)
                                {
                                    if (vm.Length > 0)
                                    {
                                        orderEntity.Length = vm.Length;
                                        lengthorwidth = Math.Round((vm.Length * caEntity.AspectRatio), 2);
                                        orderEntity.Width = lengthorwidth;
                                    }
                                    if (vm.Width > 0)
                                    {
                                        orderEntity.Width = vm.Width;
                                        lengthorwidth = Math.Round((vm.Width / caEntity.AspectRatio), 2);
                                        orderEntity.Length = lengthorwidth;
                                    }

                                    predefinedDimension = (vm.Length > 0 ? vm.Length : vm.Width);

                                    dimension = lengthorwidth > predefinedDimension ? lengthorwidth : predefinedDimension;

                                    PredefinedSizeEntity pdSizeEntity = PredefinedSizeDAL.predefinedSizeTable.CreateQuery<PredefinedSizeEntity>().Select(x => x).FirstOrDefault();

                                    if (pdSizeEntity != null)
                                    {
                                        if (clist != null && clist.Count() > 0)
                                        {
                                            if (clist.Count == 1)
                                            {
                                                designImagePrice = dimension * pdSizeEntity.OneColorPrice;
                                            }
                                            else if (clist.Count == 2)
                                            {
                                                designImagePrice = dimension * pdSizeEntity.TwoColorPrice;
                                            }
                                            else if (clist.Count > 2)
                                            {
                                                designImagePrice = dimension * pdSizeEntity.MoreColorPrice;
                                            }
                                            orderEntity.DesignImagePrice = designImagePrice * orderEntity.Quantity;
                                            vm.DesignImagePrice = orderEntity.DesignImagePrice;
                                        }
                                    }
                                }

                                orderEntity.PurchaseTypeID = vm.PurchaseDesignImage == true && vm.PurchaseVectorFile == true ? (int)StatusEnum.PurchaseType.Both : (vm.PurchaseDesignImage == true ? (int)StatusEnum.PurchaseType.DesignSticker
                                    : (int)StatusEnum.PurchaseType.VectorFile);

                                if (vectorFilePrice > 0 && designImagePrice > 0)
                                {
                                    orderEntity.Amount = vectorFilePrice + orderEntity.DesignImagePrice + orderEntity.ShippingPrice;
                                }
                                else if (vectorFilePrice > 0)
                                {
                                    orderEntity.Amount = vectorFilePrice + orderEntity.ShippingPrice;
                                }
                                else
                                {
                                    orderEntity.Amount = orderEntity.DesignImagePrice + orderEntity.ShippingPrice;
                                }
                                if (vm.Amount != orderEntity.Amount)
                                {
                                    return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
                                }

                                insertOrder = TableOperation.InsertOrReplace(orderEntity);

                                AppointmentEntity appEntity = CalendarAppointmentDAL.appointmentTable.CreateQuery<AppointmentEntity>().Where(x => x.PartitionKey == caEntity.Date && x.RowKey == vm.AppointmentNumber && x.DesignNumber == vm.DesignNumber && x.AppointmentStatusID != (int)StatusEnum.AppointmentStatus.Cancelled).Select(c => c).FirstOrDefault();

                                if (appEntity != null)
                                {
                                    appEntity.StatusID = Convert.ToInt32(StatusEnum.Status.OrderCreated);
                                    appEntity.UpdatedBy = userID;
                                    appEntity.UpdatedTS = DateTime.UtcNow;
                                    appointmentUpdate = TableOperation.InsertOrReplace(appEntity);
                                }

                                caEntity.StatusID = Convert.ToInt32(StatusEnum.Status.OrderCreated);
                                caEntity.UpdatedBy = userID;
                                caEntity.UpdatedTS = DateTime.UtcNow;
                                customerAppointmentUpdate = TableOperation.InsertOrReplace(caEntity);

                                DesignerAppointmentEntity daEntity = CalendarAppointmentDAL.designerAppointmentTable.ExecuteQuery(new TableQuery<DesignerAppointmentEntity>()).Where(x => x.PartitionKey == caEntity.UserID && x.RowKey == vm.AppointmentNumber && x.DesignNumber == vm.DesignNumber && x.AppointmentStatusID != (int)StatusEnum.AppointmentStatus.Cancelled).Select(x => x).FirstOrDefault();

                                if (daEntity != null)
                                {
                                    daEntity.StatusID = Convert.ToInt32(StatusEnum.Status.OrderCreated);
                                    daEntity.UpdatedBy = userID;
                                    daEntity.UpdatedTS = DateTime.UtcNow;
                                    designerAppointmentUpdate = TableOperation.InsertOrReplace(daEntity);
                                }

                                icustomerOrderOperation = await FillCustomerOrderEntityAsync(vm, userID, userName, orderEntity);
                                idesignerOrderOperation = await FillDesignerOrderEntityAsync(vm, userID, userName, orderEntity);
                                insertOperation = await FillDesignerAppointmentDetailAsync(vm, orderEntity);

                                await orderTable.ExecuteAsync(insertOrder);
                                await CalendarAppointmentDAL.appointmentTable.ExecuteAsync(appointmentUpdate);
                                await CalendarAppointmentDAL.customerAppointmentTable.ExecuteAsync(customerAppointmentUpdate);
                                await CalendarAppointmentDAL.designerAppointmentTable.ExecuteAsync(designerAppointmentUpdate);

                                if (icustomerOrderOperation != null)
                                {
                                    await customerOrderTable.ExecuteAsync(icustomerOrderOperation);
                                }
                                if (idesignerOrderOperation != null)
                                {
                                    await designerOrderTable.ExecuteAsync(idesignerOrderOperation);
                                }
                                if (insertOperation != null)
                                {
                                    await designerAppointmentDetailTable.ExecuteAsync(insertOperation);
                                }
                                TransactionLogDAL.InsertTransactionLog(orderEntity.PartitionKey, "Order", DateTime.UtcNow.Date, DateTime.UtcNow, orderEntity.CreatedBy, "Added", userName);

                                vm.Amount = orderEntity.Amount;
                                vm.OrderNumber = orderEntity.OrderNumber;

                                return new InternalOperationResult(Result.Success, "Order placed successfully, order number is : " + orderEntity.OrderNumber, orderEntity.PartitionKey);
                            }
                            else
                            {
                                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
                            }
                        }
                        else
                        {
                            return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
                        }
                    }
                    else
                    {
                        return new InternalOperationResult(Result.SDError, "Maximum limit for color selection is 5.", null);
                    }
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
                }
            }
            catch (Exception e)
            {
                string _requestJSON = Newtonsoft.Json.JsonConvert.SerializeObject(vm);
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", "OrderDAL", "CreateAsync", DateTime.UtcNow, e, null, _requestJSON, userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<InternalOperationResult> DeleteAsync(OrderViewModel vm, string userID)
        {
            try
            {
                int appointmentStatus = 0;
                TableOperation deleteOrder = null, designerOrderDelete = null, customerOrderDelete = null, designerAppointmentDetailUpdate = null;

                string orderNumber = vm.OrderNumber.ToString().PadLeft(8, '0');

                List<OrderEntity> orderEntityList = orderTable.CreateQuery<OrderEntity>().Where(c => c.DesignNumber == vm.DesignNumber).ToList();
                OrderEntity orderEntity = orderEntityList.Where(c => c.PartitionKey == vm.OrderDate.ToString("yyyy-MM-dd") && c.RowKey == orderNumber).FirstOrDefault();
                orderEntityList.Remove(orderEntity);

                if (orderEntity != null)
                {
                    deleteOrder = TableOperation.Delete(orderEntity);

                    DesignerOrderEntity designerOrderEntity = designerOrderTable.CreateQuery<DesignerOrderEntity>().Where(c => c.PartitionKey == vm.UserID && c.RowKey == orderNumber).FirstOrDefault();

                    designerOrderDelete = TableOperation.Delete(designerOrderEntity);

                    CustomerOrderEntity customerOrderEntity = customerOrderTable.CreateQuery<CustomerOrderEntity>().Where(c => c.PartitionKey == userID && c.RowKey == orderNumber).FirstOrDefault();

                    customerOrderDelete = TableOperation.Delete(customerOrderEntity);

                    DesignerAppointmentDetailEntity desAppDetailEntity = designerAppointmentDetailTable.CreateQuery<DesignerAppointmentDetailEntity>().Where(c => c.PartitionKey == orderEntity.OrderDate.ToString("yyyy-MM-dd") && c.RowKey == vm.UserID).FirstOrDefault();

                    desAppDetailEntity.Amount = desAppDetailEntity.Amount - orderEntity.Amount;
                    desAppDetailEntity.NoOfOrder = desAppDetailEntity.NoOfOrder - 1;

                    designerAppointmentDetailUpdate = TableOperation.InsertOrReplace(desAppDetailEntity);

                    if (orderEntityList.Count() == 0)
                    {
                        appointmentStatus = Convert.ToInt32(StatusEnum.Status.AppointmentCompleted);
                    }
                    else if (orderEntityList.Where(c => c.OrderStatus == StatusEnum.OrderStatus.Placed.ToString()).Count() > 0)
                    {
                        appointmentStatus = Convert.ToInt32(StatusEnum.Status.OrderCreated);
                    }
                    else if (orderEntityList.Where(c => c.OrderStatus == StatusEnum.OrderStatus.Shipped.ToString()).Count() > 0)
                    {
                        appointmentStatus = Convert.ToInt32(StatusEnum.Status.OrderShipped);
                    }

                    if (appointmentStatus > 0)
                    {
                        TableOperation appointmentupdate = null, customerAppointmentUpdate = null, designerAppointmentUpdate = null;

                        AppointmentEntity appEntity = CalendarAppointmentDAL.appointmentTable.CreateQuery<AppointmentEntity>().Where(x => x.DesignNumber == vm.DesignNumber && x.RowKey == vm.AppointmentNumber && x.AppointmentStatusID != (int)StatusEnum.AppointmentStatus.Cancelled).Select(c => c).FirstOrDefault();

                        if (appEntity != null)
                        {
                            appEntity.StatusID = appointmentStatus;
                            appEntity.UpdatedBy = userID;
                            appEntity.UpdatedTS = DateTime.UtcNow;
                            appointmentupdate = TableOperation.InsertOrReplace(appEntity);
                        }

                        CustomerAppointmentEntity caEntity = CalendarAppointmentDAL.customerAppointmentTable.ExecuteQuery
                                                             (new TableQuery<CustomerAppointmentEntity>()).Where(x => x.PartitionKey == userID && x.DesignNumber == vm.DesignNumber &&
                                                             x.RowKey == vm.AppointmentNumber && x.AppointmentStatusID != (int)StatusEnum.AppointmentStatus.Cancelled).Select(x => x).FirstOrDefault();

                        if (caEntity != null)
                        {
                            caEntity.StatusID = appointmentStatus;
                            caEntity.UpdatedBy = userID;
                            caEntity.UpdatedTS = DateTime.UtcNow;
                            customerAppointmentUpdate = TableOperation.InsertOrReplace(caEntity);
                        }

                        DesignerAppointmentEntity daEntity = CalendarAppointmentDAL.designerAppointmentTable.ExecuteQuery(
                                                             new TableQuery<DesignerAppointmentEntity>()).Where(x => x.CustomerID == userID && x.DesignNumber == vm.DesignNumber &&
                                                             x.RowKey == vm.AppointmentNumber && x.AppointmentStatusID != (int)StatusEnum.AppointmentStatus.Cancelled).Select(x => x).FirstOrDefault();

                        if (daEntity != null)
                        {
                            daEntity.StatusID = appointmentStatus;
                            daEntity.UpdatedBy = userID;
                            daEntity.UpdatedTS = DateTime.UtcNow;
                            designerAppointmentUpdate = TableOperation.InsertOrReplace(daEntity);
                        }

                        await CalendarAppointmentDAL.appointmentTable.ExecuteAsync(appointmentupdate);
                        await CalendarAppointmentDAL.customerAppointmentTable.ExecuteAsync(customerAppointmentUpdate);
                        await CalendarAppointmentDAL.designerAppointmentTable.ExecuteAsync(designerAppointmentUpdate);
                    }

                    await orderTable.ExecuteAsync(deleteOrder);
                    await designerOrderTable.ExecuteAsync(designerOrderDelete);
                    await customerOrderTable.ExecuteAsync(customerOrderDelete);
                    await designerAppointmentDetailTable.ExecuteAsync(designerAppointmentDetailUpdate);

                    return new InternalOperationResult(Result.Success, "Order deleted successfully.", null);
                }
                else
                {
                    return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
                }
            }
            catch (Exception e)
            {
                ExceptionTableStorage.InsertOrReplaceEntity("Customer", "OrderDAL", "DeleteAsync", DateTime.UtcNow, e, null, vm.OrderNumber.ToString(), userID);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        #region Order Detail

        public static async Task GetListAsync<OrderListViewModel>(Pager<OrderListViewModel> pagerVM, string customerID)
        {
            string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, customerID);
            TableQuery<CustomerOrderEntity> listQuery = new TableQuery<CustomerOrderEntity>().Where(filter);

            List<CustomerOrderEntity> customerOrderList = customerOrderTable.ExecuteQuery(listQuery).ToList();

            if (customerOrderList != null && customerOrderList.Count() > 0)
            {
                pagerVM.RecordsCount = customerOrderList.Count();

                if (pagerVM.Sort == "OrderNumber")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        customerOrderList = customerOrderList.OrderByDescending(s => s.OrderNumber).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerOrderEntity>();
                    }
                    else
                    {
                        customerOrderList = customerOrderList.OrderBy(s => s.OrderNumber).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerOrderEntity>();
                    }
                }
                else if (pagerVM.Sort == "Date")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        customerOrderList = customerOrderList.OrderByDescending(s => s.OrderDate).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerOrderEntity>();
                    }
                    else
                    {
                        customerOrderList = customerOrderList.OrderBy(s => s.OrderDate).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerOrderEntity>();
                    }
                }
                else if (pagerVM.Sort == "DesignNumber")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        customerOrderList = customerOrderList.OrderByDescending(s => s.DesignNumber).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerOrderEntity>();
                    }
                    else
                    {
                        customerOrderList = customerOrderList.OrderBy(s => s.DesignNumber).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerOrderEntity>();
                    }
                }
                else if (pagerVM.Sort == "OrderStatus")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        customerOrderList = customerOrderList.OrderByDescending(s => s.OrderStatus).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerOrderEntity>();
                    }
                    else
                    {
                        customerOrderList = customerOrderList.OrderBy(s => s.OrderStatus).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerOrderEntity>();
                    }
                }
                else if (pagerVM.Sort == "Amount")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        customerOrderList = customerOrderList.OrderByDescending(s => s.Amount).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerOrderEntity>();
                    }
                    else
                    {
                        customerOrderList = customerOrderList.OrderBy(s => s.Amount).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerOrderEntity>();
                    }
                }
                else if (pagerVM.Sort == "UpdatedTS")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        customerOrderList = customerOrderList.OrderByDescending(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerOrderEntity>();
                    }
                    else
                    {
                        customerOrderList = customerOrderList.OrderBy(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerOrderEntity>();
                    }
                }
                else
                {
                    customerOrderList = customerOrderList.Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<CustomerOrderEntity>();
                }
            }
            Mapper.Initialize(a =>
            {
                a.CreateMap<ViewModel.Customer.OrderListViewModel, CustomerOrderEntity>().ReverseMap();
            });

            await Task.FromResult(pagerVM.Data = Mapper.Map<List<OrderListViewModel>>(customerOrderList));
        }

        public static async Task<InternalOperationResult> GetByIDAsync(string customerID, int orderNo)
        {
            CustomerOrderEntity customerOrderEntity = customerOrderTable.ExecuteQuery(new TableQuery<CustomerOrderEntity>()).
                                      Where(x => x.RowKey == orderNo.ToString().PadLeft(8, '0') && x.PartitionKey == customerID).FirstOrDefault();
            if (customerOrderEntity != null)
            {
                Mapper.Initialize(a =>
                {
                    a.CreateMap<OrderListViewModel, CustomerOrderEntity>().ReverseMap();
                });

                OrderListViewModel orderVM = Mapper.Map<CustomerOrderEntity, OrderListViewModel>(customerOrderEntity);
                if (orderVM.DesignNumber != null)
                {
                    DownloadImageViewModel vm = BlobStorage.DownloadBlobByteArray(StatusEnum.Blob.designimage.ToString(), orderVM.DesignNumber);
                    if (vm != null)
                    {
                        orderVM.ImageBuffer = vm.ImageBuffer;
                        orderVM.ImageExtension = vm.FileExtension;
                    }
                }
                string uspsLink = CommonDAL.GetUSPSLink();
                orderVM.USPSLink = uspsLink + orderVM.TrackingNumber;
                string url = CommonDAL.GetImageURL("colorimage");
                OrderEntity oEntity = orderTable.CreateQuery<OrderEntity>().Where(o => o.CustomerID == customerID && o.OrderNumber == orderNo).Select(o => o).FirstOrDefault();
                if (oEntity != null)
                {
                    orderVM.PurchaseTypeID = oEntity.PurchaseTypeID;
                    orderVM.Length = oEntity.Length;
                    orderVM.Width = oEntity.Width;
                    List<ColorViewModel> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ColorViewModel>>(oEntity.ColorJSON) as List<ColorViewModel>;
                    orderVM.ColorList = list.Select(x => new ColorViewModel { Name = x.Name, ImageGUID = url + x.ImageGUID, ColorSequence = x.ColorSequence }).OrderBy(x => x.ColorSequence).ToList();
                }
                return await Task.FromResult(new InternalOperationResult(Result.Success, "", orderVM));
            }
            else
            {
                return await Task.FromResult(new InternalOperationResult(Result.UDError, "Order details does't exists", null));
            }
        }

        #endregion

        #region Helper Methods 

        public static async Task<List<ColorViewModel>> GetColorListAsync()
        {
            List<ColorViewModel> ColorList = new List<ColorViewModel>();
            List<ColorChartEntity> ColorChartEntityList = ColorChartDAL.colorChartTable.CreateQuery<ColorChartEntity>().Where(s => s.IsAllowForSale == true).Select(s => s).ToList();
            if (ColorChartEntityList != null && ColorChartEntityList.Count() > 0)
            {
                string imageURL = CommonDAL.GetImageURL(StatusEnum.Blob.colorimage.ToString());
                ColorChartEntityList.ForEach(l => ColorList.Add(new ColorViewModel { ColorID = l.PartitionKey, Name = l.Name, ImageGUID = l.ImageGUID, ImageURL = imageURL + l.ImageGUID, ColorSequence = 0 }));
            }
            return await Task.FromResult(ColorList);
        }

        public static async Task<TableOperation> FillCustomerOrderEntityAsync(OrderViewModel vm, string userID, string userName, OrderEntity orderEntity)
        {
            TableOperation insertOperation = null;
            CustomerOrderEntity customerOrderEntity = new CustomerOrderEntity();

            customerOrderEntity.PartitionKey = userID;
            customerOrderEntity.RowKey = orderEntity.RowKey;
            customerOrderEntity.OrderNumber = orderEntity.OrderNumber;
            customerOrderEntity.OrderDate = vm.OrderDate.ToString("yyyy-MM-dd");
            customerOrderEntity.DesignNumber = orderEntity.DesignNumber;
            customerOrderEntity.OrderStatusID = orderEntity.OrderStatusID;
            customerOrderEntity.OrderStatus = orderEntity.OrderStatus;
            customerOrderEntity.ShippingPrice = orderEntity.ShippingPrice;
            customerOrderEntity.VectorFilePrice = orderEntity.VectorFilePrice;
            customerOrderEntity.DesignImagePrice = orderEntity.DesignImagePrice;
            customerOrderEntity.Amount = orderEntity.Amount;
            customerOrderEntity.CustomerName = orderEntity.CustomerName;
            customerOrderEntity.CustomerCodeName = orderEntity.CustomerCodeName;
            customerOrderEntity.UserID = vm.UserID;
            customerOrderEntity.UserName = vm.UserName;
            customerOrderEntity.CreatedBy = userID;
            customerOrderEntity.CreatedTS = DateTime.UtcNow;
            customerOrderEntity.UpdatedBy = userID;
            customerOrderEntity.UpdatedTS = DateTime.UtcNow;
            customerOrderEntity.Quantity = orderEntity.Quantity;

            insertOperation = TableOperation.InsertOrReplace(customerOrderEntity);
            return await Task.FromResult(insertOperation);
        }

        public static async Task<TableOperation> FillDesignerOrderEntityAsync(OrderViewModel vm, string userID, string userName, OrderEntity orderEntity)
        {
            TableOperation insertOperation = null;
            DesignerOrderEntity designerOrderEntity = new DesignerOrderEntity();

            designerOrderEntity.PartitionKey = vm.UserID;
            designerOrderEntity.RowKey = orderEntity.RowKey;
            designerOrderEntity.OrderNumber = orderEntity.OrderNumber;
            designerOrderEntity.OrderDate = vm.OrderDate.ToString("yyyy-MM-dd");
            designerOrderEntity.DesignNumber = orderEntity.DesignNumber;
            designerOrderEntity.OrderStatusID = orderEntity.OrderStatusID;
            designerOrderEntity.OrderStatus = orderEntity.OrderStatus;
            designerOrderEntity.ShippingPrice = orderEntity.ShippingPrice;
            designerOrderEntity.VectorFilePrice = orderEntity.VectorFilePrice;
            designerOrderEntity.DesignImagePrice = orderEntity.DesignImagePrice;
            designerOrderEntity.Amount = orderEntity.Amount;
            designerOrderEntity.CustomerName = orderEntity.CustomerName;
            designerOrderEntity.CustomerCodeName = orderEntity.CustomerCodeName;
            designerOrderEntity.CustomerID = orderEntity.CustomerID;
            designerOrderEntity.UserName = vm.UserName;
            designerOrderEntity.CreatedBy = userID;
            designerOrderEntity.CreatedTS = DateTime.UtcNow;
            designerOrderEntity.UpdatedBy = userID;
            designerOrderEntity.UpdatedTS = DateTime.UtcNow;

            insertOperation = TableOperation.InsertOrReplace(designerOrderEntity);
            return await Task.FromResult(insertOperation);
        }

        public static async Task<TableOperation> FillDesignerAppointmentDetailAsync(OrderViewModel vm, OrderEntity orderEntity)
        {
            DesignerAppointmentDetailEntity designerAppointmentDetailEntity = designerAppointmentDetailTable.CreateQuery<DesignerAppointmentDetailEntity>().Where(c => c.PartitionKey == orderEntity.OrderDate.ToString("yyyy-MM-dd") && c.RowKey == vm.UserID).Select(c => c).FirstOrDefault();

            if (designerAppointmentDetailEntity != null)
            {
                designerAppointmentDetailEntity.UserName = vm.UserName;
                designerAppointmentDetailEntity.NoOfOrder = designerAppointmentDetailEntity.NoOfOrder + 1;
                designerAppointmentDetailEntity.Amount = designerAppointmentDetailEntity.Amount + orderEntity.Amount;

                TableOperation updateOperation = TableOperation.InsertOrReplace(designerAppointmentDetailEntity);
                return await Task.FromResult(updateOperation);
            }
            else
            {
                DesignerAppointmentDetailEntity designerAppDetailEntity = new DesignerAppointmentDetailEntity();

                designerAppDetailEntity.PartitionKey = vm.OrderDate.ToString("yyyy-MM-dd");
                designerAppDetailEntity.RowKey = vm.UserID;
                designerAppDetailEntity.UserName = vm.UserName;
                designerAppDetailEntity.NoOfOrder = 1;
                designerAppDetailEntity.Amount = orderEntity.Amount;

                TableOperation insertOperation = TableOperation.InsertOrReplace(designerAppDetailEntity);
                return await Task.FromResult(insertOperation);
            }
        }

        public static async Task UpdateTransactionDetail(string stripeCardID, string paymentTransactionID, string stripeCustomerID, DateTime orderDate, int orderNumber)
        {
            OrderEntity orderEntity = orderTable.CreateQuery<OrderEntity>().Where(c => c.PartitionKey == orderDate.ToString("yyyy-MM-dd") && c.RowKey == orderNumber.ToString().PadLeft(8, '0')).FirstOrDefault();
            orderEntity.StripeCardID = stripeCardID;
            orderEntity.PaymentTransactionID = paymentTransactionID;
            orderEntity.StripeCustomerID = stripeCustomerID;

            TableOperation update = TableOperation.Replace(orderEntity);
            await orderTable.ExecuteAsync(update);
        }

        public static async Task<EmailCustomerDetailViewModel> GetCustomerDetailOnSendEmailAsync(string customerID, int orderNo)
        {
            EmailCustomerDetailViewModel ecdVM = new EmailCustomerDetailViewModel();

            CustomerEntity cEntity = CustomerDAL.customerTable.CreateQuery<CustomerEntity>().Where(c => c.PartitionKey == customerID).Select(c => c).FirstOrDefault();
            if (cEntity != null)
            {
                StringBuilder s = new StringBuilder();

                ecdVM.CustomerID = cEntity.PartitionKey;
                ecdVM.FullName = cEntity.FullName;
                ecdVM.EmailID = cEntity.EmailID;
                ecdVM.ContactNo = cEntity.ContactNo;
                s.Append(cEntity.Address1 != null && cEntity.Address1 != "" ? cEntity.Address1 : "");
                s.Append(cEntity.Address2 != null && cEntity.Address2 != "" ? ", " + cEntity.Address2 : "");
                s.Append(cEntity.City != null && cEntity.City != "" ? ", " + cEntity.City : "");
                s.Append(cEntity.StateName != null && cEntity.StateName != "" ? ", " + cEntity.StateName : "");
                s.Append(cEntity.PostalCode != null && cEntity.PostalCode != "" ? ", " + cEntity.PostalCode : "");
                s.Append(cEntity.CountryName != null && cEntity.CountryName != "" ? ", " + cEntity.CountryName : "");

                ecdVM.BillingAddress = s.ToString() != null && s.ToString() != "" ? s.ToString() : null;
            }

            OrderEntity oEntity = orderTable.CreateQuery<OrderEntity>().Where(o => o.OrderNumber == orderNo).Select(o => o).FirstOrDefault();
            if (oEntity != null)
            {
                StringBuilder s = new StringBuilder();

                s.Append(oEntity.Address1 != null && oEntity.Address1 != "" ? oEntity.Address1 : "");
                s.Append(oEntity.Address2 != null && oEntity.Address2 != "" ? ", " + oEntity.Address2 : "");
                s.Append(oEntity.City != null && oEntity.City != "" ? ", " + oEntity.City : "");
                s.Append(oEntity.StateName != null && oEntity.StateName != "" ? ", " + oEntity.StateName : "");
                s.Append(oEntity.PostalCode != null && oEntity.PostalCode != "" ? ", " + oEntity.PostalCode : "");
                s.Append(oEntity.CountryName != null && oEntity.CountryName != "" ? ", " + oEntity.CountryName : "");

                ecdVM.DeliveryAdderss = s.ToString() != null && s.ToString() != "" ? s.ToString() : null;
            }

            return await Task.FromResult(ecdVM);
        }

        #endregion
    }
}

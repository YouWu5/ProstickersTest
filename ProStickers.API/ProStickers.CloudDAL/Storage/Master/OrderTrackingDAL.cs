using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.WindowsAzure.Storage.Table;
using ProStickers.CloudDAL.Entity.Customer;
using ProStickers.CloudDAL.Entity.Designer;
using ProStickers.CloudDAL.Storage.Customer;
using ProStickers.CloudDAL.Storage.ExceptionStorage;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using static ProStickers.ViewModel.Core.StatusEnum;
using ProStickers.ViewModel.Master;
using System.Text;

namespace ProStickers.CloudDAL.Storage.Master
{
    public class OrderTrackingDAL
    {
        #region Master Order Tracking

        public static async Task GetListAsync<OrderTrackingViewModel>(Pager<OrderTrackingViewModel> pagerVM)
        {
            string filter = "";
            foreach (var item in pagerVM.SearchList.Where(s => s.Value != null && s.Value != ""))
            {
                if (item.Name == "DateFrom")
                {
                    filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, item.Value);
                }
                else if (item.Name == "DateTo")
                {
                    filter = TableQuery.CombineFilters(filter, TableOperators.And, TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.LessThanOrEqual, item.Value));
                }
                if (item.Name == "Status")
                {
                    if (Convert.ToInt32(item.Value) > 0)
                    {
                        filter = TableQuery.CombineFilters(filter, TableOperators.And, TableQuery.GenerateFilterConditionForInt("OrderStatusID", QueryComparisons.Equal, Convert.ToInt32(item.Value)));
                    }
                }
                else if (item.Name == "CustomerCodeName")
                {
                    filter = TableQuery.CombineFilters(filter, TableOperators.And, TableQuery.GenerateFilterCondition("CustomerCodeName", QueryComparisons.Equal, item.Value));
                }
            }

            TableQuery<OrderEntity> query = new TableQuery<OrderEntity>().Where(filter);

            List<OrderEntity> orderList = OrderDAL.orderTable.ExecuteQuery(query).ToList();

            if (orderList != null && orderList.Count() > 0)
            {
                pagerVM.RecordsCount = orderList.Count();

                if (pagerVM.Sort == "OrderNumber")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        orderList = orderList.OrderByDescending(s => s.OrderNumber).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        orderList = orderList.OrderBy(s => s.OrderNumber).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "OrderDate")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        orderList = orderList.OrderByDescending(s => s.PartitionKey).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        orderList = orderList.OrderBy(s => s.PartitionKey).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "CustomerName")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        orderList = orderList.OrderByDescending(s => s.CustomerName).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        orderList = orderList.OrderBy(s => s.CustomerName).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "DesignNumber")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        orderList = orderList.OrderByDescending(s => s.DesignNumber).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        orderList = orderList.OrderBy(s => s.DesignNumber).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "OrderStatus")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        orderList = orderList.OrderByDescending(s => s.OrderStatus).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        orderList = orderList.OrderBy(s => s.OrderStatus).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "Amount")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        orderList = orderList.OrderByDescending(s => s.Amount).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        orderList = orderList.OrderBy(s => s.Amount).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "UpdatedTS")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        orderList = orderList.OrderByDescending(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        orderList = orderList.OrderBy(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
            }
            Mapper.Initialize(a =>
            {
                a.CreateMap<ViewModel.Customer.OrderTrackingViewModel, OrderEntity>().ReverseMap().
                ForMember(x => x.OrderDate, y => y.MapFrom(z => z.PartitionKey)).
                ForMember(x => x.CustomerName, y => y.MapFrom(z => z.CustomerCodeName));
            });

            List<ViewModel.Customer.OrderTrackingViewModel> vm = Mapper.Map<List<OrderEntity>, List<ViewModel.Customer.OrderTrackingViewModel>>(orderList);
            vm.Select(x => { x.OrderDate = Convert.ToDateTime(x.OrderDate.ToString()).ToString("MM/dd/yyyy").Replace('-', '/'); return x; }).ToList();

            await Task.FromResult(pagerVM.Data = Mapper.Map<List<OrderTrackingViewModel>>(vm));
        }

        public static async Task<InternalOperationResult> GetByIDAsync(int orderNo)
        {
            string url = CommonDAL.GetImageURL("colorimage");
            StringBuilder s = new StringBuilder();
            OrderEntity orderEntity = OrderDAL.orderTable.ExecuteQuery(new TableQuery<OrderEntity>()).
                                      Where(x => x.RowKey == orderNo.ToString().PadLeft(8, '0')).FirstOrDefault();

            if (orderEntity != null)
            {
                Mapper.Initialize(a =>
                {
                    a.CreateMap<OrderTrackingViewModel, OrderEntity>().ReverseMap().
                    ForMember(x => x.OrderDate, y => y.MapFrom(z => z.PartitionKey));
                });
                OrderTrackingViewModel orderVM = Mapper.Map<OrderTrackingViewModel>(orderEntity);
               
                s.Append(orderEntity.Address1 != null && orderEntity.Address1 != "" ?  orderEntity.Address1 : "");
                s.Append(orderEntity.Address2 != null && orderEntity.Address2 != "" ? ", " + orderEntity.Address2 : "");
                s.Append(orderEntity.City != null && orderEntity.City != "" ? ", " + orderEntity.City : "");
                s.Append(orderEntity.StateName != null && orderEntity.StateName != "" ? ", " + orderEntity.StateName : "");
                s.Append(orderEntity.PostalCode != null && orderEntity.PostalCode != "" ? ", " + orderEntity.PostalCode : "");
                s.Append(orderEntity.CountryName != null && orderEntity.CountryName != "" ? ", " + orderEntity.CountryName : "");
                orderVM.ShippingAddress = s.ToString() != null && s.ToString() != "" ? s.ToString() : null;

                var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ColorViewModel>>(orderVM.ColorJSON) as List<ColorViewModel>;
                orderVM.ColorList = list.Select(x => new ColorViewModel { Name = x.Name, ImageGUID = url + x.ImageGUID, ColorSequence = x.ColorSequence }).OrderBy(x => x.ColorSequence).ToList();

                if (orderVM.DesignNumber != null)
                {
                    orderVM.ImageBuffer = BlobStorage.DownloadBlobByteArray(StatusEnum.Blob.designimage.ToString(), orderVM.DesignNumber);
                }
                return await Task.FromResult(new InternalOperationResult(Result.Success, "", orderVM));
            }
            else
            {
                return new InternalOperationResult(Result.UDError, "Order details does'nt exists", null);
            }
        }

        public static async Task<InternalOperationResult> UpdateTrackingNumberAsync(string userName, string userID, OrderTrackingViewModel orderVM)
        {
            try
            {
                int appointmentStatus = 0;
                TableOperation updateAppointment = null; TableOperation customerAppUpdate = null; TableOperation designerAppUpdate = null;

                OrderEntity orderEntity = OrderDAL.orderTable.ExecuteQuery(new TableQuery<OrderEntity>()).
                                          Where(x => x.PartitionKey == orderVM.OrderDate.ToString() && x.RowKey == orderVM.OrderNumber.ToString().PadLeft(8, '0')).
                                          FirstOrDefault();

                orderEntity.TrackingNumber = orderVM.TrackingNumber;
                orderEntity.OrderStatus = OrderStatus.Shipped.ToString();
                orderEntity.OrderStatusID = (int)OrderStatus.Shipped;
                orderEntity.StatusID = (int)OrderStatus.Shipped;
                orderEntity.UpdatedBy = userID;
                orderEntity.UpdatedTS = DateTime.UtcNow;

                TableOperation updateOrder = TableOperation.InsertOrReplace(orderEntity);

                DesignerOrderEntity designerOrderEntity = OrderDAL.designerOrderTable.ExecuteQuery(new TableQuery<DesignerOrderEntity>()).
                                          Where(x => x.RowKey == orderVM.OrderNumber.ToString().PadLeft(8, '0')).
                                          FirstOrDefault();

                designerOrderEntity.OrderStatus = OrderStatus.Shipped.ToString();
                designerOrderEntity.OrderStatusID = (int)OrderStatus.Shipped;
                designerOrderEntity.TrackingNumber = orderVM.TrackingNumber;
                designerOrderEntity.UpdatedBy = userID;
                designerOrderEntity.UpdatedTS = DateTime.UtcNow;

                TableOperation updateDesignerOrder = TableOperation.InsertOrReplace(designerOrderEntity);

                CustomerOrderEntity customerOrderEntity = OrderDAL.customerOrderTable.ExecuteQuery(new TableQuery<CustomerOrderEntity>()).
                                          Where(x => x.RowKey == orderVM.OrderNumber.ToString().PadLeft(8, '0')).
                                          FirstOrDefault();

                customerOrderEntity.OrderStatus = OrderStatus.Shipped.ToString();
                customerOrderEntity.OrderStatusID = (int)OrderStatus.Shipped;
                customerOrderEntity.TrackingNumber = orderVM.TrackingNumber;
                customerOrderEntity.UpdatedBy = userID;
                customerOrderEntity.UpdatedTS = DateTime.UtcNow;

                TableOperation updateCustomerOrder = TableOperation.InsertOrReplace(customerOrderEntity);

                List<OrderEntity> odrEntityList = OrderDAL.orderTable.ExecuteQuery(new TableQuery<OrderEntity>()).
                                       Where(x => x.DesignNumber == orderVM.DesignNumber).
                                       ToList();

                if (odrEntityList.Where(c => c.OrderStatus == OrderStatus.Placed.ToString()).Count() > 0)
                {
                    appointmentStatus = Convert.ToInt32(Status.OrderCreated);
                }
                else if (odrEntityList.Where(c => c.OrderStatus == OrderStatus.Shipped.ToString()).Count() > 0)
                {
                    appointmentStatus = Convert.ToInt32(Status.OrderShipped);
                }

                if (appointmentStatus > 0)
                {
                    AppointmentEntity appointmentEntity = CalendarAppointmentDAL.appointmentTable.ExecuteQuery(new TableQuery<AppointmentEntity>()).
                                              Where(x => x.RowKey == orderVM.AppointmentNumber).
                                              FirstOrDefault();

                    appointmentEntity.StatusID = appointmentStatus;
                    appointmentEntity.UpdatedBy = userID;
                    appointmentEntity.UpdatedTS = DateTime.UtcNow;
                    updateAppointment = TableOperation.InsertOrReplace(appointmentEntity);

                    CustomerAppointmentEntity customerAppointmentEntity = CalendarAppointmentDAL.customerAppointmentTable.ExecuteQuery(new TableQuery<CustomerAppointmentEntity>()).
                                             Where(x => x.PartitionKey == orderEntity.CustomerID && x.RowKey == orderEntity.AppointmentNumber).
                                             FirstOrDefault();

                    customerAppointmentEntity.StatusID = appointmentStatus;
                    customerAppointmentEntity.UpdatedBy = userID;
                    customerAppointmentEntity.UpdatedTS = DateTime.UtcNow;

                    customerAppUpdate = TableOperation.InsertOrReplace(customerAppointmentEntity);

                    DesignerAppointmentEntity designerAppointmentEntity = CalendarAppointmentDAL.designerAppointmentTable.ExecuteQuery(new TableQuery<DesignerAppointmentEntity>()).
                                             Where(x => x.PartitionKey == orderEntity.UserID && x.RowKey == orderEntity.AppointmentNumber).
                                             FirstOrDefault();

                    designerAppointmentEntity.StatusID = appointmentStatus;
                    designerAppointmentEntity.UpdatedBy = userID;
                    designerAppointmentEntity.UpdatedTS = DateTime.UtcNow;
                    designerAppUpdate = TableOperation.InsertOrReplace(designerAppointmentEntity);
                }

                await OrderDAL.orderTable.ExecuteAsync(updateOrder);
                await OrderDAL.designerOrderTable.ExecuteAsync(updateDesignerOrder);
                await OrderDAL.customerOrderTable.ExecuteAsync(updateCustomerOrder);
                await CalendarAppointmentDAL.appointmentTable.ExecuteAsync(updateAppointment);
                await CalendarAppointmentDAL.customerAppointmentTable.ExecuteAsync(customerAppUpdate);
                await CalendarAppointmentDAL.designerAppointmentTable.ExecuteAsync(designerAppUpdate);

                return new InternalOperationResult(Result.Success, "Tracking number updated successfully.", orderEntity.OrderNumber);
            }
            catch (Exception e)
            {
                string _requestJSON = Newtonsoft.Json.JsonConvert.SerializeObject(orderVM);
                ExceptionTableStorage.InsertOrReplaceEntity("Master", "OrderTracking", "UpdateTrackingNumberAsync", DateTime.UtcNow, e, null, _requestJSON, userName);
                return new InternalOperationResult(Result.SDError, "Oop's something went wrong.", null);
            }
        }

        public static async Task<List<ListItemTypes>> GetStatusListAsync()
        {
            List<ListItemTypes> list = new List<ListItemTypes>();
            list.Add(new ListItemTypes { Text = "All", Value = 0 });
            list.Add(new ListItemTypes { Text = "Shipped", Value = 2 });
            list.Add(new ListItemTypes { Text = "Placed", Value = 1 });
            return await Task.FromResult(list);
        }

        public static async Task<List<ListItem>> GetCustomerNameListAsync(string codeName)
        {
            List<ListItem> customerlist = OrderDAL.orderTable.CreateQuery<OrderEntity>().Select(c => new ListItem
            {
                Text = c.CustomerCodeName,
                Value = c.CustomerID
            }).ToList();

            customerlist = customerlist.Where(c => c.Text.ToLower().Contains(codeName.ToLower())).GroupBy(c => c.Text).Select(c => c.First()).Distinct().OrderBy(c => c.Text).ToList();
            return await Task.FromResult(customerlist);
        }

        #endregion

        #region Designer Order Tracking

        public static async Task GetListAsync<OrderTrackingViewModel>(Pager<OrderTrackingViewModel> pagerVM, string userID)
        {
            string filter = "";
            foreach (var item in pagerVM.SearchList.Where(s => s.Value != null && s.Value != ""))
            {
                if (item.Name == "DateFrom")
                {
                    filter = TableQuery.GenerateFilterCondition("OrderDate", QueryComparisons.GreaterThanOrEqual, item.Value);
                }

                else if (item.Name == "DateTo")
                {
                    filter = TableQuery.CombineFilters(filter, TableOperators.And, TableQuery.GenerateFilterCondition("OrderDate", QueryComparisons.LessThanOrEqual, item.Value));
                }

                else if (item.Name == "Status")
                {
                    if (Convert.ToInt32(item.Value) > 0)
                    {
                        filter = TableQuery.CombineFilters(filter, TableOperators.And, TableQuery.GenerateFilterConditionForInt("OrderStatusID", QueryComparisons.Equal, Convert.ToInt32(item.Value)));
                    }
                }

                else if (item.Name == "CustomerCodeName")
                {
                    filter = TableQuery.CombineFilters(filter, TableOperators.And, TableQuery.GenerateFilterCondition("CustomerCodeName", QueryComparisons.Equal, item.Value));
                }
                else if (item.Name == "IsDisplayAllDesignerOrder")
                {
                    bool IsDisplayAllDesignerOrder = Convert.ToBoolean(item.Value);
                    if (!IsDisplayAllDesignerOrder)
                    {
                        filter = TableQuery.CombineFilters(filter, TableOperators.And, TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userID));
                    }
                }
            }
            TableQuery<DesignerOrderEntity> query = new TableQuery<DesignerOrderEntity>().Where(filter);

            List<DesignerOrderEntity> orderList = OrderDAL.designerOrderTable.ExecuteQuery(query).ToList();

            if (orderList != null && orderList.Count() > 0)
            {
                pagerVM.RecordsCount = orderList.Count();

                if (pagerVM.Sort == "OrderNumber")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        orderList = orderList.OrderByDescending(s => s.OrderNumber).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        orderList = orderList.OrderBy(s => s.OrderNumber).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "OrderDate")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        orderList = orderList.OrderByDescending(s => s.PartitionKey).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        orderList = orderList.OrderBy(s => s.PartitionKey).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "CustomerName")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        orderList = orderList.OrderByDescending(s => s.CustomerName).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        orderList = orderList.OrderBy(s => s.CustomerName).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }

                else if (pagerVM.Sort == "DesignNumber")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        orderList = orderList.OrderByDescending(s => s.DesignNumber).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        orderList = orderList.OrderBy(s => s.DesignNumber).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "OrderStatus")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        orderList = orderList.OrderByDescending(s => s.OrderStatus).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        orderList = orderList.OrderBy(s => s.OrderStatus).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "Amount")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        orderList = orderList.OrderByDescending(s => s.Amount).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        orderList = orderList.OrderBy(s => s.Amount).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "UpdatedTS")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        orderList = orderList.OrderByDescending(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        orderList = orderList.OrderBy(s => s.UpdatedTS).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
            }

            Mapper.Initialize(a =>
            {
                a.CreateMap<ViewModel.Customer.OrderTrackingViewModel, DesignerOrderEntity>().ReverseMap().
                     ForMember(x => x.UserID, y => y.MapFrom(z => z.PartitionKey)).
                     ForMember(x => x.CustomerName, y => y.MapFrom(z => z.CustomerCodeName));
            });
            orderList.Select(x => { x.OrderDate = Convert.ToDateTime(x.OrderDate).ToString("MM/dd/yyyy").Replace('-', '/'); return x; }).ToList();
            await Task.FromResult(pagerVM.Data = Mapper.Map<List<OrderTrackingViewModel>>(orderList));
        }

        #endregion
    }
}

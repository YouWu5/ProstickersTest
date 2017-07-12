using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.WindowsAzure.Storage.Table;
using ProStickers.CloudDAL.Entity.Master;
using ProStickers.CloudDAL.Storage.Customer;
using ProStickers.ViewModel.Core;
using ProStickers.CloudDAL.Storage.Master;
using ProStickers.CloudDAL.Entity.Designer;

namespace ProStickers.CloudDAL
{
    public class ReportDAL
    {
        public static async Task GetListAsync<SalesReportViewModel>(Pager<SalesReportViewModel> pagerVM)
        {
            string filter = null;
            string designFilter = "";
            bool filterGet = true;

            if (pagerVM.SearchList != null && pagerVM.SearchList.Count() > 0)
            {
                foreach (var item in pagerVM.SearchList.Where(s => s.Value != null && s.Value != ""))
                {
                    filterGet = false;
                    if (item.DisplayName == "DateFrom")
                    {
                        filter = TableQuery.GenerateFilterCondition(item.Name, QueryComparisons.GreaterThanOrEqual, DateTime.Parse(item.Value).ToString("yyyy-MM-dd"));
                        designFilter = TableQuery.GenerateFilterCondition("Date", QueryComparisons.GreaterThanOrEqual, DateTime.Parse(item.Value).ToString("yyyy-MM-dd"));
                    }
                    else if (item.DisplayName == "DateTo")
                    {
                        filter = TableQuery.CombineFilters(filter, TableOperators.And, (TableQuery.GenerateFilterCondition(item.Name, QueryComparisons.LessThanOrEqual, DateTime.Parse(item.Value).ToString("yyyy-MM-dd"))));
                        designFilter = TableQuery.CombineFilters(designFilter, TableOperators.And, TableQuery.GenerateFilterCondition("Date", QueryComparisons.LessThanOrEqual, DateTime.Parse(item.Value).ToString("yyyy-MM-dd")));
                    }
                    else if (item.Value != "All")
                    {
                        filter = TableQuery.CombineFilters(filter, TableOperators.And, (TableQuery.GenerateFilterCondition(item.Name, QueryComparisons.Equal, item.Value)));
                        designFilter = TableQuery.CombineFilters(designFilter, TableOperators.And, TableQuery.GenerateFilterCondition("UserName", QueryComparisons.Equal, item.Value));
                    }
                }
            }

            if (filterGet == true)
            {
                filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Utility.GetCSTDateTime().ToString("yyyy-MM-dd"));

                designFilter = TableQuery.GenerateFilterCondition("Date", QueryComparisons.Equal, Utility.GetCSTDateTime().ToString("yyyy-MM-dd"));
            }

            TableQuery<DynamicTableEntity> projectionQuery = new TableQuery<DynamicTableEntity>().Select(new string[] { "Amount", "NoOfOrder", "UserName", }).Where(filter);

            EntityResolver<ViewModel.Master.SalesReportViewModel> resolver = (pk, rk, ts, props, etag) => new ViewModel.Master.SalesReportViewModel
            {
                UserID = rk,
                Amount = props["Amount"].DoubleValue.Value,
                NoOfOrder = props["NoOfOrder"].Int32Value.Value,
                UserName = props["UserName"].StringValue,
            };

            List<ViewModel.Master.SalesReportViewModel> list = OrderDAL.designerAppointmentDetailTable.ExecuteQuery(projectionQuery, resolver, null, null).ToList();

            TableQuery<DesignerAppointmentEntity> query = new TableQuery<DesignerAppointmentEntity>().Where(designFilter);

            List<DesignerAppointmentEntity> designsList = CalendarAppointmentDAL.designerAppointmentTable.ExecuteQuery(query).ToList();

            if (list != null && list.Count() > 0)
            {
                pagerVM.RecordsCount = list.Count();

                list = (from ol in list
                        group ol by ol.UserID
                   into grp
                        select new ViewModel.Master.SalesReportViewModel
                        {
                            Amount = grp.Sum(c => c.Amount),
                            NoOfDesigns = designsList.Where(c => c.DesignNumber != null && c.PartitionKey == grp.Select(x => x.UserID).FirstOrDefault()).Count(),
                            NoOfOrder = grp.Sum(ex => ex.NoOfOrder),
                            UserName = grp.Select(c => c.UserName).FirstOrDefault(),
                            UserID = grp.Select(c => c.UserID).FirstOrDefault()
                        }).ToList();

                if (pagerVM.Sort == "UserName")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        list = list.OrderByDescending(s => s.UserName).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(s => s.UserName).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }

                else if (pagerVM.Sort == "NoOfOrder")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        list = list.OrderByDescending(s => s.NoOfOrder).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(s => s.NoOfOrder).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "Amount")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        list = list.OrderByDescending(s => s.Amount).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(s => s.Amount).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else if (pagerVM.Sort == "NoOfDesigns")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        list = list.OrderByDescending(s => s.NoOfDesigns).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(s => s.NoOfDesigns).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                    }
                }
                else
                {
                    list = list.Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList();
                }
            }

            Mapper.Initialize(a =>
            {
                a.CreateMap<DesignerAppointmentDetailEntity, SalesReportViewModel>().ReverseMap();
            });
            await Task.FromResult(pagerVM.Data = Mapper.Map<List<SalesReportViewModel>>(list));
        }

        public static async Task<List<ListItem>> GetDesignerListAsync()
        {
            IEnumerable<ListItem> list;
            List<ListItem> lst = new List<ListItem>();
            lst.Add(new ListItem { Text = "All", Value = "0" });
            {
                list = UserDAL.userTable.CreateQuery<UserEntity>().Where(c => c.UserTypeID == 2).
                     Select(c => new ListItem
                     {
                         Text = c.FullName,
                         Value = c.PartitionKey
                     }).ToList();

                lst.AddRange(list.Where(c => c.Text != null).ToList());
                return await Task.FromResult(lst);
            }
        }
    }
}
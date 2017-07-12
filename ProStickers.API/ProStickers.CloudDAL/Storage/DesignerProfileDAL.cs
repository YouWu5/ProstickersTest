using AutoMapper;
using Microsoft.WindowsAzure.Storage.Table;
using ProStickers.CloudDAL.Entity.Master;
using ProStickers.CloudDAL.Storage.Master;
using ProStickers.ViewModel;
using ProStickers.ViewModel.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProStickers.CloudDAL.Storage
{
    public class DesignerProfileDAL
    {
        public static CloudTable userTable;

        static DesignerProfileDAL()
        {
            userTable = Utility.GetStorageTable("User");
        }

        public static async Task<InternalOperationResult> GetListAsync()
        {
            List<UserEntity> userEntity = userTable.ExecuteQuery(new TableQuery<UserEntity>()).Where(x => x.UserTypeID == 2).ToList();
            Mapper.Initialize(a =>
            {
                a.CreateMap<DesignerProfileViewModel, UserEntity>().ReverseMap().
                ForMember(x => x.UserID, y => y.MapFrom(z => z.PartitionKey));
            });

            List<DesignerProfileViewModel> designerVM = Mapper.Map<List<UserEntity>, List<DesignerProfileViewModel>>(userEntity);
            foreach (var item in designerVM)
            {
                item.ImageURL = BlobStorage.DownloadBlobUri(StatusEnum.Blob.userimage.ToString(), item.ImageGUID);
                designerVM.Add(item);
            }

            return await Task.FromResult(new InternalOperationResult(Result.Success, "", designerVM));
        }

        public static async Task GetListAsync<DesignerFeedbackListViewModel>(Pager<DesignerFeedbackListViewModel> pagerVM, string userID)
        {
            string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userID);

            TableQuery<FeedbackEntity> listQuery = new TableQuery<FeedbackEntity>();

            List<FeedbackEntity> feedbackList = FeedbackDAL.feedbackTable.ExecuteQuery(listQuery).ToList();

            if (feedbackList != null && feedbackList.Count() > 0)
            {
                pagerVM.RecordsCount = feedbackList.Count();

                if (pagerVM.Sort == "Feedback")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        feedbackList = feedbackList.OrderByDescending(s => s.CustomerFeedback).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<FeedbackEntity>();
                    }
                    else
                    {
                        feedbackList = feedbackList.OrderBy(s => s.CustomerFeedback).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<FeedbackEntity>();
                    }
                }
                if (pagerVM.Sort == "CustomerName")
                {
                    if (pagerVM.SortDir == SortDirection.DESC.ToString())
                    {
                        feedbackList = feedbackList.OrderByDescending(s => s.CustomerName).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<FeedbackEntity>();
                    }
                    else
                    {
                        feedbackList = feedbackList.OrderBy(s => s.CustomerName).Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<FeedbackEntity>();
                    }
                }
                else
                {
                    feedbackList = feedbackList.Skip((pagerVM.PageNumber - 1) * pagerVM.PageSize).Take(pagerVM.PageSize).ToList<FeedbackEntity>();
                }
            }
            Mapper.Initialize(a =>
            {
                a.CreateMap<ViewModel.DesignerFeedbackListViewModel, FeedbackEntity>().ReverseMap();
            });

            await Task.FromResult(pagerVM.Data = Mapper.Map<List<DesignerFeedbackListViewModel>>(feedbackList));
        }
    }
}

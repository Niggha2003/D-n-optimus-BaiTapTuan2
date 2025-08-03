using BaiTap2.DTOs;
using BaiTap2.Models.APIResponseModels;

namespace BaiTap2.Repository.IRepository
{
    public interface IReviewerRepository
    {
        Task<JsonResponseModel> GetReviewerList();
        Task<JsonResponseModel> GetReviewerById(int id);
        Task<JsonResponseModel> GetListBookReviewedById(int id);
        Task<JsonResponseModel> CreateNewReviewer(ReviewerCreateDTO reviewerRequest);
        Task<JsonResponseModel> UpdateReviewer(ReviewerCreateDTO reviewerRequest);
        Task<JsonResponseModel> Delete(int id);
    }
}

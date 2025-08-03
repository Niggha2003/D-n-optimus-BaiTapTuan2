using BaiTap2.DTOs;
using BaiTap2.Models;
using BaiTap2.Models.APIResponseModels;

namespace BaiTap2.Repository.IRepository
{
    public interface IReviewRepository
    {
        Task<JsonResponseModel> GetReviewList();
        Task<JsonResponseModel> GetReviewById(int id);
        Task<JsonResponseModel> CreateNewReview(ReviewCreateDTO reviewRequest);
        Task<JsonResponseModel> UpdateReview(ReviewCreateDTO reviewRequest);
        Task<JsonResponseModel> Delete(int id);
        JsonResponseModel AddNewReviewSub(ReviewModel reviewModel);
    }
}

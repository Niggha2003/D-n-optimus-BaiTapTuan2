using BaiTap2.DTOs;
using BaiTap2.Models.APIResponseModels;
using BaiTap2.Models.MongoDBModels;

namespace BaiTap2.Repository.MongoDBRepository.IMongoDBRepository
{
    public interface IReviewMongoDBRepository
    {
        Task<JsonResponseModel> GetReviewList();
        Task<JsonResponseModel> GetReviewById(string id);
        Task<JsonResponseModel> CreateNewReview(BookReviewMongoDBModel reviewRequest);
        Task<JsonResponseModel> UpdateReview(BookReviewMongoDBModel reviewRequest);
        Task<JsonResponseModel> Delete(string reviewId);
    }
}

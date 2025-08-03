using BaiTap2.Models.APIResponseModels;
using BaiTap2.Contexts;
using BaiTap2.DTOs;
using BaiTap2.Models;
using BaiTap2.Services;
using Microsoft.EntityFrameworkCore;
using BaiTap2.Repository.IRepository;

namespace BaiTap2.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _context;
        private readonly RedisCacheService _redisCacheService;

        public ReviewRepository(DataContext context, RedisCacheService redisCacheService)
        {
            _context = context;
            _redisCacheService = redisCacheService;
        }

        public async Task<JsonResponseModel> GetReviewList()
        {
            try
            {
                var reviewList = await _context.Reviews.ToListAsync();
                return new JsonResponseModel(true, 200, "Success", reviewList);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public async Task<JsonResponseModel> GetReviewById(int id)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(id);
                if (review == null)
                {
                    return new JsonResponseModel(true, 404, "Not found", null);
                }

                return new JsonResponseModel(true, 200, "Success", review);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public async Task<JsonResponseModel> CreateNewReview(ReviewCreateDTO reviewRequest)
        {
            if (string.IsNullOrWhiteSpace(reviewRequest.ReviewContent))
            {
                return new JsonResponseModel(true, 400, "Bad parameter. ReviewContent is null or empty", null);
            }

            using (UnitOfWork unitOfWork = new UnitOfWork(_context, _redisCacheService))
            {
                try
                {

                    var reviewer = await unitOfWork.ReviewerRepository.GetReviewerById(reviewRequest.ReviewerId);
                    var book = await unitOfWork.BookRepository.GetBookById(reviewRequest.BookId);

                    if (reviewer.Status == false || book.Status == false)
                    {
                        throw new Exception("Review: " + reviewer.Message + " || " + "Book: " + book.Message);
                    }

                    if (reviewer.Code == 404 || book.Code == 404)
                    {
                        return new JsonResponseModel(true, 400, "Reviewer or Book not found", null);
                    }

                    var newReview = new ReviewModel
                    {
                        ReviewContent = reviewRequest.ReviewContent,
                        ReviewerId = reviewRequest.ReviewerId,
                        BookId = reviewRequest.BookId
                    };

                    unitOfWork.ReviewRepository.AddNewReviewSub(newReview);
                    await unitOfWork.SaveAsync();
                    await unitOfWork.CommitAsync();

                    return new JsonResponseModel(true, 200, "Success", null);
                }
                catch (Exception ex)
                {
                    await unitOfWork.RollbackAsync();
                    return new JsonResponseModel(false, 500, ex.Message, null);
                }
            }
        }

        public async Task<JsonResponseModel> UpdateReview(ReviewCreateDTO reviewRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reviewRequest.ReviewContent))
                {
                    return new JsonResponseModel(true, 400, "Bad parameter. ReviewContent is null or empty", null);
                }

                var review = await _context.Reviews.FindAsync(reviewRequest.ReviewId);

                if (review == null)
                {
                    return new JsonResponseModel(true, 404, "Not found", null);
                }

                _context.Entry(review).State = EntityState.Modified;
                review.ReviewContent = reviewRequest.ReviewContent;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Reviews.Any(a => a.ReviewId == reviewRequest.ReviewId))
                    {
                        return new JsonResponseModel(true, 404, "Not found", null);
                    }
                    else
                    {
                        throw;
                    }
                }

                return new JsonResponseModel(true, 200, "Success", null);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public async Task<JsonResponseModel> Delete(int id)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(id);

                if (review == null)
                {
                    return new JsonResponseModel(true, 404, "Not found", null);
                }

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                return new JsonResponseModel(true, 200, "Success", null);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public JsonResponseModel AddNewReviewSub(ReviewModel reviewModel)
        {
            try
            {
                _context.Reviews.Add(reviewModel);
                return new JsonResponseModel(true, 200, "Success", null);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }
    }
}

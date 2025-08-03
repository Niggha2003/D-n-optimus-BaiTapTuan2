using BaiTap2.Contexts;
using BaiTap2.DTOs;
using BaiTap2.Models;
using BaiTap2.Models.APIResponseModels;
using BaiTap2.Models.MongoDBModels;
using BaiTap2.Repository.MongoDBRepository.IMongoDBRepository;
using BaiTap2.Services;
using BaiTap2.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BaiTap2.Repository.MongoDBRepository
{
    public class ReviewMongoDBRepository : IReviewMongoDBRepository
    {
        private readonly DataContext _context;
        private readonly RedisCacheService _redisCacheService;
        private readonly IMongoClient _client;
        private readonly IMongoCollection<BookReviewMongoDBModel> _reviewCollection;

        public ReviewMongoDBRepository(DataContext context, IOptions<MongoDbSettings> settings, IMongoClient client, RedisCacheService redisCache)
        {
            _context = context;

            var database = client.GetDatabase(settings.Value.DatabaseName);
            _redisCacheService = redisCache;

            _reviewCollection = database.GetCollection<BookReviewMongoDBModel>("BookReviews");
            _client = client;
        }

        public async Task<JsonResponseModel> GetReviewList()
        {
            try
            {
                var reviewListRedis = await _redisCacheService.GetData<List<BookReviewMongoDBModel>>("review-list");
                if (reviewListRedis == null)
                {
                    var reviewList = await _reviewCollection.Find(a => true).ToListAsync();
                    await _redisCacheService.SetAsync<List<BookReviewMongoDBModel>>("review-list", reviewList, TimeSpan.FromMinutes(5));

                    return new JsonResponseModel(true, 200, "Success", reviewList);
                }

                return new JsonResponseModel(true, 200, "Success", reviewListRedis);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public async Task<JsonResponseModel> GetReviewById(string id)
        {
            try
            {
                var reviewInfoRedis = await _redisCacheService.GetData<BookReviewMongoDBModel>("review-" + id);
                if (reviewInfoRedis == null)
                {
                    var reviewInfo = await _reviewCollection.Find(r => r.ReviewId == id).FirstOrDefaultAsync();
                    if (reviewInfo == null)
                    {
                        return new JsonResponseModel(true, 404, "Not found", reviewInfo);
                    }

                    // lấy thông tin của book và reviewer
                    var book = await _context.Books.FindAsync(reviewInfo.BookId);
                    var reviewer = await _context.Reviewers.FindAsync(reviewInfo.ReviewerId);
                    if (book == null || reviewer == null)
                    {
                        return new JsonResponseModel(true, 404, "Book or Reviewer not found", reviewInfo);
                    }

                    var reviewInfoWithDetails = new
                    {
                        Id = reviewInfo.ReviewId,
                        ReviewContent = reviewInfo.ReviewContent,
                        BookTitle = book.Title,
                        ReviewerName = reviewer.ReviewerName
                    };

                    await _redisCacheService.SetAsync<Object>("review-" + id, reviewInfoWithDetails, TimeSpan.FromMinutes(5));
                    return new JsonResponseModel(true, 200, "Success", reviewInfoWithDetails);
                }

                return new JsonResponseModel(true, 200, "Success", reviewInfoRedis);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public async Task<JsonResponseModel> CreateNewReview(BookReviewMongoDBModel reviewRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(reviewRequest.ReviewContent))
                {
                    return new JsonResponseModel(true, 400, "Bad parameter. ReviewContent is null or empty", null);
                }

                //using (var session = await _client.StartSessionAsync())
                //{
                //    session.StartTransaction();
                    try
                    {
                        var reviewer = await _context.Reviewers.Where(b => b.ReviewerId == reviewRequest.ReviewerId).FirstOrDefaultAsync();
                        var book = await _context.Books.Where(b => b.BookId == reviewRequest.BookId).FirstOrDefaultAsync();

                        if (reviewer == null || book == null)
                        {
                            return new JsonResponseModel(true, 400, "Reviewer or Book not found", null);
                        }

                        var newReview = new BookReviewMongoDBModel
                        {
                            ReviewContent = reviewRequest.ReviewContent,
                            ReviewerId = reviewRequest.ReviewerId,
                            BookId = reviewRequest.BookId,
                        };

                        await _reviewCollection.InsertOneAsync(newReview);

                        var reviewList = await _reviewCollection.Find(a => true).ToListAsync();
                        await _redisCacheService.SetAsync<List<BookReviewMongoDBModel>>("review-list", reviewList, TimeSpan.FromMinutes(5));

                        //await session.CommitTransactionAsync();
                        return new JsonResponseModel(true, 200, "Success", newReview);
                    }
                    catch (Exception ex)
                    {
                        //await session.AbortTransactionAsync();
                        return new JsonResponseModel(false, 500, ex.Message, null);
                    }
                //}
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public async Task<JsonResponseModel> UpdateReview(BookReviewMongoDBModel reviewRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(reviewRequest.ReviewContent))
                {
                    return new JsonResponseModel(true, 400, "Bad parameter. ReviewContent is null or empty", null);
                }

                var review = await _reviewCollection.Find(r => r.ReviewId == reviewRequest.ReviewId).FirstOrDefaultAsync();
                if (review == null)
                {
                    return new JsonResponseModel(true, 404, "Not found", null);
                }

                var filterUpdate = Builders<BookReviewMongoDBModel>.Filter.Eq(b => b.ReviewId, reviewRequest.ReviewId);
                var update = Builders<BookReviewMongoDBModel>.Update.Set("ReviewContent", reviewRequest.ReviewContent);

                await _reviewCollection.UpdateOneAsync(filterUpdate, update);

                var reviewInfoRedis = await _redisCacheService.GetData<BookReviewMongoDBModel>("review-" + reviewRequest.ReviewId);
                if(reviewInfoRedis != null)
                {
                    await _redisCacheService.DeleteCache("review-" + reviewRequest.ReviewId);
                    await GetReviewById(reviewRequest.ReviewId);
                }

                var reviewList = await _reviewCollection.Find(a => true).ToListAsync();
                await _redisCacheService.SetAsync<List<BookReviewMongoDBModel>>("review-list", reviewList, TimeSpan.FromMinutes(5));

                return new JsonResponseModel(true, 200, "Success", null);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public async Task<JsonResponseModel> Delete(string reviewId)
        {
            try
            {
                var filter = Builders<BookReviewMongoDBModel>.Filter.Eq(b => b.ReviewId, reviewId);
                var reviewInfo = await _reviewCollection.Find(filter).FirstOrDefaultAsync();

                if (reviewInfo == null)
                {
                    return new JsonResponseModel(true, 404, "Not found", null);
                }

                await _reviewCollection.DeleteOneAsync(filter);

                var reviewList = await _reviewCollection.Find(a => true).ToListAsync();
                await _redisCacheService.SetAsync<List<BookReviewMongoDBModel>>("review-list", reviewList, TimeSpan.FromMinutes(5));

                return new JsonResponseModel(true, 200, "Success", null);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

    }
}

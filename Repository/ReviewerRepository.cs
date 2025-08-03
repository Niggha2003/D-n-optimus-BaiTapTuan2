using BaiTap2.Models.APIResponseModels;
using BaiTap2.Contexts;
using BaiTap2.DTOs;
using BaiTap2.Models;
using Microsoft.EntityFrameworkCore;
using BaiTap2.Repository.IRepository;

namespace BaiTap2.Repository
{
    public class ReviewerRepository : IReviewerRepository
    {
        private readonly DataContext _context;

        public ReviewerRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<JsonResponseModel> GetReviewerList()
        {
            try
            {
                var reviewerList = await _context.Reviewers.ToListAsync();
                return new JsonResponseModel(true, 200, "Success", reviewerList);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public async Task<JsonResponseModel> GetReviewerById(int id)
        {
            try
            {
                var reviewer = await _context.Reviewers.FindAsync(id);
                if (reviewer == null)
                {
                    return new JsonResponseModel(true, 404, "Not found", null);
                }
                return new JsonResponseModel(true, 200, "Success", reviewer);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public async Task<JsonResponseModel> GetListBookReviewedById(int id)
        {
            try
            {
                var reviewer = await _context.Reviewers
                    .Where(r => r.ReviewerId == id)
                    .Include(r => r.Reviews)
                        .ThenInclude(rv => rv.Book)
                    .FirstOrDefaultAsync();

                if (reviewer == null)
                {
                    return new JsonResponseModel(true, 404, "Not found", null);
                }

                var bookNameList = reviewer.Reviews.Select(r => r.Book.Title).ToList();
                return new JsonResponseModel(true, 200, "Success", bookNameList);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public async Task<JsonResponseModel> CreateNewReviewer(ReviewerCreateDTO reviewerRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reviewerRequest.ReviewerName))
                {
                    return new JsonResponseModel(true, 400, "Bad parameter. ReviewerName is null or empty", null);
                }

                var existing = await _context.Reviewers
                    .FirstOrDefaultAsync(r => r.ReviewerName == reviewerRequest.ReviewerName);
                if (existing != null)
                {
                    return new JsonResponseModel(true, 400, "Bad parameter. Reviewer already exists", null);
                }

                var newReviewer = new ReviewerModel
                {
                    ReviewerName = reviewerRequest.ReviewerName
                };

                _context.Reviewers.Add(newReviewer);
                await _context.SaveChangesAsync();

                return new JsonResponseModel(true, 200, "Success", newReviewer);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public async Task<JsonResponseModel> UpdateReviewer(ReviewerCreateDTO reviewerRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reviewerRequest.ReviewerName))
                {
                    return new JsonResponseModel(true, 400, "Bad parameter. ReviewerName is null or empty", null);
                }

                var duplicate = await _context.Reviewers
                    .FirstOrDefaultAsync(r => r.ReviewerName == reviewerRequest.ReviewerName);
                if (duplicate != null && duplicate.ReviewerId != reviewerRequest.ReviewerId)
                {
                    return new JsonResponseModel(true, 400, "Bad parameter. Reviewer already exists", null);
                }

                var reviewer = await _context.Reviewers.FindAsync(reviewerRequest.ReviewerId);
                if (reviewer == null)
                {
                    return new JsonResponseModel(true, 404, "Not found", null);
                }

                reviewer.ReviewerName = reviewerRequest.ReviewerName;
                _context.Entry(reviewer).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return new JsonResponseModel(true, 200, "Success", null);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Reviewers.Any(r => r.ReviewerId == reviewerRequest.ReviewerId))
                {
                    return new JsonResponseModel(true, 404, "Not found", null);
                }
                else
                {
                    throw;
                }
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
                var reviewer = await _context.Reviewers.FindAsync(id);
                if (reviewer == null)
                {
                    return new JsonResponseModel(true, 404, "Not found", null);
                }

                _context.Reviewers.Remove(reviewer);
                await _context.SaveChangesAsync();

                return new JsonResponseModel(true, 200, "Success", null);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }
    }
}

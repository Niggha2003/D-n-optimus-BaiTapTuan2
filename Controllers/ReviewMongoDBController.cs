using BaiTap2.DTOs;
using BaiTap2.Models.APIResponseModels;
using BaiTap2.Models.MongoDBModels;
using BaiTap2.Repository;
using BaiTap2.Repository.MongoDBRepository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BaiTap2.Controllers
{
    [ApiController]
    [Route("api/reviewMongoDB")]
    public class ReviewMongoDBController : ControllerBase
    {
        private readonly ReviewMongoDBRepository _reviewMongoDBRepository;

        public ReviewMongoDBController(ReviewMongoDBRepository reviewMongoDBRepository)
        {
            _reviewMongoDBRepository = reviewMongoDBRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviewList()
        {
            var response = await _reviewMongoDBRepository.GetReviewList();
            return StatusCode(response.Code, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById(string id)
        {
            var response = await _reviewMongoDBRepository.GetReviewById(id);
            return StatusCode(response.Code, response);
        }

        [HttpPost("createNewReview")]
        public async Task<IActionResult> CreateNewReview(BookReviewMongoDBModel reviewRequest)
        {
            var response = await _reviewMongoDBRepository.CreateNewReview(reviewRequest);
            return StatusCode(response.Code, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateReview(BookReviewMongoDBModel reviewRequest)
        {
            var response = await _reviewMongoDBRepository.UpdateReview(reviewRequest);
            return StatusCode(response.Code, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _reviewMongoDBRepository.Delete(id);
            return StatusCode(response.Code, response);
        }
    }
}

using BaiTap2.Models.APIResponseModels;
using BaiTap2.DTOs;
using BaiTap2.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BaiTap2.Controllers
{
    [ApiController]
    [Route("api/review")]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewRepository _reviewRepository;

        public ReviewController(ReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviewList()
        {
            var response = await _reviewRepository.GetReviewList();
            return StatusCode(response.Code, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById(int id)
        {
            var response = await _reviewRepository.GetReviewById(id);
            return StatusCode(response.Code, response);
        }

        [HttpPost("createNewReview")]
        public async Task<IActionResult> CreateNewReview([FromBody] ReviewCreateDTO reviewRequest)
        {
            var response = await _reviewRepository.CreateNewReview(reviewRequest);
            return StatusCode(response.Code, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateReview([FromBody] ReviewCreateDTO reviewRequest)
        {
            var response = await _reviewRepository.UpdateReview(reviewRequest);
            return StatusCode(response.Code, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _reviewRepository.Delete(id);
            return StatusCode(response.Code, response);
        }
    }
}

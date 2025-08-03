using BaiTap2.Models.APIResponseModels;
using BaiTap2.DTOs;
using BaiTap2.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BaiTap2.Controllers
{
    [ApiController]
    [Route("api/reviewer")]
    public class ReviewerController : ControllerBase
    {
        private readonly ReviewerRepository _reviewerRepository;

        public ReviewerController(ReviewerRepository reviewerRepository)
        {
            _reviewerRepository = reviewerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviewerList()
        {
            var response = await _reviewerRepository.GetReviewerList();
            return StatusCode(response.Code, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewerById(int id)
        {
            var response = await _reviewerRepository.GetReviewerById(id);
            return StatusCode(response.Code, response);
        }

        [HttpPost("createNewReviewer")]
        public async Task<IActionResult> CreateNewReviewer([FromBody] ReviewerCreateDTO reviewerRequest)
        {
            var response = await _reviewerRepository.CreateNewReviewer(reviewerRequest);
            return StatusCode(response.Code, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateReviewer([FromBody] ReviewerCreateDTO reviewerRequest)
        {
            var response = await _reviewerRepository.UpdateReviewer(reviewerRequest);
            return StatusCode(response.Code, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _reviewerRepository.Delete(id);
            return StatusCode(response.Code, response);
        }
    }
}

using BaiTap2.Models.APIResponseModels;
using BaiTap2.DTOs;
using BaiTap2.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BaiTap2.Controllers
{
    [ApiController]
    [Route("api/author")]
    public class AuthorController : ControllerBase
    {
        private readonly AuthorRepository _authorRepository;

        public AuthorController(AuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthorList()
        {
            var response = await _authorRepository.GetAuthorList();
            return StatusCode(response.Code, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            var response = await _authorRepository.GetAuthorById(id);
            return StatusCode(response.Code, response);
        }

        [HttpPost("createNewAuthor")]
        public async Task<IActionResult> CreateNewAuthor([FromBody] AuthorCreateDTO authorRequest)
        {
            var response = await _authorRepository.CreateNewAuthor(authorRequest);
            return StatusCode(response.Code, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAuthor([FromBody] AuthorCreateDTO authorRequest)
        {
            var response = await _authorRepository.UpdateAuthor(authorRequest);
            return StatusCode(response.Code, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _authorRepository.Delete(id);
            return StatusCode(response.Code, response);
        }
    }
}

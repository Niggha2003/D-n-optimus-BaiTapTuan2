using BaiTap2.Models.APIResponseModels;
using BaiTap2.DTOs;
using BaiTap2.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BaiTap2.Controllers
{
    [ApiController]
    [Route("api/book")]
    public class BookController : ControllerBase
    {
        private readonly BookRepository _bookRepository;

        public BookController(BookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookList()
        {
            var response = await _bookRepository.GetBookList();
            return StatusCode(response.Code, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var response = await _bookRepository.GetBookById(id);
            return StatusCode(response.Code, response);
        }

        [HttpPost("createNewBook")]
        public async Task<IActionResult> CreateNewBook([FromBody] BookCreateDTO bookRequest)
        {
            var response = await _bookRepository.CreateNewBook(bookRequest);
            return StatusCode(response.Code, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBook([FromBody] BookCreateDTO bookRequest)
        {
            var response = await _bookRepository.UpdateBook(bookRequest);
            return StatusCode(response.Code, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _bookRepository.Delete(id);
            return StatusCode(response.Code, response);
        }
    }
}

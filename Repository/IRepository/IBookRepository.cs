using BaiTap2.DTOs;
using BaiTap2.Models.APIResponseModels;

namespace BaiTap2.Repository.IRepository
{
    public interface IBookRepository
    {
        Task<JsonResponseModel> GetBookList();
        Task<JsonResponseModel> GetBookById(int id);
        Task<JsonResponseModel> GetBookByAuthorId(int authorId);
        Task<JsonResponseModel> CreateNewBook(BookCreateDTO bookRequest);
        Task<JsonResponseModel> UpdateBook(BookCreateDTO bookRequest);
        Task<JsonResponseModel> Delete(int id);
    }
}

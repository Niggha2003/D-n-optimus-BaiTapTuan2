using BaiTap2.DTOs;
using BaiTap2.Models.APIResponseModels;

namespace BaiTap2.Repository.IRepository
{
    public interface IAuthorRepository
    {
        Task<JsonResponseModel> GetAuthorList();
        Task<JsonResponseModel> GetAuthorById(int id);
        Task<JsonResponseModel> CreateNewAuthor(AuthorCreateDTO authorRequest);
        Task<JsonResponseModel> UpdateAuthor(AuthorCreateDTO authorRequest);
        Task<JsonResponseModel> Delete(int id);
    }
}

using BaiTap2.Contexts;
using BaiTap2.DTOs;
using BaiTap2.Models;
using BaiTap2.Models.APIResponseModels;
using BaiTap2.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaiTap2.Repository
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly DataContext _context;

        public AuthorRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<JsonResponseModel> GetAuthorList()
        {
            try
            {
                var authorList = await _context.Authors.ToListAsync();
                return new JsonResponseModel(true, 200, "Success", authorList);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public async Task<JsonResponseModel> GetAuthorById(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);
                if (author == null)
                {
                    return new JsonResponseModel(true, 404, "Not found", author);
                }
                return new JsonResponseModel(true, 200, "Success", author);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public async Task<JsonResponseModel> CreateNewAuthor(AuthorCreateDTO authorRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(authorRequest.AuthorName))
                {
                    return new JsonResponseModel(true, 400, "Bad parameter. AuthorName is null or empty", null);
                }

                var author = await _context.Authors
                    .Where(a => a.AuthorName == authorRequest.AuthorName)
                    .FirstOrDefaultAsync();

                if (author != null)
                {
                    return new JsonResponseModel(true, 400, "Bad parameter. Author is already exist", null);
                }

                var newAuthor = new AuthorModel
                {
                    AuthorName = authorRequest.AuthorName
                };

                _context.Authors.Add(newAuthor);
                await _context.SaveChangesAsync();

                return new JsonResponseModel(true, 200, "Success", newAuthor);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public async Task<JsonResponseModel> UpdateAuthor(AuthorCreateDTO authorRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(authorRequest.AuthorName))
                {
                    return new JsonResponseModel(true, 400, "Bad parameter. AuthorName is null or empty", null);
                }

                var authorCheck = await _context.Authors
                    .Where(a => a.AuthorName == authorRequest.AuthorName)
                    .FirstOrDefaultAsync();

                if (authorCheck != null)
                {
                    return new JsonResponseModel(true, 400, "Bad parameter. Author is already exist", null);
                }

                var author = await _context.Authors.FindAsync(authorRequest.AuthorId);

                if (author == null)
                {
                    return new JsonResponseModel(true, 404, "Not found", author);
                }

                _context.Entry(author).State = EntityState.Modified;
                author.AuthorName = authorRequest.AuthorName;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Authors.Any(a => a.AuthorId == author.AuthorId))
                    {
                        return new JsonResponseModel(true, 404, "Not found", author);
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
                var author = await _context.Authors.FindAsync(id);

                if (author == null)
                {
                    return new JsonResponseModel(true, 404, "Not found", author);
                }

                _context.Authors.Remove(author);
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

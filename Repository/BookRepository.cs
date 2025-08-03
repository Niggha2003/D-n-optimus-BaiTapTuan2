using BaiTap2.Contexts;
using BaiTap2.DTOs;
using BaiTap2.Models;
using BaiTap2.Services;
using BaiTap2.Models.APIResponseModels;
using Microsoft.EntityFrameworkCore;
using BaiTap2.Repository.IRepository;

namespace BaiTap2.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly DataContext _context;
        private readonly RedisCacheService _redisCacheService;

        public BookRepository(DataContext context, RedisCacheService redisCacheService)
        {
            _context = context;
            _redisCacheService = redisCacheService;
        }

        public async Task<JsonResponseModel> GetBookList()
        {
            try
            {
                var bookListRedis = await _redisCacheService.GetListData<BookModel>("bookList");
                if (bookListRedis == null)
                {
                    var bookList = await _context.Books.ToListAsync();
                    await _redisCacheService.SetAsync("bookList", bookList, TimeSpan.FromMinutes(1));
                    return new JsonResponseModel(true, 200, "Success", bookList);
                }

                return new JsonResponseModel(true, 200, "Success (from cache)", bookListRedis);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public async Task<JsonResponseModel> GetBookById(int id)
        {
            try
            {
                BookModel? book = null;
                var bookListRedis = await _redisCacheService.GetListData<BookModel>("bookList");

                if (bookListRedis != null)
                {
                    book = bookListRedis.Find(a => a.BookId == id);
                    if (book != null)
                    {
                        return new JsonResponseModel(true, 200, "Success (from cache)", book);
                    }
                }

                book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return new JsonResponseModel(true, 404, "Not found", null);
                }

                return new JsonResponseModel(true, 200, "Success", book);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public async Task<JsonResponseModel> GetBookByAuthorId(int authorId)
        {
            try
            {
                var book = await _context.Books
                    .Where(a => a.AuthorId == authorId)
                    .FirstOrDefaultAsync();

                if (book == null)
                    return new JsonResponseModel(true, 404, "Not found", null);

                return new JsonResponseModel(true, 200, "Success", book);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public async Task<JsonResponseModel> CreateNewBook(BookCreateDTO bookRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(bookRequest.Title))
                {
                    return new JsonResponseModel(true, 400, "Bad parameter. Title is null or empty", null);
                }

                var book = await _context.Books
                    .Where(a => a.Title == bookRequest.Title && a.AuthorId == bookRequest.AuthorId)
                    .FirstOrDefaultAsync();

                if (book != null)
                {
                    return new JsonResponseModel(true, 400, "Bad parameter. Book is already exist.", null);
                }

                var newBook = new BookModel
                {
                    Title = bookRequest.Title,
                    AuthorId = bookRequest.AuthorId
                };

                _context.Books.Add(newBook);
                await _context.SaveChangesAsync();

                // Set lại cache
                var bookList = await _context.Books.ToListAsync();
                await _redisCacheService.SetAsync("bookList", bookList, TimeSpan.FromMinutes(1));

                return new JsonResponseModel(true, 200, "Success", newBook);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }

        public async Task<JsonResponseModel> UpdateBook(BookCreateDTO bookRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(bookRequest.Title))
                {
                    return new JsonResponseModel(true, 400, "Bad parameter. Title is null or empty", null);
                }

                var bookCheck = await _context.Books
                    .Where(a => a.Title == bookRequest.Title && a.AuthorId == bookRequest.AuthorId)
                    .FirstOrDefaultAsync();

                if (bookCheck != null)
                {
                    return new JsonResponseModel(true, 400, "Bad parameter. Book is already exist.", null);
                }

                var book = await _context.Books.FindAsync(bookRequest.BookId);

                if (book == null)
                {
                    return new JsonResponseModel(true, 404, "Not found", null);
                }

                _context.Entry(book).State = EntityState.Modified;
                book.Title = bookRequest.Title;
                book.AuthorId = bookRequest.AuthorId;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Books.Any(a => a.BookId == bookRequest.BookId))
                    {
                        return new JsonResponseModel(true, 404, "Not found", null);
                    }
                    else
                    {
                        throw;
                    }
                }

                // Set lại cache
                var bookList = await _context.Books.ToListAsync();
                await _redisCacheService.SetAsync("bookList", bookList, TimeSpan.FromMinutes(1));

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
                var book = await _context.Books.FindAsync(id);

                if (book == null)
                {
                    return new JsonResponseModel(true, 404, "Not found", null);
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                // Set lại cache
                var bookList = await _context.Books.ToListAsync();
                await _redisCacheService.SetAsync("bookList", bookList, TimeSpan.FromMinutes(1));

                return new JsonResponseModel(true, 200, "Success", null);
            }
            catch (Exception ex)
            {
                return new JsonResponseModel(false, 500, ex.Message, null);
            }
        }
    }
}

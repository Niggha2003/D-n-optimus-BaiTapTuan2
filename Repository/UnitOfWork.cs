using BaiTap2.Contexts;
using BaiTap2.Services;
using Microsoft.EntityFrameworkCore.Storage;

namespace BaiTap2.Repository
{
    public class UnitOfWork : IDisposable
    {
        private readonly DataContext _context;
        private readonly RedisCacheService _redisCacheService;
        private readonly IDbContextTransaction _transaction;
        private bool disposed = false;  // ngăn chặn việc gọi Dispose nhiều lần
        public UnitOfWork(DataContext context, RedisCacheService redisCacheService)
        {
            _context = context;
            _redisCacheService = redisCacheService;

            _transaction = StartAsync();
        }
        public AuthorRepository AuthorRepository => new AuthorRepository(_context);
        public ReviewerRepository ReviewerRepository => new ReviewerRepository(_context);
        public BookRepository BookRepository => new BookRepository(_context, _redisCacheService);
        public ReviewRepository ReviewRepository => new ReviewRepository(_context, _redisCacheService);

        // start transaction
        public IDbContextTransaction StartAsync()
        {
            return _context.Database.BeginTransaction();
        }

        // save change transaction
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        // commit transaction
        public async Task CommitAsync()
        {
            await _transaction.CommitAsync();
        }

        // roll back transaction
        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposed = true;
            }
        }

        // dispose transaction
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Ngăn GC gọi finalizer sau khi đã được dispose thủ công, giúp hiệu năng tốt hơn.
        }
    }
}

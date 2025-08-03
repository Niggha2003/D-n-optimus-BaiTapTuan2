using Microsoft.EntityFrameworkCore;
using BaiTap2.Models;

namespace BaiTap2.Contexts
{
    // Lớp DbContext là cầu nối chính giữa các model và database.
    // Nó chịu trách nhiệm cho việc truy vấn và lưu dữ liệu.
    public class DataContext : DbContext
    {
        // Constructor này nhận vào các tùy chọn cấu hình (như chuỗi kết nối)
        // và truyền chúng cho lớp DbContext cơ sở.
        // Đây là cách Dependency Injection cung cấp cấu hình cho DbContext.
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // Mỗi thuộc tính DbSet<T> đại diện cho một bảng trong database.
        // EF Core sẽ sử dụng chúng để thực hiện các thao tác CRUD.
        public DbSet<AuthorModel> Authors { get; set; }
        public DbSet<BookModel> Books { get; set; }
        public DbSet<ReviewModel> Reviews { get; set; }
        public DbSet<ReviewerModel> Reviewers { get; set; }

        // fluentAPI
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Đánh index cho bảng review
            modelBuilder.Entity<ReviewModel>()
                .HasIndex(r => r.ReviewId)
                .IsUnique(); // Nếu muốn index là duy nhất
        }

    }
}



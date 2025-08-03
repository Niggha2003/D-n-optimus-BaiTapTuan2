using BaiTap2.Repository;
using BaiTap2.Repository.MongoDBRepository;

namespace BaiTap2.Services
{
    public static class AddRepositoryService
    {
        public static void AddMyRepositories(this IServiceCollection services)
        {
            // unit of work
            services.AddScoped<UnitOfWork>();

            // add repositories for controllers
            services.AddScoped<AuthorRepository>();
            services.AddScoped<BookRepository>();
            services.AddScoped<ReviewerRepository>();
            services.AddScoped<ReviewRepository>();

            // mongo repository
            services.AddScoped<ReviewMongoDBRepository>();
        }
    }
}

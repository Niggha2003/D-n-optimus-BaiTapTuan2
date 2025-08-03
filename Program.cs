using BaiTap2.Contexts;
using BaiTap2.Services;
using BaiTap2.Settings;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using StackExchange.Redis;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Lấy chuỗi kết nối từ file appsettings.json.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Đăng ký DataContext với hệ thống Dependency Injection (DI) của ASP.NET Core.
// Điều này cho phép các thành phần khác (như Controller) có thể yêu cầu một instance của DataContext.
builder.Services.AddDbContext<DataContext>(options =>
    // Cấu hình để DbContext sử dụng SQL Server.
    // connectionString được lấy từ appsettings.json ở trên.
    options.UseSqlServer(connectionString)
);

// Cấu hình Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnectionString");
    return ConnectionMultiplexer.Connect(redisConnectionString);
});
builder.Services.AddScoped<RedisCacheService>();

// config mongoDb
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

// Cấu hình repo
builder.Services.AddMyRepositories();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

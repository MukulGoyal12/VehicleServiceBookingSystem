using Microsoft.EntityFrameworkCore;
using OwnerService.Services;
using Serilog;

namespace OwnerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddScoped<IOwnerService, OwnerServiceImpl>();
            builder.Services.AddDbContext<Models.OwnerContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("OwnerDb"))
            );
            builder.Services.AddControllers();
            builder.Services.AddHttpClient();

            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
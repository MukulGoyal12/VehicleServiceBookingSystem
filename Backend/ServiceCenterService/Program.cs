using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using ServiceCenterService.Services;

namespace ServiceCenterService
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

            // Add services to the container.

            builder.Services.AddScoped<IServiceCenterService, ServiceCenterServiceImpl>();

            builder.Services.AddDbContext<Models.ServiceCenterContext>((options) =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("ServiceCenterDb"))
            );
            builder.Services.AddControllers().AddXmlSerializerFormatters();
            builder.Services.AddHttpClient();

            // Configure JWT authentication
            var secret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "fallback_super_secret_key_!ChangeThis!";
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "ownerService";
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "ServiceCenterService";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,

                };
            });
            var app = builder.Build();

            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

            // Configure the HTTP request pipeline.

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}

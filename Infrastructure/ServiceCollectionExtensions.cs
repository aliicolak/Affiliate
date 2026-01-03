using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Domain.Entities.Identity;
using Infrastructure.Persistence;
using Infrastructure.Security;
using Infrastructure.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Application.Abstractions.Persistence;
using Application.Abstractions.Services;

namespace Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // Identity Configuration
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // JWT options
            services.Configure<JwtOptions>(config.GetSection(JwtOptions.SectionName));

            var jwt = config.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));

            services
              .AddAuthentication(options =>
              {
                  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
              })
              .AddJwtBearer(o =>
              {
                  o.RequireHttpsMetadata = false;
                  o.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = jwt.Issuer,
                      ValidAudience = jwt.Audience,
                      IssuerSigningKey = key,
                      ClockSkew = TimeSpan.FromSeconds(30)
                  };
              });

            services.AddAuthorization();

            // Core services
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

            // Tracking services
            services.AddScoped<ITrackingCodeGenerator, TrackingCodeGenerator>();
            services.AddScoped<IGeoIpService, GeoIpService>();
            services.AddScoped<IUserAgentParser, UserAgentParser>();

            // Commission services
            services.AddScoped<ICommissionCalculator, CommissionCalculator>();

            // Notification services
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}


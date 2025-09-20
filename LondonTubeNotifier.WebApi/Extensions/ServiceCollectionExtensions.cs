using LondonTubeNotifier.Core.Configuration;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using LondonTubeNotifier.Core.MapperContracts;
using LondonTubeNotifier.Core.Mappers;
using LondonTubeNotifier.Core.ServiceContracts;
using LondonTubeNotifier.Core.Services;
using LondonTubeNotifier.Infrastructure.Data;
using LondonTubeNotifier.Infrastructure.Entities;
using LondonTubeNotifier.Infrastructure.ExternalAPIs;
using LondonTubeNotifier.Infrastructure.Repositories;
using LondonTubeNotifier.Infrastructure.Services;
using LondonTubeNotifier.Infrastructure.Workers;
using LondonTubeNotifier.WebApi.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RazorLight;
using System.Text;


namespace LondonTubeNotifier.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCaching(this IServiceCollection services)
        {
            services.AddMemoryCache();
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<LineRepository>();
            services.AddScoped<ILineRepository>(sp =>
                new CachedLineRepository(
                    sp.GetRequiredService<IMemoryCache>(),
                    sp.GetRequiredService<LineRepository>()
                ));

            services.AddScoped<LineStatusRepository>();
            services.AddScoped<ILineStatusRepository>(sp =>
                new CachedLineStatusRepository(
                    sp.GetRequiredService<IMemoryCache>(),
                    sp.GetRequiredService<LineStatusRepository>()
                ));

            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }

        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<ILineService, LineService>();
            services.AddScoped<IUserLineSubscriptionService, UserLineSubscriptionService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ITflStatusMonitorService, TflStatusMonitorService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<ILineMapper, LineMapper>();
            return services;
        }

        public static IServiceCollection AddExternalAPIs(this IServiceCollection services)
        {
            services.AddSingleton<ITflApiService, TflApiService>();
            return services;
        }

        public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JwtSettings:Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]!))
                };
            });
            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(AppContext.BaseDirectory + "WebApi.xml");
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by your JWT token"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            return services;
        }

        public static IServiceCollection AddSignalRHubs(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<IUserOnlineChecker, OnlineUsersTracker>();
            services.AddSingleton<IRealtimeNotifier, SignalRRealtimeNotifier>();
            return services;
        }

        public static IServiceCollection AddCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:5173")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
            return services;
        }

        public static IServiceCollection AddNotificationsAndEmail(this IServiceCollection services)
        {
            services.AddScoped<INotificationService, NotificationService>();
            var razorEngine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(typeof(EmailTemplateService))
                .UseMemoryCachingProvider()
                .Build();
            services.AddSingleton<IRazorLightEngine>(razorEngine);
            services.AddSingleton<IEmailTemplateService, EmailTemplateService>();
            return services;
        }

        public static IServiceCollection AddHostedServices(this IServiceCollection services)
        {
            services.AddHostedService<TflLineStatusWorker>();
            return services;
        }

        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Default"));
            });
            return services;
        }

        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<ITflApiService, TflApiService>((sp, client) =>
            {
                var tflOptions = sp.GetRequiredService<IOptions<TflSettings>>().Value;
                client.BaseAddress = new Uri(tflOptions.BaseUrl);
                client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            });
            return services;
        }
    }
}

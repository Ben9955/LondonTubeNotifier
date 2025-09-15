using LondonTubeNotifier.Core.Configuration;

namespace LondonTubeNotifier.WebApi.Extensions
{
    public static class ConfigureServicesExtensions
    {
        public static void ConfigureServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            services.AddDbContext(builder.Configuration)
                    .AddCaching()
                    .AddRepositories()
                    .AddCoreServices()
                    .AddHttpClients(builder.Configuration)
                    .AddAuthenticationAndAuthorization(builder.Configuration)
                    .AddSwagger()
                    .AddSignalRHubs()
                    .AddNotificationsAndEmail();

            services.AddHostedServices();

            services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
            services.Configure<TflSettings>(builder.Configuration.GetSection("Tfl"));
            services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGrid"));

            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders |
                                        Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseStatusCode;
            });
            builder.Logging.ClearProviders().AddConsole();
            if (builder.Environment.IsDevelopment())
            {
                builder.Logging.AddDebug();
            }
        }
    }
}
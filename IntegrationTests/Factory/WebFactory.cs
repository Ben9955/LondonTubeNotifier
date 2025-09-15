using LondonTubeNotifier.Core.ServiceContracts;
using LondonTubeNotifier.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Factory
{
    public class WebFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                options.UseInMemoryDatabase("DatabaseForTesting");
                });

                // Remove real JwtService if registered
                var jwtDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IJwtService));
                if (jwtDescriptor != null)
                    services.Remove(jwtDescriptor);

                // Add fake JWT service for testing
                services.AddSingleton<IJwtService, FakeJwtService>();

                // Remove existing authentication scheme provider
                var authDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAuthenticationSchemeProvider));
                if (authDescriptor != null)
                    services.Remove(authDescriptor);

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "TestScheme";
                    options.DefaultChallengeScheme = "TestScheme";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });


                // Remove real EmailService
                var emailDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmailService));
                if (emailDescriptor != null)
                    services.Remove(emailDescriptor);

                // Add fake EmailService
                services.AddScoped<IEmailService, FakeEmailService>();
            });
        }
    }
}
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;
using LondonTubeNotifier.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests
{
    public class WebFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("DatabaseForTesting"); 
                });


                // Remove real JwtService if registered
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IJwtService));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add a fake implementation for testing
                services.AddSingleton<IJwtService, FakeJwtService>();
            });
        }
    }
}

public class FakeJwtService : IJwtService
{
    public AuthenticationDto CreateJwtToken(JwtUserDto user)
    {
        return new AuthenticationDto
        {
            AccessToken = "fake-access-token",
            AccessTokenExpiration = DateTime.UtcNow.AddHours(1),
            RefreshToken = "fake-refresh-token",
            RefreshTokenExpiration = DateTime.UtcNow.AddDays(7)
        };
    }
}

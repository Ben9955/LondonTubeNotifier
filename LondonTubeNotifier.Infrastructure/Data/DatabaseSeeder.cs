using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.ServiceContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace LondonTubeNotifier.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var lineColors = new Dictionary<string, string>
            {
                { "bakerloo", "#B36305" },
                { "central", "#E32017" },
                { "circle", "#FFD300" },
                { "district", "#00782A" },
                { "dlr", "#00A4A7" },
                { "elizabeth", "#997A8D" },
                { "hammersmith-city", "#F3A9BB" },
                { "jubilee", "#A0A5A9" },
                { "liberty", "#E86A10" },
                { "lioness", "#E86A10" },
                { "metropolitan", "#9B0056" },
                { "mildmay", "#E86A10" },
                { "northern", "#000000" },
                { "piccadilly", "#0019A8" },
                { "suffragette", "#E86A10" },
                { "tram", "#84B817" },
                { "victoria", "#00A0E2" },
                { "waterloo-city", "#95CDBA" },
                { "weaver", "#E86A10" },
                { "windrush", "#E86A10" }
            };
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        var tflService = services.GetRequiredService<ITflApiService>();
        var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            // Ensure the database is created and migrations are applied
            await dbContext.Database.MigrateAsync();

                // Check if the Lines table is already populated to avoid duplicate entries
                if (!await dbContext.Lines.AnyAsync())
            {
                logger.LogInformation("Seeding database with TFL line data...");
                var linesFromApi = await tflService.GetLinesAsync();

                var linesToInsert = linesFromApi?.Select(l => new Line
                {
                    Id = l.Id,
                    Name = l.Name,
                    ModeName = l.ModeName,
                    Color = lineColors.ContainsKey(l.Id) ? lineColors[l.Id] : "#CCCCCC" 
                }).ToList();

                await dbContext.Lines.AddRangeAsync(linesToInsert);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("TFL line data successfully seeded.");
            }
            else
            {
                logger.LogInformation("TFL line data already exists. Skipping seeding.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database with TFL data.");
        }
    }
}
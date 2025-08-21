using LondonTubeNotifier.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LondonTubeNotifier.Core.ServiceContracts;
using LondonTubeNotifier.Core.Services;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using LondonTubeNotifier.Infrastructure.Repositories;
using LondonTubeNotifier.Core.MapperContracts;
using LondonTubeNotifier.Core.Mappers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});


builder.Services.AddScoped<ILineService, LineService>();
builder.Services.AddScoped<ILineRepository, LineRepository>();
builder.Services.AddSingleton<ILineMapper, LineMapper>();

builder.Logging.ClearProviders().AddConsole();
if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddDebug();
}

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders |
                            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseStatusCode;
});

var app = builder.Build();

app.UseHttpLogging();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();

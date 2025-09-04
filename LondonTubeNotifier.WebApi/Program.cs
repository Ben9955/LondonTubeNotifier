using LondonTubeNotifier.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LondonTubeNotifier.Core.ServiceContracts;
using LondonTubeNotifier.Core.Services;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using LondonTubeNotifier.Infrastructure.Repositories;
using LondonTubeNotifier.Core.MapperContracts;
using LondonTubeNotifier.Core.Mappers;
using LondonTubeNotifier.WebApi.Middleware;
using Microsoft.AspNetCore.Identity;
using LondonTubeNotifier.Core.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LondonTubeNotifier.Infrastructure.Entities;
using System.IdentityModel.Tokens.Jwt;
using LondonTubeNotifier.Infrastructure.ExternalAPIs;
using Microsoft.Extensions.Options;
using System.Net.Http;
using LondonTubeNotifier.Infrastructure.Workers;
using Microsoft.Extensions.Caching.Memory;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});


builder.Services.AddScoped<ILineService, LineService>();
builder.Services.AddScoped<IUserLineSubscriptionService, UserLineSubscriptionService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<ILineMapper, LineMapper>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddSingleton<ITflApiService, TflApiService>();

builder.Services.AddScoped<LineRepository>();
builder.Services.AddScoped<ILineRepository>(sp =>
    new CachedLineRepository(
        sp.GetRequiredService<IMemoryCache>(),
        sp.GetRequiredService<LineRepository>()
    )
);

builder.Services.AddScoped<LineStatusRepository>();
builder.Services.AddScoped<ILineStatusRepository>(sp =>
    new CachedLineStatusRepository(
        sp.GetRequiredService<IMemoryCache>(),
        sp.GetRequiredService<LineStatusRepository>()
    )
);


builder.Services.AddHostedService<TflLineStatusWorker>();

builder.Services.AddHttpClient<ITflApiService, TflApiService>((sp, client) =>
{
    var tflOptions = sp.GetRequiredService<IOptions<TflSettings>>().Value;
    client.BaseAddress = new Uri(tflOptions.BaseUrl);
    client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
});

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<TflSettings>(
    builder.Configuration.GetSection("Tfl"));


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




// Identity
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
        };
    });


//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(AppContext.BaseDirectory + "WebApi.xml");

    // Add JWT Bearer support
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your JWT token"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    }); 
});

var app = builder.Build();

app.UseHttpLogging();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseSwagger();    
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();


public partial class Program { }

using LondonTubeNotifier.WebApi.Middleware;
using System.IdentityModel.Tokens.Jwt;
using LondonTubeNotifier.WebApi.Hubs;
using LondonTubeNotifier.WebApi.Extensions;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

// Configure all services
builder.Services.ConfigureServices(builder);


var app = builder.Build();

app.UseHttpLogging();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseSwagger();    
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<TflLiveHub>("/TflLiveHub");

app.MapControllers();

app.Run();

public partial class Program { }

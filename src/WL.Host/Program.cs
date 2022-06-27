using System.Net;
using System.Text;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using WL.Host.DbContexts;
using WL.Host.Extensions;
using WL.Host.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Host.UseSerilogProvider(builder.Configuration["Elasticsearch:Url"]);

builder.Services
    .AddControllers()
    .AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters();

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

builder.Services.AddTransient<IBlackListService, BlackListService>();

builder.Services.AddGrpcClient<WL.BlackList.BlackListService.BlackListServiceClient>(client =>
        client.Address = new Uri(builder.Configuration["ApiConfig:BlackList:Uri"]))
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IWishBasketService, WishBasketService>(client =>
        client.BaseAddress = new Uri(builder.Configuration["ApiConfig:WishBasket:Uri"]))
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddDbContext<WishesContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Main");
    options.UseSqlite(connectionString);
});

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
builder.Services.AddScoped<IWishRepository, WishRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddHealthChecks();
builder.Services.AddResponseCompression();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new ()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"])),
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustHaveUsernamePolaroid15", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("user_name", "Polaroid15");
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "wl v1"); });
}

app.UseExceptionHandler("/error");
app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseResponseCompression();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health", new HealthCheckOptions()
    {
        ResultStatusCodes = new Dictionary<HealthStatus, int>()
        {
            [HealthStatus.Healthy] = 200,
            [HealthStatus.Degraded] = 400,
            [HealthStatus.Unhealthy] = 500,
        },
    });
});

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(3, TimeSpan.FromSeconds(3));
}
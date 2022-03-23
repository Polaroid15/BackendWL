using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WL.Host.DbContexts;
using WL.Host.Extensions;
using WL.Host.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters();

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddDbContext<WishesContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Main");
    options.UseSqlite(connectionString);
});
builder.Services.AddScoped<IWishRepository, WishRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddHealthChecks();
builder.Services.AddResponseCompression();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "wl v1"); });
}

app.UseExceptionHandler("/error");
app.UseRouting();
app.UseHttpsRedirection();

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
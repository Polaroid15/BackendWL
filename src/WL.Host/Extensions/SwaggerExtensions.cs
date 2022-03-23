using Microsoft.OpenApi.Models;

namespace WL.Host.Extensions;

public static class SwaggerExtensions
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Whishlist API",
                Description = "Backend for wishlist API",
                Contact = new OpenApiContact
                {
                    Name = "Our Contact",
                    Url = new Uri("https://wishlist.com/contact")
                }
            });
        });
    }
}
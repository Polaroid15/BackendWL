using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WL.Host.Extensions;

/// <summary>
/// Swagger extensions
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Using AddSwaggerGen extension.
    /// </summary>
    /// <param name="services"></param>
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(setup =>
        {
            var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
            setup.IncludeXmlComments(xmlCommentsFullPath);

            var schemeId = "WishInfoApiBearerAuth";
            setup.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                Description = "Input a valid token to access this API",
            });
            setup.SchemaFilter<SwaggerExcludeFilter>();
            setup.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = schemeId,
                        },
                    },
                    new List<string>()
                },
            });
        });
    }
}

/// <summary>
/// Exclude properties from swagger documentation.
/// </summary>
public class SwaggerExcludeFilter : ISchemaFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema?.Properties == null)
        {
            return;
        }
    
        var excludeMemberProperties = context.Type.GetProperties()
            .Where(t => t.GetCustomAttribute<SwaggerExcludeAttribute>() != null);
    
        foreach (var ignoreDataMemberProperty in excludeMemberProperties)
        {
            var propertyToHide = schema.Properties.Keys
                .SingleOrDefault(x => x.ToLower() == ignoreDataMemberProperty.Name.ToLower());
    
            if (propertyToHide != null)
            {
                schema.Properties.Remove(propertyToHide);
            }
        }
    }
}

/// <summary>
/// attribute to exclude from swagger documentation.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SwaggerExcludeAttribute : Attribute
{
}
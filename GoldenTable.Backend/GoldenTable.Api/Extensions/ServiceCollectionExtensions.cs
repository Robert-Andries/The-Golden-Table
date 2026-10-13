using Microsoft.OpenApi;

namespace GoldenTable.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddSwaggerGenWithAuthSupport(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSwaggerGen(opts =>
        {
            opts.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
            opts.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    AuthorizationCode =  new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri(configuration["Keycloak:AuthorizationUrl"]!),
                        TokenUrl =  new Uri(configuration["Keycloak:TokenUrl"]!),
                        Scopes = new Dictionary<string, string>()
                        {
                            {"openid", "openid"},
                            {"profile", "profile"}
                        }
                    }
                }
            });
            
            opts.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Keycloak", doc),
                    []
                }
            });
        });
        
        return services;
    }
}

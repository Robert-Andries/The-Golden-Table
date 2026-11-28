using Microsoft.AspNetCore.Builder;

namespace GoldenTable.Common.Presentation.Endpoints;

public static class PermissionsExtensions
{
    public static TBuilder RequirePermission<TBuilder>(this TBuilder builder, params string[] permissions)
        where TBuilder : IEndpointConventionBuilder
    {
        if (permissions.Length == 0)
        {
            builder.RequireAuthorization();
            return builder;
        }

        return builder.RequireAuthorization(policy =>
        {
            foreach (string permission in permissions)
            {
                policy.RequireRole(permission);
            }
        });
    }
}

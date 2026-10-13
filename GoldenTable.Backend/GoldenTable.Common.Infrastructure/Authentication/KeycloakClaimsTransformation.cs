using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

namespace GoldenTable.Common.Infrastructure.Authentication;

internal sealed class KeycloakClaimsTransformation(IConfiguration configuration) : IClaimsTransformation
{
    private const string TransformedClaimType = "urn:goldentable:transformed";

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
        {
            return Task.FromResult(principal);
        }

        if (identity.HasClaim(c => c.Type == TransformedClaimType))
        {
            return Task.FromResult(principal);
        }

        identity.AddClaim(new Claim(TransformedClaimType, "true"));

        string? clientId = configuration["Keycloak:ClientId"];

        string? resourceAccessJson = principal.FindFirst("resource_access")?.Value;
        if (!string.IsNullOrWhiteSpace(resourceAccessJson))
        {
            try
            {
                using var doc = JsonDocument.Parse(resourceAccessJson);
                foreach (JsonProperty client in doc.RootElement.EnumerateObject())
                {
                    if (clientId is not null && !string.Equals(client.Name, clientId, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (client.Value.TryGetProperty("roles", out JsonElement roles) &&
                        roles.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement role in roles.EnumerateArray())
                        {
                            if (role.GetString() is { } r)
                            {
                                AddRoleClaim(identity, r);
                            }
                        }
                    }
                }
            }
            catch (JsonException)
            {
                // Ignore tokens with invalid JSON in claims
            }
        }

        string? realmAccessJson = principal.FindFirst("realm_access")?.Value;
        if (!string.IsNullOrWhiteSpace(realmAccessJson))
        {
            try
            {
                using var doc = JsonDocument.Parse(realmAccessJson);
                if (doc.RootElement.TryGetProperty("roles", out JsonElement roles) &&
                    roles.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement role in roles.EnumerateArray())
                    {
                        if (role.GetString() is { } r)
                        {
                            AddRoleClaim(identity, r);
                        }
                    }
                }
            }
            catch (JsonException)
            {
                // Ignore tokens with invalid JSON in claims
            }
        }

        return Task.FromResult(principal);
    }

    private static void AddRoleClaim(ClaimsIdentity identity, string role)
    {
        if (!identity.HasClaim(identity.RoleClaimType, role))
        {
            identity.AddClaim(new Claim(identity.RoleClaimType, role));
        }

        if (identity.RoleClaimType != "roles" && !identity.HasClaim("roles", role))
        {
            identity.AddClaim(new Claim("roles", role));
        }
    }
}

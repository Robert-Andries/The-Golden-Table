using System.Security.Claims;
using GoldenTable.Common.Domain;

namespace GoldenTable.Common.Presentation;


public static class Authentication
{
    /// <summary>
    /// Gets the user id from the provided claims
    /// </summary>
    /// <returns>The user id</returns>
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        string? id = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
        return Guid.TryParse(id, out Guid g) ? g : throw new InvalidOperationException("User id unavailable");
    }
}

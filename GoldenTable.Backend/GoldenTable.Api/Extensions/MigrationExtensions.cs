using GoldenTable.Modules.Catalog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Api.Extensions;

internal static class MigrationExtensions
{
    internal static void ApplyMigrations(this WebApplication app)
    {
        using IServiceScope scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        ApplyMigration<CatalogDbContext>(scope);
    }

    private static void ApplyMigration<TDbContext>(IServiceScope scope)
        where TDbContext : DbContext
    {
        using TDbContext context = scope.ServiceProvider.GetRequiredService<TDbContext>();
        if (context.Database.IsRelational())
        {
            context.Database.Migrate();
        }
    }
}

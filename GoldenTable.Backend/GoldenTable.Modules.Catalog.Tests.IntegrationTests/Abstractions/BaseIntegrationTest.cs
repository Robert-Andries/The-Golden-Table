using Bogus;
using GoldenTable.Modules.Catalog.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

[Collection(nameof(IntegrationTestCollection))]
public abstract class BaseIntegrationTest : IDisposable
{
    protected static CatalogDbContext context;
    private readonly IServiceScope _scope;
    protected readonly Faker Faker = new();
    protected readonly ISender Sender;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        context = _scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        GlobalRules.FluentAssertions.ConfigureGlobalSettings();
    }

    public void Dispose()
    {
        _scope.Dispose();
    }

    protected async Task ClearDatabaseAsync()
    {
        var tableNames = context.Model.GetEntityTypes()
            .Select(t => $"\"{t.GetSchema() ?? "public"}\".\"{t.GetTableName()}\"")
            .Distinct()
            .ToList();

        if (!tableNames.Any())
        {
            return;
        }

        string sql = $"TRUNCATE TABLE {string.Join(", ", tableNames)} RESTART IDENTITY CASCADE;";

        await context.Database.ExecuteSqlRawAsync(sql);
    }
}

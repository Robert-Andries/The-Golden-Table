using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace GoldenTable.Modules.Orders.Tests.IntegrationTests.Abstractions;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly RedisContainer _cacheContainer = new RedisBuilder("redis:latest")
        .Build();

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:latest")
        .WithDatabase("goldentable")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public async Task InitializeAsync()
    {
        await _cacheContainer.StartAsync();
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _cacheContainer.StopAsync();
        await _dbContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        Environment.SetEnvironmentVariable("ConnectionStrings:Database", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings:Cache", _cacheContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("Catalog:Outbox:IntervalInSeconds", "1");
        Environment.SetEnvironmentVariable("Catalog:Inbox:IntervalInSeconds", "1");
        Environment.SetEnvironmentVariable("Orders:Outbox:IntervalInSeconds", "1");
        Environment.SetEnvironmentVariable("Orders:Inbox:IntervalInSeconds", "1");
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

internal class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly RedisContainer _cacheContainer = new RedisBuilder("redis:8.6").Build();
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:18")
        .WithDatabase("goldentable")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings:Database", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings:Cache", _cacheContainer.GetConnectionString());
    }


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
}

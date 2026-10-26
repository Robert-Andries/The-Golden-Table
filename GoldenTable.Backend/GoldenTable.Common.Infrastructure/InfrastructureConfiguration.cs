using GoldenTable.Common.Application.Caching;
using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Data;
using GoldenTable.Common.Infrastructure.Authentication;
using GoldenTable.Common.Infrastructure.Caching;
using GoldenTable.Common.Infrastructure.Clock;
using GoldenTable.Common.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Quartz;
using StackExchange.Redis;

namespace GoldenTable.Common.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string databaseConnectionString,
        string redisConnectionString)
    {
        services.AddAuthenticationInternal();
        
        NpgsqlDataSource npgsqlDataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();
        services.TryAddSingleton(npgsqlDataSource);

        services.TryAddSingleton<IDbConnectionFactory, DbConnectionFactory>();

        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
        
        services.AddQuartz();
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
        services.TryAddSingleton(multiplexer);
        services.AddStackExchangeRedisCache(options =>
            options.ConnectionMultiplexerFactory = () => Task.FromResult(multiplexer));

        services.TryAddSingleton<ICacheService, CacheService>();

        return services;
    }
}

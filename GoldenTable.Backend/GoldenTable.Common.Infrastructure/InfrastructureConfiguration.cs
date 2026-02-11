using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Data;
using GoldenTable.Common.Infrastructure.Clock;
using GoldenTable.Common.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;

namespace GoldenTable.Common.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string databaseConnectionString)
    {
        NpgsqlDataSource npgsqlDataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();
        services.TryAddSingleton(npgsqlDataSource);
        
        services.TryAddSingleton<IDbConnectionFactory, DbConnectionFactory>();
        
        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
        
        return services;
    }
}

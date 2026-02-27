using System.Text.Json.Serialization;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Application.Abstractions.Dataset;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;
using GoldenTable.Modules.Catalog.Infrastructure.Database;
using GoldenTable.Modules.Catalog.Infrastructure.Dishes;
using GoldenTable.Modules.Catalog.Infrastructure.Images;
using GoldenTable.Modules.Catalog.Presentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoldenTable.Modules.Catalog.Infrastructure;

public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpoints(AssemblyReference.Assembly);

        services.AddInfrastructure(configuration);

        return services;
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string databaseConnectionString = configuration.GetConnectionString("Database");
        if (string.IsNullOrEmpty(databaseConnectionString))
        {
            throw new Exception("Database connection string is empty");
        }

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        services.AddDbContext<CatalogDbContext>((sp, options) =>
            options.UseNpgsql(databaseConnectionString, npgsqlOptions =>
                npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Catalog)));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CatalogDbContext>());

        services.AddScoped<IDishRepository, DishRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddSingleton<IDishCacheService, DishCacheService>();
        services.AddSingleton<IImageCacheService, ImageCacheService>();
        services.AddScoped<IDishDbSets>(sp => sp.GetRequiredService<CatalogDbContext>());
    }
}

using System.Text.Json.Serialization;
using GoldenTable.Common.Presentation.Endpoints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoldenTable.Modules.Catalog.Infrastructure;

public static class CatalogModule
{
    public static IServiceCollection Add(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        services.AddInfrastructure(configuration);

        return services;
    }
    
    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string databaseConnectionString = configuration.GetConnectionString("Catalog");
        if (string.IsNullOrEmpty(databaseConnectionString))
        {
            throw new Exception("Database connection string is empty");
        }
        
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });  
        //
        // services.AddDbContext<EventsDbContext>((sp, options) =>
        //     options
        //         .UseNpgsql(
        //             databaseConnectionString,
        //             npgsqlOptions => npgsqlOptions
        //                 .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Events))
        //         .UseSnakeCaseNamingConvention()
        //         .AddInterceptors(sp.GetRequiredService<PublishDomainEventsInterceptor>()));
        //
        // services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<EventsDbContext>());
        //
        // services.AddScoped<IEventRepository, EventRepository>();
        // services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
        // services.AddScoped<ICategoryRepository, CategoryRepository>();
    }
}

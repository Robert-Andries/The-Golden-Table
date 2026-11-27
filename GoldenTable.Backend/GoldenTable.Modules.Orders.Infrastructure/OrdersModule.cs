using GoldenTable.Common.Application.EventBus;
using GoldenTable.Common.Infrastructure.Inbox;
using GoldenTable.Common.Infrastructure.Outbox;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Modules.Catalog.IntegrationEvents;
using GoldenTable.Modules.Orders.Application.Abstractions.Data;
using GoldenTable.Modules.Orders.Domain.Dishes.Abstractions;
using GoldenTable.Modules.Orders.Domain.Orders.Abstractions;
using GoldenTable.Modules.Orders.Infrastructure.Database;
using GoldenTable.Modules.Orders.Infrastructure.Dishes;
using GoldenTable.Modules.Orders.Infrastructure.Inbox;
using GoldenTable.Modules.Orders.Infrastructure.Orders;
using GoldenTable.Modules.Orders.Infrastructure.Outbox;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoldenTable.Modules.Orders.Infrastructure;

public static class OrdersModule
{
    public static IServiceCollection AddOrdersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        services.AddDomainEventHandlers(Application.AssemblyReference.Assembly);

        // The handlers that keep the local dish copies in sync live in the Application layer
        services.AddIntegrationEventHandlers(Application.AssemblyReference.Assembly);

        services.AddInfrastructure(configuration);

        services.AddOutboxJob<ProcessOutboxJob>(configuration.GetSection("Orders:Outbox"));

        services.AddInboxJob<ProcessInboxJob>(configuration.GetSection("Orders:Inbox"));

        return services;
    }

    /// <summary>
    ///     Stores the Catalog dish events in the Orders inbox. The inbox job runs the handlers later.
    /// </summary>
    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator)
    {
        AddInboxConsumer<DishCreatedIntegrationEvent>(registrationConfigurator, "orders-dish-created");
        AddInboxConsumer<DishDeletedIntegrationEvent>(registrationConfigurator, "orders-dish-deleted");
        AddInboxConsumer<DishUpdatedNameIntegrationEvent>(registrationConfigurator, "orders-dish-updated-name");
        AddInboxConsumer<DishUpdatedPriceIntegrationEvent>(registrationConfigurator, "orders-dish-updated-price");
        AddInboxConsumer<DishUpdatedSizesIntegrationEvent>(registrationConfigurator, "orders-dish-updated-sizes");
    }

    // Every consumer is the same generic type, so each gets an explicit queue name
    // instead of one derived from the type, which would be shared between them
    private static void AddInboxConsumer<TIntegrationEvent>(
        IRegistrationConfigurator registrationConfigurator,
        string endpointName)
        where TIntegrationEvent : class, IIntegrationEvent
    {
        registrationConfigurator
            .AddConsumer<IntegrationEventConsumer<TIntegrationEvent, OrdersDbContext>>()
            .Endpoint(endpoint => endpoint.Name = endpointName);
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string? databaseConnectionString = configuration.GetConnectionString("Database");
        if (string.IsNullOrEmpty(databaseConnectionString))
        {
            throw new InvalidOperationException("Database connection string is empty");
        }

        services.AddDbContext<OrdersDbContext>((sp, options) =>
            options.UseNpgsql(databaseConnectionString, npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Orders))
                .AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>()));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<OrdersDbContext>());

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IDishRepository, DishRepository>();
        services.AddSingleton<IOrderCacheService, OrderCacheService>();
        services.AddSingleton<IDishCacheService, DishCacheService>();
    }
}

using Bogus;
using GoldenTable.Common.Application.EventBus;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Domain.Money;
using GoldenTable.Common.Infrastructure.Inbox;
using GoldenTable.Common.Infrastructure.Outbox;
using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Domain.Dishes.ValueObjects;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;
using GoldenTable.Modules.Orders.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GoldenTable.Modules.Orders.Tests.IntegrationTests.Abstractions;

[Collection(nameof(IntegrationTestCollection))]
public abstract class BaseIntegrationTest : IAsyncLifetime
{
    private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(30);

    private readonly AsyncServiceScope _scope;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        Factory = factory;
        _scope = factory.Services.CreateAsyncScope();
        Context = _scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    }

    protected IntegrationTestWebAppFactory Factory { get; }

    protected Faker Faker { get; } = new();

    /// <summary>
    ///     Used to seed and inspect the database. Read with AsNoTracking so assertions see what was saved,
    ///     not an instance this context already tracks.
    /// </summary>
    protected OrdersDbContext Context { get; }

    public async Task InitializeAsync()
    {
        await ClearDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    /// <summary>
    ///     Sends a command or query in its own scope, like a request does, so its DbContext starts empty
    /// </summary>
    protected async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        await using AsyncServiceScope scope = Factory.Services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<ISender>().Send(request);
    }

    /// <summary>
    ///     Publishes an integration event on the bus, the way the Catalog module does
    /// </summary>
    protected async Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent)
        where TIntegrationEvent : IIntegrationEvent
    {
        await using AsyncServiceScope scope = Factory.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IEventBus>().PublishAsync(integrationEvent);
    }

    protected Address NewAddress()
    {
        return new Address(
            Faker.Address.County(),
            Faker.Address.City(),
            Faker.Address.StreetName(),
            Faker.Address.BuildingNumber(),
            Faker.Address.ZipCode());
    }

    protected async Task<Dish> PutDishInDbAsync(
        decimal basePrice = 10m,
        string currency = "EUR",
        params DishSize[] sizes)
    {
        Dish dish = Dish.Create(
            Guid.NewGuid(),
            new Name(Faker.Commerce.ProductName()),
            Money.Create(basePrice, currency).Value,
            DateTime.UtcNow,
            sizes.ToList()).Value;
        dish.ClearDomainEvents();

        Context.Dishes.Add(dish);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        return dish;
    }

    /// <summary>
    ///     Saves an order with one dish, moved through the allowed transitions until it reaches the given status
    /// </summary>
    protected async Task<Order> PutOrderInDbAsync(
        Guid customerId,
        OrderStatus status = OrderStatus.Placed,
        DateTime? createdOnUtc = null)
    {
        Dish dish = await PutDishInDbAsync();
        Order order = Order.Create(
            customerId,
            dish.BasePrice,
            [new DishItem(dish.Id, null, 1)],
            NewAddress(),
            createdOnUtc ?? DateTime.UtcNow).Value;

        foreach (OrderStatus step in TransitionsTo(status))
        {
            order.UpdateStatus(step, DateTime.UtcNow);
        }

        order.ClearDomainEvents();

        Context.Orders.Add(order);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        return order;
    }

    protected Task<Order> GetOrderFromDbAsync(Guid orderId)
    {
        return Context.Orders.AsNoTracking().FirstAsync(o => o.Id == orderId);
    }

    protected Task<Dish?> FindDishInDbAsync(Guid dishId)
    {
        return Context.Dishes.AsNoTracking().FirstOrDefaultAsync(d => d.Id == dishId);
    }

    /// <summary>
    ///     Waits until the inbox job has processed the message, successfully or not
    /// </summary>
    protected async Task<InboxMessage> WaitForProcessedInboxMessageAsync(Guid messageId)
    {
        InboxMessage? message = null;
        await WaitUntilAsync(async () =>
        {
            message = await Context.Set<InboxMessage>()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == messageId && m.ProcessedOnUtc != null);
            return message is not null;
        }, $"the inbox message '{messageId}' to be processed");

        return message!;
    }

    /// <summary>
    ///     Waits until the outbox job has processed every message of the given type for the given order
    /// </summary>
    protected async Task<OutboxMessage> WaitForProcessedOutboxMessageAsync(string type, Guid orderId)
    {
        OutboxMessage? message = null;
        await WaitUntilAsync(async () =>
        {
            message = await Context.Set<OutboxMessage>()
                .AsNoTracking()
                .Where(m => m.Type == type && m.ProcessedOnUtc != null)
                .AsAsyncEnumerable()
                .FirstOrDefaultAsync(m => m.Content.Contains(orderId.ToString(), StringComparison.Ordinal));
            return message is not null;
        }, $"the outbox message '{type}' of order '{orderId}' to be processed");

        return message!;
    }

    /// <summary>
    ///     The background jobs run on their own schedule, so poll until the condition holds
    /// </summary>
    protected static async Task WaitUntilAsync(Func<Task<bool>> condition, string description)
    {
        DateTime deadlineUtc = DateTime.UtcNow.Add(WaitTimeout);
        while (DateTime.UtcNow < deadlineUtc)
        {
            if (await condition())
            {
                return;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(200));
        }

        throw new TimeoutException($"Timed out waiting for {description}.");
    }

    private static OrderStatus[] TransitionsTo(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.Placed => [],
            OrderStatus.Confirmed => [OrderStatus.Confirmed],
            OrderStatus.Preparing => [OrderStatus.Confirmed, OrderStatus.Preparing],
            OrderStatus.Sent => [OrderStatus.Confirmed, OrderStatus.Preparing, OrderStatus.Sent],
            OrderStatus.Rejected => [OrderStatus.Rejected],
            OrderStatus.Cancelled => [OrderStatus.Cancelled],
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    private async Task ClearDatabaseAsync()
    {
        var tableNames = Context.Model.GetEntityTypes()
            .Where(t => t.GetTableName() is not null && !t.IsOwned())
            .Select(t => $"\"{t.GetSchema()}\".\"{t.GetTableName()}\"")
            .Distinct()
            .ToList();

        // Table names come from the EF model, not from input
        string sql = $"TRUNCATE TABLE {string.Join(", ", tableNames)} CASCADE;";
        await Context.Database.ExecuteSqlRawAsync(sql);
    }
}

using FluentAssertions;
using GoldenTable.Common.Application.EventBus;
using GoldenTable.Common.Infrastructure.Inbox;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace GoldenTable.Common.Tests.UnitTests.Inbox;

public sealed class ReceivedIntegrationEvent(Guid id, DateTime occurredOnUtc, string value)
    : IntegrationEvent(id, occurredOnUtc)
{
    public string Value { get; } = value;
}

public sealed class TestInboxDbContext(DbContextOptions<TestInboxDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
    }
}

public sealed class ReceiveIntegrationEvents : IAsyncLifetime
{
    private readonly ServiceProvider _serviceProvider;
    private readonly ITestHarness _harness;

    public ReceiveIntegrationEvents()
    {
        ServiceCollection services = new();
        string databaseName = Guid.NewGuid().ToString();
        services.AddDbContext<TestInboxDbContext>(options => options.UseInMemoryDatabase(databaseName));
        services.AddMassTransitTestHarness(configure =>
            configure.AddConsumer<IntegrationEventConsumer<ReceivedIntegrationEvent, TestInboxDbContext>>());
        _serviceProvider = services.BuildServiceProvider(true);
        _harness = _serviceProvider.GetRequiredService<ITestHarness>();
    }

    public Task InitializeAsync()
    {
        return _harness.Start();
    }

    public async Task DisposeAsync()
    {
        await _harness.Stop();
        await _serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Should_StoreTheIntegrationEventInTheInbox()
    {
        // Arrange
        ReceivedIntegrationEvent integrationEvent = new(Guid.NewGuid(), DateTime.UtcNow, "Pizza");

        // Act
        await _harness.Bus.Publish(integrationEvent);
        await WaitUntilConsumedAsync(1);

        // Assert
        List<InboxMessage> inboxMessages = await GetInboxMessagesAsync();
        InboxMessage inboxMessage = inboxMessages.Should().ContainSingle().Subject;
        inboxMessage.Id.Should().Be(integrationEvent.Id);
        inboxMessage.Type.Should().Be(nameof(ReceivedIntegrationEvent));
        inboxMessage.OccurredOnUtc.Should().Be(integrationEvent.OccurredOnUtc);
        inboxMessage.ProcessedOnUtc.Should().BeNull();

        // The inbox job reads the content back with type names, the same way as the outbox
        object? content = JsonConvert.DeserializeObject<IIntegrationEvent>(inboxMessage.Content,
            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
        content.Should().BeOfType<ReceivedIntegrationEvent>().Which.Value.Should().Be("Pizza");
    }

    [Fact]
    public async Task Should_StoreADuplicateDeliveryOnlyOnce()
    {
        // Arrange
        ReceivedIntegrationEvent integrationEvent = new(Guid.NewGuid(), DateTime.UtcNow, "Pizza");

        // Act
        await _harness.Bus.Publish(integrationEvent);
        await _harness.Bus.Publish(integrationEvent);
        await WaitUntilConsumedAsync(2);

        // Assert
        (await GetInboxMessagesAsync()).Should().ContainSingle();
        (await _harness.Published.Any<Fault<ReceivedIntegrationEvent>>()).Should().BeFalse();
    }

    private async Task WaitUntilConsumedAsync(int count)
    {
        IConsumerTestHarness<IntegrationEventConsumer<ReceivedIntegrationEvent, TestInboxDbContext>> consumer =
            _harness.GetConsumerHarness<IntegrationEventConsumer<ReceivedIntegrationEvent, TestInboxDbContext>>();

        // Waits until the consumer received this many messages, or the harness goes quiet.
        // Called by class name, because MassTransit.Testing has its own Take for async sequences
        IAsyncEnumerable<IReceivedMessage<ReceivedIntegrationEvent>> received =
            consumer.Consumed.SelectAsync<ReceivedIntegrationEvent>();
        int consumed = await AsyncEnumerable.CountAsync(AsyncEnumerable.Take(received, count));

        consumed.Should().Be(count);
    }

    private async Task<List<InboxMessage>> GetInboxMessagesAsync()
    {
        await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
        TestInboxDbContext dbContext = scope.ServiceProvider.GetRequiredService<TestInboxDbContext>();
        return await dbContext.Set<InboxMessage>().AsNoTracking().ToListAsync();
    }
}

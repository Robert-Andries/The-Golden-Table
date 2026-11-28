using FluentAssertions;
using GoldenTable.Common.Infrastructure.Inbox;
using GoldenTable.Modules.Catalog.IntegrationEvents;
using GoldenTable.Modules.Catalog.IntegrationEvents.Models;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Inbox;

public sealed class ProcessInboxMessages : BaseIntegrationTest
{
    // Same settings as the consumer that writes the inbox messages
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    public ProcessInboxMessages(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ProcessInboxMessage_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        // Catalog has no integration event handlers yet, so this checks the tables, the job and that a real
        // integration event can be read back from the inbox
        // A List, like the event MassTransit hands to the consumer. Passed straight to the IReadOnlyCollection
        // parameter, [...] would create a compiler-generated collection that Newtonsoft can't recreate
        List<DishSizeModel> sizes = [new("Large", 5, 400)];
        DishCreatedIntegrationEvent integrationEvent = new(
            Guid.NewGuid(),
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Margherita",
            12.50m,
            "EUR",
            sizes);

        // Act
        context.Set<InboxMessage>().Add(new InboxMessage
        {
            Id = integrationEvent.Id,
            Type = nameof(DishCreatedIntegrationEvent),
            Content = JsonConvert.SerializeObject(integrationEvent, SerializerSettings),
            OccurredOnUtc = integrationEvent.OccurredOnUtc
        });
        await context.SaveChangesAsync();

        // Assert
        InboxMessage inboxMessage = await WaitUntilProcessedAsync(integrationEvent.Id);
        inboxMessage.Error.Should().BeNull();
    }

    private static async Task<InboxMessage> WaitUntilProcessedAsync(Guid inboxMessageId)
    {
        // The inbox job runs every second in the background, so poll until it picks the message up
        DateTime deadlineUtc = DateTime.UtcNow.AddSeconds(30);
        while (DateTime.UtcNow < deadlineUtc)
        {
            InboxMessage? inboxMessage = await context.Set<InboxMessage>()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == inboxMessageId && m.ProcessedOnUtc != null);

            if (inboxMessage is not null)
            {
                return inboxMessage;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(200));
        }

        throw new TimeoutException($"The inbox message '{inboxMessageId}' was not processed in time.");
    }
}

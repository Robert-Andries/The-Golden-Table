using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Infrastructure.Outbox;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class ProcessOutboxMessages : DishesBaseTest
{
    public ProcessOutboxMessages(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ProcessDishCreatedDomainEvent_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutTagsInDb(dish.Tags);

        // Act
        Result<Guid> result = await Sender.Send(ToCreateDishCommand(dish));

        // Assert
        result.IsSuccess.Should().BeTrue();
        OutboxMessage outboxMessage = await WaitUntilProcessedAsync(nameof(DishCreatedDomainEvent));
        outboxMessage.Error.Should().BeNull();
        (await context.Set<OutboxMessageConsumer>().AsNoTracking()
                .CountAsync(c => c.OutboxMessageId == outboxMessage.Id))
            .Should().Be(1);
    }

    private static async Task<OutboxMessage> WaitUntilProcessedAsync(string type)
    {
        // The outbox job runs every few seconds in the background, so poll until it picks the message up
        DateTime deadlineUtc = DateTime.UtcNow.AddSeconds(30);
        while (DateTime.UtcNow < deadlineUtc)
        {
            OutboxMessage? outboxMessage = await context.Set<OutboxMessage>()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Type == type && m.ProcessedOnUtc != null);

            if (outboxMessage is not null)
            {
                return outboxMessage;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }

        throw new TimeoutException($"The outbox message '{type}' was not processed in time.");
    }
}

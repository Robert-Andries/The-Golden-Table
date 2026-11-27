using GoldenTable.Common.Application.EventBus;
using GoldenTable.Common.Infrastructure.Serialization;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GoldenTable.Common.Infrastructure.Inbox;

/// <summary>
///     Receives an integration event from the bus and only stores it in the module's inbox.
///     The module's inbox job runs the handlers later.
/// </summary>
/// <typeparam name="TIntegrationEvent">The integration event to receive</typeparam>
/// <typeparam name="TDbContext">The receiving module's DbContext, which owns its inbox table</typeparam>
public sealed class IntegrationEventConsumer<TIntegrationEvent, TDbContext>(TDbContext dbContext)
    : IConsumer<TIntegrationEvent>
    where TIntegrationEvent : class, IIntegrationEvent
    where TDbContext : DbContext
{
    public async Task Consume(ConsumeContext<TIntegrationEvent> context)
    {
        TIntegrationEvent integrationEvent = context.Message;

        bool alreadyReceived = await dbContext.Set<InboxMessage>()
            .AnyAsync(m => m.Id == integrationEvent.Id, context.CancellationToken);
        if (alreadyReceived)
        {
            return;
        }

        dbContext.Set<InboxMessage>().Add(new InboxMessage
        {
            Id = integrationEvent.Id,
            Type = integrationEvent.GetType().Name,
            Content = JsonConvert.SerializeObject(integrationEvent, SerializerSettings.Instance),
            OccurredOnUtc = integrationEvent.OccurredOnUtc
        });

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}

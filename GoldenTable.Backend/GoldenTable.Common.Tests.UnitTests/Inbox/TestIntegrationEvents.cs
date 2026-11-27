using GoldenTable.Common.Application.EventBus;

namespace GoldenTable.Common.Tests.UnitTests.Inbox;

internal sealed class HandlerCalls
{
    public List<(string Handler, Guid EventId)> Calls { get; } = [];
}

internal sealed class SingleHandlerIntegrationEvent(Guid id, DateTime occurredOnUtc)
    : IntegrationEvent(id, occurredOnUtc);

internal sealed class PartiallyFailingIntegrationEvent(Guid id, DateTime occurredOnUtc)
    : IntegrationEvent(id, occurredOnUtc);

internal sealed class RecordingIntegrationEventHandler(HandlerCalls handlerCalls)
    : IntegrationEventHandler<SingleHandlerIntegrationEvent>
{
    public override Task Handle(SingleHandlerIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        handlerCalls.Calls.Add((nameof(RecordingIntegrationEventHandler), integrationEvent.Id));
        return Task.CompletedTask;
    }
}

internal sealed class FailingIntegrationEventHandler : IntegrationEventHandler<PartiallyFailingIntegrationEvent>
{
    public override Task Handle(PartiallyFailingIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("The handler failed");
    }
}

internal sealed class SucceedingIntegrationEventHandler(HandlerCalls handlerCalls)
    : IntegrationEventHandler<PartiallyFailingIntegrationEvent>
{
    public override Task Handle(PartiallyFailingIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        handlerCalls.Calls.Add((nameof(SucceedingIntegrationEventHandler), integrationEvent.Id));
        return Task.CompletedTask;
    }
}

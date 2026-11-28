using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;

namespace GoldenTable.Common.Tests.UnitTests.Outbox;

internal sealed class HandlerCalls
{
    public List<(string Handler, Guid EventId)> Calls { get; } = [];
    public List<decimal> Amounts { get; } = [];
}

internal sealed class SingleHandlerDomainEvent : DomainEvent;

internal sealed class AmountDomainEvent(decimal amount) : DomainEvent
{
    public decimal Amount { get; } = amount;
}

internal sealed class AmountDomainEventHandler(HandlerCalls handlerCalls) : DomainEventHandler<AmountDomainEvent>
{
    public override Task Handle(AmountDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        handlerCalls.Amounts.Add(domainEvent.Amount);
        return Task.CompletedTask;
    }
}

internal sealed class PartiallyFailingDomainEvent : DomainEvent;

internal sealed class RecordingDomainEventHandler(HandlerCalls handlerCalls)
    : DomainEventHandler<SingleHandlerDomainEvent>
{
    public override Task Handle(SingleHandlerDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        handlerCalls.Calls.Add((nameof(RecordingDomainEventHandler), domainEvent.Id));
        return Task.CompletedTask;
    }
}

internal sealed class FailingDomainEventHandler : DomainEventHandler<PartiallyFailingDomainEvent>
{
    public override Task Handle(PartiallyFailingDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("The handler failed");
    }
}

internal sealed class SucceedingDomainEventHandler(HandlerCalls handlerCalls)
    : DomainEventHandler<PartiallyFailingDomainEvent>
{
    public override Task Handle(PartiallyFailingDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        handlerCalls.Calls.Add((nameof(SucceedingDomainEventHandler), domainEvent.Id));
        return Task.CompletedTask;
    }
}

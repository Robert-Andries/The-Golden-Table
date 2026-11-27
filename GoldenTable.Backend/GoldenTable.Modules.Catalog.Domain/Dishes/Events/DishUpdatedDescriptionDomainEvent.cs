using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.Events;

public sealed record DishUpdatedDescriptionDomainEvent(Guid DishId, Description NewDescription, DateTime OccurredOnUtc)
    : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}

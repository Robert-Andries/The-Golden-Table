using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.Events;

public sealed class DishUpdatedDescriptionDomainEvent(Guid dishId, Description newDescription) : DomainEvent
{
    public Guid DishId { get; } = dishId;
    public Description NewDescription { get; } = newDescription;
}

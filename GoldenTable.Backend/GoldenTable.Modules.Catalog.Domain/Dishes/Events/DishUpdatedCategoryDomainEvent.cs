using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.Events;

public sealed class DishUpdatedCategoryDomainEvent(Guid dishId, string newCategory) : DomainEvent
{
    public Guid DishId { get; } = dishId;
    public string NewCategory { get; } = newCategory;
}

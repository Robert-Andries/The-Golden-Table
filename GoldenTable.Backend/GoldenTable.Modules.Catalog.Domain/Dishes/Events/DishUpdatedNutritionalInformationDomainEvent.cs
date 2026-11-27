using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.Events;

public sealed class DishUpdatedNutritionalInformationDomainEvent(Guid dishId, NutritionalValues nutritionalInformation)
    : DomainEvent
{
    public Guid DishId { get; } = dishId;
    public NutritionalValues NutritionalInformation { get; } = nutritionalInformation;
}

using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Common.Image.Events;

public sealed record ImageDescriptionUpdatedDomainEvent(
    Guid Id,
    Guid ImageId,
    Description? NewDescription,
    DateTime OccurredOnUtc) : IDomainEvent;

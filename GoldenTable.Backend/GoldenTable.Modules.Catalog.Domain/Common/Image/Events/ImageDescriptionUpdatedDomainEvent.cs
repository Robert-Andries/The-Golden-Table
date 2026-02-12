using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.Image.ValueTypes;

namespace GoldenTable.Modules.Catalog.Domain.Common.Image.Events;

public record ImageDescriptionUpdatedDomainEvent
    (Guid Id, Guid ImageId, Description NewDescription, DateTime OccurredOnUtc) : IDomainEvent;

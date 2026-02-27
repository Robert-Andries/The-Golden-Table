using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Common.Image.Events;

public record ImageCreatedDomainEvent(Guid Id, Guid ImageId, DateTime OccurredOnUtc) : IDomainEvent;

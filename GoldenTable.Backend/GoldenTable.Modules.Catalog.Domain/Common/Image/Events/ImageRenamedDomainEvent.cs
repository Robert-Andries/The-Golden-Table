using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Common.Image.Events;

public sealed record ImageRenamedDomainEvent(Guid Id, Guid ImageId, Name NewName, DateTime OccurredOnUtc)
    : IDomainEvent;

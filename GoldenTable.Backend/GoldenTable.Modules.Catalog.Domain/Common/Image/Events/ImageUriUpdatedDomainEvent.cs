using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Common.Image.Events;

public record ImageUriUpdatedDomainEvent(Guid Id, Guid ImageId, Uri NewUri, DateTime OccurredOnUtc) : IDomainEvent;

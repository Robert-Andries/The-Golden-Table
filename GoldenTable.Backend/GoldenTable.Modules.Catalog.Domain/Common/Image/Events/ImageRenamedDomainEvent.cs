using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

namespace GoldenTable.Modules.Catalog.Domain.Common.Image.Events;

public record ImageRenamedDomainEvent(Guid Id, Guid ImageId,Name NewName, DateTime OccurredOnUtc) : IDomainEvent;

using System.Text.Json.Serialization;

namespace GoldenTable.Common.Domain;

public abstract class Entity
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    private readonly List<IDomainEvent> _domainEvents = [];

    public Guid Id { get; init; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.ToList();

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}

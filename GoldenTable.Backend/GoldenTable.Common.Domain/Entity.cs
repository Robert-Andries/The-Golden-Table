using System.Text.Json.Serialization;

namespace GoldenTable.Common.Domain;

public abstract class Entity
{
    public Guid Id { get; init; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    private readonly List<IDomainEvent> _domainEvents = [];

    protected Entity()
    {
    }

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


using Bogus;
using GoldenTable.Common.Domain;
using Newtonsoft.Json;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests;

// CA1515 = Because an application's API isn't typically referenced from outside the assembly, types can be made internal
#pragma warning disable CA1515
public abstract class BaseTest
#pragma warning restore CA1515
{
    protected static readonly Faker Faker = new();

    public static T AssertDomainEventWasPublished<T>(Entity entity)
        where T : IDomainEvent
    {
        T? domainEvent = entity.DomainEvents.OfType<T>().SingleOrDefault();

        if (domainEvent is null)
        {
            throw new Exception($"{typeof(T).Name} was not published");
        }

        return domainEvent;
    }
}

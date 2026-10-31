using System.Collections.Concurrent;
using System.Reflection;
using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Common.Infrastructure.Outbox;

internal static class DomainEventHandlerTypes
{
    private static readonly ConcurrentDictionary<(Assembly Assembly, Type EventType), Type[]> Cache = new();

    internal static Type[] For(Type domainEventType, Assembly assembly)
    {
        return Cache.GetOrAdd((assembly, domainEventType), static key =>
        {
            Type handlerInterface = typeof(IDomainEventHandler<>).MakeGenericType(key.EventType);

            return key.Assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false } && t.IsAssignableTo(handlerInterface))
                .ToArray();
        });
    }
}

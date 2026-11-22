using System.Collections.Concurrent;
using System.Reflection;
using GoldenTable.Common.Application.EventBus;

namespace GoldenTable.Common.Infrastructure.Inbox;

internal static class IntegrationEventHandlerTypes
{
    private static readonly ConcurrentDictionary<(Assembly Assembly, Type EventType), Type[]> Cache = new();

    internal static Type[] For(Type integrationEventType, Assembly assembly)
    {
        return Cache.GetOrAdd((assembly, integrationEventType), static key =>
        {
            Type handlerInterface = typeof(IIntegrationEventHandler<>).MakeGenericType(key.EventType);

            return key.Assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false } && t.IsAssignableTo(handlerInterface))
                .ToArray();
        });
    }
}

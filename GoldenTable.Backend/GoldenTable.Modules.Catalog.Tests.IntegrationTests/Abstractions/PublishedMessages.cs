using System.Collections.Concurrent;
using MassTransit;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

/// <summary>
///     Collects every message published on the bus while it is connected
/// </summary>
internal sealed class PublishedMessages : IPublishObserver
{
    private readonly ConcurrentQueue<object> _messages = new();

    public Task PrePublish<T>(PublishContext<T> context)
        where T : class
    {
        return Task.CompletedTask;
    }

    public Task PostPublish<T>(PublishContext<T> context)
        where T : class
    {
        _messages.Enqueue(context.Message);
        return Task.CompletedTask;
    }

    public Task PublishFault<T>(PublishContext<T> context, Exception exception)
        where T : class
    {
        return Task.CompletedTask;
    }

    public async Task<T> WaitForAsync<T>(Func<T, bool> predicate)
    {
        // The outbox job publishes in the background, so poll until the message shows up
        DateTime deadlineUtc = DateTime.UtcNow.AddSeconds(30);
        while (DateTime.UtcNow < deadlineUtc)
        {
            T? message = _messages.OfType<T>().FirstOrDefault(predicate);
            if (message is not null)
            {
                return message;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(200));
        }

        throw new TimeoutException($"No {typeof(T).Name} was published in time.");
    }
}

using GoldenTable.Common.Infrastructure.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace GoldenTable.Common.Tests.UnitTests.Outbox;

/// <summary>
///     Keeps the outbox in memory, so the tests can check what the base job reads and writes
/// </summary>
internal sealed class TestOutboxJob(IOptionsMonitor<OutboxOptions> outboxOptions, IServiceScopeFactory scopeFactory)
    : OutboxBaseJob("Tests", typeof(TestOutboxJob).Assembly, outboxOptions, NullLogger.Instance, scopeFactory)
{
    private readonly List<OutboxMessageResponse> _messages = [];

    public Dictionary<Guid, Exception?> ProcessedMessages { get; } = [];
    public HashSet<(Guid OutboxMessageId, string ConsumerName)> Consumers { get; } = [];
    public int? RequestedBatchSize { get; private set; }

    public void AddMessage(Guid id, string content)
    {
        _messages.Add(new OutboxMessageResponse(id, content));
    }

    protected override Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(int batchSize,
        CancellationToken cancellationToken)
    {
        RequestedBatchSize = batchSize;
        return Task.FromResult<IReadOnlyList<OutboxMessageResponse>>(_messages.Take(batchSize).ToList());
    }

    protected override Task UpdateOutboxMessageAsync(OutboxMessageResponse outboxMessage, Exception? exception,
        CancellationToken cancellationToken)
    {
        ProcessedMessages[outboxMessage.Id] = exception;
        return Task.CompletedTask;
    }

    protected override Task<bool> IsConsumedAsync(Guid outboxMessageId, string consumerName,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Consumers.Contains((outboxMessageId, consumerName)));
    }

    protected override Task MarkConsumedAsync(Guid outboxMessageId, string consumerName,
        CancellationToken cancellationToken)
    {
        Consumers.Add((outboxMessageId, consumerName));
        return Task.CompletedTask;
    }
}

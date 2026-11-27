using GoldenTable.Common.Infrastructure.Inbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace GoldenTable.Common.Tests.UnitTests.Inbox;

/// <summary>
///     Keeps the inbox in memory, so the tests can check what the base job reads and writes
/// </summary>
internal sealed class TestInboxJob(IOptionsMonitor<InboxOptions> inboxOptions, IServiceScopeFactory scopeFactory)
    : InboxBaseJob("Tests", typeof(TestInboxJob).Assembly, inboxOptions, NullLogger.Instance, scopeFactory)
{
    private readonly List<InboxMessageResponse> _messages = [];

    public Dictionary<Guid, Exception?> ProcessedMessages { get; } = [];
    public HashSet<(Guid InboxMessageId, string ConsumerName)> Consumers { get; } = [];
    public int? RequestedBatchSize { get; private set; }

    public void AddMessage(Guid id, string content)
    {
        _messages.Add(new InboxMessageResponse(id, content));
    }

    protected override Task<IReadOnlyList<InboxMessageResponse>> GetInboxMessagesAsync(int batchSize,
        CancellationToken cancellationToken)
    {
        RequestedBatchSize = batchSize;
        return Task.FromResult<IReadOnlyList<InboxMessageResponse>>(_messages.Take(batchSize).ToList());
    }

    protected override Task UpdateInboxMessageAsync(InboxMessageResponse inboxMessage, Exception? exception,
        CancellationToken cancellationToken)
    {
        ProcessedMessages[inboxMessage.Id] = exception;
        return Task.CompletedTask;
    }

    protected override Task<bool> IsConsumedAsync(Guid inboxMessageId, string consumerName,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Consumers.Contains((inboxMessageId, consumerName)));
    }

    protected override Task MarkConsumedAsync(Guid inboxMessageId, string consumerName,
        CancellationToken cancellationToken)
    {
        Consumers.Add((inboxMessageId, consumerName));
        return Task.CompletedTask;
    }
}

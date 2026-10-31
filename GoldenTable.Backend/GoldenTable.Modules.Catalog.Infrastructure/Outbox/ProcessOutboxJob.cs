using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Infrastructure.Outbox;
using GoldenTable.Modules.Catalog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GoldenTable.Modules.Catalog.Infrastructure.Outbox;

internal sealed class ProcessOutboxJob(
    CatalogDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IOptionsMonitor<OutboxOptions> outboxOptions,
    ILogger<ProcessOutboxJob> logger,
    IServiceScopeFactory scopeFactory)
    : OutboxBaseJob("Catalog", Application.AssemblyReference.Assembly, outboxOptions, logger, scopeFactory)
{
    protected override async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(int batchSize,
        CancellationToken cancellationToken)
    {
        return await dbContext.Set<OutboxMessage>()
            .AsNoTracking()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(batchSize)
            .Select(m => new OutboxMessageResponse(m.Id, m.Content))
            .ToListAsync(cancellationToken);
    }

    protected override async Task UpdateOutboxMessageAsync(OutboxMessageResponse outboxMessage, Exception? exception,
        CancellationToken cancellationToken)
    {
        DateTime processedOnUtc = dateTimeProvider.UtcNow;
        string? error = exception?.ToString();

        await dbContext.Set<OutboxMessage>()
            .Where(m => m.Id == outboxMessage.Id)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.ProcessedOnUtc, processedOnUtc)
                    .SetProperty(m => m.Error, error),
                cancellationToken);
    }

    protected override Task<bool> IsConsumedAsync(Guid outboxMessageId, string consumerName,
        CancellationToken cancellationToken)
    {
        return dbContext.Set<OutboxMessageConsumer>()
            .AnyAsync(c => c.OutboxMessageId == outboxMessageId && c.Name == consumerName, cancellationToken);
    }

    protected override async Task MarkConsumedAsync(Guid outboxMessageId, string consumerName,
        CancellationToken cancellationToken)
    {
        dbContext.Set<OutboxMessageConsumer>().Add(new OutboxMessageConsumer(outboxMessageId, consumerName));
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

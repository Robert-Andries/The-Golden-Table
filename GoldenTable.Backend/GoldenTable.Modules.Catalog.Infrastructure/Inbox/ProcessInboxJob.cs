using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Infrastructure.Inbox;
using GoldenTable.Modules.Catalog.Infrastructure.Database;
using GoldenTable.Modules.Catalog.Presentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GoldenTable.Modules.Catalog.Infrastructure.Inbox;

internal sealed class ProcessInboxJob(
    CatalogDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IOptionsMonitor<InboxOptions> inboxOptions,
    ILogger<ProcessInboxJob> logger,
    IServiceScopeFactory scopeFactory)
    : InboxBaseJob("Catalog", AssemblyReference.Assembly, inboxOptions, logger, scopeFactory)
{
    protected override async Task<IReadOnlyList<InboxMessageResponse>> GetInboxMessagesAsync(int batchSize,
        CancellationToken cancellationToken)
    {
        return await dbContext.Set<InboxMessage>()
            .AsNoTracking()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(batchSize)
            .Select(m => new InboxMessageResponse(m.Id, m.Content))
            .ToListAsync(cancellationToken);
    }

    protected override async Task UpdateInboxMessageAsync(InboxMessageResponse inboxMessage, Exception? exception,
        CancellationToken cancellationToken)
    {
        DateTime processedOnUtc = dateTimeProvider.UtcNow;
        string? error = exception?.ToString();

        await dbContext.Set<InboxMessage>()
            .Where(m => m.Id == inboxMessage.Id)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.ProcessedOnUtc, processedOnUtc)
                    .SetProperty(m => m.Error, error),
                cancellationToken);
    }

    protected override Task<bool> IsConsumedAsync(Guid inboxMessageId, string consumerName,
        CancellationToken cancellationToken)
    {
        return dbContext.Set<InboxMessageConsumer>()
            .AnyAsync(c => c.InboxMessageId == inboxMessageId && c.Name == consumerName, cancellationToken);
    }

    protected override async Task MarkConsumedAsync(Guid inboxMessageId, string consumerName,
        CancellationToken cancellationToken)
    {
        dbContext.Set<InboxMessageConsumer>().Add(new InboxMessageConsumer(inboxMessageId, consumerName));
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

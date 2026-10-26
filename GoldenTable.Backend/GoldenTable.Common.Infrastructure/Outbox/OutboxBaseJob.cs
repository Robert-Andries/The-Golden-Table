using GoldenTable.Common.Domain;
using GoldenTable.Common.Infrastructure.Serialization;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;

namespace GoldenTable.Common.Infrastructure.Outbox;

[DisallowConcurrentExecution]
public abstract class OutboxBaseJob : IJob
{
    private readonly string _moduleName;
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    protected OutboxBaseJob(string moduleName, ILogger logger, IServiceScopeFactory scopeFactory)
    {
        _moduleName = moduleName;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async ValueTask Execute(IJobExecutionContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        OutboxLogs.BeginProcess(_logger, _moduleName);

        IReadOnlyList<OutboxMessageResponse> outboxMessages = await GetOutboxMessagesAsync(cancellationToken);

        foreach (OutboxMessageResponse outboxMessage in outboxMessages)
        {
            Exception? exception = null;
            try
            {
                IDomainEvent domainEvent =
                    JsonConvert.DeserializeObject<IDomainEvent>(outboxMessage.Content, SerializerSettings.Instance)!;
                
                using IServiceScope scope = _scopeFactory.CreateScope();
                
                IPublisher publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
                
                await publisher.Publish(domainEvent, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception e)
            {
                OutboxLogs.Error(_logger, _moduleName, outboxMessage.Id, e);
                exception = e;
            }

            await UpdateOutboxMessageAsync(outboxMessage, exception, cancellationToken);
        }
        
        OutboxLogs.CompleteProcess(_logger, _moduleName);
    }

    protected abstract Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(
        CancellationToken cancellationToken);

    protected abstract Task UpdateOutboxMessageAsync(OutboxMessageResponse outboxMessage, Exception? exception,
        CancellationToken cancellationToken);

    protected sealed record OutboxMessageResponse(Guid Id, string Content);
}

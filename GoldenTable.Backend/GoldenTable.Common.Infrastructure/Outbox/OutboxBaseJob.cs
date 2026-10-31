using System.Reflection;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Infrastructure.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;

namespace GoldenTable.Common.Infrastructure.Outbox;

[DisallowConcurrentExecution]
public abstract class OutboxBaseJob : IJob
{
    private readonly string _moduleName;
    private readonly Assembly _handlersAssembly;
    private readonly IOptionsMonitor<OutboxOptions> _outboxOptions;
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    protected OutboxBaseJob(
        string moduleName,
        Assembly handlersAssembly,
        IOptionsMonitor<OutboxOptions> outboxOptions,
        ILogger logger,
        IServiceScopeFactory scopeFactory)
    {
        _moduleName = moduleName;
        _handlersAssembly = handlersAssembly;
        _outboxOptions = outboxOptions;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async ValueTask Execute(IJobExecutionContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        OutboxLogs.BeginProcess(_logger, _moduleName);

        int batchSize = _outboxOptions.Get(GetType().FullName!).BatchSize;
        IReadOnlyList<OutboxMessageResponse> outboxMessages =
            await GetOutboxMessagesAsync(batchSize, cancellationToken);

        foreach (OutboxMessageResponse outboxMessage in outboxMessages)
        {
            Exception? exception;
            try
            {
                IDomainEvent domainEvent =
                    JsonConvert.DeserializeObject<IDomainEvent>(outboxMessage.Content, SerializerSettings.Instance)!;

                exception = await RunHandlersAsync(outboxMessage.Id, domainEvent, cancellationToken);
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

    private async Task<Exception?> RunHandlersAsync(
        Guid outboxMessageId,
        IDomainEvent domainEvent,
        CancellationToken cancellationToken)
    {
        List<Exception> errors = [];

        foreach (Type handlerType in DomainEventHandlerTypes.For(domainEvent.GetType(), _handlersAssembly))
        {
            string consumerName = handlerType.FullName!;

            if (await IsConsumedAsync(outboxMessageId, consumerName, cancellationToken))
            {
                continue;
            }

            try
            {
                // A scope per handler, so a failed handler's tracked changes can't leak into the next one
                await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
                var handler = (IDomainEventHandler)scope.ServiceProvider.GetRequiredService(handlerType);

                await handler.Handle(domainEvent, cancellationToken);
            }
            catch (Exception e) when (!cancellationToken.IsCancellationRequested)
            {
                OutboxLogs.HandlerError(_logger, _moduleName, outboxMessageId, consumerName, e);
                errors.Add(e);
                continue;
            }

            await MarkConsumedAsync(outboxMessageId, consumerName, cancellationToken);
        }

        return errors.Count switch
        {
            0 => null,
            1 => errors[0],
            _ => new AggregateException(errors)
        };
    }

    protected abstract Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(int batchSize,
        CancellationToken cancellationToken);

    protected abstract Task UpdateOutboxMessageAsync(OutboxMessageResponse outboxMessage, Exception? exception,
        CancellationToken cancellationToken);

    protected abstract Task<bool> IsConsumedAsync(Guid outboxMessageId, string consumerName,
        CancellationToken cancellationToken);

    protected abstract Task MarkConsumedAsync(Guid outboxMessageId, string consumerName,
        CancellationToken cancellationToken);

    protected sealed record OutboxMessageResponse(Guid Id, string Content);
}

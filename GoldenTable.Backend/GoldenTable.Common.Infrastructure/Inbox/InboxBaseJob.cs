using System.Reflection;
using GoldenTable.Common.Application.EventBus;
using GoldenTable.Common.Infrastructure.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;

namespace GoldenTable.Common.Infrastructure.Inbox;

[DisallowConcurrentExecution]
public abstract class InboxBaseJob : IJob
{
    private readonly string _moduleName;
    private readonly Assembly _handlersAssembly;
    private readonly IOptionsMonitor<InboxOptions> _inboxOptions;
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    protected InboxBaseJob(
        string moduleName,
        Assembly handlersAssembly,
        IOptionsMonitor<InboxOptions> inboxOptions,
        ILogger logger,
        IServiceScopeFactory scopeFactory)
    {
        _moduleName = moduleName;
        _handlersAssembly = handlersAssembly;
        _inboxOptions = inboxOptions;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async ValueTask Execute(IJobExecutionContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        InboxLogs.BeginProcess(_logger, _moduleName);

        int batchSize = _inboxOptions.Get(GetType().FullName!).BatchSize;
        IReadOnlyList<InboxMessageResponse> inboxMessages =
            await GetInboxMessagesAsync(batchSize, cancellationToken);

        foreach (InboxMessageResponse inboxMessage in inboxMessages)
        {
            Exception? exception;
            try
            {
                IIntegrationEvent integrationEvent =
                    JsonConvert.DeserializeObject<IIntegrationEvent>(inboxMessage.Content,
                        SerializerSettings.Instance)!;

                exception = await RunHandlersAsync(inboxMessage.Id, integrationEvent, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception e)
            {
                InboxLogs.Error(_logger, _moduleName, inboxMessage.Id, e);
                exception = e;
            }

            await UpdateInboxMessageAsync(inboxMessage, exception, cancellationToken);
        }

        InboxLogs.CompleteProcess(_logger, _moduleName);
    }

    private async Task<Exception?> RunHandlersAsync(
        Guid inboxMessageId,
        IIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        List<Exception> errors = [];

        foreach (Type handlerType in IntegrationEventHandlerTypes.For(integrationEvent.GetType(), _handlersAssembly))
        {
            string consumerName = handlerType.FullName!;

            if (await IsConsumedAsync(inboxMessageId, consumerName, cancellationToken))
            {
                continue;
            }

            try
            {
                // A scope per handler, so a failed handler's tracked changes can't leak into the next one
                await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
                var handler = (IIntegrationEventHandler)scope.ServiceProvider.GetRequiredService(handlerType);

                await handler.Handle(integrationEvent, cancellationToken);
            }
            catch (Exception e) when (!cancellationToken.IsCancellationRequested)
            {
                InboxLogs.HandlerError(_logger, _moduleName, inboxMessageId, consumerName, e);
                errors.Add(e);
                continue;
            }

            await MarkConsumedAsync(inboxMessageId, consumerName, cancellationToken);
        }

        return errors.Count switch
        {
            0 => null,
            1 => errors[0],
            _ => new AggregateException(errors)
        };
    }

    protected abstract Task<IReadOnlyList<InboxMessageResponse>> GetInboxMessagesAsync(int batchSize,
        CancellationToken cancellationToken);

    protected abstract Task UpdateInboxMessageAsync(InboxMessageResponse inboxMessage, Exception? exception,
        CancellationToken cancellationToken);

    protected abstract Task<bool> IsConsumedAsync(Guid inboxMessageId, string consumerName,
        CancellationToken cancellationToken);

    protected abstract Task MarkConsumedAsync(Guid inboxMessageId, string consumerName,
        CancellationToken cancellationToken);

    protected sealed record InboxMessageResponse(Guid Id, string Content);
}

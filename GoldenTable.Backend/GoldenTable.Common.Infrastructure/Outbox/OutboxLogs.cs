using Microsoft.Extensions.Logging;

namespace GoldenTable.Common.Infrastructure.Outbox;

internal static partial class OutboxLogs
{
    [LoggerMessage(LogLevel.Information, "{module} - Beginning to process outbox messages")]
    internal static partial void BeginProcess(ILogger logger, string module);
    
    [LoggerMessage(LogLevel.Information, "{module} - Completed processing outbox messages")]
    internal static partial void CompleteProcess(ILogger logger, string module);

    [LoggerMessage(LogLevel.Error, "{module} - Exception while processing outbox message {messageId}")]
    internal static partial void Error(ILogger logger, string module, Guid messageId, Exception exception);

    [LoggerMessage(LogLevel.Error, "{module} - Exception while handling outbox message {messageId} with {handler}")]
    internal static partial void HandlerError(ILogger logger, string module, Guid messageId, string handler,
        Exception exception);
}

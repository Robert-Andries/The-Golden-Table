using Microsoft.Extensions.Logging;

namespace GoldenTable.Common.Infrastructure.Inbox;

internal static partial class InboxLogs
{
    [LoggerMessage(LogLevel.Information, "{module} - Beginning to process inbox messages")]
    internal static partial void BeginProcess(ILogger logger, string module);

    [LoggerMessage(LogLevel.Information, "{module} - Completed processing inbox messages")]
    internal static partial void CompleteProcess(ILogger logger, string module);

    [LoggerMessage(LogLevel.Error, "{module} - Exception while processing inbox message {messageId}")]
    internal static partial void Error(ILogger logger, string module, Guid messageId, Exception exception);

    [LoggerMessage(LogLevel.Error, "{module} - Exception while handling inbox message {messageId} with {handler}")]
    internal static partial void HandlerError(ILogger logger, string module, Guid messageId, string handler,
        Exception exception);
}

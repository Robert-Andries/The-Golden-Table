using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Orders.Application.Orders;

internal static partial  class OrderLogs
{
    [LoggerMessage(LogLevel.Information, "Orders - order with id: '{orderId}' not found")]
    internal static partial void OrderNotFound(ILogger logger, Guid orderId);
}

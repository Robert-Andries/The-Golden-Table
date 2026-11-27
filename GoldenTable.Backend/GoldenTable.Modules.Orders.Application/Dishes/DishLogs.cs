using GoldenTable.Common.Domain;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Orders.Application.Dishes;

internal static partial class DishLogs
{
    [LoggerMessage(LogLevel.Information, "Orders - Dish with id: '{dishId}' already exists, skipping copy")]
    internal static partial void DishAlreadyExists(ILogger logger, Guid dishId);

    [LoggerMessage(LogLevel.Warning, "Orders - Could not create a copy of dish with id: '{dishId}'. Error: {error}")]
    internal static partial void CreateCopyError(ILogger logger, Guid dishId, Error error);

    [LoggerMessage(LogLevel.Information, "Orders - Copy of dish with id: '{dishId}' created successfully")]
    internal static partial void DishCopyCreated(ILogger logger, Guid dishId);

    [LoggerMessage(LogLevel.Information, "Orders - Dish with id: '{dishId}' not found")]
    internal static partial void DishNotFound(ILogger logger, Guid dishId);

    [LoggerMessage(LogLevel.Warning, "Orders - Could not update the copy of dish with id: '{dishId}'. Error: {error}")]
    internal static partial void UpdateCopyError(ILogger logger, Guid dishId, Error error);

    [LoggerMessage(LogLevel.Information, "Orders - Copy of dish with id: '{dishId}' updated successfully")]
    internal static partial void DishCopyUpdated(ILogger logger, Guid dishId);

    [LoggerMessage(LogLevel.Information, "Orders - Copy of dish with id: '{dishId}' deleted")]
    internal static partial void DishCopyDeleted(ILogger logger, Guid dishId);
}

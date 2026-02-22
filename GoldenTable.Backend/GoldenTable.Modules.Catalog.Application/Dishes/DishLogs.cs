using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes.UpdateDishCategory;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Dishes;

internal static partial class DishLogs
{
    [LoggerMessage(LogLevel.Information, "Dish with id: '{dishId}' not found")]
    internal static partial void DishNotFound(ILogger logger, Guid dishId);
    
    [LoggerMessage(LogLevel.Information, "Image {imageId} not found")]
    internal static partial void ImageIdNotFound(ILogger logger, Guid imageId);

    [LoggerMessage(LogLevel.Information, "Could not add size to dish with id: {dishId}. Error: {error}")]
    internal static partial void AddSizeError(ILogger logger, Guid dishId, Error error);

    [LoggerMessage(LogLevel.Information, "Could not add tags to dish with id: {dishId}. Error: {error}")]
    internal static partial void AddTagsError(ILogger logger, Guid dishId, Error error);

    [LoggerMessage(LogLevel.Information, "Attempted to create a dish but failed. Error: {error}")]
    internal static partial void CreateError(ILogger logger, Error error);

    [LoggerMessage(LogLevel.Information, "Dish created successfully. Id: '{dishId}'")]
    internal static partial void DishCreatedSuccessfully(ILogger logger, Guid dishId);

    [LoggerMessage(LogLevel.Information,
        "There was a problem removing an image with id: {imageId} for a dish with id: {dishId}" +
        "Error: {error}")]
    internal static partial void RemoveImageError(ILogger logger, Guid imageId, Guid dishId, Error error);

    [LoggerMessage(LogLevel.Information, "Cannot remove Size: {size} from dish with id: {dishId}. Error: {error}")]
    internal static partial void RemoveSizeError(ILogger logger, string size, Guid dishId, Error error);

    [LoggerMessage(LogLevel.Information, "Cannot remove tags from dish with id: {dishId}. Error: {error}")]
    internal static partial void RemoveTagsError(ILogger logger, Guid dishId, Error error);

    [LoggerMessage(LogLevel.Information, "Cannot rename dish with id: {dishId}. Error: {error}")]
    internal static partial void RenameError(ILogger logger, Guid dishId, Error error);

    [LoggerMessage(LogLevel.Information, "There was an error trying to create money object: {error}")]
    internal static partial void CreateMoneyObjectError(ILogger logger, Error error);

    [LoggerMessage(LogLevel.Information, "Cannot update base price for dish with id: {dishId}. Error: {error}")]
    internal static partial void UpdateBasePriceError(ILogger logger, Guid dishId, Error error);

    [LoggerMessage(LogLevel.Information, "Cannot update description for dish dish with id: {dishId}. Error: {error}")]
    internal static partial void UpdateDescriptionError(ILogger logger, Guid dishId, Error error);

    [LoggerMessage(LogLevel.Information, "Cannot update category for dish dish with id: {dishId}. Error: {error}")]
    internal static partial void UpdateCategoryError(ILogger logger, Guid dishId, Error error);

    [LoggerMessage(LogLevel.Information,
        "Cannot update nutritional information for dish dish with id: {dishId}. Error: {error}")]
    internal static partial void UpdateNutritionalInformationError(ILogger logger, Guid dishId, Error error);
    
    [LoggerMessage(LogLevel.Information,
        "Unable to create dish category. Error: {error}")]
    internal static partial void CreateCategoryError(ILogger logger, Error error);
}

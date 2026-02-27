using GoldenTable.Common.Domain;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Images;

internal static partial class ImagesLogs
{
    [LoggerMessage(LogLevel.Error, "Error when creating image: {error}")]
    internal static partial void CreateImageError(ILogger logger, Error error);

    [LoggerMessage(LogLevel.Information, "Image with id: '{imageId}' not found")]
    internal static partial void ImageNotFound(ILogger logger, Guid imageId);

    [LoggerMessage(LogLevel.Information, "There was an error trying to rename image: '{imageId}'. Error: {error}")]
    internal static partial void RenameImageError(ILogger logger, Guid imageId, Error error);

    [LoggerMessage(LogLevel.Information,
        "There was an error trying to update description for image: '{imageId}'. Error: {error}")]
    internal static partial void UpdateDescriptionError(ILogger logger, Guid imageId, Error error);
}

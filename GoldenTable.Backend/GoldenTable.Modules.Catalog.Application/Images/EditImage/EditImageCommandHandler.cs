using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Images.EditImage;

public sealed class EditImageCommandHandler(
    IImageRepository imageRepository,
    IUnitOfWork unitOfWork,
    IImageCacheService imageCacheService,
    IDateTimeProvider dateTimeProvider,
    ILogger<EditImageCommandHandler> logger)
    : ICommandHandler<EditImageCommand>
{
    public async Task<Result> Handle(EditImageCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Image? image = await imageCacheService.GetAsync(request.ImageId, cancellationToken) ??
                       await imageRepository.GetAsync(request.ImageId, cancellationToken);
        if (image is null)
        {
            ImagesLogs.ImageNotFound(logger, request.ImageId);
            return ImageErrors.NotFound;
        }

        DateTime now = dateTimeProvider.UtcNow;

        Result renameResult = image.Rename(new Name(request.Name), now);
        if (renameResult.IsFailure)
        {
            ImagesLogs.EditImageError(logger, request.ImageId, renameResult.Error);
            return Result.Failure(renameResult.Error);
        }

        Description? description = request.Description is not null ? new Description(request.Description) : null;
        Result descriptionResult = image.UpdateDescription(description, now);
        if (descriptionResult.IsFailure)
        {
            ImagesLogs.EditImageError(logger, request.ImageId, descriptionResult.Error);
            return Result.Failure(descriptionResult.Error);
        }
        
        Result uriResult = image.UpdateUri(request.Uri, now);
        if (descriptionResult.IsFailure)
        {
            ImagesLogs.EditImageError(logger, request.ImageId, uriResult.Error);
            return Result.Failure(descriptionResult.Error);
        }

        await imageRepository.UpdateAsync(image, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await imageCacheService.UpdateAsync(image, cancellationToken);

        return Result.Success();
    }
}

using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Images.UpdateDescription;

public sealed class UpdateDescriptionCommandHandler(
    IImageRepository imageRepository,
    IUnitOfWork unitOfWork,
    IImageCacheService imageCacheService,
    IDateTimeProvider dateTimeProvider,
    ILogger<UpdateDescriptionCommandHandler> logger)
    : ICommandHandler<UpdateDescriptionCommand>
{
    public async Task<Result> Handle(UpdateDescriptionCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Image? image = await imageCacheService.GetAsync(request.ImageId, cancellationToken) ??
                       await imageRepository.GetAsync(request.ImageId, cancellationToken);
        if (image is null)
        {
            logger.LogInformation("Can't find image {ImageId}", request.ImageId);
            return ImageErrors.NotFound;
        }
        
        Result result = image.UpdateDescription(request.Description, dateTimeProvider.UtcNow);
        if (result.IsFailure)
        {
            logger.LogInformation("Can't update description of image with id: {ImageId}. Error: {Error}",
                image.Id, result.Error);
            return result.Error;
        }
        
        await imageRepository.UpdateAsync(image, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await imageCacheService.CreateOrUpdate(image, cancellationToken);

        return Result.Success();
    }
}

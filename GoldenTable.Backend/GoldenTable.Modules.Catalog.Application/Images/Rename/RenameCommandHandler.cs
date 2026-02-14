using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Images.Rename;

public sealed class RenameCommandHandler(
    IImageRepository imageRepository,
    IUnitOfWork unitOfWork,
    IImageCacheService imageCacheService,
    IDateTimeProvider dateTimeProvider,
    ILogger<RenameCommandHandler> logger) 
    : ICommandHandler<RenameCommand>
{
    public async Task<Result> Handle(RenameCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Image? image = await imageCacheService.GetAsync(request.ImageId, cancellationToken) ??
                       await imageRepository.GetAsync(request.ImageId, cancellationToken);
        if (image is null)
        {
            logger.LogInformation("Can't find image {ImageId}", request.ImageId);
            return ImageErrors.NotFound;
        }
        
        Result result = image.Rename(request.NewName, dateTimeProvider.UtcNow);
        if (result.IsFailure)
        {
            logger.LogInformation("Can't rename image {ImageId}. Error: {Error}", request.ImageId, result.Error);
            return Result.Failure(result.Error);
        }
        
        await imageRepository.UpdateAsync(image, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await imageCacheService.CreateOrUpdate(image, cancellationToken);

        return Result.Success();
    }
}

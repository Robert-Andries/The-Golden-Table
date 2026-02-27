using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.AddImage;

public sealed class AddImageCommandHandler(
    IDishRepository dishRepository,
    IImageRepository imageRepository,
    IUnitOfWork unitOfWork,
    IImageCacheService imageCacheService,
    IDishCacheService dishCacheService,
    ILogger<AddImageCommandHandler> logger,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<AddImageCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddImageCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Image image = await imageCacheService.GetAsync(request.ImageId, cancellationToken) ??
                      await imageRepository.GetAsync(request.ImageId, cancellationToken);
        if (image is null)
        {
            DishLogs.ImageIdNotFound(logger, request.ImageId);
            return Result.Failure<Guid>(ImageErrors.NotFound);
        }

        Dish? dish = await dishRepository.GetAsync(request.DishId, cancellationToken);
        if (dish is null)
        {
            DishLogs.DishNotFound(logger, request.DishId);
            return Result.Failure<Guid>(DishErrors.NotFound);
        }

        Result result = dish.AddImage(image, dateTimeProvider.UtcNow);
        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await dishCacheService.UpdateAsync(dish, cancellationToken);

        return image.Id;
    }
}

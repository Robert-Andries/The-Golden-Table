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
    : ICommandHandler<AddImageCommand>
{
    public async Task<Result> Handle(AddImageCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        bool existImage =  await imageCacheService.ExistsImageAsync(request.ImageId) || 
                           await imageRepository.GetAsync(request.ImageId, cancellationToken) != null;
        if (!existImage)
        {
            logger.LogInformation("Image {ImageId} not found", request.ImageId);
            return ImageErrors.NotFound;
        }

        Dish? dish = await dishCacheService.GetAsync(request.DishId, cancellationToken) ?? 
                     await dishRepository.GetAsync(request.DishId, cancellationToken);
        if (dish is null)
        {
            logger.LogInformation("Dish {DishId} not found", request.DishId);
            return DishErrors.NotFound;
        }

        dish.AddImage(request.ImageId, dateTimeProvider.UtcNow);
        
        await dishRepository.UpdateAsync(dish, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await dishCacheService.UpdateAsync(dish, cancellationToken);
        
        return Result.Success();
    }
}

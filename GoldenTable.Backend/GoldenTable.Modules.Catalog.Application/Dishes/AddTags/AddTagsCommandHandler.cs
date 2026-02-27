using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.AddTags;

public sealed class AddTagsCommandHandler(
    IUnitOfWork unitOfWork,
    IDishRepository dishRepository,
    IDishCacheService dishCacheService,
    IDateTimeProvider dateTimeProvider,
    ILogger<AddTagsCommandHandler> logger)
    : ICommandHandler<AddTagsCommand>
{
    public async Task<Result> Handle(AddTagsCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Dish? dish = await dishRepository.GetAsync(request.DishId, cancellationToken);
        if (dish is null)
        {
            DishLogs.DishNotFound(logger, request.DishId);
            return DishErrors.NotFound;
        }

        List<DishTag> tags = new();
        foreach (string tag in request.Tags.Where(t => dish.Tags.All(dt => dt.Value != t)).ToList())
        {
            Result<DishTag> dishTagResult = DishTag.Create(tag);
            if (dishTagResult.IsFailure)
            {
                DishLogs.AddTagsError(logger, request.DishId, dishTagResult.Error);
                return dishTagResult.Error;
            }

            tags.Add(dishTagResult.Value);
        }

        Result result = dish.AddTags(tags, dateTimeProvider.UtcNow);
        if (result.IsFailure)
        {
            DishLogs.AddTagsError(logger, request.DishId, result.Error);
            return Result.Failure(result.Error);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await dishCacheService.UpdateAsync(dish, cancellationToken);

        return Result.Success();
    }
}

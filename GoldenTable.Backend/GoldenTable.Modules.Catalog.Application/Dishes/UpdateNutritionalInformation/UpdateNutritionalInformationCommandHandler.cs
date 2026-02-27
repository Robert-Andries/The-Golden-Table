using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.UpdateNutritionalInformation;

public sealed class UpdateNutritionalInformationCommandHandler(
    ILogger<UpdateNutritionalInformationCommandHandler> logger,
    IDishRepository dishRepository,
    IUnitOfWork unitOfWork,
    IDishCacheService cacheService,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<UpdateNutritionalInformationCommand>
{
    public async Task<Result> Handle(UpdateNutritionalInformationCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Dish? dish = await dishRepository.GetAsync(request.DishId, cancellationToken);
        if (dish is null)
        {
            DishLogs.DishNotFound(logger, request.DishId);
            return DishErrors.NotFound;
        }

        Result result = dish.UpdateNutritionalInformation(request.NutritionalInformation, dateTimeProvider.UtcNow);
        if (result.IsFailure)
        {
            DishLogs.UpdateNutritionalInformationError(logger, request.DishId, result.Error);
            return result.Error;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.UpdateAsync(dish, cancellationToken);

        return Result.Success();
    }
}

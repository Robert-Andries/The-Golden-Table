using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Application.Dishes.RemoveSize;
using GoldenTable.Modules.Catalog.Application.Dishes.Rename;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.UpdateBasePrice;

public sealed partial class UpdateBasePriceCommandHandler(
    ILogger<UpdateBasePriceCommandHandler> logger,
    IDishRepository dishRepository,
    IUnitOfWork unitOfWork,
    IDishCacheService cacheService,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<UpdateBasePriceCommand>
{
    public async Task<Result> Handle(UpdateBasePriceCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Dish? dish = await cacheService.GetAsync(request.DishId, cancellationToken) ??
                     await dishRepository.GetAsync(request.DishId, cancellationToken);
        if (dish is null)
        {
            DishLogs.DishNotFound(logger, request.DishId);
            return DishErrors.NotFound;
        }

        Result<Money> moneyResult = Money.Create(request.NewBasePrice, dish.BasePrice.Currency);
        if (moneyResult.IsFailure)
        {
            DishLogs.CreateMoneyObjectError(logger, moneyResult.Error);
        }
        Money money = moneyResult.Value;
        
        Result result = dish.UpdateBasePrice(money, dateTimeProvider.UtcNow);
        if (result.IsFailure)
        {
            DishLogs.UpdateBasePriceError(logger, request.DishId, result.Error);
            return result.Error;
        }

        await dishRepository.UpdateAsync(dish, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.CreateOrUpdateAsync(dish, cancellationToken);
        
        return Result.Success();
    }

    
}

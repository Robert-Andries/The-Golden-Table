using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;

namespace GoldenTable.Modules.Catalog.Application.Dishes.DeleteDish;

internal sealed class DeleteDishCommandHandler(
    IUnitOfWork unitOfWork,
    IDishRepository dishRepository,
    IDishCacheService dishCache)
    : ICommandHandler<DeleteDishCommand>
{
    public async Task<Result> Handle(DeleteDishCommand request, CancellationToken cancellationToken)
    {
        Dish? dish = await dishRepository.GetAsync(request.DishId, cancellationToken);
        if (dish is null)
        {
            return Result.Failure(DishErrors.NotFound);
        }

        dish.Delete();
        dishRepository.Remove(dish);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await dishCache.RemoveAsync(request.DishId, cancellationToken);

        return Result.Success();
    }
}

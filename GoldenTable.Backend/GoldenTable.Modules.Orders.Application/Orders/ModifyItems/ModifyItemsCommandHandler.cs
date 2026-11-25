using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Domain.Money;
using GoldenTable.Modules.Orders.Application.Abstractions.Data;
using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Domain.Dishes.Abstractions;
using GoldenTable.Modules.Orders.Domain.Dishes.ValueObjects;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.Abstractions;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;

namespace GoldenTable.Modules.Orders.Application.Orders.ModifyItems;

internal sealed class ModifyItemsCommandHandler (
    IOrderRepository orderRepository,
    IDishRepository dishRepository,
    IDishCacheService dishCacheService,
    IOrderCacheService orderCacheService,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<ModifyItemsCommand>
{
    public async Task<Result> Handle(ModifyItemsCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (request.Items.Count == 0)
        {
            return Result.Failure<Order>(OrderErrors.InvalidItems);
        }
        
        // Changes start from the database, never the cache: a cached copy may be stale, and EF cannot
        // save a detached order whose items it has never tracked
        Order? order = await orderRepository.GetAsync(request.OrderId, cancellationToken);
        if (order is null || order.CustomerId != request.CustomerId)
        {
            return Result.Failure(OrderErrors.NotFound);
        }
        
        Money? totalPrice = null;
        foreach ((Guid id, string? itemSizeName, int quantity) requestItem in request.Items)
        {
            if (requestItem.quantity <= 0)
            {
                return Result.Failure<Order>(OrderErrors.InvalidQuantity);
            }

            Dish? dish = await dishCacheService.GetAsync(requestItem.id, cancellationToken) ??
                         await dishRepository.GetAsync(requestItem.id, cancellationToken);
            if (dish is null)
            {
                return Result.Failure<Order>(DishErrors.NotFound);
            }

            decimal itemPrice = dish.BasePrice.Amount;
            if (requestItem.itemSizeName is not null)
            {
                DishSize? size = dish.Sizes.FirstOrDefault(s => s.Name == requestItem.itemSizeName);
                if (size is null)
                {
                    return Result.Failure<Order>(DishErrors.InvalidSize);
                }

                itemPrice += size.PriceAdded;
            }

            itemPrice *= requestItem.quantity;

            Result<Money> currentDishPriceResult = Money.Create(itemPrice, dish.BasePrice.Currency.Code);
            if (currentDishPriceResult.IsFailure)
            {
                return Result.Failure<Order>(currentDishPriceResult.Error);
            }

            if (totalPrice is null)
            {
                totalPrice = currentDishPriceResult.Value;
            }
            else
            {
                // Money is immutable: Add returns the new total
                Result<Money> totalResult = totalPrice.Add(currentDishPriceResult.Value);
                if (totalResult.IsFailure)
                {
                    return Result.Failure(totalResult.Error);
                }

                totalPrice = totalResult.Value;
            }
        }

        var items = request.Items
            .Select(item => new DishItem(item.id, item.itemSizeName, item.quantity))
            .ToList();

        Result modifyItemsResult = order.ModifyItems(items, totalPrice!, dateTimeProvider.UtcNow);
        if (modifyItemsResult.IsFailure)
        {
            return Result.Failure(modifyItemsResult.Error);
        }
        
        await orderRepository.UpdateAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await orderCacheService.UpdateAsync(order, cancellationToken);

        return Result.Success();
    }
}

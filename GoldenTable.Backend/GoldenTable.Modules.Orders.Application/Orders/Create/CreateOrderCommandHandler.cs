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

namespace GoldenTable.Modules.Orders.Application.Orders.Create;

internal sealed class CreateOrderCommandHandler(
    IDishRepository dishRepository,
    IDishCacheService dishCacheService,
    IOrderRepository orderRepository,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateOrderCommand, Order>
{
    public async Task<Result<Order>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (request.Items.Count == 0)
        {
            return Result.Failure<Order>(OrderErrors.InvalidItems);
        }

        Money? priceAmount = null;
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

            if (priceAmount is null)
            {
                priceAmount = currentDishPriceResult.Value;
            }
            else
            {
                // Money is immutable: Add returns the new total
                Result<Money> totalResult = priceAmount.Add(currentDishPriceResult.Value);
                if (totalResult.IsFailure)
                {
                    return Result.Failure<Order>(totalResult.Error);
                }

                priceAmount = totalResult.Value;
            }
        }

        var items = request.Items
            .Select(item => new DishItem(item.id, item.itemSizeName, item.quantity))
            .ToList();

        Result<Order> orderResult = Order.Create(request.CustomerId,
            priceAmount!,
            items,
            request.Address,
            dateTimeProvider.UtcNow);
        if (orderResult.IsFailure)
        {
            return Result.Failure<Order>(orderResult.Error);
        }

        Order order = orderResult.Value;

        await orderRepository.AddAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(order);
    }
}

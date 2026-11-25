using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Orders.Application.Abstractions.Data;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.Abstractions;

namespace GoldenTable.Modules.Orders.Application.Orders.UpdateAddress;

internal sealed class UpdateAddressCommandHandler(
    IOrderRepository orderRepository,
    IOrderCacheService orderCacheService,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<UpdateAddressCommand>
{
    public async Task<Result> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Order? order = await orderCacheService.GetAsync(request.OrderId, cancellationToken) ??
                      await orderRepository.GetAsync(request.OrderId, cancellationToken);
        
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        Result orderResult = order.UpdateAddress(request.newAddress, dateTimeProvider.UtcNow);
        if (orderResult.IsFailure)
        {
            return Result.Failure(orderResult.Error);
        }
        
        await orderRepository.UpdateAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await orderCacheService.UpdateAsync(order, cancellationToken);

        return Result.Success();
    }
}

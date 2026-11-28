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

        // Changes start from the database, never the cache: a cached copy may be stale, and EF cannot
        // save a detached order whose items it has never tracked
        Order? order = await orderRepository.GetAsync(request.OrderId, cancellationToken);
        
        // Someone else's order is reported as missing so its existence isn't revealed
        if (order is null || order.CustomerId != request.CustomerId)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        Result orderResult = order.UpdateAddress(request.NewAddress, dateTimeProvider.UtcNow);
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

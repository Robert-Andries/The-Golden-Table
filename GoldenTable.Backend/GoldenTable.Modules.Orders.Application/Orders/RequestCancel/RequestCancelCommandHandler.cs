using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Orders.Application.Abstractions.Data;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.Abstractions;

namespace GoldenTable.Modules.Orders.Application.Orders.RequestCancel;

internal sealed class RequestCancelCommandHandler(
    IOrderRepository orderRepository,
    IOrderCacheService orderCacheService,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<RequestCancelCommand>
{
    public async Task<Result> Handle(RequestCancelCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        // Changes start from the database, never the cache: a cached copy may be stale, and EF cannot
        // save a detached order whose items it has never tracked
        Order? order = await orderRepository.GetAsync(request.OrderId, cancellationToken);

        if (order is null || order.CustomerId != request.CustomerId)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        Result cancelResult = order.RequestCancel(dateTimeProvider.UtcNow);
        if (cancelResult.IsFailure)
        {
            return Result.Failure(cancelResult.Error);
        }
        
        await orderRepository.UpdateAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await orderCacheService.UpdateAsync(order, cancellationToken);

        return Result.Success();
    }
}

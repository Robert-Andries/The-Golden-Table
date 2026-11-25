using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Orders.Application.Abstractions.Data;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.Abstractions;

namespace GoldenTable.Modules.Orders.Application.Orders.ChangeStatus;

internal sealed class ChangeStatusCommandHandler(
    IOrderRepository orderRepository,
    IOrderCacheService orderCacheService,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<ChangeStatusCommand>
{
    public async Task<Result> Handle(ChangeStatusCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Order? order = await orderCacheService.GetAsync(request.OrderId, cancellationToken) ??
                       await orderRepository.GetAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        Result changeStatusResult = order.UpdateStatus(request.NewStatus, dateTimeProvider.UtcNow);
        if (changeStatusResult.IsFailure)
        {
            return Result.Failure(changeStatusResult.Error);
        }
        
        await orderRepository.UpdateAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await orderCacheService.UpdateAsync(order, cancellationToken);
        
        return Result.Success();
    }
}

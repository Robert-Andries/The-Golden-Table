using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Orders.Application.Orders.GetOrderById;

internal sealed class GetOrderByIdQueryHandler(
    IOrderRepository orderRepository,
    IOrderCacheService orderCacheService,
    ILogger<GetOrderByIdQueryHandler> logger) 
    : IQueryHandler<GetOrderByIdQuery, Order>
{
    public async Task<Result<Order>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Order? order = await orderCacheService.GetAsync(request.OrderId, cancellationToken) ??
                       await orderRepository.GetAsync(request.OrderId, cancellationToken);

        // Someone else's order is reported as missing so its existence isn't revealed
        if (order is null || request.CustomerId is { } customerId && order.CustomerId != customerId)
        {
            OrderLogs.OrderNotFound(logger, request.OrderId);
            return Result.Failure<Order>(OrderErrors.NotFound);
        }

        return Result.Success(order);
    }
}

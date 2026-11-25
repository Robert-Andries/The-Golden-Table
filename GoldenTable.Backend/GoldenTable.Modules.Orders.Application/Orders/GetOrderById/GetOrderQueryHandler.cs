using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Orders.Application.Orders.GetOrderById;

internal sealed class GetOrderQueryHandler(
    IOrderRepository orderRepository,
    IOrderCacheService orderCacheService,
    ILogger<GetOrderQueryHandler> logger) 
    : IQueryHandler<GetOrderQuery, Order>
{
    public async Task<Result<Order>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Order? order = await orderCacheService.GetAsync(request.OrderId, cancellationToken) ??
                       await orderRepository.GetAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            OrderLogs.OrderNotFound(logger, request.OrderId);
            return Result.Failure<Order>(OrderErrors.NotFound);
        }

        return Result.Success(order);
    }
}

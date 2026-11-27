using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.Abstractions;

namespace GoldenTable.Modules.Orders.Application.Orders.GetOrders;

internal sealed class GetOrdersQueryHandler(IOrderRepository orderRepository) 
    : IQueryHandler<GetOrdersQuery, IReadOnlyCollection<Order>>
{
    public async Task<Result<IReadOnlyCollection<Order>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        IReadOnlyCollection<Order>  orders = await orderRepository.GetAllAsync(cancellationToken);
        
        return Result.Success(orders);
    }
}

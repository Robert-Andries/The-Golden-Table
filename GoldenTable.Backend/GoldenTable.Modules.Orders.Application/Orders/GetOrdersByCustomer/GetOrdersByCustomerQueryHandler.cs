using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.Abstractions;

namespace GoldenTable.Modules.Orders.Application.Orders.GetOrdersByCustomer;

internal sealed class GetOrdersByCustomerQueryHandler(IOrderRepository orderRepository)
    : IQueryHandler<GetOrdersByCustomerQuery, IReadOnlyCollection<Order>>
{
    public async Task<Result<IReadOnlyCollection<Order>>> Handle(GetOrdersByCustomerQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyCollection<Order> orders = await orderRepository.GetByCustomerAsync(request.CustomerId, cancellationToken);

        return Result.Success(orders);
    }
}

using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.Abstractions;
using GoldenTable.Modules.Orders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Orders.Infrastructure.Orders;

internal sealed class OrderRepository(OrdersDbContext context) : IOrderRepository
{
    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await context.Orders.AddAsync(order, cancellationToken);
    }

    public Task<Order?> GetAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return context.Orders.FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
    }

    public Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return context.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedOnUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Order>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return context.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedOnUtc)
            .ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        context.Orders.Update(order);
        return Task.CompletedTask;
    }

    public void Remove(Order order)
    {
        context.Orders.Remove(order);
    }
}

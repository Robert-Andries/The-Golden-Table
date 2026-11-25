using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Domain.Dishes.Abstractions;
using GoldenTable.Modules.Orders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Orders.Infrastructure.Dishes;

internal sealed class DishRepository(OrdersDbContext context) : IDishRepository
{
    public async Task AddAsync(Dish dish, CancellationToken cancellationToken = default)
    {
        await context.Dishes.AddAsync(dish, cancellationToken);
    }

    public Task<Dish?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.Dishes.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public Task<List<Dish>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return context.Dishes
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(Dish dish, CancellationToken cancellationToken = default)
    {
        context.Dishes.Update(dish);
        return Task.CompletedTask;
    }

    public void Remove(Dish dish)
    {
        context.Dishes.Remove(dish);
    }
}

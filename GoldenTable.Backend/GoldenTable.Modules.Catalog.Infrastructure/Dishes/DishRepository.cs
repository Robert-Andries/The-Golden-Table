using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;
using GoldenTable.Modules.Catalog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Catalog.Infrastructure.Dishes;

internal sealed class DishRepository(CatalogDbContext context) : IDishRepository
{
    public async Task AddAsync(Dish dish, CancellationToken cancellationToken = default)
    {
        await context.Dishes.AddAsync(dish, cancellationToken);
    }

    public async Task<Dish?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Dish? dish = await context.Dishes
            .Include(d => d.Tags)
            .Include(d => d.Images)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        return dish;
    }

    public async Task<List<Dish>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<Dish> dishes = await context.Dishes
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return dishes;
    }

    public Task UpdateAsync(Dish dish, CancellationToken cancellationToken = default)
    {
        context.Dishes.Update(dish);
        return Task.CompletedTask;
    }
}

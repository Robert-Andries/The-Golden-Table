namespace GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;

public interface IDishCacheService
{
    Task<Dish?> GetAsync(Guid dishId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Dish dish, CancellationToken cancellationToken = default);
}

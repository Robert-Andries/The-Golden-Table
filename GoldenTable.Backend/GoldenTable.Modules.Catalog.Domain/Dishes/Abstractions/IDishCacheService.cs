namespace GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;

/// <summary>
/// A simple way to interact with the dish cache
/// </summary>
public interface IDishCacheService
{
    /// <summary>
    /// Gets the cache entry for the dish with the provided id
    /// </summary>
    /// <param name="dishId">Id to search for</param>
    /// <returns>Dish object with the specified id, null if not found</returns>
    Task<Dish?> GetAsync(Guid dishId, CancellationToken cancellationToken = default);
    /// <summary>
    /// Updates the cache entry of a dish
    /// </summary>
    /// <param name="dish">The dish to update</param>
    Task UpdateAsync(Dish dish, CancellationToken cancellationToken = default);
}

namespace GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;

/// <summary>
///     Repository for dishes
/// </summary>
/// <remarks>No changes will be persisted on the repository until saving is called</remarks>
public interface IDishRepository
{
    /// <summary>
    ///     Adds a dish to the repository
    /// </summary>
    /// <param name="dish">What to add</param>
    Task AddAsync(Dish dish, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a dish with the specified id from the repository
    /// </summary>
    /// <param name="id">The id to search for</param>
    /// <returns>Dish object if is found, null otherwise</returns>
    Task<Dish?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all dishes from the repository
    /// </summary>
    /// <returns>A list of dishes</returns>
    Task<List<Dish>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates the dish object of a repository
    /// </summary>
    /// <param name="dish">Dish to update</param>
    /// <remarks>If the provided dish does not exist in the repository, the method will simply return</remarks>
    Task UpdateAsync(Dish dish, CancellationToken cancellationToken = default);
}

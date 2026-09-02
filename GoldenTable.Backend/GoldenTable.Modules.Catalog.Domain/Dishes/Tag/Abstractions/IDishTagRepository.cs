namespace GoldenTable.Modules.Catalog.Domain.Dishes.Tag.Abstractions;

/// <summary>
///     Repository for dish tags providing simple CRUD operations
/// </summary>
/// <remarks>
///     The methods just modifies the state, no operations will be executed until you manually call to save changes
/// </remarks>
public interface IDishTagRepository
{
    /// <summary>
    ///     Gets the tag with the specified id from the repository
    /// </summary>
    /// <param name="id">The id to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tag with that id or null if there aren't any match.</returns>
    Task<DishTag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets tags with the specified ids from the repository
    /// </summary>
    /// <param name="ids">The ids to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of tags with those ids.</returns>
    Task<List<DishTag>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the tag with the specified value from the repository
    /// </summary>
    /// <param name="value">The value to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tag with that value or null if there aren't any match.</returns>
    Task<DishTag?> GetByValueAsync(string value, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all the tags from the repository
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All the tags from the repository</returns>
    Task<List<DishTag>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Method to add a tag to the repository
    /// </summary>
    /// <param name="tag">What to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAsync(DishTag tag, CancellationToken cancellationToken = default);
}

namespace GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;

/// <summary>
///     A simple way to interact with image cache
/// </summary>
public interface IImageCacheService
{
    /// <summary>
    ///     Retrieves an image from cache using imageId.
    /// </summary>
    /// <param name="imageId">The image id to retrive.</param>
    /// <returns>Respective image with that id or null if there are no image with that id.</returns>
    Task<Image?> GetAsync(Guid imageId, CancellationToken cancellationToken);

    /// <summary>
    ///     Updates/add the cache to include the provided image.
    /// </summary>
    /// <param name="image">What to update/add.</param>
    /// <remarks>If the provided image id does not exist in the cache, it will create a new cache entry and add it.</remarks>
    Task UpdateAsync(Image image, CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes cache entry for image with specified id.
    /// </summary>
    /// <param name="imageId">What to delete.</param>
    Task DeleteAsync(Guid imageId, CancellationToken cancellationToken);
}

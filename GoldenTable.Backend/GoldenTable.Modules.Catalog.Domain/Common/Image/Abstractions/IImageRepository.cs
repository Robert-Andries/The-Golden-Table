namespace GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;

/// <summary>
/// Repository for images providing simple CRUD operations
/// </summary>
/// <remarks>
/// The methods just modifies the state, no operations will be executed until you manually call to save changes
/// </remarks>
public interface IImageRepository
{
    /// <summary>
    /// Gets the image with the specified id from the repository
    /// </summary>
    /// <param name="ImageId">The id to search for</param>
    /// <returns>Image with that id or null if there aren't any match.</returns>
    Task<Image?> GetAsync(Guid ImageId, CancellationToken cancellationToken = default);
    /// <summary>
    /// Updates the specified image
    /// </summary>
    /// <param name="image">What to update</param>
    /// <remarks>
    /// If the provided image is not in the repository the method will mark itself as completed
    /// </remarks>
    Task UpdateAsync(Image image, CancellationToken cancellationToken = default);
    /// <summary>
    /// Method to add image to the repository
    /// </summary>
    /// <param name="image">What to add</param>
    Task AddAsync(Image image, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Method to delete an image from the repository
    /// </summary>
    /// <param name="image">What to delete</param>
    /// <remarks>If the provided image is not in the repository the method will just return</remarks>
    void Remove(Image image);
}

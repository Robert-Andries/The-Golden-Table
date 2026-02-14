namespace GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;

public interface IImageCacheService
{
    Task CreateAsync(Image image,  CancellationToken cancellationToken);
    Task<Image?> GetAsync(Guid imageId, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid imageId, CancellationToken cancellationToken);
    Task CreateOrUpdate(Image image, CancellationToken cancellationToken);
    Task DeleteAsync(Guid imageId, CancellationToken cancellationToken);
}

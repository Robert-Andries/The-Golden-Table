namespace GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;

public interface IImageCacheService
{
    Task<Image?> GetAsync(Guid imageId, CancellationToken cancellationToken);
    Task UpdateAsync(Image image, CancellationToken cancellationToken);
    Task DeleteAsync(Guid imageId, CancellationToken cancellationToken);
}

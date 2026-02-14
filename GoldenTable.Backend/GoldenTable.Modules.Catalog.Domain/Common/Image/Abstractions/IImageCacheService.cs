namespace GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;

public interface IImageCacheService
{
    Task<Image?> GetImageAsync(Guid imageId);
    Task<bool> ExistsImageAsync(Guid imageId);
}

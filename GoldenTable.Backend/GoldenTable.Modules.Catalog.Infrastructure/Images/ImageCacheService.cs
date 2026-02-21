using GoldenTable.Common.Application.Caching;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;

namespace GoldenTable.Modules.Catalog.Infrastructure.Images;

public class ImageCacheService(ICacheService cacheService) : IImageCacheService
{
    public Task<Image?> GetAsync(Guid imageId, CancellationToken cancellationToken)
    {
        string cacheKey = GetCacheKey(imageId);
        return cacheService.GetAsync<Image?>(cacheKey, cancellationToken);
    }

    public Task UpdateAsync(Image image, CancellationToken cancellationToken)
    {
        string cacheKey = GetCacheKey(image.Id);
        return cacheService.SetAsync(cacheKey, image, ExpirationTime, cancellationToken);
    }

    public Task DeleteAsync(Guid imageId, CancellationToken cancellationToken)
    {
        string cacheKey = GetCacheKey(imageId);
        return cacheService.RemoveAsync(cacheKey, cancellationToken);
    }

    private static readonly TimeSpan ExpirationTime = TimeSpan.FromMinutes(5);
    private static string GetCacheKey(Guid imageId)
    {
        return $"Image-{imageId}";
    }
}

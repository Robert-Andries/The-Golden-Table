namespace GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;

public interface IImageRepository
{
    Task<Image?> GetAsync(Guid ImageId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Image image, CancellationToken cancellationToken = default);
    Task AddAsync(Image image, CancellationToken cancellationToken);
    Task DeleteAsync(Guid imageId, CancellationToken cancellationToken);
}

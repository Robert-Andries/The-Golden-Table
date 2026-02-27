using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;
using GoldenTable.Modules.Catalog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Catalog.Infrastructure.Images;

internal sealed class ImageRepository(CatalogDbContext context) : IImageRepository
{
    public async Task<Image?> GetAsync(Guid ImageId, CancellationToken cancellationToken = default)
    {
        Image? image = await context.Images.FirstOrDefaultAsync(i => i.Id == ImageId, cancellationToken);
        return image;
    }

    public Task UpdateAsync(Image image, CancellationToken cancellationToken = default)
    {
        if (!context.Images.Any(i => i.Id == image.Id))
        {
            return Task.CompletedTask;
        }

        context.Images.Update(image);
        return Task.CompletedTask;
    }

    public async Task AddAsync(Image image, CancellationToken cancellationToken = default)
    {
        await context.Images.AddAsync(image, cancellationToken);
    }

    public void Remove(Image image)
    {
        if (!context.Images.Any(i => i.Id == image.Id))
        {
            return;
        }

        context.Images.Remove(image);
    }
}

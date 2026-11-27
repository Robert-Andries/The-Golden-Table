using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Builders;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Images;

public class ImageBaseTest : BaseIntegrationTest
{
    public ImageBaseTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
        DateTime pastUtc = DateTime.UtcNow.Subtract(TimeSpan.FromDays(2));
        ImageBuilder = new ImageBuilder(Faker, pastUtc);
    }
    
    protected ImageBuilder ImageBuilder { get; init; }
    
    protected static async Task PutImageInDb(Image image)
    {
        await context.Images.AddAsync(image);
        await context.SaveChangesAsync();
    }
    
    protected static async Task<Image> GetImageFromDb(Guid imageId)
    {
        return await context.Images.AsNoTracking().FirstAsync(i => i.Id == imageId);
    }
}

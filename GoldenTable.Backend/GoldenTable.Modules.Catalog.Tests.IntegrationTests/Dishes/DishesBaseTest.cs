using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Builders;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public class DishesBaseTest : BaseIntegrationTest
{
    public DishesBaseTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
        DateTime pastUtc = DateTime.UtcNow.Subtract(TimeSpan.FromDays(2));
        DishBuilder = new DishBuilder(Faker, pastUtc);
        ImageBuilder = new ImageBuilder(Faker, pastUtc);
    }

    protected DishBuilder DishBuilder { get; init; }
    protected ImageBuilder ImageBuilder { get; init; }

    protected async Task PutDishInDb(Dish dish)
    {
        context.Dishes.Add(dish);
        await context.SaveChangesAsync();
    }

    protected async Task PutImageInDb(Image image)
    {
        await context.Images.AddAsync(image);
        await context.SaveChangesAsync();
    }

    protected async Task<Dish> GetDishFromDb(Guid dishId)
    {
        return await context.Dishes
            .Include(d => d.Tags)
            .Include(d => d.Images).FirstAsync(d => d.Id == dishId);
    }

    protected async Task<Image> GetImageFromDb(Guid imageId)
    {
        return await context.Images.AsNoTracking().FirstAsync(i => i.Id == imageId);
    }
}

using GoldenTable.Modules.Catalog.Application.Dishes.CreateDish;
using GoldenTable.Modules.Catalog.Application.Dishes.EditDish;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
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

    protected static async Task PutDishInDb(Dish dish)
    {
        context.Dishes.Add(dish);
        await context.SaveChangesAsync();
    }

    protected static async Task PutImageInDb(Image image)
    {
        await context.Images.AddAsync(image);
        await context.SaveChangesAsync();
    }

    protected static async Task PutTagInDb(DishTag tag)
    {
        context.Set<DishTag>().Add(tag);
        await context.SaveChangesAsync();
    }

    protected static async Task PutTagsInDb(IEnumerable<DishTag> tags)
    {
        context.Set<DishTag>().AddRange(tags);
        await context.SaveChangesAsync();
    }

    protected static async Task<Dish> GetDishFromDb(Guid dishId)
    {
        return await context.Dishes
            .Include(d => d.Tags)
            .Include(d => d.Images).FirstAsync(d => d.Id == dishId);
    }

    protected static async Task<Image> GetImageFromDb(Guid imageId)
    {
        return await context.Images.AsNoTracking().FirstAsync(i => i.Id == imageId);
    }

    /// <summary>
    ///     Creates a command that creates a dish with the same values as the provided one
    /// </summary>
    protected static CreateDishCommand ToCreateDishCommand(Dish dish)
    {
        return new CreateDishCommand(
            dish.Name.Value,
            dish.Description.Value,
            dish.BasePrice.Amount,
            dish.BasePrice.Currency.Code,
            dish.Sizes.ToList(),
            dish.NutritionalInformation.Energy.Kcal,
            dish.NutritionalInformation.GramsOfFat,
            dish.NutritionalInformation.GramsOfCarbohydrates.Total,
            dish.NutritionalInformation.GramsOfCarbohydrates.OfWhichSugar,
            dish.NutritionalInformation.GramsOfProtein,
            dish.NutritionalInformation.GramsOfSalt,
            dish.Category.Name,
            dish.Tags.Select(tag => tag.Id).ToList());
    }

    /// <summary>
    ///     Creates an edit command that keeps the dish as it is, except for the provided values
    /// </summary>
    protected static EditDishCommand CreateEditDishCommand(
        Dish dish,
        string? name = null,
        decimal? basePriceAmount = null,
        List<DishSize>? sizes = null,
        List<Guid>? tagIds = null)
    {
        return new EditDishCommand(
            dish.Id,
            name ?? dish.Name.Value,
            dish.Description.Value,
            basePriceAmount ?? dish.BasePrice.Amount,
            dish.BasePrice.Currency.Code,
            sizes ?? dish.Sizes.ToList(),
            dish.NutritionalInformation.Energy.Kcal,
            dish.NutritionalInformation.GramsOfFat,
            dish.NutritionalInformation.GramsOfCarbohydrates.Total,
            dish.NutritionalInformation.GramsOfCarbohydrates.OfWhichSugar,
            dish.NutritionalInformation.GramsOfProtein,
            dish.NutritionalInformation.GramsOfSalt,
            dish.Category.Name,
            tagIds ?? dish.Tags.Select(t => t.Id).ToList(),
            dish.Images.Select(i => i.Id).ToList());
    }
}

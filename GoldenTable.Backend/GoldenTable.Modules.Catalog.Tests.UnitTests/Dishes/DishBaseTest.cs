using Bogus;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;
using Name = GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Name;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Dishes;

// CA1515 = Because an application's API isn't typically referenced from outside the assembly, types can be made internal
#pragma warning disable CA1515
public abstract class DishBaseTest : BaseTest
#pragma warning restore CA1515
{
    protected readonly Faker<Dish> DishFaker;

    protected DateTime SometimeUtc;

    protected DishBaseTest()
    {
        SometimeUtc = DateTime.UtcNow;
        DishFaker = new Faker<Dish>().CustomInstantiator(f =>
        {
            Name name = new(f.Name.FullName());
            Description description = new(f.Hacker.Phrase());
            Money basePrice = Money.Create(f.Random.Decimal(0M, 100M), "USD").Value;
            List<DishSize> sizes = CreateDishSizes(f, 3);
            NutritionalValues nutritionalValues = NutritionalValues.Create(200, 200, 200, 200, 200, 200).Value;
            DishCategory category = DishCategory.Create(f.Locale).Value;
            List<Image> images = CreateRandomImages(f, 4);
            List<DishTag> tags = CreateDishTags(f, 4);
            DateTime sometimeUtc = SometimeUtc.AddHours(-3);

            Dish dish = Dish.Create(name, description, basePrice, sizes, nutritionalValues, category, tags, sometimeUtc)
                .Value;

            foreach (Image image in images)
            {
                Result result = dish.AddImage(image, sometimeUtc);
                if (result.IsFailure)
                {
                    throw new Exception(result.Error.ToString());
                }
            }
            
            dish.ClearDomainEvents();

            return dish;
        });
    }

    private List<Image> CreateRandomImages(Faker faker, int count)
    {
        List<Image> images = new();
        for (int i = 0; i < count; i++)
        {
            Uri uri = new(faker.Internet.Url());
            Name name = new(faker.Name.FullName());
            Description description = new(faker.Name.JobDescriptor());
            images.Add(Image.Create(SometimeUtc, uri, name, description).Value);
        }

        return images;
    }

    private List<DishSize> CreateDishSizes(Faker faker, int count)
    {
        List<DishSize> sizes = new();
        for (int i = 0; i < count; i++)
        {
            string name = faker.Name.FirstName();
            float priceAdded = faker.Random.Float(-100f, 100f);
            float weight = faker.Random.Float(1f, 200f);
            sizes.Add(new DishSize(name, priceAdded, weight));
        }

        return sizes;
    }

    private List<DishTag> CreateDishTags(Faker faker, int count)
    {
        List<DishTag> tags = new();
        for (int i = 0; i < count; i++)
        {
            string tagName = faker.Name.LastName();
            tags.Add(DishTag.Create(tagName).Value);
        }

        return tags;
    }
}

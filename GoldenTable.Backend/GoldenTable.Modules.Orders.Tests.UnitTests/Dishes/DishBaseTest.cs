using Bogus;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Domain.Money;
using GoldenTable.Common.Tests.UnitTests;
using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Domain.Dishes.ValueObjects;

namespace GoldenTable.Modules.Orders.Tests.UnitTests.Dishes;

public abstract class DishBaseTest : BaseTest
{
    protected readonly Faker<Dish> DishFaker;

    protected DateTime SometimeUtc;

    protected DishBaseTest()
    {
        SometimeUtc = DateTime.UtcNow;
        DishFaker = new Faker<Dish>().CustomInstantiator(f =>
        {
            Name name = new(f.Name.FullName());
            Money basePrice = Money.Create(f.Random.Decimal(0M, 100M), "USD").Value;
            List<DishSize> sizes = CreateDishSizes(f, 3);
            DateTime sometimeUtc = SometimeUtc.AddHours(-3);

            Dish dish = Dish.Create(name, basePrice, sometimeUtc, sizes).Value;

            dish.ClearDomainEvents();

            return dish;
        });
    }

    private static List<DishSize> CreateDishSizes(Faker faker, int count)
    {
        List<DishSize> sizes = new();
        for (int i = 0; i < count; i++)
        {
            string name = faker.Name.FirstName();
            decimal priceAdded = faker.Random.Decimal(-100, 100);
            decimal weight = faker.Random.Decimal(1, 200);
            sizes.Add(new DishSize(name, priceAdded, weight));
        }

        return sizes;
    }
}

using Bogus;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Domain.Money;
using GoldenTable.Common.Tests.UnitTests;
using GoldenTable.Modules.Orders.Domain.Dishes.ValueObjects;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;

namespace GoldenTable.Modules.Orders.Tests.UnitTests.Orders;

public abstract class OrderBaseTest : BaseTest
{
    protected readonly Faker<Order> OrderFaker;

    protected DateTime SometimeUtc;

    protected OrderBaseTest()
    {
        SometimeUtc = DateTime.UtcNow;
        OrderFaker = new Faker<Order>().CustomInstantiator(f =>
        {
            Guid customerId = f.Random.Guid();
            Money totalPrice = Money.Create(f.Random.Decimal(1M, 500M), "RON").Value;
            List<DishItem> items = CreateDishItems(f, 3);
            Address address = CreateAddress(f);
            DateTime sometimeUtc = SometimeUtc.AddHours(-3);

            Order order =
                Order.Create(customerId, totalPrice, items, address, sometimeUtc).Value;

            order.ClearDomainEvents();

            return order;
        });
    }

    protected static Address CreateAddress(Faker faker)
    {
        return new Address(
            faker.Address.County(),
            faker.Address.City(),
            faker.Address.StreetName(),
            faker.Address.BuildingNumber(),
            faker.Address.ZipCode());
    }

    protected static List<DishItem> CreateDishItems(Faker faker, int count)
    {
        List<DishItem> items = new();
        for (int i = 0; i < count; i++)
        {
            items.Add(new DishItem(faker.Random.Guid(), faker.Name.FirstName(), faker.Random.Int(1, 5)));
        }

        return items;
    }

    /// <summary>
    ///     Moves a freshly created (Placed) order to the requested status through valid transitions
    ///     and clears the raised domain events.
    /// </summary>
    protected Order MoveToStatus(Order order, OrderStatus status)
    {
        OrderStatus[] path = status switch
        {
            OrderStatus.Placed => [],
            OrderStatus.Confirmed => [OrderStatus.Confirmed],
            OrderStatus.Preparing => [OrderStatus.Confirmed, OrderStatus.Preparing],
            OrderStatus.Sent => [OrderStatus.Confirmed, OrderStatus.Preparing, OrderStatus.Sent],
            OrderStatus.Rejected => [OrderStatus.Rejected],
            OrderStatus.Cancelled => [OrderStatus.Cancelled],
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };

        foreach (OrderStatus step in path)
        {
            Result result = order.UpdateStatus(step, SometimeUtc.AddHours(-1));
            if (result.IsFailure)
            {
                throw new InvalidOperationException(result.Error.ToString());
            }
        }

        order.ClearDomainEvents();

        return order;
    }
}

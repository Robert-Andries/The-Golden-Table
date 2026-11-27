using Bogus;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Moneys;

#pragma warning disable CA1515
public abstract class MoneyBaseTest : BaseTest
#pragma warning restore CA1515
{
    public Faker<Money> MoneyFaker;

    protected MoneyBaseTest()
    {
        MoneyFaker = new Faker<Money>().CustomInstantiator(f =>
        {
            Currency[] availableCurrencies = [Currency.USD, Currency.RON, Currency.EUR];
            Currency currency = f.PickRandom(availableCurrencies, 1).First();
            decimal amount = Faker.Random.Decimal(0M, 100M);

            return Money.Create(amount, currency).Value;
        });
    }
}

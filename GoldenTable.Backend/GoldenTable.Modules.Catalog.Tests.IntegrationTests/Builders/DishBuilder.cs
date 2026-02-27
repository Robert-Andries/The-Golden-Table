using System.Diagnostics.CodeAnalysis;
using Bogus;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Builders;

public sealed class DishBuilder(Faker faker, DateTime nowUtc)
{
    private Money? _basePrice;
    private DishCategory? _category;
    private Description? _description;
    private Name? _name;
    private NutritionalValues? _nutritionalInformation;
    private List<DishSize>? _sizes;
    private List<DishTag>? _tags;
    private DateTime _nowUtc { get; } = nowUtc;

    public Dish Build()
    {
        AsignValuesToNullProperties();
        Dish output = Dish.Create(
            _name,
            _description,
            _basePrice,
            _sizes,
            _nutritionalInformation,
            _category,
            _tags,
            _nowUtc).Value;
        output.ClearDomainEvents();

        _name = null;
        _description = null;
        _basePrice = null;
        _sizes = null;
        _nutritionalInformation = null;
        _category = null;
        _tags = null;
        return output;
    }

    public DishBuilder WithName(Name name)
    {
        _name = name;
        return this;
    }

    public DishBuilder WithDescription(Description description)
    {
        _description = description;
        return this;
    }

    public DishBuilder WithBasePrice(Money basePrice)
    {
        _basePrice = basePrice;
        return this;
    }

    public DishBuilder WithSizes(List<DishSize> sizes)
    {
        _sizes ??= new List<DishSize>();
        foreach (DishSize size in sizes)
        {
            _sizes.Add(size);
        }

        return this;
    }

    public DishBuilder WithNutritionalInformation(NutritionalValues values)
    {
        _nutritionalInformation = values;
        return this;
    }

    public DishBuilder WithCategory(DishCategory category)
    {
        _category = category;
        return this;
    }

    public DishBuilder WithTags(List<DishTag> tags)
    {
        _tags ??= new List<DishTag>();
        foreach (DishTag tag in tags)
        {
            _tags.Add(tag);
        }

        return this;
    }

    [MemberNotNull(nameof(_name))]
    [MemberNotNull(nameof(_description))]
    [MemberNotNull(nameof(_basePrice))]
    [MemberNotNull(nameof(_category))]
    [MemberNotNull(nameof(_nutritionalInformation))]
    [MemberNotNull(nameof(_sizes))]
    [MemberNotNull(nameof(_tags))]
    private void AsignValuesToNullProperties()
    {
        _name ??= new Name(faker.Name.FullName());

        _description ??= new Description(faker.Name.JobDescriptor());

        _basePrice ??= Money.Create(faker.Random.Decimal(5M, 1000M), "USD").Value;

        _category ??= DishCategory.Create(faker.Name.FirstName()).Value;

        _nutritionalInformation ??= NutritionalValues.Create(
            faker.Random.Float(1F, 700F),
            faker.Random.Float(1F, 100F),
            faker.Random.Float(100F, 200F),
            faker.Random.Float(1F, 100F),
            faker.Random.Float(1F, 100F),
            faker.Random.Float(1F, 100F)).Value;

        _sizes ??= new List<DishSize>();
        int random = faker.Random.Int(5, 10);
        while (random-- > 0)
        {
            _sizes.Add(
                new DishSize(faker.Name.FirstName(), faker.Random.Float(-10F, 10F), faker.Random.Float(1F, 500F)));
        }

        _tags ??= new List<DishTag>();
        int numberOfTags = faker.Random.Int(5, 10);
        while (numberOfTags-- > 0)
        {
            _tags.Add(DishTag.Create(faker.Name.LastName()).Value);
        }
    }
}

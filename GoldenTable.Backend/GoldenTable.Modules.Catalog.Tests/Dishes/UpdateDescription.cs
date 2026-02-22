using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using GoldenTable.Modules.Catalog.Domain.Dishes;

namespace GoldenTable.Modules.Catalog.Tests.Dishes;

public class UpdateDescription : DishBaseTest
{
    [Fact]
    public void Should_UpdateDescription_Successfully()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        Description description = new(Faker.Name.JobDescriptor());

        // Act
        Result result = sut.UpdateDescription(description, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, opts =>
        {
            return opts.Excluding(d => d.ModifiedOnUtc)
                .Excluding(d => d.Description)
                .Excluding(d => d.DomainEvents);
        });
        sut.Description.Should().Be(description);
        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
    }
}

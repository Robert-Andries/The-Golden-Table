using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Dishes;

public class Rename : DishBaseTest
{
    [Fact]
    public void Should_Rename_Successfully()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        Name validName = new(Faker.Name.FullName());

        // Act
        Result result = sut.Rename(validName, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, opts =>
        {
            return opts.Excluding(d => d.ModifiedOnUtc)
                .Excluding(d => d.Name)
                .Excluding(d => d.DomainEvents);
        });
        sut.Name.Should().Be(validName);
        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        sut.ModifiedOnUtc.Should().NotBe(sut.CreatedOnUtc);
        AssertDomainEventWasPublished<DishRenamedDomainEvent>(sut);
    }

    [Fact]
    public void Should_RenameWithInvalidName_Failure()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        Name invalidName = new("");


        // Act
        Result result = sut.Rename(invalidName, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.InvalidName);
        sut.Should().BeEquivalentTo(original);
    }
}

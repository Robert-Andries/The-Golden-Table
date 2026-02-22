using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;

namespace GoldenTable.Modules.Catalog.Tests.Dishes;

public sealed class UpdateNutritionalInformation : DishBaseTest
{
    [Fact]
    public void Should_UpdateNutritionalInformation_Successfully()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        NutritionalValues validNutritionalValues =
            NutritionalValues.Create(100, 100, 100, 100, 100, 100).Value;

        // Act
        Result result = sut.UpdateNutritionalInformation(validNutritionalValues, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options =>
        {
            return options.Excluding(d => d.NutritionalInformation)
                .Excluding(d => d.ModifiedOnUtc)
                .Excluding(d => d.DomainEvents);
        });
        sut.NutritionalInformation.Should().Be(validNutritionalValues);
        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        AssertDomainEventWasPublished<DishUpdatedNutritionalInformation>(sut);
    }

    [Fact]
    public void Should_NotUpdateNutritionalInformation_SameValueAsBefore()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        NutritionalValues sameNutritionalValues = sut.NutritionalInformation;

        // Act
        Result result = sut.UpdateNutritionalInformation(sameNutritionalValues, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.NutritionalInformationIsTheSame);
        sut.Should().BeEquivalentTo(original);
    }
}

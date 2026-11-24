using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Order.Domain.Dishes;
using GoldenTable.Modules.Order.Domain.Dishes.Events;

namespace GoldenTable.Modules.Order.Tests.UnitTests.Dishes;

public sealed class UpdateName : DishBaseTest
{
    [Fact]
    public void Should_UpdateName_Successfully()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        Name validName = new($"{Faker.Name.FullName()} unique");

        // Act
        Result result = sut.UpdateName(validName, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options =>
        {
            return options.Excluding(d => d.Name)
                .Excluding(d => d.ModifiedOnUtc)
                .Excluding(d => d.DomainEvents);
        });
        sut.Name.Should().Be(validName);
        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        sut.ModifiedOnUtc.Should().NotBe(sut.CreatedOnUtc);
        DishRenamedDomainEvent domainEvent = AssertDomainEventWasPublished<DishRenamedDomainEvent>(sut);
        domainEvent.DishId.Should().Be(sut.Id);
        domainEvent.NewName.Should().Be(validName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void Should_NotUpdateName_InvalidName(string invalidValue)
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        Name invalidName = new(invalidValue);

        // Act
        Result result = sut.UpdateName(invalidName, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.InvalidName);
        sut.Should().BeEquivalentTo(original);
        sut.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Should_NotUpdateName_SameName()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        Name sameName = new(sut.Name.Value);

        // Act
        Result result = sut.UpdateName(sameName, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.SameName);
        sut.Should().BeEquivalentTo(original);
        sut.DomainEvents.Should().BeEmpty();
    }
}

using FluentAssertions;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Dishes;

public sealed class Delete : DishBaseTest
{
    [Fact]
    public void Should_RaiseDishDeletedDomainEvent()
    {
        // Arrange
        Dish sut = DishFaker.Generate();

        // Act
        sut.Delete();

        // Assert
        AssertDomainEventWasPublished<DishDeletedDomainEvent>(sut).DishId.Should().Be(sut.Id);
    }
}

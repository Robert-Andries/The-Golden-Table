using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes.CreateTag;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class CreateTag : TagsBaseTest
{
    public CreateTag(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_CreateTag_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        Result<Guid> result = await Sender.Send(new CreateTagCommand("Vegan"));

        // Assert
        result.IsSuccess.Should().BeTrue();
        DishTag? tag = await GetTagFromDb(result.Value);
        tag.Should().NotBeNull();
        tag.Value.Should().Be("Vegan");
    }

    [Fact]
    public async Task Should_NotCreateTag_ValueAlreadyExists()
    {
        // Arrange
        await ClearDatabaseAsync();
        await PutTagInDb(DishTag.Create("Vegan").Value);

        // Act
        Result<Guid> result = await Sender.Send(new CreateTagCommand("Vegan"));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishTagErrors.AlreadyExists);
    }

    [Fact]
    public async Task Should_NotCreateTag_InvalidValue()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        Result<Guid> result = await Sender.Send(new CreateTagCommand(string.Empty));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishTagErrors.InvalidValue);
    }
}

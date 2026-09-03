using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes.EditTag;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class EditTag : TagsBaseTest
{
    public EditTag(IntegrationTestWebAppFactory factory) : base(factory)
    { }

    [Fact]
    public async Task Should_EditTag_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        DishTag tag = DishTag.Create("OriginalTag").Value;
        await PutTagInDb(tag);

        // Act
        Result result = await Sender.Send(new EditTagCommand(tag.Id, "UpdatedTag"));

        // Assert
        result.IsSuccess.Should().BeTrue();
        DishTag? updatedTag = await GetTagFromDb(tag.Id);
        updatedTag.Should().NotBeNull();
        updatedTag!.Value.Should().Be("UpdatedTag");
    }

    [Fact]
    public async Task Should_NotEditTag_IdNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        Result result = await Sender.Send(new EditTagCommand(Guid.NewGuid(), "SomeValue"));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishTagErrors.NotFound);
    }

    [Fact]
    public async Task Should_NotEditTag_InvalidValue()
    {
        // Arrange
        await ClearDatabaseAsync();
        DishTag tag = DishTag.Create("OriginalTag").Value;
        await PutTagInDb(tag);

        // Act
        Result result = await Sender.Send(new EditTagCommand(tag.Id, ""));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishTagErrors.InvalidValue);
    }

    [Fact]
    public async Task Should_NotEditTag_SameValue()
    {
        // Arrange
        await ClearDatabaseAsync();
        DishTag tag = DishTag.Create("OriginalTag").Value;
        await PutTagInDb(tag);

        // Act
        Result result = await Sender.Send(new EditTagCommand(tag.Id, "OriginalTag"));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishTagErrors.SameValue);
    }

    [Fact]
    public async Task Should_NotEditTag_DuplicateValue()
    {
        // Arrange
        await ClearDatabaseAsync();
        DishTag tag1 = DishTag.Create("Tag1").Value;
        DishTag tag2 = DishTag.Create("Tag2").Value;
        await PutTagInDb(tag1);
        await PutTagInDb(tag2);

        // Act
        Result result = await Sender.Send(new EditTagCommand(tag2.Id, "Tag1"));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishTagErrors.AlreadyExists);
    }
}

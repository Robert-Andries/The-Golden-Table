using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes.DeleteTag;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class DeleteTag : TagsBaseTest
{
    public DeleteTag(IntegrationTestWebAppFactory factory) : base(factory)
    { }

    [Fact]
    public async Task Should_DeleteTag_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        DishTag tag = DishTag.Create("TagToDelete").Value;
        await PutTagInDb(tag);
        for (int i = 0; i < 3; i++)
        {
            await PutTagInDb(DishTag.Create($"OtherTag{i}").Value);
        }

        // Act
        Result result = await Sender.Send(new DeleteTagCommand(tag.Id));

        // Assert
        result.IsSuccess.Should().BeTrue();
        DishTag? deletedTag = await GetTagFromDb(tag.Id);
        deletedTag.Should().BeNull();
        context.Set<DishTag>().Count().Should().Be(3);
    }

    [Fact]
    public async Task Should_NotDeleteTag_IdNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        DishTag tag = DishTag.Create("ExistingTag").Value;
        await PutTagInDb(tag);

        // Act
        Result result = await Sender.Send(new DeleteTagCommand(Guid.NewGuid()));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishTagErrors.NotFound);
        context.Set<DishTag>().Count().Should().Be(1);
    }
}

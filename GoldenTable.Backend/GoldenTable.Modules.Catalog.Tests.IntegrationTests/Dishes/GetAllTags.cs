using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes;
using GoldenTable.Modules.Catalog.Application.Dishes.GetAllTags;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class GetAllTags : TagsBaseTest
{
    public GetAllTags(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_GetAllTags_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        List<DishTag> tags = [DishTag.Create("Vegan").Value, DishTag.Create("Spicy").Value];
        foreach (DishTag tag in tags)
        {
            await PutTagInDb(tag);
        }

        // Act
        Result<List<TagResponse>> result = await Sender.Send(new GetAllTagsQuery());

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(tags.Select(t => new TagResponse(t)));
    }

    [Fact]
    public async Task Should_GetNoTags_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        Result<List<TagResponse>> result = await Sender.Send(new GetAllTagsQuery());

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}

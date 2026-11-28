using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Images;
using GoldenTable.Modules.Catalog.Application.Images.GetAll;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Images;

public sealed class GetAllImages : ImageBaseTest
{
    public GetAllImages(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_GetAllImages_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        List<Image> images = [ImageBuilder.Build(), ImageBuilder.Build()];
        foreach (Image image in images)
        {
            await PutImageInDb(image);
        }

        // Act
        Result<List<ImageResponse>> result = await Sender.Send(new GetAllImagesQuery());

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(images.Select(i => new ImageResponse(i)));
    }

    [Fact]
    public async Task Should_GetNoImages_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        Result<List<ImageResponse>> result = await Sender.Send(new GetAllImagesQuery());

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}

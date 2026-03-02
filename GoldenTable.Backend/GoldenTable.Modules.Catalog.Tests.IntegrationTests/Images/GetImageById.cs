using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Images;
using GoldenTable.Modules.Catalog.Application.Images.GetImageById;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Images;

public sealed class GetImageById : ImageBaseTest
{
    public GetImageById(IntegrationTestWebAppFactory factory) : base(factory)
    { }
    
    [Fact]
    public async Task Should_GetImage_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        Image image = ImageBuilder.Build();
        await PutImageInDb(image);
        ImageResponse expected = new(image);

        // Act
        Result<ImageResponse> result = await Sender.Send(new GetImageByIdQuery(image.Id));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task Should_GetImage_IdNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        Result<ImageResponse> result = await Sender.Send(new GetImageByIdQuery(Guid.NewGuid()));

        // Assert
        result.Error.Should().Be(ImageErrors.NotFound);
    }
}

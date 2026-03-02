using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Images.Delete;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Images;

public sealed class Delete : ImageBaseTest
{
    public Delete(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_DeleteImage_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        Image image = ImageBuilder.Build();
        await PutImageInDb(image);
        for (int i = 0; i < 10; i++)
        {
            await PutImageInDb(ImageBuilder.Build());
        }

        // Act
        Result result = await Sender.Send(new DeleteImageCommand(image.Id));

        // Assert
        result.IsSuccess.Should().BeTrue();
        context.Images.Count().Should().Be(10);
    }
    
    [Fact]
    public async Task Should_NotDeleteImage_InvalidId()
    {
        // Arrange
        await ClearDatabaseAsync();
        Image image = ImageBuilder.Build();
        await PutImageInDb(image);

        // Act
        Result result = await Sender.Send(new DeleteImageCommand(Guid.NewGuid()));

        // Assert
        result.Error.Should().Be(ImageErrors.NotFound);
        context.Images.Count().Should().Be(1);
    }
}

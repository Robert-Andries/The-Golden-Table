using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Images.UpdateDescription;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Images;

public sealed class UpdateDescription : ImageBaseTest
{
    public UpdateDescription(IntegrationTestWebAppFactory factory) : base(factory)
    { }
    
    [Fact]
    public async Task Should_UpdateDescription_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        Image image = ImageBuilder.Build();
        await PutImageInDb(image);
        
        // Act
        Result result = await Sender.Send(new UpdateDescriptionCommand(image.Id, new("NewDescription")));

        // Assert
        result.IsSuccess.Should().BeTrue();
        image.Description!.Value.Should().Be("NewDescription");
    }
    
    [Fact]
    public async Task Should_NotUpdateDescription_IdNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        
        // Act
        Result result = await Sender.Send(new UpdateDescriptionCommand(Guid.NewGuid(), new("NewDescription")));

        // Assert
        result.Error.Should().Be(ImageErrors.NotFound);
    }
}

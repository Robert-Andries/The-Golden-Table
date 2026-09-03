using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Images.EditImage;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Images;

public sealed class EditImage : ImageBaseTest
{
    public EditImage(IntegrationTestWebAppFactory factory) : base(factory)
    { }
    
    [Fact]
    public async Task Should_EditImage_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        Image image = ImageBuilder.Build();
        await PutImageInDb(image);
        
        // Act
        Uri uri = new("https://www.test.com");
        Result result = await Sender.Send(new EditImageCommand(image.Id, "NewName", "NewDescription", uri));

        // Assert
        result.IsSuccess.Should().BeTrue();
        image.Name.Value.Should().Be("NewName");
        image.Description!.Value.Should().Be("NewDescription");
        image.Uri.Should().Be(uri);
    }
    
    [Fact]
    public async Task Should_NotEditImage_IdNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        
        // Act
        Result result = await Sender.Send(new EditImageCommand(Guid.NewGuid(), "NewName", "NewDescription", new Uri("https://www.test.com")));

        // Assert
        result.Error.Should().Be(ImageErrors.NotFound);
    }
    
    [Fact]
    public async Task Should_NotEditImage_InvalidName()
    {
        // Arrange
        await ClearDatabaseAsync();
        Image image = ImageBuilder.Build();
        await PutImageInDb(image);
        
        // Act
        Result result = await Sender.Send(new EditImageCommand(image.Id, "", "NewDescription",  new Uri("https://www.test.com")));

        // Assert
        result.Error.Should().Be(ImageErrors.InvalidName);
    }
}

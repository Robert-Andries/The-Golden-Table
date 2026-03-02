using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Images.Rename;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Images;

public sealed class Rename : ImageBaseTest
{
    public Rename(IntegrationTestWebAppFactory factory) : base(factory)
    { }
    
    [Fact]
    public async Task Should_RenameImage_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        Image image = ImageBuilder.Build();
        await PutImageInDb(image);
        
        // Act
        Result result = await Sender.Send(new RenameCommand(image.Id, new("NewName")));

        // Assert
        result.IsSuccess.Should().BeTrue();
        image.Name.Value.Should().Be("NewName");
    }
    
    [Fact]
    public async Task Should_NotRenameImage_IdNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        
        // Act
        Result result = await Sender.Send(new RenameCommand(Guid.NewGuid(), new("NewName")));

        // Assert
        result.Error.Should().Be(ImageErrors.NotFound);
    }
    
    [Fact]
    public async Task Should_NotRenameImage_DomainError()
    {
        // Arrange
        await ClearDatabaseAsync();
        Image image = ImageBuilder.Build();
        await PutImageInDb(image);
        
        // Act
        Result result = await Sender.Send(new RenameCommand(image.Id, new("")));

        // Assert
        result.Error.Should().Be(ImageErrors.InvalidName);
    }
}

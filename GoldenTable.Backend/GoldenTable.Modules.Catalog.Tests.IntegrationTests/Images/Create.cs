using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Images.Create;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Images;

public sealed class Create : ImageBaseTest
{
    public Create(IntegrationTestWebAppFactory factory) : base(factory)
    { }

    [Fact]
    public async Task Should_CreateImage_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        Image image = ImageBuilder.Build();

        // Act
        Result<Guid> result = await Sender.Send(
            new CreateImageCommand(image.Uri, image.Name.Value, image.Description?.Value));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
        context.Images.Count().Should().Be(1);
    }
    
    [Fact]
    public async Task Should_NotCreateImage_NameAlreadyExists()
    {
        // Arrange
        await ClearDatabaseAsync();
        Image image = ImageBuilder.Build();
        await PutImageInDb(image);

        // Act
        Result<Guid> result = await Sender.Send(
            new CreateImageCommand(new(Faker.Internet.Url()), image.Name.Value, image.Description?.Value));

        // Assert
        result.Error.Should().Be(ImageErrors.NameAlreadyExists);
        context.Images.Count().Should().Be(1);
    }
    
    [Fact]
    public async Task Should_NotCreateImage_UriAlreadyExists()
    {
        // Arrange
        await ClearDatabaseAsync();
        Image image = ImageBuilder.Build();
        await PutImageInDb(image);

        // Act
        Result<Guid> result = await Sender.Send(
            new CreateImageCommand(image.Uri, "RandomName123", image.Description?.Value));

        // Assert
        result.Error.Should().Be(ImageErrors.UriAlreadyExists);
        context.Images.Count().Should().Be(1);
    }
    
    [Fact]
    public async Task Should_NotCreateImage_DomainError()
    {
        // Arrange
        await ClearDatabaseAsync();
        Image image = ImageBuilder.Build();

        // Act
        Result<Guid> result = await Sender.Send(
            new CreateImageCommand(image.Uri, "", image.Description?.Value));

        // Assert
        result.Error.Should().Be(ImageErrors.InvalidName);
        context.Images.Count().Should().Be(0);
    }
}

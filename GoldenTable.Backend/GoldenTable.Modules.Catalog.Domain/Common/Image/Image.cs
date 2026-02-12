using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Events;
using GoldenTable.Modules.Catalog.Domain.Common.Image.ValueTypes;

namespace GoldenTable.Modules.Catalog.Domain.Common.Image;

public sealed class Image : Entity
{
    private Image()
    {
    }

    public Guid Id { get; private set; }
    public Uri Uri { get; private set; }
    public Name Name { get; private set; }
    public Description Description { get; private set; }

    public static Result<Image> Create(DateTime createdOnUtc, Uri uri, string name, string? description = null)
    {
        if (string.IsNullOrEmpty(name))
        {
            return Result.Failure<Image>(ImageErrors.InvalidName);
        }

        if (!uri.IsWellFormedOriginalString())
        {
            return Result.Failure<Image>(ImageErrors.InvalidUri);
        }

        var image = new Image
        {
            Id = Guid.NewGuid(),
            Uri = uri,
            Name = new(name),
            Description = new(description)
        };

        image.Raise(new ImageCreatedDomainEvent(Guid.NewGuid(), image.Id, createdOnUtc));

        return image;
    }

    public Result Rename(string name, DateTime nowUtc)
    {
        if (string.IsNullOrEmpty(name))
        {
            return ImageErrors.InvalidName;
        }

        Name = new(name);
        Raise(new ImageRenamedDomainEvent(Guid.NewGuid(), Id, Name, nowUtc));
        return Result.Success();
    }

    public Result UpdateDescription(string? description, DateTime nowUtc)
    {
        Description = new(description);
        Raise(new ImageDescriptionUpdatedDomainEvent(Guid.NewGuid(), Id, Description, nowUtc));
        return Result.Success();
    }

    public Result UpdateUri(Uri uri, DateTime nowUtc)
    {
        if (!uri.IsWellFormedOriginalString())
        {
            return ImageErrors.InvalidUri;
        }

        Uri = uri;
        Raise(new ImageUriUpdatedDomainEvent(Guid.NewGuid(), Id, Uri, nowUtc));
        return Result.Success();
    }
}

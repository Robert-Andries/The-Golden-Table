using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Events;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

namespace GoldenTable.Modules.Catalog.Domain.Common.Image;

public sealed class Image : Entity
{
    private Image()
    {
    }
    public Uri Uri { get; private set; }
    public Name Name { get; private set; }
    public Description? Description { get; private set; }

    public static Result<Image> Create(DateTime createdOnUtc, Uri uri, Name name, Description? description = null)
    {
        if (!name.IsValid())
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
            Name = name,
            Description = description
        };

        image.Raise(new ImageCreatedDomainEvent(Guid.NewGuid(), image.Id, createdOnUtc));

        return image;
    }

    public Result Rename(Name name, DateTime nowUtc)
    {
        if (!name.IsValid())
        {
            return ImageErrors.InvalidName;
        }

        if (name == Name)
        {
            return Result.Success();
        }
        Name = name;
        Raise(new ImageRenamedDomainEvent(Guid.NewGuid(), Id, Name, nowUtc));
        return Result.Success();
    }

    public Result UpdateDescription(Description? description, DateTime nowUtc)
    {
        if (Description == description)
        {
            return Result.Success();
        }
        Description = description;
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

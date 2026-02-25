using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Events;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

namespace GoldenTable.Modules.Catalog.Domain.Common.Image;

/// <summary>
/// Entity holding the needed data for an image
/// </summary>
public sealed class Image : Entity
{
    private Image()
    {
    }
    /// <summary>
    /// Location of the image
    /// </summary>
    public Uri Uri { get; private set; }
    public Name Name { get; private set; }
    public Description? Description { get; private set; }

    /// <summary>
    /// Factory method used to create an image object
    /// </summary>
    /// <param name="createdOnUtc">The moment the image is created</param>
    /// <param name="uri">Uri for the location of the image</param>
    /// <param name="name">Name of the image</param>
    /// <param name="description">Description of the image</param>
    /// <returns>Result indicating success and holding the newly created image</returns>
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

    /// <summary>
    /// Method to update Name property
    /// </summary>
    /// <param name="name">The new name</param>
    /// <param name="nowUtc">When operation occured</param>
    /// <returns>Result indicating success and respective error</returns>
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


    /// <summary>
    /// Method used to update Description property
    /// </summary>
    /// <param name="description">The new description</param>
    /// <param name="nowUtc">When operation occured</param>
    /// <returns>Result indicating success and respective error</returns>
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

    /// <summary>
    /// Method used to update uri property
    /// </summary>
    /// <param name="uri">The new uri</param>
    /// <param name="nowUtc">When operation occured</param>
    /// <returns>Result indicating success and respective error</returns>
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

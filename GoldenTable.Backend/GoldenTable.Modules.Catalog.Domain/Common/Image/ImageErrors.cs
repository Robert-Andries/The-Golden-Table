using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Common.Image;

public static class ImageErrors
{
    public static Error InvalidName { get; } = new("Image.InvalidName",
        "The provided name is invalid.", ErrorType.Validation);

    public static Error InvalidUri { get; } = new("Image.InvalidUri",
        "The provided uri is invalid.", ErrorType.Validation);

    public static Error NotFound { get; } = new("Image.NotFound",
        "The image with the provided ID was not found.", ErrorType.NotFound);

    public static Error NameAlreadyExists { get; } = new("Image.NameAlreadyExists",
        "The provided name is already in use.", ErrorType.Validation);

    public static Error UriAlreadyExists { get; } = new("Image.UriAlreadyExists",
        "The provided uri is already in use.", ErrorType.Validation);
}

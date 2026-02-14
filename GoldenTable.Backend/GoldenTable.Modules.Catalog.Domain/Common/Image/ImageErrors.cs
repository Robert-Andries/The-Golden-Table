using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Common.Image;

public static class ImageErrors
{
    public static Error InvalidName { get; } = new Error("Image.InvalidName",
        "The provided name is invalid.", ErrorType.Validation);

    public static Error InvalidUri { get; } = new Error("Image.InvalidUri",
        "The provided uri is invalid.", ErrorType.Validation);

    public static Error NotFound { get; } = new Error("Image.NotFound",
        "The image with the provided ID was not found.", ErrorType.NotFound);
}

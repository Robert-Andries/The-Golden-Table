using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.Tag;

public static class DishTagErrors
{
    public static Error InvalidValue { get; } =
        new("DishTagErrors.InvalidValue", "The provided value for tag is invalid.", ErrorType.Validation);

    public static Error AlreadyExists { get; } =
        new("DishTagErrors.AlreadyExists", "A tag with the provided value already exists.", ErrorType.Validation);

    public static Error NotFound { get; } =
        new("DishTagErrors.NotFound", "The tag was not found.", ErrorType.NotFound);

    public static Error SomeTagsNotFound { get; } =
        new("DishTagErrors.SomeTagsNotFound", "One or more tags were not found.", ErrorType.NotFound);
}

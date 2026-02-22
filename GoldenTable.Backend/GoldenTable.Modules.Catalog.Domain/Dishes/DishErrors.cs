using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes;

public static class DishErrors
{
    public static Error InvalidName { get; } =
        new("DishErrors.InvalidName", "The provided name is invalid.", ErrorType.Validation);

    public static Error SameName { get; } = new("DishErrors.SameName",
        "The provided name is same as the old one.", ErrorType.Validation);

    public static Error InvalidDescription { get; } = new("DishErrors.InvalidDescription",
        "The provided description is invalid.", ErrorType.Validation);

    public static Error ImageAlreadyPresent { get; } = new("DishErrors.ImageAlreadyPresent",
        "The provided image is already present.", ErrorType.Validation);

    public static Error ImageNotPresent { get; } = new("DishErrors.ImageNotPresent",
        "The provided image is not present.", ErrorType.Validation);

    public static Error InvalidTags { get; } = new("DishErrors.InvalidTags",
        "The provided tags are invalid.", ErrorType.Validation);

    public static Error SameDescription { get; } = new("DishErrors.SameDescription",
        "The provided description is same as the old one.", ErrorType.Validation);

    public static Error SamePrice { get; } = new("DishErrors.SamePrice",
        "The provided price is same as the old one.", ErrorType.Validation);

    public static Error SizeAlreadyPresent { get; } = new("DishErrors.SizeAlreadyPresent",
        "The dish already have the provided size.", ErrorType.Validation);

    public static Error SizeDoesNotExist { get; } = new("DishErrors.SizeDoesNotExist",
        "The provided size does not exist and cannot be removed.", ErrorType.Validation);

    public static Error NutritionalInformationIsTheSame { get; } = new(
        "DishErrors.NutritionalInformationIsTheSame",
        "The provided nutritional information is the same as the dish's one.", ErrorType.Validation);

    public static Error NotFound { get; } = new("DishErrors.NotFound",
        "The dish with the provided id not found.", ErrorType.NotFound);

    public static Error InvalidValueForTag { get; } = new("DishErrors.InvalidValueForTag",
        "The provided value for tag is invalid.", ErrorType.Validation);

    public static Error InvalidSize { get; } = new("DishErrors.InvalidSize",
        "The provided size is invalid.", ErrorType.Validation);

    public static Error SameCategory { get; } = new("DishErrors.SameCategory",
        "The provided category is same as the old one.", ErrorType.Validation);

    public static Error InvalidCategoryName { get; } = new("DishErrors.InvalidCategoryName",
        "The provided category name is invalid.", ErrorType.Validation);

    public static Error TagsAlreadyPresent { get; } = new("DishErrors.TagsAlreadyPresent",
        "The provided tags are already present.", ErrorType.Validation);
}

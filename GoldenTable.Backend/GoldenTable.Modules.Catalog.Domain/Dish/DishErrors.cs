using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dish;

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
}

using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;

namespace GoldenTable.Modules.Catalog.Application.Dishes.UpdateNutritionalInformation;

public sealed record UpdateNutritionalInformationCommand(Guid DishId, NutritionalValues NutritionalInformation)
    : ICommand;

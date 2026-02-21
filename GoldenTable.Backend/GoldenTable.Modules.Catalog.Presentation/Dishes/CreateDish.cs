using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Dishes.CreateDish;
using GoldenTable.Modules.Catalog.Domain.Dishes.Enums;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using GoldenTable.Modules.Catalog.Presentation.Dishes.Common.NutritionalInformation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Dishes;

internal sealed class CreateDish : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("dishes/create", async (Request request, ISender sender) =>
            {
                Result result = await sender.Send(new CreateDishCommand(
                    request.Name,
                    request.Description,
                    request.BasePriceAmount,
                    request.BasePriceCurrency,
                    request.DishSizes,
                    request.NutritionalInformation.Kcal,
                    request.NutritionalInformation.GramsOfFat,
                    request.NutritionalInformation.GramsOfCarbohydrates,
                    request.NutritionalInformation.GramsOfSugar,
                    request.NutritionalInformation.GramsOfProtein,
                    request.NutritionalInformation.GramsOfSalt,
                    request.ImageIds,
                    request.DishCategory,
                    request.DishTags));

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
            .WithTags(Tags.Dish);
    }

    internal sealed class Request
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePriceAmount { get; set; }
        public string BasePriceCurrency { get; set; }
        public List<DishSize> DishSizes { get; set; }
        public NutritionalRequest NutritionalInformation { get; set; }
        public List<Guid> ImageIds { get; set; }
        public string DishCategory { get; set; }
        public List<DishTag> DishTags { get; set; }
    }
}

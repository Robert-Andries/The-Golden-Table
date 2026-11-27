using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Dishes.EditDish;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using GoldenTable.Modules.Catalog.Presentation.Dishes.Common.NutritionalInformation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Dishes;

internal sealed class EditDish : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("dishes/edit/{id}", async (Guid id, [FromBody] Request request, [FromServices] ISender sender) =>
            {
                Result result = await sender.Send(new EditDishCommand(
                    id,
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
                    request.DishCategory,
                    request.TagIds,
                    request.ImageIds));

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
        public List<Guid> TagIds { get; set; }
    }
}

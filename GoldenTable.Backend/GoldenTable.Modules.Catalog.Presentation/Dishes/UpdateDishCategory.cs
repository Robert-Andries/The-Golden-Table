using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Dishes.UpdateDishCategory;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Dishes;

public class UpdateDishCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("dishes/update-category/", async ( [FromBody] Request request, [FromServices] ISender sender) =>
        {
            Result result = await sender.Send(new UpdateDishCategoryCommand(request.DishId, request.DishCategory));

            return result.Match(Results.NoContent, ApiResults.Problem);
        })
        .WithTags(Tags.Dish);
    }

    internal sealed class Request
    {
        public Guid DishId { get; set; }
        public string DishCategory { get; set; }
    }
}
